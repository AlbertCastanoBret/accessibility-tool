using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;

public class ACC_SubtitlesEditorWindow : EditorWindow
{
    private VisualElement table;
    private TextField nameInput;
    private ColorField fontColorInput;
    private ColorField backgroundColorInput;
    private IntegerField fontSizeInput;
    
    private bool isEditing = false;
    private string oldName;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseSubtitleWindow;

    private void OnDestroy()
    {
        OnCloseSubtitleWindow?.Invoke();
    }

    public static void ShowWindow(string name)
    {
        var window = GetWindow<ACC_SubtitlesEditorWindow>();
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        if (name != null)
        {
            window.isEditing = true;
            window.LoadJson(name);
        }
        //window.ShowModal();
    }
    
    private void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Subtitles/ACC_SubtitlesWindowStyles.uss");
        var mainContainer = new VisualElement();
        var tableScrollView = new ScrollView();
        tableScrollView.AddToClassList("table-scroll-view");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#4f4f4f", out backgroundColor);
        rootVisualElement.style.backgroundColor = new StyleColor(backgroundColor);
        mainContainer.AddToClassList("main-container");

        var subtitlesTitle = new Label("Subtitles");
        subtitlesTitle.AddToClassList("subtitles-title");
            
        table = CreateTable();
        CreateRow(1, "Hello", 1);
        tableScrollView.Add(table);
        
        mainContainer.Add(subtitlesTitle);
        mainContainer.Add(tableScrollView);

        var settingsContainer = CreateSettingsContainer();
        var bottomContainer = CreateBottomContainer();
        
