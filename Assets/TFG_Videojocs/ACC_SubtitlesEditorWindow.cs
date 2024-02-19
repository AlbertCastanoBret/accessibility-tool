using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_SubtitlesEditorWindow : EditorWindow
{
    private VisualElement table;
    public static void ShowWindow()
    {
        //var window = CreateInstance<ACC_SubtitlesEditorWindow>();
        var window = GetWindow<ACC_SubtitlesEditorWindow>();
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 500);
        window.maxSize = new Vector2(600, 500);
        window.ShowModal();
    }
    
    private void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_SubtitlesWindowStyles.uss");
        var mainContainer = new VisualElement();
        var tableScrollView = new ScrollView();
        tableScrollView.AddToClassList("table-scroll-view");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#4f4f4f", out backgroundColor);
        rootVisualElement.style.backgroundColor = new StyleColor(backgroundColor);
        mainContainer.AddToClassList("main-container");

        table = CreateTable();
        CreateRow(10);
        tableScrollView.Add(table);
        mainContainer.Add(tableScrollView);

        var addSubtitlesContainer = new VisualElement();
        addSubtitlesContainer.AddToClassList("add-subtitles-container");
        
        var addSubtitlesLabel = new Label("Add Subtitles:");
        addSubtitlesLabel.AddToClassList("add-subtitles-label");
        
        var addSubtitle1 = new Button(){text = "+1"};
        addSubtitle1.AddToClassList("add-subtitle-button");
        addSubtitle1.clicked += () => { CreateRow(1); };
        var addSubtitle5 = new Button(){text = "+5"};
        addSubtitle5.AddToClassList("add-subtitle-button");
        addSubtitle5.clicked += () => { CreateRow(5); };
        var addSubtitle10 = new Button(){text = "+10"};
        addSubtitle10.AddToClassList("add-subtitle-button");
        addSubtitle10.clicked += () => { CreateRow(10); };
        
        addSubtitlesContainer.Add(addSubtitlesLabel);
        addSubtitlesContainer.Add(addSubtitle1);
        addSubtitlesContainer.Add(addSubtitle5);
        addSubtitlesContainer.Add(addSubtitle10);
        mainContainer.Add(addSubtitlesContainer);
        
        rootVisualElement.Add(mainContainer);
    }

    private VisualElement CreateTable()
    {
        var table = new VisualElement();
        var mainRow = new VisualElement();
        var subtitles = new Label("Subtitles");
        var time = new Label("Time");
        var fontColor = new Label("Color");
        
        table.AddToClassList("table");
        mainRow.AddToClassList("main-row");
        subtitles.AddToClassList("subtitles-cell");
        time.AddToClassList("time-cell");
        fontColor.AddToClassList("font-color-cell");
        
        mainRow.Add(subtitles);
        mainRow.Add(time);
        mainRow.Add(fontColor);
        table.Add(mainRow);

        return table;
    }

    private void CreateRow(int numberOfRows)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            var newRow = new VisualElement();
            newRow.AddToClassList("row");
            
            var subtitleField = new TextField();
            subtitleField.value = "Hello";
            subtitleField.AddToClassList("subtitles-new-cell");
            subtitleField[0].AddToClassList("subtitles-input-cell");
            newRow.Add(subtitleField);
            
            var timeField = new TextField();
            timeField.value = "1";
            timeField.AddToClassList("time-new-cell");
            timeField[0].AddToClassList("time-input-cell");
            newRow.Add(timeField);
                    
            var fontColorField = new ColorField("");
            fontColorField.AddToClassList("font-color-new-cell");
            newRow.Add(fontColorField);
            
            var deleteButton = new Button(() => table.Remove(newRow)) { text = "-" };
            deleteButton.AddToClassList("delete-row-button");
            newRow.Add(deleteButton);
            
            table.Add(newRow);
        }
    }
}
