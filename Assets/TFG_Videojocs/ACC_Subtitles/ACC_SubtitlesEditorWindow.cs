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

public class ACC_SubtitlesEditorWindow : ACC_BaseFloatingWindow<ACC_SubtitleEditorWindowController, ACC_SubtitlesEditorWindow, ACC_SubtitleData>
{
    private VisualElement table;
    
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
    
    
    /*private void OnCompilationStarted(object obj)
    {
        var container = new ACC_PreCompilationDataStorage();
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("name", nameInput.value));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("fontColor", ColorUtility.ToHtmlStringRGBA(fontColorInput.value)));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("backgroundColor", ColorUtility.ToHtmlStringRGBA(backgroundColorInput.value)));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("fontSize", fontSizeInput.ToString()));

        for (int i = 1; i < table.childCount; i++)
        {
            var row = table[i];
            var subtitleElement = row.Query<TextField>().First();
            var timeElement = row.Query<IntegerField>().First();
            container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("subtitleText" + i, subtitleElement.value));
            container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("timeText" + i, timeElement.value.ToString()));
        }

        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("isEditing", isEditing.ToString()));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("oldName", oldName));
        container.keyValuePairs.Add(new ACC_KeyValuePair<string, string>("lastSubtitleData", JsonUtility.ToJson(lastSubtitleData)));

        var json = JsonUtility.ToJson(container);
        SessionState.SetString("subtitle_tempData", json);
    }*/
    private void OnDestroy()
    {
        OnCloseSubtitleWindow?.Invoke();
        controller.ConfirmSaveChangesIfNeeded(controller.oldName, this);
    }

    public static void ShowWindow(string name)
    {
        var window = GetWindow<ACC_SubtitlesEditorWindow>();
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        if (name != null)
        {
            window.controller.isEditing = true;
            window.controller.LoadJson(name);
        }
        window.controller.lastData = window.controller.currentData.Clone() as ACC_SubtitleData;
        //window.ShowModal();
    }
    
    private new void CreateGUI()
    {
        base.CreateGUI();
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
        
        controller.RestoreDataAfterCompilation();
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
            int currentRow = table.childCount - 1;
            var newRow = uiElementFactory.CreateVisualElement("new-row");
            var subtitleField = uiElementFactory.CreateTextField(value: subtitle, classList: "subtitles-new-cell", subClassList: "subtitles-input-cell",
                onValueChanged: value => { controller.currentData.subtitleText.AddOrUpdate(currentRow, value); });
            var timeField = uiElementFactory.CreateIntegerField(value: time, classList: "time-new-cell", subClassList: "time-input-cell",
                onValueChanged: value => controller.currentData.timeText.AddOrUpdate(currentRow, value));
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
        var nameInput = uiElementFactory.CreateTextField( "option-input-name", "Name: ", "", "option-input-name-label", 
            value => controller.currentData.name = value);
        var fontColorInput = uiElementFactory.CreateColorField("option-input", "Font Color:", Color.black, "option-input-label",
            value => controller.currentData.fontColor = value);
        var backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background color:", Color.white, "option-input-label",
            value => controller.currentData.backgroundColor = value);
        var fontSizeContainer =
            uiElementFactory.CreateSliderWithIntegerField("option-slider", "Font size:", 10, 60, 20,
                onValueChanged: value => controller.currentData.fontSize = value);
        
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
}
