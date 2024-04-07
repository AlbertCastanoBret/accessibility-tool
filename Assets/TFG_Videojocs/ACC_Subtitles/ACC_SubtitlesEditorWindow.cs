using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class ACC_SubtitlesEditorWindow : EditorWindow
{
    private VisualElement table;
    private TextField nameInput;
    private ColorField fontColorInput;
    private ColorField backgroundColorInput;
    private SliderInt fontSizeInput;
    
    private bool isEditing, isClosing;
    private string oldName;
    private ACC_SubtitleData lastSubtitleData;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseSubtitleWindow;

    private void OnEnable()
    {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
    }

    private void OnDisable()
    {
        CompilationPipeline.compilationStarted -= OnCompilationStarted;
    }
    
    private void OnCompilationStarted(object obj)
    {
        var container = new ACC_PreCompilationDataStorage();
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("name", nameInput.value));
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("fontColor", ColorUtility.ToHtmlStringRGBA(fontColorInput.value)));
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("backgroundColor", ColorUtility.ToHtmlStringRGBA(backgroundColorInput.value)));
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("fontSize", fontSizeInput.value.ToString()));
        
        for (int i = 1; i < table.childCount; i++)
        {
            var row = table[i];
            var subtitleElement = row.Query<TextField>().First();
            var timeElement = row.Query<IntegerField>().First();
            container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("subtitleText" + i, subtitleElement.value));
            container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("timeText" + i, timeElement.value.ToString()));
        }
        
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("isEditing", isEditing.ToString()));
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("oldName", oldName));
        container.keyValuePairs.Add(new ACC_KeyValuePairData<string, string>("lastSubtitleData", JsonUtility.ToJson(lastSubtitleData)));
        
        var json = JsonUtility.ToJson(container);
        SessionState.SetString("tempData", json);
    }

    private void OnDestroy()
    {
        ConfirmSaveChangesIfNeeded();
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
        ColorUtility.TryParseHtmlString("#4f4f4f", out var backgroundColor);
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
        
        lastSubtitleData = new ACC_SubtitleData();
        for (int i = 1; i < table.childCount; i++)
        {
            var row = table[i];
            var subtitleElement = row.Query<TextField>().First();
            lastSubtitleData.subtitleText.Add(new ACC_KeyValuePairData<int, string>(i, subtitleElement.value));

            var timeElement = row.Query<IntegerField>().First();
            lastSubtitleData.timeText.Add(new ACC_KeyValuePairData<int, int>(i, timeElement.value));
        }
        lastSubtitleData.name = nameInput.value;
        lastSubtitleData.fontColor = fontColorInput.value;
        lastSubtitleData.backgroundColor = backgroundColorInput.value;
        lastSubtitleData.fontSize = fontSizeInput.value;
        
        RestoreDataAfterCompile();
    }

    private void RestoreDataAfterCompile()
    {
        var serializedData = SessionState.GetString("tempData", "");
        if (serializedData != "")
        {
            var tempData = JsonUtility.FromJson<ACC_PreCompilationDataStorage>(serializedData);
            nameInput.value = tempData.keyValuePairs[0].value;
            
            ColorUtility.TryParseHtmlString("#" + tempData.keyValuePairs[1].value, out var fontColor);
            fontColorInput.value = new Color(fontColor.r, fontColor.g, fontColor.b, fontColor.a);
            
            ColorUtility.TryParseHtmlString("#" + tempData.keyValuePairs[2].value, out var newBackgroundColor);
            backgroundColorInput.value = new Color(newBackgroundColor.r, newBackgroundColor.g, newBackgroundColor.b, newBackgroundColor.a);
            
            fontSizeInput.value = int.TryParse(tempData.keyValuePairs[3].value, out var fontSize) ? fontSize : 20;
            table.Remove(table[1]);
            
            for (int i = 4; i < tempData.keyValuePairs.Count; i += 2)
            {
                if (tempData.keyValuePairs[i].key.Contains("subtitleText"))
                {
                    CreateRow(1, tempData.keyValuePairs[i].value, int.TryParse(tempData.keyValuePairs[i + 1].value, out var time) ? time : 1);
                }
            }
            
            isEditing = bool.TryParse(tempData.keyValuePairs[^3].value, out var editing) && editing;
            oldName = tempData.keyValuePairs[^2].value;
            lastSubtitleData = JsonUtility.FromJson<ACC_SubtitleData>(tempData.keyValuePairs[^1].value);
        }
        SessionState.EraseString("tempData");
        
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
            
            newRow.Add(subtitleField);
            
            var timeField = new IntegerField();
            timeField.value = time;
            timeField.AddToClassList("time-new-cell");
            timeField[0].AddToClassList("time-input-cell");
            newRow.Add(timeField); 
            
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
        
        nameContainer.Add(nameTitle);
        nameContainer.Add(nameInput);
        
        fontColorContainer.Add(fontColorTitle);
        fontColorContainer.Add(fontColorInput);
        
        backgroundColorContainer.Add(backgroundColorTitle);
        backgroundColorContainer.Add(backgroundColorInput);

        fontSizeContainer.Add(fontSizeInput);
        fontSizeContainer.Add(fontSizeField);
        
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
            HandleSave();
        };
        bottomContainer.Add(createSubtitleButton);

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

    private void ConfigureJSON()
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
        lastSubtitleData = subtitleData;
        
        if(isEditing) oldName = nameInput.value;
    }

    public void LoadJson(string name)
    {
        string path = "/ACC_JSONSubtitle/" + name;
        ACC_SubtitleData subtitleData = ACC_JSONHelper.LoadJson<ACC_SubtitleData>(path);

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
        fontColorInput.value = subtitleData.fontColor;
        backgroundColorInput.value = subtitleData.backgroundColor;
        fontSizeInput.value = subtitleData.fontSize;
        
        lastSubtitleData = subtitleData;
    }

    private void HandleSave()
    {
        if (nameInput.value.Length > 0)
        {
            var fileExists = ACC_JSONHelper.FileNameAlreadyExists("/ACC_JSONSubtitle/" + nameInput.value);
            if (!fileExists && !isEditing || fileExists && isEditing && nameInput.value == oldName)
            {
                ConfigureJSON();
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
                        ConfigureJSON();
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
                        ConfigureJSON();
                        break;
                    case 1:
                        if(isClosing) Cancel();
                        break;
                    case 2:
                        ACC_JSONHelper.RenameFile("/ACC_JSONSubtitle/" + oldName, "/ACC_JSONSubtitle/" + nameInput.value);
                        ConfigureJSON();
                        break;
                }
            }
            if (!isEditing && !isClosing) Close();
        }
        else
        {
            EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
            if(isClosing) Cancel();
        }
    }

    private bool IsThereAnyChange()
    {
        if (lastSubtitleData.name != nameInput.value) return true;
        if (lastSubtitleData.fontColor != fontColorInput.value) return true;
        if (lastSubtitleData.backgroundColor != backgroundColorInput.value) return true;
        if (lastSubtitleData.fontSize != fontSizeInput.value) return true;
        if (lastSubtitleData.subtitleText.Count != table.childCount - 1) return true;
        for (int i = 1; i < table.childCount; i++)
        {
            var row = table[i];
            var subtitleElement = row.Query<TextField>().First();
            var timeElement = row.Query<IntegerField>().First();
            if (lastSubtitleData.subtitleText[i - 1].value != subtitleElement.value) return true;
            if (lastSubtitleData.timeText[i - 1].value != timeElement.value) return true;
        }
        return false;
    }

    private void Cancel()
    {
        var window = Instantiate(this);
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        window.Show();
        
        window.lastSubtitleData = lastSubtitleData;
        window.nameInput.value = nameInput.value;
        window.fontColorInput.value = fontColorInput.value;
        window.backgroundColorInput.value = backgroundColorInput.value;
        window.fontSizeInput.value = fontSizeInput.value;
        window.table.Remove(window.table[1]);
        for (int i = 1; i < table.childCount; i++)
        {
            window.CreateRow(1, table[i].Query<TextField>().First().text, table[i].Query<IntegerField>().First().value);
        }
        
        if (isEditing)
        {
            window.oldName = oldName;
            window.isEditing = true;
        }
    }
    
    private void ConfirmSaveChangesIfNeeded()
    {
        if (IsThereAnyChange())
        {
            var result = EditorUtility.DisplayDialogComplex("Subtitles file has been modified",
                $"Do you want to save the changes you made in:\n./ACC_JSONSubtitle/{nameInput.value}.json\n\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
            switch (result)
            {
                case 0:
                    isClosing = true;
                    HandleSave();
                    OnCloseSubtitleWindow?.Invoke();
                    break;
                case 1:
                    Cancel();
                    break;
                case 2:
                    break;
            }
        }
    }
}
