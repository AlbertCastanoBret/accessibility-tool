#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Sound.ACC_Example;
using TFG_Videojocs.ACC_VisualNotification;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class ACC_VisualNotificationEditorWindow : ACC_BaseFloatingWindow<ACC_VisualNotificationEditorWindowController, ACC_VisualNotificationEditorWindow, ACC_VisualNotificationData>
{
    private VisualElement tableScrollView, tableContainer;
    public static event WindowDelegate OnCloseWindow;
    
    private new void OnDestroy()
    {
        base.OnDestroy();
        OnCloseWindow?.Invoke("ACC_VisualNotification/");
    }

    public static void ShowWindow(string name)
    {
        ACC_VisualNotificationEditorWindow window = GetWindow<ACC_VisualNotificationEditorWindow>();
        if(window.controller.isEditing) return;
        if(window.controller.isCreatingNewFileOnCreation) return;
        
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 630);
        window.maxSize = new Vector2(600, 630);
        
        if (name != null)
        {
            window.controller.isEditing = true;
            window.controller.LoadJson(name);
        }
        else
        {
            window.controller.isCreatingNewFileOnCreation = true;
            window.controller.lastData = window.controller.currentData.Clone() as ACC_VisualNotificationData;
        }
        
        window.PositionWindowInBottomRight();
        window.SetFixedPosition();
    }

    public new void CreateGUI()
    {
        base.CreateGUI();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_VisualNotification/ACC_VisualNotificationEditorWindowStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        tableContainer = uiElementFactory.CreateVisualElement("container");
        rootVisualElement.Add(tableContainer);
        CreateTable();
        
        CreateSettingsContainer();
        CreateBottomContainer();
        controller.RestoreDataAfterCompilation();
        
        if (!String.IsNullOrEmpty(EditorPrefs.GetString(typeof(ACC_VisualNotificationEditorWindow) + "Open")))
        {
            var name = EditorPrefs.GetString(typeof(ACC_VisualNotificationEditorWindow) + "Open");
            controller.isEditing = true;
            controller.LoadJson(name);
            EditorPrefs.SetString(typeof(ACC_VisualNotificationEditorWindow) + "Open", "");
        }
    }

    public void CreateTable()
    {
        tableContainer.Clear();
        
        var soundsTitle = uiElementFactory.CreateLabel("title", "Sounds");
        tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
        
        var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
        containerTableTitle.style.width = new Length(100, LengthUnit.Percent);
        var containerTableNameTitle = uiElementFactory.CreateLabel("table-title-name", "Audio Sources");
        
        containerTableTitle.Add(containerTableNameTitle);
        tableScrollView.Add(containerTableTitle);
        
        var audioManagerData = ACC_JSONHelper.LoadJson<ACC_AudioManagerData>("ACC_AudioManager/ACC_AudioManager");

        foreach (var audioSource in audioManagerData.audioSources.Items)
        {
            CreateAudioSource(audioSource.value.name, audioManagerData);
        }
        
        tableContainer.Add(soundsTitle);
        tableContainer.Add(tableScrollView);
    }
    private void CreateAudioSource(string name, ACC_AudioManagerData audioManagerData)
    {
        var row = uiElementFactory.CreateVisualElement("table-row");
        
        var mainRow = uiElementFactory.CreateVisualElement("table-main-row");
        var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
        tableCell.style.width = new Length(100, LengthUnit.Percent);
        var arrowButton = uiElementFactory.CreateButton("\u25b6", "table-arrow-button");
        arrowButton.clicked += () => ToggleControlSchemeDisplay(arrowButton, row);
        var icon = uiElementFactory.CreateImage("table-cell-icon",
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Sound/d_AudioSource Icon.png"));
        icon.style.width = new StyleLength(16);
        icon.style.height = new StyleLength(16);
        var nameLabel = uiElementFactory.CreateLabel("table-cell", name);
        
        tableCell.Add(arrowButton);
        tableCell.Add(icon);
        tableCell.Add(nameLabel);
        mainRow.Add(tableCell);
        row.Add(mainRow);
        tableScrollView.Add(row);
        CreateSounds(row, audioManagerData);
    }
    private void CreateSounds(VisualElement row, ACC_AudioManagerData audioData)
    {
        foreach (var audioSource in audioData.audioSources.Items)
        {
            foreach (var audioSource2 in audioData.audioClips.Items)
            {
                if (audioSource.key == audioSource2.key && tableScrollView.IndexOf(row) - 1 == audioSource.key)
                {
                    foreach (var guid in audioData.audioClips.Items.Find(x => x.key == audioSource.key).value.Items)
                    {
                        CreateSound(row, guid.value, audioSource.key);
                    }
                }
            }
        }
    }
    private void CreateSound(VisualElement row, string soundGuid, int key)
    {
        var audioClipPath = AssetDatabase.GUIDToAssetPath(soundGuid);
        var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);
        
        if (audioClip == null) return;
        
        var soundContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
        soundContainer.style.display = DisplayStyle.None;
        
        var soundCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
        soundCell.AddToClassList("table-secondary-row-content-hover");
        soundCell.style.width = new Length(100, LengthUnit.Percent);
        
        var icon = uiElementFactory.CreateImage("table-cell-icon",
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Sound/d_AudioImporter Icon.png"));
        icon.AddToClassList("table-cell-icon-hover");
        
        var soundLabel = uiElementFactory.CreateLabel("table-cell-label-hover", audioClip.name);
        
        if(controller.currentData.soundsList.Find(s => s.name == soundLabel.text && s.audioSourceKey == key && s.guid == soundGuid) != null)
        {
            soundCell.AddToClassList("selected-sound");
        }
        
        soundCell.RegisterCallback<MouseDownEvent>(evt =>
        {
            var soundCell = evt.currentTarget as VisualElement;
            if (soundCell != null)
            {
                if(controller.currentData.soundsList.Find(s => s.name == soundLabel.text && s.audioSourceKey == key && s.guid == soundGuid) != null)
                {
                    controller.currentData.soundsList.Remove(controller.currentData.soundsList.Find(s => s.name == soundLabel.text && s.audioSourceKey == key && s.guid == soundGuid));
                    soundCell.RemoveFromClassList("selected-sound");
                }
                else
                {
                    controller.currentData.soundsList.Add(new ACC_Sound(key, soundLabel.text, soundGuid));
                    soundCell.AddToClassList("selected-sound");
                }
            }
        });
        
        soundCell.Add(icon);
        soundCell.Add(soundLabel);
        soundContainer.Add(soundCell);
        row.Add(soundContainer);
    }
    private void ToggleControlSchemeDisplay(Button arrowButton, VisualElement audioSource)
    {
        if (arrowButton.text == "\u25b6")
        {
            arrowButton.text = "\u25bc";
            for (int j = 1; j < audioSource.childCount; j++)
            {
                audioSource[j].style.display = DisplayStyle.Flex;
            }
        }
        else
        {
            arrowButton.text = "\u25b6";
            for (int j = 1; j < audioSource.childCount; j++)
            {
                audioSource[j].style.display = DisplayStyle.None;
            }
        }
    }
    private void CreateSettingsContainer()
    {
        var settingsContainer = uiElementFactory.CreateVisualElement("container-2");
        
        var settingsLabelTitle = uiElementFactory.CreateLabel("title", "Settings");
        
        var nameInput = uiElementFactory.CreateTextField("option-input", "Name: ", "", "option-input-label", 
            onValueChanged: value => controller.currentData.name = value);
        
        var messageInput = uiElementFactory.CreateTextField("option-input", "Message: ", "", "option-input-label",
            onValueChanged: value => controller.currentData.message = value);
        
        var dropdownHorizontalAlignment = (DropdownField)uiElementFactory.CreateDropdownField("option-input", "Horizontal alignment:", 
            new List<string> { "Left", "Center", "Right" }, subClassList: "option-input-label",
            onValueChanged: value => controller.currentData.horizontalAlignment = value);
        
        var dropdownVerticalAlignment = (DropdownField)uiElementFactory.CreateDropdownField("option-input", "Vertical alignment:", 
            new List<string> { "Top", "Center", "Down" }, subClassList: "option-input-label",
            onValueChanged:value => controller.currentData.verticalAlignment = value);

        var timeOnScreen = uiElementFactory.CreateIntegerField("option-input", "Time on screen (seconds): ", 1, "option-input-label",
            onValueChanged: value => controller.currentData.timeOnScreen = value);

        var fontColorInput = uiElementFactory.CreateColorField("option-input", "Font Color:", Color.black, "option-input-label",
            onValueChanged: value => controller.currentData.fontColor = value);
        
        var backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background color:", Color.white, "option-input-label",
            onValueChanged: value => controller.currentData.backgroundColor = value);

        var fontSizeContainer = uiElementFactory.CreateSliderWithIntegerField("option-multi-input-last", "Font size:", 20,
            100, 40,
            onValueChanged: value => controller.currentData.fontSize = value);
        
        settingsContainer.Add(settingsLabelTitle);
        settingsContainer.Add(nameInput);
        settingsContainer.Add(messageInput);
        settingsContainer.Add(dropdownHorizontalAlignment);
        settingsContainer.Add(dropdownVerticalAlignment);
        settingsContainer.Add(timeOnScreen);
        settingsContainer.Add(fontColorInput);
        settingsContainer.Add(backgroundColorInput);
        settingsContainer.Add(fontSizeContainer);
        
        rootVisualElement.Add(settingsContainer);
    }
    private void CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
        bottomContainer.style.marginTop = new StyleLength(Length.Auto());
        var createVisualNotificationButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));
        bottomContainer.Add(createVisualNotificationButton);
        rootVisualElement.Add(bottomContainer);
    }
}
#endif
