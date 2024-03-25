using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_VisualNotificationEditorWindow : EditorWindow
{
    private List<ACC_Sound> selectedSounds;
    private TextField nameInput, messageInput;
    private DropdownField dropdownHorizontalAlignment, dropdownVerticalAlignment;
    private IntegerField timeOnScreen;
    private ColorField fontColorInput;
    private ColorField backgroundColorInput;
    private SliderInt fontSizeInput;
    private ScrollView soundContainer;
    private ACC_AudioManager audioManager;
    private AudioClip audioClip;

    private bool isEditing, isRenamingFile, isCreatingNewFile, isOverWriting;
    private string oldName;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseVisualNotificationWindow;

    private void OnEnable()
    {
        audioManager = GameObject.Find("ACC_AudioManager").GetComponent<ACC_AudioManager>();
        audioManager.OnSoundsChanged += ResetSoundsList;
    }

    private void OnDisable()
    {
        audioManager.OnSoundsChanged -= ResetSoundsList;
    }

    private void OnDestroy()
    {
        OnCloseVisualNotificationWindow?.Invoke();
    }

    public static void ShowWindow(string name)
    {
        ACC_VisualNotificationEditorWindow window = GetWindow<ACC_VisualNotificationEditorWindow>();
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 520);
        window.maxSize = new Vector2(600, 520);
        if (name != null)
        {
            window.isEditing = true;
            window.selectedSounds = ACC_JSONHelper.GetParamByFileName<ACC_VisualNotificationData, List<ACC_Sound>>(data => data.soundsList,
                "/ACC_JSONVisualNotification/", name);
            window.LoadJson(name);
        }
    }

    private void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_VisualNotification/ACC_VisualNotificationEditorWindow.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#4f4f4f", out backgroundColor);
        rootVisualElement.style.backgroundColor = new StyleColor(backgroundColor);
        
        var mainContainer = new VisualElement();
        mainContainer.AddToClassList("main-container");
        
        var soundsSelectedTitleLabel = new Label("Sounds");
        soundsSelectedTitleLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        
        soundContainer = new ScrollView();
        soundContainer.AddToClassList("sound-container");
        selectedSounds = new List<ACC_Sound>();
        CreateSoundsList();

        var audioClipContainer = new VisualElement();
        audioClipContainer.AddToClassList("audio-clip-container");

        var audioClipField = new ObjectField("Add new sound:")
        {
            objectType = typeof(AudioClip),
            allowSceneObjects = false
        };
        audioClipField.AddToClassList("audio-field");
        audioClipField[0].AddToClassList("audio-label");
        
        audioClipField.RegisterValueChangedCallback(evt =>
        {
            audioClip = evt.newValue as AudioClip;
        });

        var audioClipButton = new Button() { text = "Add" };
        audioClipButton.AddToClassList("audio-button");
        audioClipButton.clicked += () =>
        {
            if (audioClip != null && soundContainer.Children().OfType<Label>().All(label => label.text != audioClip.name))
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
        };
        
        audioClipContainer.Add(audioClipField);
        audioClipContainer.Add(audioClipButton);

        var settingsLabelTitle = new Label("Settings");
        settingsLabelTitle.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        settingsLabelTitle.style.marginTop = new Length(12, LengthUnit.Pixel);
        
        nameInput = new TextField("Name: ");
        nameInput.AddToClassList("visual-notification-field");
        nameInput[0].AddToClassList("visual-notification-label");
        
        messageInput = new TextField("Message: ");
        messageInput.AddToClassList("visual-notification-field");
        messageInput[0].AddToClassList("visual-notification-label");
        
        var optionsHorizontal = new List<string> { "Left", "Center", "Right" };
        dropdownHorizontalAlignment = new DropdownField("Horizontal alignment:", optionsHorizontal, 0);
        dropdownHorizontalAlignment.AddToClassList("visual-notification-dropdown");
        dropdownHorizontalAlignment[0].AddToClassList("visual-notification-label");
        
        var optionsVertical = new List<string> { "Top", "Center", "Down" };
        dropdownVerticalAlignment = new DropdownField("Horizontal alignment:", optionsVertical, 0);
        dropdownVerticalAlignment.AddToClassList("visual-notification-dropdown");
        dropdownVerticalAlignment[0].AddToClassList("visual-notification-label");

        timeOnScreen = new IntegerField(){label = "Time on screen"};
        timeOnScreen.AddToClassList("visual-notification-field");
        timeOnScreen[0].AddToClassList("visual-notification-label");
        timeOnScreen.value = 1;
        
        dropdownVerticalAlignment = new DropdownField("Vertical alignment:", optionsVertical, 0);
        dropdownVerticalAlignment.AddToClassList("visual-notification-dropdown");
        dropdownVerticalAlignment[0].AddToClassList("visual-notification-label");

        fontColorInput = new ColorField("Font color:");
        fontColorInput.value = new Color(0, 0, 0, 1);
        fontColorInput.AddToClassList("visual-notification-dropdown");
        fontColorInput[0].AddToClassList("visual-notification-label");
        
        backgroundColorInput = new ColorField("Background color:");
        backgroundColorInput.value = new Color(0, 0, 0, 1);
        backgroundColorInput.AddToClassList("visual-notification-dropdown");
        backgroundColorInput[0].AddToClassList("visual-notification-label");

        var fontSizeContainer = new VisualElement();
        fontSizeContainer.AddToClassList("font-size-container");
        
        fontSizeInput = new SliderInt("Font size:", 10, 60) { value = 20 };
        fontSizeInput.AddToClassList("font-size-slider");
        fontSizeInput[0].AddToClassList("font-size-label");
        
        var fontSizeField = new IntegerField { value = 20, name = "fontSizeField" };
        fontSizeField.AddToClassList("font-size-field");
        
        fontSizeInput.RegisterValueChangedCallback(evt =>
        {
            fontSizeField.value = evt.newValue;
        });
        
        fontSizeField.RegisterValueChangedCallback(evt =>
        {
            fontSizeInput.value = evt.newValue;
        });
        
        fontSizeContainer.Add(fontSizeInput);
        fontSizeContainer.Add(fontSizeField);
        
        var createVisualNotificationButton = new Button() { text = "Save" };
        createVisualNotificationButton.AddToClassList("create-visual-notification-button");

        createVisualNotificationButton.clicked += () =>
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
                        case 2:
                            isRenamingFile = true;
                            ACC_JSONHelper.RenameFile("/ACC_JSONVisualNotification/" + oldName, "/ACC_JSONVisualNotification/" + nameInput.value);
                            ConfigureJson();
                            break;
                    }
                }
                if (!isEditing) Close();
            }
            else
            {
                if(nameInput.value.Length == 0) EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
                else if (selectedSounds.Count == 0) EditorUtility.DisplayDialog("Required field", "Please, introduce at least one sound name to create a visual notification", "OK");
            }
        };
        
        mainContainer.Add(soundsSelectedTitleLabel);
        mainContainer.Add(soundContainer);
        mainContainer.Add(audioClipContainer);
        mainContainer.Add(settingsLabelTitle);
        mainContainer.Add(nameInput);
        mainContainer.Add(messageInput);
        mainContainer.Add(dropdownHorizontalAlignment);
        mainContainer.Add(dropdownVerticalAlignment);
        mainContainer.Add(timeOnScreen);
        mainContainer.Add(fontColorInput);
        mainContainer.Add(backgroundColorInput);
        mainContainer.Add(fontSizeContainer);
        mainContainer.Add(createVisualNotificationButton);
        rootVisualElement.Add(mainContainer);
    }

    private void ResetSoundsList()
    {
        if(soundContainer!= null) soundContainer.Clear();
        selectedSounds = ACC_JSONHelper.GetParamByFileName<ACC_VisualNotificationData, List<ACC_Sound>>(data => data.soundsList,
            "/ACC_JSONVisualNotification/", oldName);
        nameInput.value = oldName;
        CreateSoundsList();
        LoadSelectedSounds();
    }
    
    private void CreateSoundsList()
    {
        if(soundContainer!= null) soundContainer.Clear();
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

            if (soundContainer != null) soundContainer.Add(soundLabel);
        }
    }

    private void ConfigureJson()
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
            if(isEditing) oldName = nameInput.value;
        }
    }

    public void LoadJson(string name)
    {
        string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_JSONVisualNotification/" + name + ".json");
        ACC_VisualNotificationData accVisualNotificationData = JsonUtility.FromJson<ACC_VisualNotificationData>(json);

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
    }

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
        }
    }
}
