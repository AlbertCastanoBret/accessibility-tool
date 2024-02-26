using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_Window : EditorWindow
{
    private VisualElement accessibilityContainer;
    private const int numberOfColumns = 2;
    private DropdownField subtitlesDropdown;

    private ACC_AudioManager audioManager;
    private ScrollView soundContainer;
    private DropdownField visualNotificationDropdown;
    
    private void OnEnable()
    {
        ACC_SubtitlesEditorWindow.OnCloseSubtitleWindow += RefreshSubtititleWindow;
        ACC_VisualNotificationEditorWindow.OnCloseVisualNotificationWindow += RefreshVisualNotification;
        //audioManager = GameObject.Find("ACC_AudioManager").GetComponent<ACC_AudioManager>();
        //audioManager.OnSoundsChanged += CreateSoundList;
    }

    private void OnDisable()
    {
        ACC_SubtitlesEditorWindow.OnCloseSubtitleWindow -= RefreshSubtititleWindow;
        ACC_VisualNotificationEditorWindow.OnCloseVisualNotificationWindow += RefreshVisualNotification;
        //audioManager.OnSoundsChanged -= CreateSoundList;
    }

    [MenuItem("Tools/ACC/Accessibility Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_Window>("Accessibility Window");
        window.minSize = new Vector2(400, 300);
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
        accessibilityContainer = new VisualElement();
        var accessibilityRow = new VisualElement();
        
        toolbar.AddToClassList("toolbar");
        titleToolbar.AddToClassList("toolbar-title");
        mainContainer.AddToClassList("main-container");
        sidebar.AddToClassList("my-box");
        audibleButton.AddToClassList("my-button");
        visualButton.AddToClassList("my-button");
        accessibilityContainer.AddToClassList("accessibility-container");
        accessibilityRow.AddToClassList("accessibility-row");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.style.minWidth = new StyleLength(1000);
        
        toolbar.Add(titleToolbar);
        
        audibleButton.clicked += () => { UpdateAccessibilityContainer(typeof(AudioFeatures));};
        visualButton.clicked += () => { UpdateAccessibilityContainer(typeof(VisualFeatures)); };
        
        sidebar.Add(audibleButton);
        sidebar.Add(visualButton);
        mainContainer.Add(sidebar);
        mainContainer.Add(accessibilityContainer);
        rootVisualElement.Add(toolbar);
        rootVisualElement.Add(mainContainer);
    }
    
    private void UpdateAccessibilityContainer(Type featureType)
    {
        int numberOfRows = (Enum.GetNames(featureType).Length%2==0 ? Enum.GetNames(featureType).Length : Enum.GetNames(featureType).Length + 1) / numberOfColumns;
        accessibilityContainer.Clear();
        for (int i = 0; i < numberOfRows; i++)
        {
            CreateAccessibilityRow(featureType, i);
        }
    }
    
    private void CreateAccessibilityRow(Type featureType, int rowIndex)
    {
        var row = new VisualElement();
        row.AddToClassList("accessibility-row");
        for (int i = 0; i < numberOfColumns; i++)
        {
            if(Enum.GetName(featureType, numberOfColumns*rowIndex+i)!=null) row.Add(CreateFeatureBox(featureType, numberOfColumns*rowIndex+i));
        }
        accessibilityContainer.Add(row);
    }

    private VisualElement CreateFeatureBox(Type featuretype, int index)
    {
        var box = new VisualElement();
        box.AddToClassList("red-box");
        box.style.width = new Length(92f/numberOfColumns, LengthUnit.Percent);

        var titleBox = new Label(Enum.GetName(featuretype, index));
        titleBox.AddToClassList("title-box");
        box.Add(titleBox);

        if(featuretype == typeof(AudioFeatures)) CreateAudioBox(box, index);

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

    private void SubtitlesBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        var options = new List<string> { "Create a subtitle", "Add accessbility to existent subtitle", "Edit subtitle" };
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-list");
        
        var subtitleCreation = CreateASubtitle();
        dynamicContainer.Add(subtitleCreation);
                
        box.Add(dropdown);
        box.Add(dynamicContainer);
                
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == "Create a subtitle")
            {
                subtitleCreation = CreateASubtitle();
                dynamicContainer.Add(subtitleCreation);
            }
            else if (evt.newValue == "Add accessbility to existent subtitle")
            {
                
            }
            else if (evt.newValue == "Edit subtitle")
            {
                var subtitleSelection = LoadSubtitle();
                dynamicContainer.Add(subtitleSelection);
            }
        });
    }

    private VisualElement CreateASubtitle()
    {
        var subtitleCreationContainer = new VisualElement();
        subtitleCreationContainer.AddToClassList("subtitle-creation-container");

        var createSubtitlesButton = new Button() { text = "Create" };
        createSubtitlesButton.AddToClassList("create-subtitles-button");
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

        var options = ACC_JSONHelper.GetFilesListByParam<ACC_SubtitleData, string>("/ACC_JSONSubtitle/", data => data.name);
        
        subtitlesDropdown = new DropdownField("Select a subtitle:", options, 0);
        subtitlesDropdown.AddToClassList("select-subtitle-dropdown");
        subtitlesDropdown[0].AddToClassList("select-subtitle-label");

        var editSubtitleBottomContainer = new VisualElement();
        editSubtitleBottomContainer.AddToClassList("edit-subtitle-bottom-container");
        
        var loadSubtitlesButton = new Button() { text = "Load" };
        loadSubtitlesButton.AddToClassList("edit-subtitles-button");
        loadSubtitlesButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(subtitlesDropdown.value)) ACC_SubtitlesEditorWindow.ShowWindow(subtitlesDropdown.value);
            else EditorUtility.DisplayDialog("Required Field", "Please select a subtitle to load.", "OK");
        };

        var deleteSubtitleButton = new Button() { text = "Delete" };
        deleteSubtitleButton.AddToClassList("edit-subtitles-button");
        deleteSubtitleButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(subtitlesDropdown.value))
            {
                ACC_JSONHelper.DeleteFile("/ACC_JSONSubtitle/" + subtitlesDropdown.value);
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
            var options = ACC_JSONHelper.GetFilesListByParam<ACC_SubtitleData, string>("/ACC_JSONSubtitle/", data => data.name);
            subtitlesDropdown.choices = options;
            subtitlesDropdown.value = options.Count > 0 ? options[0] : "";
        }
        Repaint();
    }

    private void VisualContainerBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        
        var options = new List<string> { "Create a visual notification", "Edit visual notification" };
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-list");
        
        //soundContainer = new ScrollView();
        //soundContainer.AddToClassList("sound-container");
        //CreateSoundList();
        //dynamicContainer.Add(soundContainer);
        
        var addVisualNotificationButton = new Button() { text = "Create" };
        addVisualNotificationButton.AddToClassList("add-visual-notification-button");
        dynamicContainer.Add(addVisualNotificationButton);

        addVisualNotificationButton.clicked += () =>
        {
            ACC_VisualNotificationEditorWindow.ShowWindow(null);
        };
        
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == "Create a visual notification")
            {
                //dynamicContainer.Add(soundContainer);
                dynamicContainer.Add(addVisualNotificationButton);
            }
            else if (evt.newValue == "Edit visual notification")
            {
                var visualNotificationSelection = LoadVisualNotification();
                dynamicContainer.Add(visualNotificationSelection);
            }
        });
        
        box.Add(dropdown);
        box.Add(dynamicContainer);
    }

    /*private void CreateSoundList()
    {
        soundContainer.Clear();
        selectedSounds = new List<ACC_Sound>();   
        var SFXSounds = audioManager.GetSFXSounds();
        bool isFirst = true;
        foreach (var sound in SFXSounds)
        {
            var soundLabel = new Label(sound.name);
            soundLabel.AddToClassList("sound-label");
            soundLabel.focusable = true;
            soundLabel.userData = sound;
    
            if (isFirst)
            {
                soundLabel.AddToClassList("sound-label-first");
                isFirst = false;
            }

            soundLabel.RegisterCallback<MouseDownEvent>(evt =>
            {
                var label = evt.currentTarget as Label;
                if (label != null)
                {
                    ACC_Sound soundData = label.userData as ACC_Sound;

                    if (selectedSounds.Contains(soundData))
                    {
                        selectedSounds.Remove(soundData);
                        label.RemoveFromClassList("selected-sound");
                    }
                    else
                    {
                        selectedSounds.Add(soundData);
                        label.AddToClassList("selected-sound");
                    }
                }
            });

            soundContainer.Add(soundLabel);
        }
    }*/
    
    private VisualElement LoadVisualNotification()
    {
        var selectSubtitleContainer = new VisualElement();

        var options = ACC_JSONHelper.GetFilesListByParam<ACC_VisualNotificationData, string>("/ACC_JSONVisualNotification/", data => data.name);
        
        visualNotificationDropdown = new DropdownField("Select a visual notification:", options, 0);
        visualNotificationDropdown.AddToClassList("select-subtitle-dropdown");
        visualNotificationDropdown[0].AddToClassList("select-subtitle-label");

        var editSubtitleBottomContainer = new VisualElement();
        editSubtitleBottomContainer.AddToClassList("edit-subtitle-bottom-container");
        
        var loadSubtitlesButton = new Button() { text = "Load" };
        loadSubtitlesButton.AddToClassList("edit-subtitles-button");
        loadSubtitlesButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(visualNotificationDropdown.value))
            {
                var sounds = ACC_JSONHelper.GetParamByFileName<ACC_VisualNotificationData, List<ACC_Sound>>(data => data.soundsList,
                    "/ACC_JSONVisualNotification/", visualNotificationDropdown.value);
                ACC_VisualNotificationEditorWindow.ShowWindow(visualNotificationDropdown.value);
            }
            else EditorUtility.DisplayDialog("Required Field", "Please select a visual notification to load.", "OK");
        };

        var deleteSubtitleButton = new Button() { text = "Delete" };
        deleteSubtitleButton.AddToClassList("edit-subtitles-button");
        deleteSubtitleButton.clicked += () =>
        {
            if (!string.IsNullOrEmpty(visualNotificationDropdown.value))
            {
                ACC_JSONHelper.DeleteFile("/ACC_JSONVisualNotification/" + visualNotificationDropdown.value);
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
            var options = ACC_JSONHelper.GetFilesListByParam<ACC_SubtitleData, string>("/ACC_JSONVisualNotification/", data => data.name);
            visualNotificationDropdown.choices = options;
            visualNotificationDropdown.value = options.Count > 0 ? options[0] : "";
        }
        Repaint();
    }
}
