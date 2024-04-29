#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using TFG_Videojocs;
using TFG_Videojocs.ACC_HighContrast;
using TFG_Videojocs.ACC_HighContrast.Toilet;
using TFG_Videojocs.ACC_RemapControls;
using TFG_Videojocs.ACC_Subtitles;
using TFG_Videojocs.ACC_Utilities;
using TFG_Videojocs.ACC_VisualNotification;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;
using UnityEngine.UIElements;

public class ACC_MainWindow : EditorWindow
{
    private VisualElement accessibilityContainer;
    private const int numberOfColumns = 2;

    private bool isWindowOpen = false;

    private ACC_AudioManager audioManager;
    private ScrollView soundContainer;

    private InputActionAsset inputActionAsset;
    private ObjectField inputAction;
    
    private void OnEnable()
    {
        ACC_SubtitlesEditorWindow.OnCloseWindow += RefreshDropdown<ACC_SubtitleData>;
        ACC_VisualNotificationEditorWindow.OnCloseWindow += RefreshDropdown<ACC_VisualNotificationData>;
        ACC_HighContrastEditorWindow.OnCloseWindow += RefreshDropdown<ACC_HighContrastData>;
        ACC_HighContrastEditorWindow.OnCloseWindow += RefreshHighContrastSettings;
    }
    private void OnDisable()
    {
        ACC_SubtitlesEditorWindow.OnCloseWindow -= RefreshDropdown<ACC_SubtitleData>;
        ACC_VisualNotificationEditorWindow.OnCloseWindow -= RefreshDropdown<ACC_VisualNotificationData>;
        ACC_HighContrastEditorWindow.OnCloseWindow -= RefreshDropdown<ACC_HighContrastData>;
    }

    [MenuItem("Tools/ACC/Accessibility Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_MainWindow>("Accessibility Window");
        window.minSize = new Vector2(520, 300);
    }
    public void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Core/ACC_WindowStyles.uss");
        var toolbar = new Toolbar();
        var titleToolbar = new Label("Accessibility Plugin");
        var mainContainer = new VisualElement();
        var sidebar = new VisualElement();
        var audibleButton = new Button() { text = "Auditive" };
        var visualButton = new Button() { text = "Visual" };
        var cognitiveButton = new Button() { text = "Mobility" };
        var multifunctionalButton = new Button() { text = "Multifunctional" };
        accessibilityContainer = new VisualElement();
        var accessibilityRow = new VisualElement();
        
        toolbar.AddToClassList("toolbar");
        titleToolbar.AddToClassList("toolbar-title");
        mainContainer.AddToClassList("main-container");
        sidebar.AddToClassList("my-box");
        audibleButton.AddToClassList("my-button");
        visualButton.AddToClassList("my-button");
        cognitiveButton.AddToClassList("my-button");
        multifunctionalButton.AddToClassList("my-button");
        accessibilityContainer.AddToClassList("accessibility-container");
        accessibilityRow.AddToClassList("accessibility-row");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.style.minWidth = new StyleLength(520);
        
        toolbar.Add(titleToolbar);
        
        audibleButton.clicked += () => { UpdateAccessibilityContainer(typeof(AudioFeatures));};
        audibleButton.clicked += () => { UpdateAccessibilityContainer(typeof(AudioFeatures));};
        visualButton.clicked += () => { UpdateAccessibilityContainer(typeof(VisibilityFeatures)); };
        cognitiveButton.clicked += () => { UpdateAccessibilityContainer(typeof(MobilityFeatures)); };
        multifunctionalButton.clicked += () => { UpdateAccessibilityContainer(typeof(MultifiunctionalFeatures)); };
        
        sidebar.Add(audibleButton);
        sidebar.Add(visualButton);
        sidebar.Add(cognitiveButton);
        sidebar.Add(multifunctionalButton);
        mainContainer.Add(sidebar);
        mainContainer.Add(accessibilityContainer);
        rootVisualElement.Add(toolbar);
        rootVisualElement.Add(mainContainer);
    }
    
