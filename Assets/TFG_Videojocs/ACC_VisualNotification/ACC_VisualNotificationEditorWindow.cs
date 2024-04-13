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
    private List<ACC_Sound> selectedSounds;
    private TextField nameInput, messageInput;
    private DropdownField dropdownHorizontalAlignment, dropdownVerticalAlignment;
    private IntegerField timeOnScreen;
    private ColorField fontColorInput;
    private ColorField backgroundColorInput;
    private SliderInt fontSizeInput;
    private ScrollView soundContainer, soundScrollView;
    private ACC_AudioManager audioManager;
    private AudioClip audioClip;

    private bool isEditing, isRenamingFile, isCreatingNewFile, isOverWriting, isClosing;
    private string oldName;
    private ACC_VisualNotificationData lastVisualNotificationData;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseVisualNotificationWindow;

    private new void OnEnable()
    {
        base.OnEnable();
        audioManager = GameObject.Find("ACC_AudioManager").GetComponent<ACC_AudioManager>();
        audioManager.OnSoundsChanged += CreateGUI;
    }

    private void OnDisable()
    {
        audioManager.OnSoundsChanged -= CreateGUI;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
    }
    
    /*private void OnCompilationStarted(object obj)
    {
        var container = new ACC_PreCompilationDataStorage();
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", nameInput.value));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", messageInput.value));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", dropdownHorizontalAlignment.value));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", dropdownVerticalAlignment.value));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", timeOnScreen.value.ToString()));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", ColorUtility.ToHtmlStringRGBA(fontColorInput.value)));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", ColorUtility.ToHtmlStringRGBA(backgroundColorInput.value)));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", fontSizeInput.value.ToString()));
        
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", JsonUtility.ToJson(selectedSounds)));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("VisualNotification", JsonUtility.ToJson(lastVisualNotificationData)));
        
        var json = JsonUtility.ToJson(container);
        SessionState.SetString("visual_notification_tempData", json);
    }*/

    public static void ShowWindow(string name)
    {
        ACC_VisualNotificationEditorWindow window = GetWindow<ACC_VisualNotificationEditorWindow>();
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        if (name != null)
        {
            window.controller.isEditing = true;
            window.selectedSounds = ACC_JSONHelper.GetParamByFileName<ACC_VisualNotificationData, List<ACC_Sound>>(data => data.soundsList,
                "/ACC_JSONVisualNotification/", name);
            window.controller.LoadJson(name);
        }
        window.controller.lastData = window.controller.currentData.Clone() as ACC_VisualNotificationData;
    }

    private new void CreateGUI()
    {
        base.CreateGUI();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_VisualNotification/ACC_VisualNotificationEditorWindow.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        CreateSoundContainer();
        CreateSettingsContainer();
        CreateBottomContainer();
        
        /*lastVisualNotificationData = new ACC_VisualNotificationData();
        lastVisualNotificationData.soundsList = new List<ACC_Sound>(selectedSounds);
        lastVisualNotificationData.name = nameInput.value;
        lastVisualNotificationData.message = messageInput.value;
        lastVisualNotificationData.horizontalAlignment = dropdownHorizontalAlignment.value;
        lastVisualNotificationData.verticalAlignment = dropdownVerticalAlignment.value;
        lastVisualNotificationData.timeOnScreen = timeOnScreen.value;
        lastVisualNotificationData.fontColor = fontColorInput.value;
        lastVisualNotificationData.backgroundColor = backgroundColorInput.value;
        lastVisualNotificationData.fontSize = fontSizeInput.value;*/
        
        //RestoreDataAfterCompilation();
    }

    /*private void RestoreDataAfterCompilation()
    {
        var serializedData = SessionState.GetString("visual_notification_tempData", "");
        if (serializedData != "")
        {
            var tempData = JsonUtility.FromJson<ACC_PreCompilationDataStorage>(serializedData);
            nameInput.value = tempData.keyValuePairs[0].value;
            messageInput.value = tempData.keyValuePairs[1].value;
            dropdownHorizontalAlignment.value = tempData.keyValuePairs[2].value;
            dropdownVerticalAlignment.value = tempData.keyValuePairs[3].value;
            timeOnScreen.value = int.Parse(tempData.keyValuePairs[4].value);
            ColorUtility.TryParseHtmlString("#"+ tempData.keyValuePairs[5].value, out var fontColor);
            fontColorInput.value = fontColor;
            ColorUtility.TryParseHtmlString("#"+ tempData.keyValuePairs[6].value, out var backgroundColor);
            backgroundColorInput.value = backgroundColor;
            fontSizeInput.value = int.Parse(tempData.keyValuePairs[7].value);
            
        }
        SessionState.EraseString("visual_notification_tempData");
    }*/
    
    private void CreateSoundContainer()
    {
        var soundContainer = uiElementFactory.CreateVisualElement("container");
        var soundsSelectedTitleLabel = uiElementFactory.CreateLabel("title", "Sounds");
        soundContainer.Add(soundsSelectedTitleLabel);
        
        soundScrollView = uiElementFactory.CreateScrollView("sound-container");
        selectedSounds = new List<ACC_Sound>();
        soundContainer.Add(soundScrollView);
        soundContainer.Add(CreateAddNewSoundOption());
        
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

            if (soundScrollView != null) soundScrollView.Add(soundLabel);
        }
        rootVisualElement.Add(soundContainer);
    }
    
    private VisualElement CreateAddNewSoundOption()
    {
        return uiElementFactory.CreateObjectFieldAndButton("option-multi-input", "Add new sound:", "Add", typeof(AudioClip),
            onObjectField: value => audioClip = value as AudioClip,
            () =>
            {
                if (audioClip != null && soundScrollView.Children().OfType<Label>().All(label => label.text != audioClip.name))
                {
                    ACC_Sound accSound = new ACC_Sound(audioClip.name, audioClip);
                    audioManager.AddSFXSound(accSound);
                    selectedSounds.Add(accSound);
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
        
        nameInput = uiElementFactory.CreateTextField("option-input-name", "Name: ", "", "option-input-name-label", 
            onValueChanged: value => controller.currentData.name = value);
        
        messageInput = uiElementFactory.CreateTextField("option-input", "Message: ", "", "option-input-label",
            onValueChanged: value => controller.currentData.message = value);
        
        dropdownHorizontalAlignment = (DropdownField)uiElementFactory.CreateDropdownField("option-input", "Horizontal alignment:", 
            new List<string> { "Left", "Center", "Right" }, "option-input-label",
            onValueChanged: value => controller.currentData.horizontalAlignment = value);
        
        var optionsVertical = new List<string> { "Top", "Center", "Down" };
        dropdownVerticalAlignment = (DropdownField)uiElementFactory.CreateDropdownField("option-input", "Vertical alignment:", optionsVertical, "option-input-label",
            onValueChanged: value => controller.currentData.verticalAlignment = value);

        timeOnScreen = uiElementFactory.CreateIntegerField("option-input", "Time on screen (seconds): ", 1, "option-input-label",
            onValueChanged: value => controller.currentData.timeOnScreen = value);

        fontColorInput = uiElementFactory.CreateColorField("option-input", "Font Color:", Color.black, "option-input-label",
            onValueChanged: value => controller.currentData.fontColor = value);
        
        backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background color:", Color.white, "option-input-label",
            onValueChanged: value => controller.currentData.backgroundColor = value);

        var fontSizeContainer = uiElementFactory.CreateSliderWithIntegerField("option-multi-input", "Font size:", 10,
            60, 20,
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
        var createVisualNotificationButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));
        bottomContainer.Add(createVisualNotificationButton);
        rootVisualElement.Add(bottomContainer);
    }

    /*private void ConfigureJson()
    {
        ACC_VisualNotificationData accVisualNotificationData = new ACC_VisualNotificationData();
        accVisualNotificationData.name = nameInput.value;
        accVisualNotificationData.soundsList = new List<ACC_Sound>(selectedSounds);
        accVisualNotificationData.message = messageInput.value;
        accVisualNotificationData.horizontalAlignment = dropdownHorizontalAlignment.value;
        accVisualNotificationData.verticalAlignment = dropdownVerticalAlignment.value;
        accVisualNotificationData.timeOnScreen = timeOnScreen.value;
        accVisualNotificationData.fontColor = fontColorInput.value;
        accVisualNotificationData.backgroundColor = backgroundColorInput.value;
        accVisualNotificationData.fontSize = fontSizeInput.value;

        Dictionary<string, List<ACC_Sound>> repeatedSounds = new Dictionary<string, List<ACC_Sound>>(); 
        for (int i = 0; i < selectedSounds.Count; i++)
        {
            string fileName = ACC_JSONHelper.GetFileNameByListParameter<ACC_VisualNotificationData, ACC_Sound>(
                "/ACC_JSONVisualNotification/",
                data => data.soundsList,
                (itemInList, itemToMatch) => itemInList.name == itemToMatch.name,
                selectedSounds[i]
            );
            if (fileName != null && fileName != oldName + ".json" || fileName != null && isCreatingNewFile || fileName != null && isOverWriting)
            {
                if (!repeatedSounds.ContainsKey(fileName))
                {
                    repeatedSounds.Add(fileName, new List<ACC_Sound>());
                    repeatedSounds[fileName].Add(selectedSounds[i]);
                }
                else repeatedSounds[fileName].Add(selectedSounds[i]);
            }
        }
        
        if (repeatedSounds.Count > 0 && !isRenamingFile)
        {
            isCreatingNewFile = false;
            isOverWriting = false;
            ShowDialogRepeatedSounds(repeatedSounds, accVisualNotificationData);
        }
        else
        {
            isRenamingFile = false;
            isCreatingNewFile = false;
            isOverWriting = false;
            ACC_JSONHelper.CreateJson(accVisualNotificationData, "/ACC_JSONVisualNotification/");
            lastVisualNotificationData = accVisualNotificationData;
            if(isEditing) oldName = nameInput.value;
        }
    }*/

    /*public void LoadJson(string name)
    {
        string path = "/ACC_JSONVisualNotification/" + name;
        ACC_VisualNotificationData accVisualNotificationData = ACC_JSONHelper.LoadJson<ACC_VisualNotificationData>(path);

        if (isEditing) oldName = accVisualNotificationData.name;
        
        nameInput.value = accVisualNotificationData.name;
        messageInput.value = accVisualNotificationData.message;
        dropdownHorizontalAlignment.value = accVisualNotificationData.horizontalAlignment;
        dropdownVerticalAlignment.value = accVisualNotificationData.verticalAlignment;
        timeOnScreen.value = accVisualNotificationData.timeOnScreen;
        fontColorInput.value = accVisualNotificationData.fontColor;
        backgroundColorInput.value = accVisualNotificationData.backgroundColor;
        fontSizeInput.value = accVisualNotificationData.fontSize;
        
        LoadSelectedSounds();
        lastVisualNotificationData = accVisualNotificationData;
    }*/

    private void LoadSelectedSounds()
    {
        foreach (var visualElement in soundContainer.Children())
        {
            Label label = (Label)visualElement;
            foreach (var sound in selectedSounds)
            {
                if (sound.name == label.text && audioManager.GetSFXSounds().Contains(sound))
                {
                    ACC_Sound soundData = label.userData as ACC_Sound;
                    selectedSounds.Remove(sound);
                    selectedSounds.Add(soundData);
                    label.AddToClassList("selected-sound");
                    break;
                }
            }
        }
    }
    
    private void ShowDialogRepeatedSounds(Dictionary<string, List<ACC_Sound>> repeatedSounds, ACC_VisualNotificationData accVisualNotificationData)
    {
        string sounds = string.Join(", ", repeatedSounds);
        int option = EditorUtility.DisplayDialogComplex(
            "Some sounds already have a visual notification.",
            "Sounds \"" + sounds + "\" already have been added to another visual notification. What would you like to do?",
            "Move sounds",
            "Cancel",
            ""
        );
        switch (option)
        {
            case 0:
                foreach (KeyValuePair<string, List<ACC_Sound>> kvp in repeatedSounds)
                {
                    foreach (ACC_Sound sound in kvp.Value)
                    {
                        ACC_JSONHelper.RemoveItemFromListInFile<ACC_VisualNotificationData, ACC_Sound>(
                            "/ACC_JSONVisualNotification",
                            data => data.soundsList,
                            (itemInList, itemToMatch) => itemInList.name == itemToMatch.name,
                            sound
                        );
                    }
                }
                ACC_JSONHelper.CreateJson(accVisualNotificationData, "/ACC_JSONVisualNotification/");
                if(isEditing) oldName = nameInput.value;
                break;
            case 1:
                if(isClosing) Cancel();
                break;
        }
    }

    /*private void HandleSave()
    {
        if (nameInput.value.Length > 0 && selectedSounds.Count > 0)
        {
            var fileExists = ACC_JSONHelper.FileNameAlreadyExists("/ACC_JSONVisualNotification/" + nameInput.value);
            if (!fileExists && !isEditing || fileExists && isEditing && nameInput.value == oldName)
            {
                ConfigureJson();
            }
            else if(fileExists && !isEditing || fileExists && isEditing && nameInput.value != oldName)
            {
                int option = EditorUtility.DisplayDialogComplex(
                    "File name already exists",
                    "The name \"" + nameInput.value + "\" already exists. What would you like to do?",
                    "Overwrite",
                    "Cancel",
                    ""
                );
                switch (option)
                {
                    case 0:
                        isOverWriting = true;
                        ConfigureJson();
                        break;
                    case 1:
                        if(isClosing) Cancel();
                        break;
                }
            }
            else if(!fileExists && isEditing)
            {
                int option = EditorUtility.DisplayDialogComplex(
                    "Name Change Detected",
                    $"The name has been changed to \"{nameInput.value}\". What would you like to do?",
                    "Create New File", 
                    "Cancel",
                    "Rename Existing File"
                );
                switch (option)
                {
                    case 0:
                        isCreatingNewFile = true;
                        ConfigureJson();
                        break;
                    case 1:
                        if(isClosing) Cancel();
                        break;
                    case 2:
                        isRenamingFile = true;
                        ACC_JSONHelper.RenameFile("/ACC_JSONVisualNotification/" + oldName, "/ACC_JSONVisualNotification/" + nameInput.value);
                        ConfigureJson();
                        break;
                }
            }
            if (!isEditing && !isClosing) Close();
        }
        else
        {
            if(nameInput.value.Length == 0) EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
            else if (selectedSounds.Count == 0) EditorUtility.DisplayDialog("Required field", "Please, introduce at least one sound name to create a visual notification", "OK");
            if(isClosing) Cancel();
        }
    }*/
    
    private bool IsThereAnyChange()
    {
        if (lastVisualNotificationData.name != nameInput.value) return true;
        if (lastVisualNotificationData.message != messageInput.value) return true;
        if (lastVisualNotificationData.horizontalAlignment != dropdownHorizontalAlignment.value) return true;
        if (lastVisualNotificationData.verticalAlignment != dropdownVerticalAlignment.value) return true;
        if (lastVisualNotificationData.timeOnScreen != timeOnScreen.value) return true;
        if (lastVisualNotificationData.fontColor != fontColorInput.value) return true;
        if (lastVisualNotificationData.backgroundColor != backgroundColorInput.value) return true;
        if (lastVisualNotificationData.fontSize != fontSizeInput.value) return true;
        if (!lastVisualNotificationData.soundsList.SequenceEqual(selectedSounds)) return true;
        return false;
    }

    private void Cancel()
    {
        var window = Instantiate(this);
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 520);
        window.maxSize = new Vector2(600, 520);
        window.Show();
        
        window.nameInput.value = nameInput.value;
        window.messageInput.value = messageInput.value;
        window.dropdownHorizontalAlignment.value = dropdownHorizontalAlignment.value;
        window.dropdownVerticalAlignment.value = dropdownVerticalAlignment.value;
        window.timeOnScreen.value = timeOnScreen.value;
        window.fontColorInput.value = fontColorInput.value;
        window.backgroundColorInput.value = backgroundColorInput.value;
        window.fontSizeInput.value = fontSizeInput.value;
        window.selectedSounds = new List<ACC_Sound>(selectedSounds);
        
        if (isEditing)
        {
            window.oldName = oldName;
            window.isEditing = true;
        }
        window.LoadSelectedSounds();
    }
    
    /*private void ConfirmSaveChangesIfNeeded()
    {
        if (IsThereAnyChange())
        {
            var result = EditorUtility.DisplayDialogComplex("Visual notification file has been modified",
                $"Do you want to save the changes you made in:\n./ACC_JSONSubtitle/{nameInput.value}.json\n\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
            switch (result)
            {
                case 0:
                    isClosing = true;
                    HandleSave();
                    OnCloseVisualNotificationWindow?.Invoke();
                    break;
                case 1:
                    Cancel();
                    break;
                case 2:
                    break;
            }
        }
    }*/
}
