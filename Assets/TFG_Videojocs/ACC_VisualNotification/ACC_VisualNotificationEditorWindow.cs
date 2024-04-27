#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Utilities;
using TFG_Videojocs.ACC_VisualNotification;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_VisualNotificationEditorWindow : ACC_BaseFloatingWindow<ACC_VisualNotificationEditorWindowController, ACC_VisualNotificationEditorWindow, ACC_VisualNotificationData>
{
    private ScrollView soundScrollView;
    private ACC_AudioManager audioManager;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseVisualNotificationWindow;

    private new void OnEnable()
    {
        base.OnEnable();
        audioManager = GameObject.Find("ACC_AudioManager").GetComponent<ACC_AudioManager>();
        audioManager.OnSoundsChanged += CreateSoundList;
    }
    private void OnDisable()
    {
        audioManager.OnSoundsChanged -= CreateSoundList;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        OnCloseVisualNotificationWindow?.Invoke();
    }

    public static void ShowWindow(string name)
    {
        ACC_VisualNotificationEditorWindow window = GetWindow<ACC_VisualNotificationEditorWindow>();
        if(window.controller.isEditing) return;
        if(window.controller.isCreatingNewFileOnCreation) return;
        
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        
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
        
        CreateSoundContainer();
        CreateSettingsContainer();
        CreateBottomContainer();
        controller.RestoreDataAfterCompilation();
    }
    
    private void CreateSoundContainer()
    {
        var soundContainer = uiElementFactory.CreateVisualElement("container");
        var soundsSelectedTitleLabel = uiElementFactory.CreateLabel("title", "Sounds");
        soundContainer.Add(soundsSelectedTitleLabel);
        
        soundScrollView = uiElementFactory.CreateScrollView("sound-container");
        CreateSoundList();
        soundContainer.Add(soundScrollView);
        soundContainer.Add(CreateAddNewSoundOption());
        rootVisualElement.Add(soundContainer);
    }
    public void CreateSoundList()
    {
        if(soundScrollView!=null) soundScrollView.Clear();
        
        var SFXSounds = audioManager.GetSFXSounds();
        bool isFirst = true;
        foreach (var sound in SFXSounds)
        {
            var soundLabel = new Label(sound.name);
            soundLabel.AddToClassList("sound-label");
            if (controller.currentData.soundsList.Find(s => s.name == soundLabel.text) != null)
            {
                soundLabel.AddToClassList("selected-sound");
            }
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

                    if (controller.currentData.soundsList.Contains(soundData))
                    {
                        controller.currentData.soundsList.Remove(soundData);
                        label.RemoveFromClassList("selected-sound");
                    }
                    else
                    {
                        controller.currentData.soundsList.Add(soundData);
                        label.AddToClassList("selected-sound");
                    }
                }
            });

            if (soundScrollView != null) soundScrollView.Add(soundLabel);
        }
    }
    private VisualElement CreateAddNewSoundOption()
    {
        AudioClip audioClip = null;
        return uiElementFactory.CreateObjectFieldWithButton("option-multi-input", "Add new sound:", "Add", typeof(AudioClip),
            onObjectField: value => audioClip = value as AudioClip,
            () =>
            {
                if (audioClip != null && soundScrollView.Children().OfType<Label>().All(label => label.text != audioClip.name))
                {
                    ACC_Sound accSound = new ACC_Sound(audioClip.name, audioClip);
                    //controller.currentData.soundsList.Add(accSound);
                    audioManager.AddSFXSound(accSound);
                }
                else
                {
                    if (audioClip == null)
                    {
                        EditorUtility.DisplayDialog("No AudioClip Selected", "Please select an AudioClip to add.", "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("AudioClip Already Selected", "The selected AudioClip is already in the list.", "OK");
                    }
                }
            });
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
