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
        private bool isSaved = true;
        private bool forceQuit = false;
        private bool userCanChange = false;
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
            
            devicesDropdown.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == "All devices")
                {
                    controlSchemes = inputActionAsset.controlSchemes.Select(scheme => scheme.name).ToList();
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
                }
                controlSchemesScrollView.Add(CreateTable(controlSchemes));
                
                for (int i = 1; i < controlSchemesContainer.childCount; i++)
                {
                    userCanChange = false;
                    var row = controlSchemesContainer[i];
                    var controlSchemeLabel = row.Query<Label>().First();
                    var canRebind = row.Query<Toggle>().First();
                    if (controlSchemeToggleValues.TryGetValue(controlSchemeLabel.text, out bool value))
                    {
                        canRebind.value = value;
                    }
                }
            });
            
            controlSchemesScrollView = new ScrollView();
            controlSchemesScrollView.AddToClassList("control-schemes-scroll-view");
            controlSchemesScrollView.Add(CreateTable(controlSchemes));
            
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
            
            LoadJson();
        }

        private VisualElement CreateTable(List<string> controlSchemes)
        {
            if(controlSchemesContainer==null)controlSchemesContainer = new VisualElement();
            else controlSchemesContainer.Clear();
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
            
            for (int i=0; i<controlSchemes.Count; i++)
            {
                var controlScheme = new VisualElement();
                controlScheme.AddToClassList("control-scheme");
                if (i == controlSchemes.Count - 1)
                {
                    controlScheme.AddToClassList("control-scheme-last");
                }
                
                var controlSchemeLabel = new Label(controlSchemes[i]);
                controlSchemeLabel.AddToClassList("control-scheme-label");

                var controlSchemeToggleContainer = new VisualElement();
                controlSchemeToggleContainer.AddToClassList("control-scheme-toggle-container");
                
                var controlSchemeToggle = new Toggle();
                controlSchemeToggle.AddToClassList("control-scheme-toggle");
                controlSchemeToggle.RegisterValueChangedCallback(evt =>
                {
                    if (!userCanChange) userCanChange = true;
                    else if (userCanChange)
                    {
                        isSaved = false;
                        controlSchemeToggleValues[controlSchemeLabel.text] = evt.newValue;
                    }
                });
                
                controlSchemeToggleContainer.Add(controlSchemeToggle);
                
                controlScheme.Add(controlSchemeLabel);
                controlScheme.Add(controlSchemeToggleContainer);
                controlSchemesContainer.Add(controlScheme);
            }
            return controlSchemesContainer;
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
            isSaved = true;
        }
        
        private void LoadJson()
        {
            string path = "/ACC_JSONRemapControls/" + inputActionAsset.name;
            ACC_ControlSchemeData accControlSchemeData = ACC_JSONHelper.LoadJson<ACC_ControlSchemeData>(path);
            if(accControlSchemeData != null)
            {
                foreach (var scheme in accControlSchemeData.controlSchemesList)
                {
                    if (controlSchemeToggleValues.ContainsKey(scheme.key))
                    {
                        controlSchemeToggleValues[scheme.key] = scheme.value;
                    }
                }

                for (int i = 1; i < controlSchemesContainer.childCount; i++)
                {
                    userCanChange = false;
                    var row = controlSchemesContainer[i];
                    var controlSchemeLabel = row.Query<Label>().First();
                    
                    if (controlSchemeToggleValues.TryGetValue(controlSchemeLabel.text, out bool value))
                    {
                        var canRebind = row.Query<Toggle>().First();
                        canRebind.value = value;
                    }
                }
            }
        }
        
        /*private bool EditorWantsToQuit()
        {
            return ConfirmSaveChangesIfNeeded();
        }*/
        
        private void ConfirmSaveChangesIfNeeded()
        {
            if (/*!forceQuit &&*/ !isSaved)
            {
                var result = EditorUtility.DisplayDialogComplex("Control Schemes Configuration has been modified",
                    $"Do you want to save the changes you made in:\n./JSONRemapControls/{inputActionAsset.name}.json\n\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
                switch (result)
                {
                    case 0:
                        ConfigureJSON();
                        break;
                    case 1:
                        var window = Instantiate(this);
                        window.titleContent = new GUIContent("Control Schemes Configuration");
                        window.minSize = new Vector2(600, 450);
                        window.maxSize = new Vector2(600, 450);
                        window.inputActionAsset = inputActionAsset;
                        window.isReadyToCreateGUI = true;
                        window.CreateGUI();
                        window.Show();
                        break;
                    case 2:
                        //forceQuit = true;
                        break;
                }
            }
        }
    }
}