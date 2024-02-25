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
    
    private bool isEditing = false;
    private string oldName;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseVisualNotificationWindow;
    
    public static void ShowWindow(List<ACC_Sound> selectedSounds, string name)
    {
        ACC_VisualNotificationEditorWindow window = CreateInstance<ACC_VisualNotificationEditorWindow>();
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        window.selectedSounds = selectedSounds;
        if (name != null)
        {
            window.isEditing = true;
            window.LoadJson(name);
        }
        window = GetWindow<ACC_VisualNotificationEditorWindow>();
        window.ShowModal();
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

        var soundsSelectedContainer = new VisualElement();
        soundsSelectedContainer.AddToClassList("sounds-selected-container");
        
        var soundsSelectedTitleLabel = new Label("Selectd sounds:");
        soundsSelectedTitleLabel.AddToClassList("visual-notification-label");
        
        var soundNames = selectedSounds.Select(sound => sound.name).ToList();
        string selectedSoundsString = string.Join(", ", soundNames);
        
        var soundsSelectedLabel = new Label(selectedSoundsString);
        soundsSelectedLabel.AddToClassList("visual-notification-label");
        
        soundsSelectedContainer.Add(soundsSelectedTitleLabel);
        soundsSelectedContainer.Add(soundsSelectedLabel);
        
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
        
        dropdownVerticalAlignment = new DropdownField("Horizontal alignment:", optionsVertical, 0);
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
        
        var createVisualNotificationButton = new Button() { text = "Create" };
        createVisualNotificationButton.AddToClassList("create-visual-notification-button");

        createVisualNotificationButton.clicked += () =>
        {
            if (nameInput.value.Length > 0)
            {
                var fileExists = ACC_JSONHelper.FileNameAlreadyExists("/ACC_JSONVisualNotification/" + nameInput.value);
                if (!fileExists && !isEditing || fileExists && isEditing)
                {
                    ConfigureJson();
                }
                else if(fileExists && !isEditing)
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
                            ConfigureJson();
                            break;
                        case 2:
                            ACC_JSONHelper.RenameFile("/ACC_JSONVisualNotification/" + oldName, "/ACC_JSONVisualNotification/" + nameInput.value);
                            ConfigureJson();
                            break;
                    }
                }
                ConfigureJson();
            }
            else
            {
                EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
            }
        };
        
        mainContainer.Add(soundsSelectedContainer);
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

    private void ConfigureJson()
    {
        ACC_VisualNotificationData accVisualNotificationData = new ACC_VisualNotificationData();
        accVisualNotificationData.name = nameInput.value;
        accVisualNotificationData.soundsList = selectedSounds;
        accVisualNotificationData.message = messageInput.value;
        accVisualNotificationData.horizontalAlignment = dropdownHorizontalAlignment.value;
        accVisualNotificationData.verticalAlignment = dropdownVerticalAlignment.value;
        accVisualNotificationData.timeOnScreen = timeOnScreen.value;
        accVisualNotificationData.fontColor = fontColorInput.value;
        accVisualNotificationData.backgroundColor = backgroundColorInput.value;
        accVisualNotificationData.fontSize = fontSizeInput.value;
    
        ACC_JSONHelper.CreateJson(accVisualNotificationData, "/ACC_JSONVisualNotification/");
    }
    
    public void LoadJson(string name)
    {
        string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_JSONVisualNotification/" + name + ".json");
        ACC_SubtitleData subtitleData = JsonUtility.FromJson<ACC_SubtitleData>(json);
    }
}