        mainContainer.Add(settingsContainer);
        mainContainer.Add(bottomContainer);
        rootVisualElement.Add(mainContainer);
    }

    private VisualElement CreateTable()
    {
        var table = new VisualElement();
        var mainRow = new VisualElement();
        var subtitles = new Label("Subtitles");
        var time = new Label("Time");
        
        table.AddToClassList("table");
        mainRow.AddToClassList("main-row");
        subtitles.AddToClassList("subtitles-cell");
        time.AddToClassList("time-cell");
        
        mainRow.Add(subtitles);
        mainRow.Add(time);
        table.Add(mainRow);

        return table;
    }

    private void CreateRow(int numberOfRows, string subtitle, int time)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            var newRow = new VisualElement();
            newRow.AddToClassList("row");
            
            var subtitleField = new TextField();
            subtitleField.value = subtitle;
            subtitleField.AddToClassList("subtitles-new-cell");
            subtitleField[0].AddToClassList("subtitles-input-cell");
            /*subtitleField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    CreateRow(1);
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Backspace && subtitleField.value == "")
                {
                    table.Remove(newRow);
                }
            });*/
            
            newRow.Add(subtitleField);
            
            var timeField = new IntegerField();
            timeField.value = time;
            timeField.AddToClassList("time-new-cell");
            timeField[0].AddToClassList("time-input-cell");
            newRow.Add(timeField);
                    
            /*var fontColorField = new ColorField("");
            fontColorField.AddToClassList("font-color-new-cell");
            newRow.Add(fontColorField);*/
            
            var deleteButton = new Button(() => table.Remove(newRow)) { text = "-" };
            deleteButton.AddToClassList("delete-row-button");
            newRow.Add(deleteButton);
            
            table.Add(newRow);
            
            rootVisualElement.schedule.Execute(() =>
            {
                subtitleField[0].Focus();
            }).StartingIn((long)0.001);
        }
    }

    private VisualElement CreateSettingsContainer()
    {
        var settingsContainer = new VisualElement();
        settingsContainer.AddToClassList("settings-container");
        
        var settingsTitle = new Label("Settings");
        settingsTitle.AddToClassList("settings-title");
        
        var nameContainer = new VisualElement();
        nameContainer.AddToClassList("option-container");
        var nameTitle = new Label("Name:");
        nameTitle.AddToClassList("option-title");
        nameInput = new TextField();
        nameInput.AddToClassList("option-input");
        
        var fontColorContainer = new VisualElement();
        fontColorContainer.AddToClassList("option-container");
        var fontColorTitle = new Label("Font color:");
        fontColorTitle.AddToClassList("option-title");
        fontColorInput = new ColorField();
        fontColorInput.AddToClassList("option-input");
        fontColorInput.value = new Color(0,0,0,1);
        
        var backgroundColorContainer = new VisualElement();
        backgroundColorContainer.AddToClassList("option-container");
        var backgroundColorTitle = new Label("Background color:");
        backgroundColorTitle.AddToClassList("option-title");
        backgroundColorInput = new ColorField();
        backgroundColorInput.AddToClassList("option-input");
        backgroundColorInput.value = new Color(0,0,0,1);
        
        var fontSizeContainer = new VisualElement();
        fontSizeContainer.AddToClassList("option-container");
        var fontSizeTitle = new Label("Font size: ");
        fontSizeTitle.AddToClassList("option-title");
        fontSizeInput = new IntegerField();
        fontSizeInput.AddToClassList("option-input");
        
        nameContainer.Add(nameTitle);
        nameContainer.Add(nameInput);
        
        fontColorContainer.Add(fontColorTitle);
        fontColorContainer.Add(fontColorInput);
        
        backgroundColorContainer.Add(backgroundColorTitle);
        backgroundColorContainer.Add(backgroundColorInput);
        
        fontSizeContainer.Add(fontSizeTitle);
        fontSizeContainer.Add(fontSizeInput);
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(nameContainer);
        settingsContainer.Add(fontColorContainer);
        settingsContainer.Add(backgroundColorContainer);
        settingsContainer.Add(fontSizeContainer);

        return settingsContainer;
    }

    private VisualElement CreateBottomContainer()
    {
        var bottomContainer = new VisualElement();
        bottomContainer.AddToClassList("bottom-container");
        
        var createSubtitleButton = new Button() { text = "Save" };
        createSubtitleButton.AddToClassList("create-subtitle-button");
        createSubtitleButton.clicked += () =>
        {
            if (nameInput.value.Length > 0)
            {
                var fileExists = ACC_JSONHelper.FileNameAlreadyExists("/ACC_JSONSubtitle/" + nameInput.value);
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
                            ACC_JSONHelper.RenameFile("/ACC_JSONSubtitle/" + oldName, "/ACC_JSONSubtitle/" + nameInput.value);
                            ConfigureJson();
                            break;
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
            }
        };

       /*var loadSubtitleButton = new Button() { text = "Load" };
        loadSubtitleButton.AddToClassList("create-subtitle-button");
        loadSubtitleButton.clicked += () =>
        {
            LoadJson();
        };*/
        
        bottomContainer.Add(createSubtitleButton);
        //bottomContainer.Add(loadSubtitleButton);

        var addSubtitlesContainer = new VisualElement();
        addSubtitlesContainer.AddToClassList("add-subtitles-container");
        
        var addSubtitlesLabel = new Label("Add Subtitles:");
        addSubtitlesLabel.AddToClassList("add-subtitles-label");
        
        var addSubtitle1 = new Button(){text = "+1"};
        addSubtitle1.AddToClassList("add-subtitle-button");
        addSubtitle1.clicked += () => { CreateRow(1, "Hello", 1); };
        var addSubtitle5 = new Button(){text = "+5"};
        addSubtitle5.AddToClassList("add-subtitle-button");
        addSubtitle5.clicked += () => { CreateRow(5, "Hello", 1); };
        var addSubtitle10 = new Button(){text = "+10"};
        addSubtitle10.AddToClassList("add-subtitle-button");
        addSubtitle10.clicked += () => { CreateRow(10, "Hello", 1); };
        
        addSubtitlesContainer.Add(addSubtitlesLabel);
        addSubtitlesContainer.Add(addSubtitle1);
        addSubtitlesContainer.Add(addSubtitle5);
        addSubtitlesContainer.Add(addSubtitle10); 
        bottomContainer.Add(addSubtitlesContainer);

        return bottomContainer;
    }

    private void ConfigureJson()
    {
        ACC_SubtitleData subtitleData = new ACC_SubtitleData();
        for (int i = 1; i < table.childCount; i++)
        {
            var row = table[i];
            var subtitleElement = row.Query<TextField>().First();
            subtitleData.subtitleText.Add(new ACC_KeyValuePairData<int, string>(i, subtitleElement.value));

            var timeElement = row.Query<IntegerField>().First();
            subtitleData.timeText.Add(new ACC_KeyValuePairData<int, int>(i, timeElement.value));
        }

        subtitleData.name = nameInput.value;
        subtitleData.fontColor = fontColorInput.value;
        subtitleData.backgroundColor = backgroundColorInput.value;
        subtitleData.fontSize = fontSizeInput.value;
        
        ACC_JSONHelper.CreateJson(subtitleData, "/ACC_JSONSubtitle/");
        
        if(isEditing) oldName = nameInput.value;
    }

    public void LoadJson(string name)
    {
        string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_JSONSubtitle/" + name + ".json");
        ACC_SubtitleData subtitleData = JsonUtility.FromJson<ACC_SubtitleData>(json);

        List<VisualElement> rows = new List<VisualElement>();
        bool isFirstRow = true; 
        foreach (VisualElement child in table.Children())
        {
            if (!isFirstRow) rows.Add(child);
            isFirstRow = false;
        }
        
        foreach (VisualElement row in rows)
        {
            table.Remove(row);
        }

        if (isEditing) oldName = subtitleData.name;

        for (int i = 0; i < subtitleData.subtitleText.Count; i++)
        {
            CreateRow(1, subtitleData.subtitleText[i].value, subtitleData.timeText[i].value);
        }

        nameInput.value = subtitleData.name;
        fontSizeInput.value = subtitleData.fontSize;
        fontColorInput.value = subtitleData.fontColor;
        backgroundColorInput.value = subtitleData.backgroundColor;
    }
}
