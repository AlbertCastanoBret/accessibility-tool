using System;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_RemapControls
{
    public class ACC_ControlSchemesConfigurationEditorWindow:EditorWindow
    {
        private InputActionAsset inputActionAsset;
        private bool isReadyToCreateGUI = false;
        
        private VisualElement controlSchemesContainer;
        private ScrollView controlSchemesScrollView;
        
        private Dictionary<string, bool> controlSchemeToggleValues = new Dictionary<string, bool>();
        private Dictionary<string, bool> currentControlSchemeToggleValues = new Dictionary<string, bool>();
        private Dictionary<string, bool> lastSaveControlSchemeToggleValues = new Dictionary<string, bool>();
        
        private Dictionary<ACC_BindingData, bool> currentBindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
        private Dictionary<ACC_BindingData, bool> bindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
        private Dictionary<ACC_BindingData, bool> lastSaveBindingsToggleValues = new Dictionary<ACC_BindingData, bool>();

        private void OnDestroy()
        {
            ConfirmSaveChangesIfNeeded();
        }

        public static void ShowWindow(InputActionAsset inputActionAsset)
        {
            ACC_ControlSchemesConfigurationEditorWindow window = GetWindow<ACC_ControlSchemesConfigurationEditorWindow>();
            window.titleContent = new GUIContent("Control Schemes Configuration");
            window.minSize = new Vector2(600, 450);
            window.maxSize = new Vector2(600, 450);
            window.inputActionAsset = inputActionAsset;
            window.isReadyToCreateGUI = true;
            window.CreateGUI();
            window.LoadJson();
            //window.ShowModal();
        }

        private void CreateGUI()
        {
            if(!isReadyToCreateGUI) return;
            isReadyToCreateGUI = false;
            rootVisualElement.Clear();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_RemapControls/ACC_ControlSchemesConfigurationEditorWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            rootVisualElement.AddToClassList("main-container");

            var controlSchemesLabel = new Label("Control Schemes");
            controlSchemesLabel.AddToClassList("control-schemes-label");
            
            var devices = inputActionAsset.controlSchemes
                .Select(scheme => 
                {
                    if (!scheme.deviceRequirements.Any())
                    {
                        return "All devices";
                    }

                    return scheme.deviceRequirements
                        .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                        .Distinct()
                        .OrderBy(device => device)
                        .Aggregate((current, next) => current + ", " + next);
                })
                .Where(device => device != null)
                .Distinct()
                .ToList();
            
            devices.Insert(0, "All devices");
            var devicesDropdown = new DropdownField("Devices:", devices, 0);
            devicesDropdown.AddToClassList("devices-dropdown");
            devicesDropdown[0].AddToClassList("devices-dropdown-label");

            var controlSchemes = inputActionAsset.controlSchemes.Select(scheme => scheme.name).ToList();
            controlSchemes.ForEach(key => currentControlSchemeToggleValues[key] = false);
            
            devicesDropdown.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == "All devices")
                {
                    controlSchemes = inputActionAsset.controlSchemes.Select(scheme => scheme.name).ToList();
                    currentControlSchemeToggleValues = new Dictionary<string, bool>();
                    currentBindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
                    controlSchemes.ForEach(key => currentControlSchemeToggleValues[key] = controlSchemeToggleValues[key]);
                }
                else
                {
                    controlSchemes = inputActionAsset.controlSchemes
                        .Where(scheme => String.Join(", ", scheme.deviceRequirements
                            .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                            .Distinct()
                            .OrderBy(device => device)) == evt.newValue)
                        .Select(scheme => scheme.name)
                        .ToList();
                    currentControlSchemeToggleValues = new Dictionary<string, bool>();
                    currentBindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
                    controlSchemes.ForEach(key => currentControlSchemeToggleValues[key] = controlSchemeToggleValues[key]);
                }
                CreateTable();
            });
            
            controlSchemesScrollView = new ScrollView();
            controlSchemesScrollView.AddToClassList("control-schemes-scroll-view");
            
            for (int i = 0; i < controlSchemes.Count(); i++)
            {
                controlSchemeToggleValues[controlSchemes[i]] = false;
            }
            
            bindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
            foreach (var controlScheme in controlSchemes)
            {
                foreach (var actionMap in inputActionAsset.actionMaps)
                {
                    foreach (var action in actionMap.actions)
                    {
                        foreach (var binding in action.bindings)
                        {
                            string[] groups = binding.groups.Split(';');
                            foreach (string group in groups)
                            {
                                if (group == controlScheme)
                                {
                                    ACC_BindingData accBindingData = new ACC_BindingData(binding.id.ToString(), controlScheme);
                                    bindingsToggleValues[accBindingData] = false;
                                }
                            }
                        }
                    }
                }
            }

            CreateTable();
            
            var createButton = new Button() { text = "Save" };
            createButton.AddToClassList("create-button");
            createButton.clicked += ConfigureJSON;
            
            rootVisualElement.Add(controlSchemesLabel);
            rootVisualElement.Add(devicesDropdown);
            rootVisualElement.Add(controlSchemesScrollView);
            rootVisualElement.Add(createButton);
            
            lastSaveControlSchemeToggleValues = new Dictionary<string, bool>(controlSchemeToggleValues);
            lastSaveBindingsToggleValues = new Dictionary<ACC_BindingData, bool>(bindingsToggleValues);
        }

        private void CreateTable()
        {
            controlSchemesScrollView.Clear();
            controlSchemesContainer = new VisualElement();
            controlSchemesContainer.AddToClassList("control-schemes-container");
            
            var controlSchemeTitle = new VisualElement();
            controlSchemeTitle.AddToClassList("control-scheme-title");

            var controlSchemeNameTitle = new Label("Name");
            controlSchemeNameTitle.AddToClassList("control-scheme-name-title");
            
            var controlSchemeRebindTitle = new Label("Can Rebind?");
            controlSchemeRebindTitle.AddToClassList("control-scheme-rebind-title");
            
            controlSchemeTitle.Add(controlSchemeNameTitle);
            controlSchemeTitle.Add(controlSchemeRebindTitle);
            
            controlSchemesContainer.Add(controlSchemeTitle);
            
            for (int i=0; i<currentControlSchemeToggleValues.Count; i++)
            {
                var controlScheme = new VisualElement();
                controlScheme.AddToClassList("control-scheme");
                
                var controlSchemeFirstRow = new VisualElement();
                controlSchemeFirstRow.AddToClassList("control-scheme-first-row");
                if (i == currentControlSchemeToggleValues.Count - 1)
                {
                    controlSchemeFirstRow.AddToClassList("control-scheme-last");
                }

                var arrowButton = new Button() { text = "\u25b6" };
                arrowButton.AddToClassList("control-scheme-arrow");
                arrowButton.clicked += () =>
                {
                    ToggleControlSchemeDisplay(arrowButton, controlScheme);
                };
                
                var controlSchemeLabel = new Label(){text = currentControlSchemeToggleValues.Keys.ToList()[i]};
                controlSchemeLabel.AddToClassList("control-scheme-label");

                var controlSchemeToggleContainer = new VisualElement();
                controlSchemeToggleContainer.AddToClassList("control-scheme-toggle-container");
                
                var controlSchemeToggle = new Toggle(){value = currentControlSchemeToggleValues[controlSchemeLabel.text]};
                controlSchemeToggle.AddToClassList("control-scheme-toggle");
                controlSchemeToggle.RegisterValueChangedCallback(evt =>
                {
                    controlSchemeToggleValues[controlSchemeLabel.text] = evt.newValue;
                    for (int k = 1; k < controlScheme.childCount; k++)
                    {
                        for (int l = 1; l < controlScheme[k].childCount; l++)
                        {
                            for (int m = 1; m < controlScheme[k][l].childCount; m++)
                            {
                                controlScheme[k][l][m].Query<Toggle>().First().SetEnabled(evt.newValue);
                            }
                        }
                    }
                });
                
                controlSchemeToggleContainer.Add(controlSchemeToggle);
                
                controlSchemeFirstRow.Add(arrowButton);
                controlSchemeFirstRow.Add(controlSchemeLabel);
                controlSchemeFirstRow.Add(controlSchemeToggleContainer);
                controlScheme.Add(controlSchemeFirstRow);
                controlSchemesContainer.Add(controlScheme);
                CreateActionMaps(controlScheme);
            }
            controlSchemesScrollView.Add(controlSchemesContainer);
        }

        private void CreateActionMaps(VisualElement parent)
        {
            var actionMaps = inputActionAsset.actionMaps;
            foreach (var actionMap in actionMaps)
            {
                var mainContainer = new VisualElement();
                mainContainer.AddToClassList("control-scheme-action-map-container");
                mainContainer.style.display = DisplayStyle.None;
                    
                var firstRow = new VisualElement();
                firstRow.AddToClassList("control-scheme-action-map-first-row");
                    
                var arrowButton = new Button() { text = "\u25b6" };
                arrowButton.AddToClassList("control-scheme-arrow");
                arrowButton.clicked += () =>
                {
                    ToggleControlSchemeDisplay(arrowButton, mainContainer);
                };
                    
                var actionMapLabel = new Label(actionMap.name);
                actionMapLabel.AddToClassList("control-scheme-action-map-label");
                    
                firstRow.Add(arrowButton);
                firstRow.Add(actionMapLabel);
                mainContainer.Add(firstRow);
                parent.Add(mainContainer);
                CreateActions(mainContainer);
            }
        }
        
        private void CreateActions(VisualElement parent)
        {
            var actionMaps = inputActionAsset.actionMaps;
            var actionMap = actionMaps.FirstOrDefault(am => am.name == parent.Query<Label>().First().text);
            if (actionMap != null)
            {
                var actionMapActions = actionMap.actions;
                foreach (var action in actionMapActions)
                {
                    var mainContainer = new VisualElement();
                    mainContainer.AddToClassList("control-scheme-action-container");
                    mainContainer.style.display = DisplayStyle.None;
                    
                    var firstRow = new VisualElement();
                    firstRow.AddToClassList("control-scheme-action-first-row");
                    
                    var arrowButton = new Button() { text = "\u25b6" };
                    arrowButton.AddToClassList("control-scheme-arrow");
                    arrowButton.clicked += () =>
                    {
                        ToggleControlSchemeDisplay(arrowButton, mainContainer);
                    };
                    
                    var actionLabel = new Label(action.name);
                    actionLabel.AddToClassList("control-scheme-action-label");
                    
                    firstRow.Add(arrowButton);
                    firstRow.Add(actionLabel);
                    mainContainer.Add(firstRow);
                    parent.Add(mainContainer);
                    CreateBindings(mainContainer);
                }
            }
        }

        private void CreateBindings(VisualElement parent)
        {
            var actionMaps = inputActionAsset.actionMaps;
            var controlScheme = parent.parent.parent.Query<Label>().First().text;
            var actionMap = actionMaps.FirstOrDefault(am => am.name == parent.parent.Query<Label>().First().text);
            if (actionMap != null)
            {
                var actionMapActions = actionMap.actions;
                var action = actionMapActions.FirstOrDefault(a => a.name == parent.Query<Label>().First().text);
                if (action != null)
                {
                    var actionBindings = action.bindings;
                    foreach (var binding in actionBindings)
                    {
                        string[] groups = binding.groups.Split(';');
                        foreach (string group in groups)
                        {
                            if (group == controlScheme)
                            {
                                var presentablePath = MakePathPresentable(binding.path);
                                var mainContainer = new VisualElement();
                                mainContainer.style.display = DisplayStyle.None;
                                mainContainer.AddToClassList("control-scheme-binding-container");
                    
                                var firstRow = new VisualElement();
                                firstRow.AddToClassList("control-scheme-binding-first-row");
                                
                                var bindingLabel = new Label(presentablePath);
                                bindingLabel.AddToClassList("control-scheme-binding-label");
                                
                                var bindingToggleContainer = new VisualElement();
                                bindingToggleContainer.AddToClassList("binding-toggle-container");
                                
                                ACC_BindingData accBindingData = bindingsToggleValues.Keys.FirstOrDefault(b => b.id == binding.id.ToString() && b.controlScheme == controlScheme);
                                if (accBindingData != null)
                                {
                                    currentBindingsToggleValues[accBindingData] = bindingsToggleValues[accBindingData];
                                    var bindingToggle = new Toggle() { value = bindingsToggleValues[accBindingData] };
                                    bindingToggle.SetEnabled(controlSchemeToggleValues[controlScheme]);
                                    bindingToggle.AddToClassList("binding-toggle");
                                    bindingToggle.RegisterValueChangedCallback(evt =>
                                    {
                                        bindingsToggleValues[accBindingData] = evt.newValue;
                                    });

                                    bindingToggleContainer.Add(bindingToggle);
                                }

                                firstRow.Add(bindingLabel);
                                firstRow.Add(bindingToggleContainer);
                                mainContainer.Add(firstRow);
                                parent.Add(mainContainer);
                            }
                        }
                    }
                }
            }
        }
        
        private void ToggleControlSchemeDisplay(Button arrowButton, VisualElement controlScheme)
        {
            if (arrowButton.text == "\u25b6")
            {
                arrowButton.text = "\u25bc";
                for (int j = 1; j < controlScheme.childCount; j++)
                {
                    controlScheme[j].style.display = DisplayStyle.Flex;
                }
            }
            else
            {
                arrowButton.text = "\u25b6";
                for (int j = 1; j < controlScheme.childCount; j++)
                {
                    controlScheme[j].style.display = DisplayStyle.None;
                }
            }
        }
        
        private string MakePathPresentable(string path)
        {
            var parts = path.Split('/');
            var device = parts[0].Replace("<", "").Replace(">", "");
            var controlParts = parts[1].Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < controlParts.Length; i++)
            {
                controlParts[i] = char.ToUpper(controlParts[0][0]) + controlParts[0].Substring(1);
            }
            
            var control = string.Join(" ", controlParts);
            return $"{control} [{device}]";
        }

        private void ConfigureJSON()
        {
            ACC_ControlSchemeData accControlSchemeData = new ACC_ControlSchemeData();
            accControlSchemeData.name = inputActionAsset.name;
            accControlSchemeData.inputActionAsset = inputActionAsset;
            foreach (KeyValuePair<string, bool> item in controlSchemeToggleValues)
            {
                //accControlSchemeData.controlSchemesList.Add(new ACC_KeyValuePair<string, bool>(item.Key, item.Value));
            }

            foreach (KeyValuePair<ACC_BindingData, bool> binding in bindingsToggleValues)
            {
                //accControlSchemeData.bindingsList.Add(new ACC_KeyValuePair<ACC_BindingData, bool>(binding.Key, binding.Value));
            }
            
            ACC_JSONHelper.CreateJson(accControlSchemeData, "/ACC_JSONRemapControls/");
            lastSaveControlSchemeToggleValues = new Dictionary<string, bool>(controlSchemeToggleValues);
            lastSaveBindingsToggleValues = new Dictionary<ACC_BindingData, bool>(bindingsToggleValues);
            //ACC_AssetSaveProcessor.controlSchemesChanged.Find(x=>x.key == inputActionAsset.name).value = true;
        }
        
        private void LoadJson()
        {
            string path = "/ACC_JSONRemapControls/" + inputActionAsset.name;
            ACC_ControlSchemeData accControlSchemeData = ACC_JSONHelper.LoadJson<ACC_ControlSchemeData>(path);
            if(accControlSchemeData != null)
            {
                /*for (int i=0; i<inputActionAsset.controlSchemes.Count; i++)
                {
                    if (accControlSchemeData.controlSchemesList.Count > i)
                    {
                        if (accControlSchemeData.controlSchemesList[i].key != inputActionAsset.controlSchemes[i].name &&
                            inputActionAsset.controlSchemes.Skip(i).Any(controlsScheme =>
                                controlsScheme.name == accControlSchemeData.controlSchemesList[i].key))
                        {
                            accControlSchemeData.controlSchemesList.Remove(accControlSchemeData.controlSchemesList[i]);
                            accControlSchemeData.controlSchemesList.Add(accControlSchemeData.controlSchemesList[i]);
                            while (accControlSchemeData.controlSchemesList[i].key != inputActionAsset.controlSchemes[i].name)
                            {
                                accControlSchemeData.controlSchemesList.Insert(i, new ACC_KeyValuePairData<string, bool>(inputActionAsset.controlSchemes[i].name, false));
                                i++;
                            
                                if (accControlSchemeData.controlSchemesList.Count-1 == i)
                                {
                                    break;
                                }
                            
                                if (accControlSchemeData.controlSchemesList[i].key ==
                                    inputActionAsset.controlSchemes[i].name)
                                {
                                    break;
                                }
                            }
                        }
                        else if (accControlSchemeData.controlSchemesList[i].key != inputActionAsset.controlSchemes[i].name &&
                                 inputActionAsset.controlSchemes.Take(i).Any(controlsScheme => controlsScheme.name == accControlSchemeData.controlSchemesList[i].key))
                        {
                            accControlSchemeData.controlSchemesList.Remove(accControlSchemeData.controlSchemesList[i]);
                            accControlSchemeData.controlSchemesList.Add(accControlSchemeData.controlSchemesList[i]);
                            while (accControlSchemeData.controlSchemesList[i].key != inputActionAsset.controlSchemes[i].name)
                            {
                                accControlSchemeData.controlSchemesList.Insert(i, new ACC_KeyValuePairData<string, bool>(inputActionAsset.controlSchemes[i].name, false));
                                i--;
                            
                                if (accControlSchemeData.controlSchemesList.Count-1 == i)
                                {
                                    break;
                                }
                            
                                if (accControlSchemeData.controlSchemesList[i].key ==
                                    inputActionAsset.controlSchemes[i].name)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            accControlSchemeData.controlSchemesList[i].key = inputActionAsset.controlSchemes[i].name;
                        }
                    }
                    else
                    {
                        accControlSchemeData.controlSchemesList.Add(new ACC_KeyValuePairData<string, bool>(inputActionAsset.controlSchemes[i].name, false));
                    }
                }
                
                ACC_JSONHelper.CreateJson(accControlSchemeData, "/ACC_JSONRemapControls/");*/
                
                currentControlSchemeToggleValues = new Dictionary<string, bool>();
                /*foreach (var scheme in accControlSchemeData.controlSchemesList)
                {
                    if (controlSchemeToggleValues.ContainsKey(scheme.key))
                    {
                        controlSchemeToggleValues[scheme.key] = scheme.value;
                        currentControlSchemeToggleValues[scheme.key] = scheme.value;
                    }
                }*/
                
                bindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
                currentBindingsToggleValues = new Dictionary<ACC_BindingData, bool>();
                /*foreach (var binding in accControlSchemeData.bindingsList)
                {
                    bindingsToggleValues[binding.key] = binding.value;
                    currentBindingsToggleValues[binding.key] = binding.value;
                }*/
                
                lastSaveControlSchemeToggleValues = new Dictionary<string, bool>(controlSchemeToggleValues);
                lastSaveBindingsToggleValues = new Dictionary<ACC_BindingData, bool>(bindingsToggleValues);
                CreateTable();
            }
        }
        
        private bool IsThereAnyChange()
        {
            return !controlSchemeToggleValues.SequenceEqual(lastSaveControlSchemeToggleValues) ||
                   !bindingsToggleValues.SequenceEqual(lastSaveBindingsToggleValues);
        }

        private void Cancel()
        {
            var window = Instantiate(this);
            window.titleContent = new GUIContent("Control Schemes Configuration");
            window.minSize = new Vector2(600, 450);
            window.maxSize = new Vector2(600, 450);
            window.inputActionAsset = inputActionAsset;
            window.isReadyToCreateGUI = true;
            window.CreateGUI();
            
           
            window.controlSchemeToggleValues = controlSchemeToggleValues;
            window.currentControlSchemeToggleValues = controlSchemeToggleValues;
            window.bindingsToggleValues = bindingsToggleValues;
            window.currentBindingsToggleValues = bindingsToggleValues;
            window.CreateTable();
            
            window.Show();
        }
        
        private void ConfirmSaveChangesIfNeeded()
        {
            if (IsThereAnyChange())
            {
                var result = EditorUtility.DisplayDialogComplex("Control Schemes Configuration has been modified",
                    $"Do you want to save the changes you made in:\n./JSONRemapControls/{inputActionAsset.name}.json\n\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
                switch (result)
                {
                    case 0:
                        ConfigureJSON();
                        break;
                    case 1:
                        Cancel();
                        break;
                    case 2:
                        break;
                }
            }
        }
    }
}