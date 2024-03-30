using System;
using System.Collections.Generic;
using System.Linq;
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
        
        //private bool isSaved = true;
        //private bool forceQuit = false;
        private void OnEnable()
        {
            //EditorApplication.wantsToQuit += EditorWantsToQuit;
        }

        private void OnDestroy()
        {
            ConfirmSaveChangesIfNeeded();
            //EditorApplication.wantsToQuit -= EditorWantsToQuit;
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
                .Select(scheme => scheme.deviceRequirements
                    .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                    .Distinct()
                    .OrderBy(device => device)
                    .Aggregate((current, next) => current + ", " + next))
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
                    controlSchemes.ForEach(key => currentControlSchemeToggleValues[key] = controlSchemeToggleValues[key]);
                }
                CreateTable();
            });
            
            controlSchemesScrollView = new ScrollView();
            controlSchemesScrollView.AddToClassList("control-schemes-scroll-view");
            CreateTable();
            
            var createButton = new Button() { text = "Save" };
            createButton.AddToClassList("create-button");
            createButton.clicked += ConfigureJSON;
            
            rootVisualElement.Add(controlSchemesLabel);
            rootVisualElement.Add(devicesDropdown);
            rootVisualElement.Add(controlSchemesScrollView);
            rootVisualElement.Add(createButton);
            
            for (int i = 1; i < controlSchemesContainer.childCount; i++)
            {
                var row = controlSchemesContainer[i];
                var controlSchemeLabel = row.Query<Label>().First();
                controlSchemeToggleValues[controlSchemeLabel.text] = false;
            }
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
                if (i == currentControlSchemeToggleValues.Count - 1)
                {
                    controlScheme.AddToClassList("control-scheme-last");
                }
                
                var controlSchemeLabel = new Label(currentControlSchemeToggleValues.Keys.ToList()[i]);
                controlSchemeLabel.AddToClassList("control-scheme-label");

                var controlSchemeToggleContainer = new VisualElement();
                controlSchemeToggleContainer.AddToClassList("control-scheme-toggle-container");
                
                var controlSchemeToggle = new Toggle(){value = currentControlSchemeToggleValues[controlSchemeLabel.text]};
                controlSchemeToggle.AddToClassList("control-scheme-toggle");
                controlSchemeToggle.RegisterValueChangedCallback(evt =>
                {
                    controlSchemeToggleValues[controlSchemeLabel.text] = evt.newValue;
                });
                
                controlSchemeToggleContainer.Add(controlSchemeToggle);
                
                controlScheme.Add(controlSchemeLabel);
                controlScheme.Add(controlSchemeToggleContainer);
                controlSchemesContainer.Add(controlScheme);
            }
            controlSchemesScrollView.Add(controlSchemesContainer);
        }

        private void ConfigureJSON()
        {
            ACC_ControlSchemeData accControlSchemeData = new ACC_ControlSchemeData();
            accControlSchemeData.name = inputActionAsset.name;
            accControlSchemeData.inputActionAsset = inputActionAsset;
            foreach (KeyValuePair<string, bool> item in controlSchemeToggleValues)
            {
                accControlSchemeData.controlSchemesList.Add(new ACC_KeyValuePairData<string, bool>(item.Key, item.Value));
                ACC_JSONHelper.CreateJson(accControlSchemeData, "/ACC_JSONRemapControls/");
            }
            lastSaveControlSchemeToggleValues = new Dictionary<string, bool>(controlSchemeToggleValues);        
        }
        
        private void LoadJson()
        {
            string path = "/ACC_JSONRemapControls/" + inputActionAsset.name;
            ACC_ControlSchemeData accControlSchemeData = ACC_JSONHelper.LoadJson<ACC_ControlSchemeData>(path);
            if(accControlSchemeData != null)
            {
                currentControlSchemeToggleValues = new Dictionary<string, bool>();
                foreach (var scheme in accControlSchemeData.controlSchemesList)
                {
                    if (controlSchemeToggleValues.ContainsKey(scheme.key))
                    {
                        controlSchemeToggleValues[scheme.key] = scheme.value;
                        currentControlSchemeToggleValues[scheme.key] = scheme.value;
                    }
                }
                lastSaveControlSchemeToggleValues = new Dictionary<string, bool>(controlSchemeToggleValues);
                CreateTable();
            }
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
            
            window.currentControlSchemeToggleValues = controlSchemeToggleValues;
            window.controlSchemeToggleValues = controlSchemeToggleValues;
            window.CreateTable();
            
            window.Show();
        }
        
        /*private bool EditorWantsToQuit()
        {
            return ConfirmSaveChangesIfNeeded();
        }*/
        
        private void ConfirmSaveChangesIfNeeded()
        {
            if (/*!forceQuit &&*/ !controlSchemeToggleValues.SequenceEqual(lastSaveControlSchemeToggleValues))
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
                        //forceQuit = true;
                        break;
                }
            }
        }
    }
}