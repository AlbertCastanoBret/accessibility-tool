using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Subtitles;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_SubtitlesEditorWindow2 : ACC_BaseFloatingWindow<ACC_SubtitleEditorWindowController, ACC_SubtitlesEditorWindow2>
{
    private VisualElement table;
    private TextField nameInput;
    private ColorField fontColorInput;
    private ColorField backgroundColorInput;
    private SliderInt fontSizeInput;
    
    [SerializeField] private bool isEditing, isClosing;
    [SerializeField] private string oldName;
    [SerializeField] private ACC_SubtitleData lastSubtitleData;
    
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseSubtitleWindow;

    /*void OnEnable()
    {
        Debug.Log("OnEnable");
        //CompilationPipeline.compilationStarted += OnCompilationStarted;
    }

    private void OnDisable()
    {
        CompilationPipeline.compilationStarted -= OnCompilationStarted;
    }*/
    
    
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
        SessionState.SetString("subtitle_tempData", json);
    }

    private void OnDestroy()
    {
        controller.ConfirmSaveChangesIfNeeded(oldName, this);
    }

    public static void ShowWindow(string name)
    {
        var window = GetWindow<ACC_SubtitlesEditorWindow2>();
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        if (name != null)
        {
            window.controller.isEditing = true;
            window.isEditing = true;
            window.controller.LoadJson(name);
        }
        //window.ShowModal();
    }
    
    private void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Subtitles/ACC_SubtitlesWindowStyles.uss");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.AddToClassList("main-container");
        
        var subtitlesTitle= uiElementFactory.CreateLabel("subtitles-title", "Subtitles");
            
        CreateTable();
        CreateRow(1, "Hello", 1);
        
        var tableScrollView = uiElementFactory.CreateScrollView(table, "table-scroll-view");
        
        rootVisualElement.Add(subtitlesTitle);
        rootVisualElement.Add(tableScrollView);
        
        rootVisualElement.Add(CreateSettingsContainer());
        rootVisualElement.Add(CreateBottomContainer());
        
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
        
        //RestoreDataAfterCompilation();
    }

    private void RestoreDataAfterCompilation()
    {
        var serializedData = SessionState.GetString("subtitle_tempData", "");
        if (serializedData != "")
        {
            var tempData = JsonUtility.FromJson<ACC_PreCompilationDataStorage>(serializedData);
            nameInput.value = tempData.keyValuePairs[0].value;
            
            ColorUtility.TryParseHtmlString("#" + tempData.keyValuePairs[1].value, out var fontColor);
            fontColorInput.value = fontColor;
            
            ColorUtility.TryParseHtmlString("#" + tempData.keyValuePairs[2].value, out var newBackgroundColor);
            backgroundColorInput.value = newBackgroundColor;
            
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
        SessionState.EraseString("subtitle_tempData");
        
    }
    
    private void CreateTable()
    {
        table = uiElementFactory.CreateVisualElement("table");
        var mainRow = uiElementFactory.CreateVisualElement("main-row");
        var subtitles = uiElementFactory.CreateLabel("subtitles-cell", "Subtitles");
        var time = uiElementFactory.CreateLabel("time-cell", "Time");
        
        mainRow.Add(subtitles);
        mainRow.Add(time);
        table.Add(mainRow);
    }

    public void CreateRow(int numberOfRows, string subtitle, int time)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            var newRow = uiElementFactory.CreateVisualElement("new-row");
            var subtitleField = uiElementFactory.CreateTextField(value: subtitle, classList: "subtitles-new-cell", subClassList: "subtitles-input-cell");
            var timeField = uiElementFactory.CreateIntegerField(value: time, classList: "time-new-cell", subClassList: "time-input-cell");
            var deleteButton = uiElementFactory.CreateButton("-", "delete-row-button", () => table.Q(name: newRow.name).RemoveFromHierarchy());
            
            newRow.Add(subtitleField);
            newRow.Add(timeField); 
            newRow.Add(deleteButton);
            
            table.Add(newRow);
            rootVisualElement.schedule.Execute(() => { subtitleField[0].Focus(); }).StartingIn((long)0.001);
        }
    }

    private VisualElement CreateSettingsContainer()
    {
        var settingsContainer = uiElementFactory.CreateVisualElement("settings-container");
        
        var settingsTitle = uiElementFactory.CreateLabel("settings-title", "Settings");
        nameInput = uiElementFactory.CreateTextField( "option-input-name", "Name: ", "", "option-input-name-label");
        fontColorInput = uiElementFactory.CreateColorField("option-input", "Font Color:", Color.black, "option-input-label");
        backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background color:", Color.white, "option-input-label");
        
        var fontSizeContainer = uiElementFactory.CreateVisualElement("option-container");
        fontSizeInput = uiElementFactory.CreateSliderInt("font-size-slider","Font size:", 10, 60, 20,
            "font-size-label");
        
        var fontSizeField = uiElementFactory.CreateIntegerField(value: 20, classList: "font-size-field");
        
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
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(nameInput);
        settingsContainer.Add(fontColorInput);
        settingsContainer.Add(backgroundColorInput);
        settingsContainer.Add(fontSizeContainer);

        return settingsContainer;
    }

    private VisualElement CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("bottom-container");
        var createSubtitleButton = uiElementFactory.CreateButton("Save", "create-subtitle-button", () => controller.HandleSave(this));

        var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-subtitles-container");
        var addSubtitlesLabel = uiElementFactory.CreateLabel("add-subtitles-label", "Add subtitles:");
        
        var addSubtitle1 = uiElementFactory.CreateButton("+1", "add-subtitle-button", () => CreateRow(1, "Hello", 1));
        var addSubtitle5 = uiElementFactory.CreateButton("+5", "add-subtitle-button", () => CreateRow(5, "Hello", 1));
        var addSubtitle10 = uiElementFactory.CreateButton("+10", "add-subtitle-button", () => CreateRow(10, "Hello", 1));
        
        addSubtitlesContainer.Add(addSubtitlesLabel);
        addSubtitlesContainer.Add(addSubtitle1);
        addSubtitlesContainer.Add(addSubtitle5);
        addSubtitlesContainer.Add(addSubtitle10); 
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(addSubtitlesContainer);

        return bottomContainer;
    }

    /*private void ConfigureJSON()
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
    }*/

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

        //if (isEditing) oldName = subtitleData.name;

        for (int i = 0; i < subtitleData.subtitleText.Count; i++)
        {
            CreateRow(1, subtitleData.subtitleText[i].value, subtitleData.timeText[i].value);
        }

        nameInput.value = subtitleData.name;
        fontColorInput.value = subtitleData.fontColor;
        backgroundColorInput.value = subtitleData.backgroundColor;
        fontSizeInput.value = subtitleData.fontSize;
        
        //lastSubtitleData = subtitleData;
    }

    private void HandleSave()
    {
        if (nameInput.value.Length > 0)
        {
            var fileExists = ACC_JSONHelper.FileNameAlreadyExists("/ACC_JSONSubtitle/" + nameInput.value);
            if (!fileExists && !isEditing || fileExists && isEditing && nameInput.value == oldName)
            {
                controller.ConfigureJson();
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
                        controller.ConfigureJson();
                        break;
                    case 1:
                        if(isClosing) controller.Cancel(this);
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
                        controller.ConfigureJson();
                        break;
                    case 1:
                        if(isClosing) controller.Cancel(this);
                        break;
                    case 2:
                        ACC_JSONHelper.RenameFile("/ACC_JSONSubtitle/" + oldName, "/ACC_JSONSubtitle/" + nameInput.value);
                        controller.ConfigureJson();
                        break;
                }
            }
            if (!isEditing && !isClosing) Close();
        }
        else
        {
            EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
            if(isClosing) controller.Cancel(this);
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
}
