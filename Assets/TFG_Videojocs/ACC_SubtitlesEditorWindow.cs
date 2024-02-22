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

public class ACC_SubtitlesEditorWindow : EditorWindow
{
    private VisualElement table;
    private ColorField fontColorInput;
    private ColorField backgroundColorInput;
    private IntegerField fontSizeInput;
    public static void ShowWindow()
    {
        //var window = CreateInstance<ACC_SubtitlesEditorWindow>();
        var window = GetWindow<ACC_SubtitlesEditorWindow>();
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 500);
        window.maxSize = new Vector2(600, 500);
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

        var subtitlesTitle = new Label("Subtitles");
        subtitlesTitle.AddToClassList("subtitles-title");
            
        table = CreateTable();
        CreateRow(1);
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
        //var fontColor = new Label("Color");
        
        table.AddToClassList("table");
        mainRow.AddToClassList("main-row");
        subtitles.AddToClassList("subtitles-cell");
        time.AddToClassList("time-cell");
        //fontColor.AddToClassList("font-color-cell");
        
        mainRow.Add(subtitles);
        mainRow.Add(time);
        //mainRow.Add(fontColor);
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
            timeField.value = 1;
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
        
        var fontColorContainer = new VisualElement();
        fontColorContainer.AddToClassList("option-container");
        var fontColorTitle = new Label("Font color:");
        fontColorTitle.AddToClassList("option-title");
        fontColorInput = new ColorField();
        fontColorInput.AddToClassList("option-input");
        
        var backgroundColorContainer = new VisualElement();
        backgroundColorContainer.AddToClassList("option-container");
        var backgroundColorTitle = new Label("Background color:");
        backgroundColorTitle.AddToClassList("option-title");
        backgroundColorInput = new ColorField();
        backgroundColorInput.AddToClassList("option-input");
        
        var fontSizeContainer = new VisualElement();
        fontSizeContainer.AddToClassList("option-container");
        var fontSizeTitle = new Label("Font size: ");
        fontSizeTitle.AddToClassList("option-title");
        fontSizeInput = new IntegerField();
        fontSizeInput.AddToClassList("option-input");
        
        fontColorContainer.Add(fontColorTitle);
        fontColorContainer.Add(fontColorInput);
        
        backgroundColorContainer.Add(backgroundColorTitle);
        backgroundColorContainer.Add(backgroundColorInput);
        
        fontSizeContainer.Add(fontSizeTitle);
        fontSizeContainer.Add(fontSizeInput);
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(fontColorContainer);
        settingsContainer.Add(backgroundColorContainer);
        settingsContainer.Add(fontSizeContainer);

        return settingsContainer;
    }

    private VisualElement CreateBottomContainer()
    {
        var bottomContainer = new VisualElement();
        bottomContainer.AddToClassList("bottom-container");
        
        var createSubtitleButton = new Button() { text = "Create Subtitle" };
        createSubtitleButton.AddToClassList("create-subtitle-button");
        createSubtitleButton.clicked += () =>
        {
          CreateJson();
          CreateSubtitleManager();
        };

        var loadSubtitleButton = new Button() { text = "Load" };
        loadSubtitleButton.AddToClassList("create-subtitle-button");
        loadSubtitleButton.clicked += () =>
        {
            LoadJson();
        };
        
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(loadSubtitleButton);

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
        bottomContainer.Add(addSubtitlesContainer);

        return bottomContainer;
    }

    private void CreateJson()
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

        subtitleData.fontColor = fontColorInput.value;
        subtitleData.backgroundColor = backgroundColorInput.value;
        subtitleData.fontSize = fontSizeInput.value;
        
        string json = JsonUtility.ToJson(subtitleData, true);
        File.WriteAllText("Assets/TFG_Videojocs/JSONSubtitles.json", json);
        AssetDatabase.Refresh();
        
        //ACC_AccessibilityManager.Instance.accAudioAccessibility.CreateSubtitle();
    }
    
    private void CreateSubtitleManager()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject subtitleManager = GameObject.Find("SubtitleText");
        Text subtitleText = FindObjectOfType<Text>();
        ACC_SubtitlesManager accSubtitleManager = FindObjectOfType<ACC_SubtitlesManager>();
        
        if (subtitleManager == null)
        {
            subtitleManager = new GameObject("SubtitleText");
            subtitleManager.transform.SetParent(canvasObject.transform, false);

            if (subtitleText == null)
            {
                subtitleText = subtitleManager.AddComponent<Text>();
                subtitleText.alignment = TextAnchor.LowerCenter;
                subtitleText.horizontalOverflow = HorizontalWrapMode.Wrap;
            }
            
            RectTransform subtitleTextRectTransform = subtitleText.GetComponent<RectTransform>();
            subtitleTextRectTransform.anchorMin = new Vector2(0.1f, 0);
            subtitleTextRectTransform.anchorMax = new Vector2(0.9f, 0);
            subtitleTextRectTransform.pivot = new Vector2(0.5f, 0);
            subtitleTextRectTransform.anchoredPosition = new Vector2(0, 50);
            subtitleTextRectTransform.sizeDelta = new Vector2(0, 40);
            
            if (accSubtitleManager == null)
            {
                accSubtitleManager = subtitleManager.AddComponent<ACC_SubtitlesManager>();
                accSubtitleManager.subtitleText = subtitleText;
            }
        }
        
        subtitleText.fontSize = fontSizeInput.value;
        subtitleText.color = new Color(fontColorInput.value.r, fontColorInput.value.g, fontColorInput.value.b);
        
        accSubtitleManager.LoadSubtitles("JSONSubtitles.json");
        accSubtitleManager.EnableSubtitles();
    }

    private void LoadJson()
    {
        string json = File.ReadAllText("Assets/TFG_Videojocs/JSONSubtitles.json");
        ACC_SubtitleData subtitleData = JsonUtility.FromJson<ACC_SubtitleData>(json);

        fontSizeInput.value = subtitleData.fontSize;
    }
}