    private void UpdateAccessibilityContainer(Type featureType)
    {
        accessibilityContainer.Clear();
        for (int i = 0; i < Enum.GetNames(featureType).Length; i++)
        {
            accessibilityContainer.Add(CreateFeatureBox(featureType, i));
        }
    }
    private VisualElement CreateFeatureBox(Type featuretype, int index)
    {
        var box = new VisualElement();
        box.AddToClassList("red-box");
        box.style.width = new Length(92f/numberOfColumns, LengthUnit.Percent);

        var titleBox = new Label(Enum.GetName(featuretype, index));
        titleBox.AddToClassList("title-box");
        box.Add(titleBox);

        if(featuretype==typeof(AudioFeatures))CreateAudioBox(box, index);
        else if (featuretype==typeof(VisibilityFeatures))CreateVisibilityBox(box, index);
        else if (featuretype==typeof(MobilityFeatures))CreateMobilityBox(box, index);
        else if (featuretype==typeof(MultifiunctionalFeatures))CreateMultifunctionalBox(box, index);
        return box;
    }
    private void CreateAudioBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(AudioFeatures), index))
        {
            case "Subtitles":
                DefaultBox(box, 
                    new List<string> {"Create A Subtitle", "Edit Subtitle", "Edit Prefab" }, 
                    () => DefaultCreateAction("Create", ACC_SubtitlesEditorWindow.ShowWindow),
                    () => DefaultLoadAction<ACC_SubtitlesEditorWindow, ACC_SubtitlesEditorWindowController, ACC_SubtitleData>(
                        "ACC_Subtitles/", "Select a subtitle:", ACC_SubtitlesEditorWindow.ShowWindow),
                    prefabName: "Subtitles");
                break;
            case "VisualNotification":
                DefaultBox(box, 
                    new List<string> {"Create a visual notification", "Edit visual notification", "Edit Prefab" }, 
                    () => DefaultCreateAction("Create", ACC_VisualNotificationEditorWindow.ShowWindow),
                    () => DefaultLoadAction<ACC_VisualNotificationEditorWindow, ACC_VisualNotificationEditorWindowController, ACC_VisualNotificationData>(
                        "ACC_VisualNotification/", "Select a visual notification:", ACC_VisualNotificationEditorWindow.ShowWindow),
                    prefabName: "VisualNotification");
                break;
        }
    }
    private void CreateVisibilityBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(VisibilityFeatures), index))
        {
            case "TextToVoice":
                break;
            case "HighContrast":
                DefaultBox( box, 
                    new List<string> {"Create a high-contrast configuration", "Edit high-contrast configuration", "Settings"}, 
                    () => DefaultCreateAction("Create", ACC_HighContrastEditorWindow.ShowWindow), 
                    () => DefaultLoadAction<ACC_HighContrastEditorWindow, ACC_HighContrastEditorWindowController, ACC_HighContrastData>(
                        "ACC_HighContrast/", "Select a configuration:", ACC_HighContrastEditorWindow.ShowWindow),
                    HighContrastSettings);
                break;
        }
    }
    private void CreateMobilityBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(MobilityFeatures), index))
        {
            case "RemapControls":
                RemapControlsBox(box);
                break;
        }
    }
    private void CreateMultifunctionalBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(MultifiunctionalFeatures), index))
        {
            case "AudioManager":
                AudioManagerBox(box);
                break;
        }
    }
    
    private VisualElement HighContrastSettings()
    {
        ACC_HighContrastEditorWindow window = (ACC_HighContrastEditorWindow)GetWindow(typeof(ACC_HighContrastEditorWindow), false);
        isWindowOpen = window != null;
        if (isWindowOpen
            && !window.controller.isCreatingNewFileOnCreation
            && !window.controller.isEditing)
        {
            isWindowOpen = false;
            window.Close();
        }
        
        var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
        
        var container = new VisualElement();
        var addShadersToggle = new Toggle("Shaders Added: ");
        addShadersToggle.AddToClassList("object-field");
        addShadersToggle[0].AddToClassList("object-label");
        addShadersToggle.SetEnabled(!isWindowOpen);
        addShadersToggle.value = accessibilityManager.shadersAdded;
        
        var previsualizeToggle = new Toggle("Previsualize: ");
        previsualizeToggle.AddToClassList("object-field");
        previsualizeToggle[0].AddToClassList("object-label");
        previsualizeToggle.SetEnabled(!isWindowOpen);
        previsualizeToggle.style.display = accessibilityManager.shadersAdded ? DisplayStyle.Flex : DisplayStyle.None;
        previsualizeToggle.value = accessibilityManager.isPrevisualizing;
        
        var refreshButton = new Button() { text = "Refresh" };
        refreshButton.AddToClassList("create-button");
        refreshButton.SetEnabled(!isWindowOpen);
        refreshButton.style.display = accessibilityManager.shadersAdded ? DisplayStyle.Flex : DisplayStyle.None;
        
        addShadersToggle.RegisterValueChangedCallback(evt =>
        {
            accessibilityManager.shadersAdded = evt.newValue;
            EditorUtility.SetDirty(accessibilityManager);
            if (evt.newValue)
            {
                previsualizeToggle.style.display = DisplayStyle.Flex;
                refreshButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (accessibilityManager.isPrevisualizing) accessibilityManager.StopPrevisualize();
                previsualizeToggle.style.display = DisplayStyle.None;
                refreshButton.style.display = DisplayStyle.None;
            }
        });
        
        previsualizeToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue) accessibilityManager.Previsualize();
            else accessibilityManager.StopPrevisualize();
        });
        
        refreshButton.clicked += () =>
        {
            accessibilityManager.shadersAdded = false;
            accessibilityManager.OnHierarchyChanged();
            accessibilityManager.shadersAdded = true;
            accessibilityManager.OnHierarchyChanged();

            if (accessibilityManager.isPrevisualizing)
            {
                accessibilityManager.StopPrevisualize();
                accessibilityManager.Previsualize();
            }
        };
        
        container.Add(addShadersToggle);
        container.Add(previsualizeToggle);
        container.Add(refreshButton);
        return container;
    }
    private void RefreshHighContrastSettings(string directory)
    {
        var dropdownField = rootVisualElement.Query<DropdownField>().ToList().Find(dropdownField => dropdownField.value == "Settings");
        if(dropdownField != null && isWindowOpen)
        {
            foreach (var child in dropdownField.parent[2][0].Children())
            {
                child.SetEnabled(true);
            }
        }
    }
    private void RemapControlsBox(VisualElement box)
    {
        inputAction = new ObjectField("Select an input action: ")
        {
            objectType = typeof(InputActionAsset),
            allowSceneObjects = false
        };

        inputAction.AddToClassList("object-field");
        inputAction[0].AddToClassList("object-label");

        var dynamicControlSchemesContainer = new VisualElement();
        
        var createActionsButton = new Button(){text = "Create Actions..."};
        createActionsButton.AddToClassList("create-button");
        dynamicControlSchemesContainer.Add(createActionsButton);
        createActionsButton.clicked += CreateInputActionAsset;

        var remapControlsButtonContainer = new VisualElement();
        remapControlsButtonContainer.AddToClassList("button-container");
        
        var editInputActionAssetButton = new Button() { text = "Edit Inputs" };
        editInputActionAssetButton.AddToClassList("create-button");
        
        remapControlsButtonContainer.Add(editInputActionAssetButton);
        
        Assembly inputSystemEditorAssembly = null;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetTypes().Any(type => type.Namespace?.StartsWith("UnityEngine.InputSystem.Editor") == true))
            {
                inputSystemEditorAssembly = assembly;
                break;
            }
        }

        if (inputSystemEditorAssembly != null)
        {
            var inputActionEditorWindowType = inputSystemEditorAssembly.GetType("UnityEngine.InputSystem.Editor.InputActionEditorWindow");
        
            editInputActionAssetButton.clicked += () =>
            {
                if (inputActionEditorWindowType != null)
                {
                    var openEditorMethod = inputActionEditorWindowType.GetMethod("OpenEditor",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (openEditorMethod != null)
                    {
                        try
                        {
                            openEditorMethod.Invoke(null, new object[] { inputActionAsset });
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            };
        }
        
        var configureControlSchemesButton = new Button() { text = "Configure Schemes" };
        configureControlSchemesButton.AddToClassList("create-button");
        configureControlSchemesButton.clicked += () =>
        {
            ACC_ControlSchemesConfigurationEditorWindow.ShowWindow(inputActionAsset);
        };
        
        remapControlsButtonContainer.Add(configureControlSchemesButton);
        
        var loadPrefabContainer = new VisualElement();
        var loadPrefabButton = new Button() { text = "Edit Prefab" };
        loadPrefabButton.AddToClassList("create-button");
        loadPrefabButton.clicked += () => { LoadPrefab("RemapControls", inputActionAsset.name); };
                
        loadPrefabContainer.Add(loadPrefabButton);
        remapControlsButtonContainer.Add(loadPrefabContainer);

        inputAction.RegisterValueChangedCallback(evt =>
        {
            inputActionAsset = evt.newValue as InputActionAsset;
            if(inputActionAsset!=null) ACC_PrefabHelper.CreatePrefab("RemapControls", inputActionAsset.name);
            dynamicControlSchemesContainer.Clear();
            if (inputActionAsset != null)
            {
                dynamicControlSchemesContainer.Add(remapControlsButtonContainer);
                string json = inputActionAsset.ToJson();
                File.WriteAllText("Assets/a.json", json);
                AssetDatabase.Refresh();
            }
            else
            {
                dynamicControlSchemesContainer.Add(createActionsButton);
            }
        });

        box.Add(inputAction);
        box.Add(dynamicControlSchemesContainer);
    }
    private void CreateInputActionAsset()
    {
        string GenerateInputActionsJson()
        {
            return @"
            {
                ""name"": ""NewInputActions"",
                ""maps"": []
            }
            ";
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Create Input Actions Asset",
            "NewInputActions",
            "inputactions",
            "Please enter a file name to save the input action asset to"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, GenerateInputActionsJson());
            AssetDatabase.Refresh();

            inputAction.value = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
        }
    }
    private void AudioManagerBox(VisualElement box)
    {
        box.Add(LoadAudioManager());
    }
    private VisualElement LoadAudioManager()
    {
        var editContainer = new VisualElement();
        var editButton = new Button() { text = "Edit" };
        editButton.AddToClassList("create-button");
        editButton.clicked+= () => { ACC_AudioManagerEditorWindow.ShowWindow(); };
        
        editContainer.Add(editButton);
        
        return editContainer;
    }

    #region HelperMethods
    private void DefaultBox(VisualElement box, List<string> options, Func<VisualElement> createAction, Func<VisualElement> loadAction, Func<VisualElement> extraAction = null, string prefabName = "")
    {
        var dynamicContainer = new VisualElement();
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-label");
        
        dynamicContainer.Add(createAction());
        
        box.Add(dropdown);
        box.Add(dynamicContainer);
                
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == options[0])
            {
                dynamicContainer.Add(createAction());
            }
            else if (evt.newValue == options[1])
            {
                var selection = loadAction.Invoke();
                dynamicContainer.Add(selection);
            }
            else
            {
                if (extraAction != null) dynamicContainer.Add(extraAction.Invoke());
                else
                {
                    var loadPrefabContainer = new VisualElement();
                    var loadPrefabButton = new Button() { text = "Edit" };
                    loadPrefabButton.AddToClassList("create-button");
                    loadPrefabButton.clicked += () => { LoadPrefab(prefabName); };

                    loadPrefabContainer.Add(loadPrefabButton);
                    dynamicContainer.Add(loadPrefabContainer);
                }
            }
        });
    }
    private VisualElement DefaultCreateAction(string buttonText, Action<string> action)
    {
        var container = new VisualElement();

        var button = new Button() { text = buttonText };
        button.AddToClassList("create-button");
        container.Add(button);
    
        button.clicked += () =>
        {
            action.Invoke(null);
        };

        return container;
    }
    private VisualElement DefaultLoadAction<TWindow, TController, TData>(string directory, string dropdownLabel, Action<string> action) 
        where TWindow : EditorWindow where TController : ACC_FloatingWindowController<TWindow, TData>, new() where TData : ACC_AbstractData, new()
    {
        var selectContainer = new VisualElement();

        var options = ACC_JSONHelper.GetFilesListByParam<TData, string>(directory, data => data.name);
        
        var dropdown = new DropdownField(dropdownLabel, options, 0);
        dropdown.name = directory;
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-label");

        var editBottomContainer = new VisualElement();
        editBottomContainer.AddToClassList("button-container");
        
        var loadButton = new Button() { text = "Load" };
        loadButton.AddToClassList("button");
        loadButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(dropdown.value)) action.Invoke(dropdown.value);
            else EditorUtility.DisplayDialog("Required Field", "Please select an existing file to load.", "OK");
        };

        var deleteButton = new Button() { text = "Delete" };
        deleteButton.AddToClassList("button");
        deleteButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(dropdown.value))
            {
                ACC_BaseFloatingWindow<TController, TWindow, TData>.CloseWindowIfExists<TWindow>();
                ACC_JSONHelper.DeleteFile(directory, dropdown.value);
                RefreshDropdown<TData>(directory);
            }
            else EditorUtility.DisplayDialog("Required Field", "Please select an existing file to load.", "OK");
        };
        
        editBottomContainer.Add(loadButton);
        editBottomContainer.Add(deleteButton);
        
        selectContainer.Add(dropdown);
        selectContainer.Add(editBottomContainer);
        
        return selectContainer;
    }
    private void RefreshDropdown<TData>(string directory) where TData : ACC_AbstractData, new()
    {
        DropdownField dropdown = rootVisualElement.Q<DropdownField>(name: directory);
        
        if (dropdown != null)
        {
            var options = ACC_JSONHelper.GetFilesListByParam<TData, string>(directory, data => data.name);
            dropdown.choices = options;
            dropdown.value = options.Count > 0 ? options[0] : "";
            Repaint();
        }
    }
    private void LoadPrefab(string feature, string jsonFile="")
    {
        var folder = "ACC_ " + feature + "/";
        var name = "ACC_" + feature + "Manager.prefab";
        var prefabPath = "Assets/Resources/ACC_Prefabs/" + folder + name;
        
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefabAsset != null)
        {
            PrefabStageUtility.OpenPrefab(prefabPath);
        }
        else
        {
            EditorUtility.DisplayDialog("Prefab not found", "The prefab for the " + feature + " manager was not found.", "OK");
        }
    }
    #endregion
}
#endif