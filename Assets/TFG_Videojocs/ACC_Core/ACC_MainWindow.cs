#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TFG_Videojocs;
using TFG_Videojocs.ACC_HighContrast;
using TFG_Videojocs.ACC_RemapControls;
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
    private DropdownField subtitlesDropdown;

    private ACC_AudioManager audioManager;
    private ScrollView soundContainer;
    private DropdownField visualNotificationDropdown;

    private InputActionAsset inputActionAsset;
    private ObjectField inputAction;
    
    private void OnEnable()
    {
        ACC_SubtitlesEditorWindow.OnCloseSubtitleWindow += RefreshSubtititleWindow;
        ACC_VisualNotificationEditorWindow.OnCloseVisualNotificationWindow += RefreshVisualNotification;
    }

    private void OnDisable()
    {
        ACC_SubtitlesEditorWindow.OnCloseSubtitleWindow -= RefreshSubtititleWindow;
        ACC_VisualNotificationEditorWindow.OnCloseVisualNotificationWindow -= RefreshVisualNotification;
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
        visualButton.clicked += () => { UpdateAccessibilityContainer(typeof(VisualFeatures)); };
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
        else if (featuretype==typeof(VisualFeatures))CreateVisibilityBox(box, index);
        else if (featuretype==typeof(MobilityFeatures))CreateMobilityBox(box, index);
        else if (featuretype==typeof(MultifiunctionalFeatures))CreateMultifunctionalBox(box, index);
        return box;
    }

    private void CreateAudioBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(AudioFeatures), index))
        {
            case "Subtitles":
                SubtitlesBox(box);
                break;
            case "VisualNotification":
                VisualContainerBox(box);
                break;
        }
    }
    private void CreateVisibilityBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(VisualFeatures), index))
        {
            case "TextToVoice":
                break;
            case "HighContrast":
                HighContrastBox(box);
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

    private void SubtitlesBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        var options = new List<string> { "Create A Subtitle", "Edit Subtitle", "Edit Prefab" };
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-label");
        
        var subtitleCreation = CreateASubtitle();
        dynamicContainer.Add(subtitleCreation);
                
        box.Add(dropdown);
        box.Add(dynamicContainer);
                
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == "Create A Subtitle")
            {
                dynamicContainer.Add(subtitleCreation);
            }
            else if (evt.newValue == "Edit Subtitle")
            {
                var subtitleSelection = LoadSubtitle();
                dynamicContainer.Add(subtitleSelection);
            }
            else
            {
                var loadPrefabContainer = new VisualElement();
                var loadPrefabButton = new Button() { text = "Edit" };
                loadPrefabButton.AddToClassList("create-button");
                loadPrefabButton.clicked += () => { LoadPrefab("Subtitles"); };
                
                loadPrefabContainer.Add(loadPrefabButton);
                dynamicContainer.Add(loadPrefabContainer);
            }
        });
    }
    private VisualElement CreateASubtitle()
    {
        var subtitleCreationContainer = new VisualElement();

        var createSubtitlesButton = new Button() { text = "Create" };
        createSubtitlesButton.AddToClassList("create-button");
        subtitleCreationContainer.Add(createSubtitlesButton);
        
        createSubtitlesButton.clicked += () =>
        {
            ACC_SubtitlesEditorWindow.ShowWindow(null);
        };

        return subtitleCreationContainer;
    }
    private VisualElement LoadSubtitle()
    {
        var selectSubtitleContainer = new VisualElement();

        var options = ACC_JSONHelper.GetFilesListByParam<ACC_SubtitleData, string>("/ACC_Subtitles/", data => data.name);
        
        subtitlesDropdown = new DropdownField("Select a subtitle:", options, 0);
        subtitlesDropdown.AddToClassList("dropdown-container");
        subtitlesDropdown[0].AddToClassList("dropdown-label");

        var editSubtitleBottomContainer = new VisualElement();
        editSubtitleBottomContainer.AddToClassList("button-container");
        
        var loadSubtitlesButton = new Button() { text = "Load" };
        loadSubtitlesButton.AddToClassList("button");
        loadSubtitlesButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(subtitlesDropdown.value)) ACC_SubtitlesEditorWindow.ShowWindow(subtitlesDropdown.value);
            else EditorUtility.DisplayDialog("Required Field", "Please select a subtitle to load.", "OK");
        };

        var deleteSubtitleButton = new Button() { text = "Delete" };
        deleteSubtitleButton.AddToClassList("button");
        deleteSubtitleButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(subtitlesDropdown.value))
            {
                ACC_JSONHelper.DeleteFile("/ACC_Subtitles/" + subtitlesDropdown.value);
                RefreshSubtititleWindow();
            }
            else EditorUtility.DisplayDialog("Required Field", "Please select a subtitle to delete.", "OK");
        };
        
        editSubtitleBottomContainer.Add(loadSubtitlesButton);
        editSubtitleBottomContainer.Add(deleteSubtitleButton);
        
        selectSubtitleContainer.Add(subtitlesDropdown);
        selectSubtitleContainer.Add(editSubtitleBottomContainer);
        
        return selectSubtitleContainer;
    }
    private void RefreshSubtititleWindow()
    {
        if (subtitlesDropdown != null)
        {
            var options = ACC_JSONHelper.GetFilesListByParam<ACC_SubtitleData, string>("/ACC_Subtitles/", data => data.name);
            subtitlesDropdown.choices = options;
            subtitlesDropdown.value = options.Count > 0 ? options[0] : "";
        }
        Repaint();
    }

    private void VisualContainerBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        
        var options = new List<string> { "Create a visual notification", "Edit visual notification", "Edit Prefab" };
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-label");
        
        var addVisualNotificationButton = CreateAVisualNotification();
        dynamicContainer.Add(addVisualNotificationButton);
        
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == "Create a visual notification")
            {
                dynamicContainer.Add(addVisualNotificationButton);
            }
            else if (evt.newValue == "Edit visual notification")
            {
                var visualNotificationSelection = LoadVisualNotification();
                dynamicContainer.Add(visualNotificationSelection);
            }
            else
            {
                var loadPrefabContainer = new VisualElement();
                var loadPrefabButton = new Button() { text = "Edit" };
                loadPrefabButton.AddToClassList("create-button");
                loadPrefabButton.clicked += () => { LoadPrefab("VisualNotification"); };
                
                loadPrefabContainer.Add(loadPrefabButton);
                dynamicContainer.Add(loadPrefabContainer);
            }
        });
        
        box.Add(dropdown);
        box.Add(dynamicContainer);
    }
    private VisualElement CreateAVisualNotification()
    {
        var visualNotificationContainer = new VisualElement();

        var createVisualNotificationButton = new Button() { text = "Create" };
        createVisualNotificationButton.AddToClassList("create-button");
        visualNotificationContainer.Add(createVisualNotificationButton);
        
        createVisualNotificationButton.clicked += () =>
        {
            ACC_VisualNotificationEditorWindow.ShowWindow(null);
        };

        return visualNotificationContainer;
    }
    private VisualElement LoadVisualNotification()
    {
        var selectSubtitleContainer = new VisualElement();

        var options = ACC_JSONHelper.GetFilesListByParam<ACC_VisualNotificationData, string>("/ACC_VisualNotification/", data => data.name);
        
        visualNotificationDropdown = new DropdownField("Select a visual notification:", options, 0);
        visualNotificationDropdown.AddToClassList("dropdown-container");
        visualNotificationDropdown[0].AddToClassList("dropdown-label");

        var editSubtitleBottomContainer = new VisualElement();
        editSubtitleBottomContainer.AddToClassList("button-container");
        
        var loadSubtitlesButton = new Button() { text = "Load" };
        loadSubtitlesButton.AddToClassList("button");
        loadSubtitlesButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(visualNotificationDropdown.value))
            {
                var sounds = ACC_JSONHelper.GetParamByFileName<ACC_VisualNotificationData, List<ACC_Sound>>(data => data.soundsList,
                    "/ACC_VisualNotification/", visualNotificationDropdown.value);
                ACC_VisualNotificationEditorWindow.ShowWindow(visualNotificationDropdown.value);
            }
            else EditorUtility.DisplayDialog("Required Field", "Please select a visual notification to load.", "OK");
        };

        var deleteSubtitleButton = new Button() { text = "Delete" };
        deleteSubtitleButton.AddToClassList("button");
        deleteSubtitleButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(visualNotificationDropdown.value))
            {
                ACC_JSONHelper.DeleteFile("/ACC_VisualNotification/" + visualNotificationDropdown.value);
                RefreshVisualNotification();
            }
            else EditorUtility.DisplayDialog("Required Field", "Please select a visual notification to delete.", "OK");
        };
        
        editSubtitleBottomContainer.Add(loadSubtitlesButton);
        editSubtitleBottomContainer.Add(deleteSubtitleButton);
        
        selectSubtitleContainer.Add(visualNotificationDropdown);
        selectSubtitleContainer.Add(editSubtitleBottomContainer);
        
        return selectSubtitleContainer;
    }
    private void RefreshVisualNotification()
    {
        if (visualNotificationDropdown != null)
        {
            var options = ACC_JSONHelper.GetFilesListByParam<ACC_SubtitleData, string>("/ACC_VisualNotification/", data => data.name);
            visualNotificationDropdown.choices = options;
            visualNotificationDropdown.value = options.Count > 0 ? options[0] : "";
        }
        Repaint();
    }
    
    private void HighContrastBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        var options = new List<string> { "Create a high-contrast configuration", /*"Add accessbility to existent subtitle",*/ "Edit high-contrast configuration" };
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-label");
        
        var subtitleCreation = CreateAHighContrastConfiguration();
        dynamicContainer.Add(subtitleCreation);
                
        box.Add(dropdown);
        box.Add(dynamicContainer);
                
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == "Create a high-contrast configuration")
            {
                subtitleCreation = CreateAHighContrastConfiguration();
                dynamicContainer.Add(subtitleCreation);
            }
            else if (evt.newValue == "Edit high-contrast configuration")
            {
                // var subtitleSelection = LoadSubtitle();
                // dynamicContainer.Add(subtitleSelection);
            }
        });
    }
    private VisualElement CreateAHighContrastConfiguration()
    {
        var highContrastConfigurationContainer = new VisualElement();
        highContrastConfigurationContainer.AddToClassList("subtitle-creation-container");

        var createHighContrastConfigurationButton = new Button() { text = "Create" };
        createHighContrastConfigurationButton.AddToClassList("create-button");
        highContrastConfigurationContainer.Add(createHighContrastConfigurationButton);
        
        createHighContrastConfigurationButton.clicked += () =>
        {
            ACC_HighContrastEditorWindow.ShowWindow(null);
        };

        return highContrastConfigurationContainer;
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
        
    }

    #region HelperMethods
    private void LoadPrefab(string feature, string jsonFile="")
    {
        var folder = "ACC_ " + feature + "/";
        
        string name;
        if(jsonFile.Length == 0) name = "ACC_" + feature + "Manager.prefab";
        else name = "ACC_" + feature + "Manager_" + jsonFile + ".prefab";
        
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
