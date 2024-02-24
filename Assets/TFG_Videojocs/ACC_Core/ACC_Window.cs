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
    private void OnEnable()
    {
        ACC_SubtitlesEditorWindow.OnCloseSubtitleWindow += RefreshDropdown;
        audioManager = GameObject.Find("ACC_AudioManager").GetComponent<ACC_AudioManager>();
        audioManager.OnSoundsChanged += CreateSoundList;
    }

    private void OnDisable()
    {
        ACC_SubtitlesEditorWindow.OnCloseSubtitleWindow -= RefreshDropdown;
        audioManager.OnSoundsChanged -= CreateSoundList;
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
                var subtitleSelection = LoadASubtitle();
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

    private VisualElement LoadASubtitle()
    {
        var selectSubtitleContainer = new VisualElement();

        var options = ACC_JSONHelper.GetFiles("/ACC_JSONSubtitle/");
        
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
                RefreshDropdown();
            }
            else EditorUtility.DisplayDialog("Required Field", "Please select a subtitle to delete.", "OK");
        };
        
        editSubtitleBottomContainer.Add(loadSubtitlesButton);
        editSubtitleBottomContainer.Add(deleteSubtitleButton);
        
        selectSubtitleContainer.Add(subtitlesDropdown);
        selectSubtitleContainer.Add(editSubtitleBottomContainer);
        
        return selectSubtitleContainer;
    }

    private void RefreshDropdown()
    {
        if (subtitlesDropdown != null)
        {
            var options = ACC_JSONHelper.GetFiles("/ACC_JSONSubtitle/");
            subtitlesDropdown.choices = options;
            subtitlesDropdown.value = options.Count > 0 ? options[0] : "";
        }
        Repaint();
    }

    private void VisualContainerBox(VisualElement box)
    {
        soundContainer = new ScrollView();
        soundContainer.AddToClassList("sound-container");
        CreateSoundList();
        box.Add(soundContainer);

        var addVisualNotificationButton = new Button() { text = "Add" };
        addVisualNotificationButton.AddToClassList("add-visual-notification-button");
        box.Add(addVisualNotificationButton);

        addVisualNotificationButton.clicked += () =>
        {
            ACC_VisualNotificationEditorWindow.ShowWindow();
        };
    }

    private void CreateSoundList()
    {
        soundContainer.Clear();
        List<string> selectedSounds = new List<string>();   
        var SFXSounds = audioManager.GetSFXSounds();
        bool isFirst = true;
        foreach (var sound in SFXSounds)
        {
            var soundLabel = new Label(sound.Name);
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
                var soundData = label.userData as ACC_Sound;

                if (soundData != null && selectedSounds.Contains(soundData.Name))
                {
                    selectedSounds.Remove(soundData.Name);
                    label.RemoveFromClassList("selected-sound");
                }
                else
                {
                    selectedSounds.Add(soundData.Name);
                    label.AddToClassList("selected-sound");
                }
            });

            soundContainer.Add(soundLabel);
        }
        
    }
}
