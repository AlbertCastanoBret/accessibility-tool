#if UNITY_EDITOR
using System;
using System.Reflection;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Subtitles;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_SubtitlesEditorWindow : ACC_BaseFloatingWindow<ACC_SubtitlesEditorWindowController, ACC_SubtitlesEditorWindow, ACC_SubtitleData>
{
    private VisualElement table;
    public delegate void SubtitleWindowDelegate();
    public static event SubtitleWindowDelegate OnCloseSubtitleWindow;
    
    private new void OnDestroy()
    {
        base.OnDestroy();
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
            window.controller.isEditing = true;
            window.controller.LoadJson(name);
        }
        window.controller.lastData = window.controller.currentData.Clone() as ACC_SubtitleData;
        window.PositionWindowInBottomRight();
        window.SetFixedPosition();
    }
    
    private new void CreateGUI()
    {
        base.CreateGUI();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Subtitles/ACC_SubtitlesEditorWindowStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        CreateTable();
        if(!controller.isEditing) CreateRow(1, "Hello", 1);
        CreateSettingsContainer();
        CreateBottomContainer();
        
        controller.RestoreDataAfterCompilation();
    }

    private void CreateTable()
    {
        var tableContainer = uiElementFactory.CreateVisualElement("container");
        var subtitlesTitle = uiElementFactory.CreateLabel("title", "Subtitles");
        var tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view", table);
        
        table = uiElementFactory.CreateVisualElement("subtitles-table");
        var mainRow = uiElementFactory.CreateVisualElement("main-row");
        var subtitles = uiElementFactory.CreateLabel("subtitles-cell", "Subtitles");
        var time = uiElementFactory.CreateLabel("time-cell", "Time");
        
        mainRow.Add(subtitles);
        mainRow.Add(time);
        table.Add(mainRow);
        tableScrollView.Add(table);
        
        tableContainer.Add(subtitlesTitle);
        tableContainer.Add(tableScrollView);
        
        rootVisualElement.Add(tableContainer);
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
            var deleteButton = uiElementFactory.CreateButton("-", "delete-row-button", () =>
            {
                table.Q(name: newRow.name).RemoveFromHierarchy();
                controller.currentData.subtitleText.Remove(currentRow);
                controller.currentData.timeText.Remove(currentRow);
                if (table.childCount-1 > currentRow + 1)
                {
                    for (var j = currentRow + 1; j < table.childCount; j++)
                    {
                        controller.currentData.subtitleText.AddOrUpdate(j - 1, controller.currentData.subtitleText.Items.Find(x => x.key == j).value);
                        controller.currentData.timeText.AddOrUpdate(j - 1, controller.currentData.timeText.Items.Find(x => x.key == j).value);
                        controller.currentData.subtitleText.Remove(j);
                        controller.currentData.timeText.Remove(j);
                    }
                }
            });
            
            newRow.Add(subtitleField);
            newRow.Add(timeField); 
            newRow.Add(deleteButton);
            
            table.Add(newRow);
            rootVisualElement.schedule.Execute(() => { subtitleField[0].Focus(); }).StartingIn((long)0.001);
        }
    }
    private void CreateSettingsContainer()
    {
        var settingsContainer = uiElementFactory.CreateVisualElement("container-2");
        
        var settingsTitle = uiElementFactory.CreateLabel("title", "Settings");
        var nameInput = uiElementFactory.CreateTextField( "option-input-name", "Name: ", "", "option-input-name-label", 
            value => controller.currentData.name = value);
        var fontColorInput = uiElementFactory.CreateColorField("option-input", "Font Color:", Color.black, "option-input-label",
            value => controller.currentData.fontColor = value);
        var backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background color:", Color.white, "option-input-label",
            value => controller.currentData.backgroundColor = value);
        var fontSizeContainer =
            uiElementFactory.CreateSliderWithIntegerField("option-multi-input-last", "Font size:", 20, 100, 40,
                onValueChanged: value => controller.currentData.fontSize = value);
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(nameInput);
        settingsContainer.Add(fontColorInput);
        settingsContainer.Add(backgroundColorInput);
        settingsContainer.Add(fontSizeContainer);

        rootVisualElement.Add(settingsContainer);
    }
    private void CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
        bottomContainer.style.marginTop = new StyleLength(Length.Auto());
        var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));

        var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
        var addSubtitlesLabel = uiElementFactory.CreateLabel("add-row-label", "Add subtitles:");
        
        var addSubtitle1 = uiElementFactory.CreateButton("+1", "add-row-button", () => CreateRow(1, "Hello", 1));
        var addSubtitle5 = uiElementFactory.CreateButton("+5", "add-row-button", () => CreateRow(5, "Hello", 1));
        var addSubtitle10 = uiElementFactory.CreateButton("+10", "add-row-button", () => CreateRow(10, "Hello", 1));
        
        addSubtitlesContainer.Add(addSubtitlesLabel);
        addSubtitlesContainer.Add(addSubtitle1);
        addSubtitlesContainer.Add(addSubtitle5);
        addSubtitlesContainer.Add(addSubtitle10); 
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(addSubtitlesContainer);

        rootVisualElement.Add(bottomContainer);
    }
}
#endif
