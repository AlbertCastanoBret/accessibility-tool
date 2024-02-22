using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_Window : EditorWindow
{
    private VisualElement accessibilityContainer;
    private const int numberOfColumns = 2;
    
    [MenuItem("Tools/ACC/Accessibility Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_Window>("Accessibility Window");
        window.minSize = new Vector2(400, 300);
    }

    public void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_WindowStyles.uss");
        var toolbar = new Toolbar();
        var titleToolbar = new Label("Accessibility Plugin");
        var mainContainer = new VisualElement();
        var sidebar = new VisualElement();
        var audibleButton = new Button() { text = "Auditive" };
        var visualButton = new Button() { text = "Visual" };
        accessibilityContainer = new VisualElement();
        var accessibilityRow = new VisualElement();
        
        toolbar.AddToClassList("toolbar");
        titleToolbar.AddToClassList("toolbar-title");
        mainContainer.AddToClassList("main-container");
        sidebar.AddToClassList("my-box");
        audibleButton.AddToClassList("my-button");
        visualButton.AddToClassList("my-button");
        accessibilityContainer.AddToClassList("accessibility-container");
        accessibilityRow.AddToClassList("accessibility-row");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.style.minWidth = new StyleLength(1000);
        
        toolbar.Add(titleToolbar);
        
        audibleButton.clicked += () => { UpdateAccessibilityContainer(typeof(AuditiveFeatures));};
        visualButton.clicked += () => { UpdateAccessibilityContainer(typeof(VisualFeatures)); };
        
        sidebar.Add(audibleButton);
        sidebar.Add(visualButton);
        mainContainer.Add(sidebar);
        mainContainer.Add(accessibilityContainer);
        rootVisualElement.Add(toolbar);
        rootVisualElement.Add(mainContainer);
    }
    
    private void UpdateAccessibilityContainer(Type featureType)
    {
        int numberOfRows = (Enum.GetNames(featureType).Length%2==0 ? Enum.GetNames(featureType).Length : Enum.GetNames(featureType).Length + 1) / numberOfColumns;
        accessibilityContainer.Clear();
        for (int i = 0; i < numberOfRows; i++)
        {
            CreateAccessibilityRow(featureType, i);
        }
    }
    
    private void CreateAccessibilityRow(Type featureType, int rowIndex)
    {
        var row = new VisualElement();
        row.AddToClassList("accessibility-row");
        for (int i = 0; i < numberOfColumns; i++)
        {
            if(Enum.GetName(featureType, numberOfColumns*rowIndex+i)!=null) row.Add(CreateFeatureBox(featureType, numberOfColumns*rowIndex+i));
        }
        accessibilityContainer.Add(row);
    }

    private VisualElement CreateFeatureBox(Type featuretype, int index)
    {
        var box = new VisualElement();
        box.AddToClassList("red-box");
        box.style.width = new Length(92f/numberOfColumns, LengthUnit.Percent);

        var titleBox = new Label(Enum.GetName(featuretype, index));
        titleBox.AddToClassList("title-box");
        box.Add(titleBox);

        if(featuretype == typeof(AuditiveFeatures)) CreateAudioBox(box, index);

        return box;
    }

    private void CreateAudioBox(VisualElement box, int index)
    {
        switch (Enum.GetName(typeof(AuditiveFeatures), index))
        {
            case "Subtitles":
                SubtitlesBox(box);
                break;
        }
    }

    private void SubtitlesBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        var options = new List<string> { "Create a subtitle", "Add accessbility to existent subtitle", "Edit subtitle" };
        var dropdown = new DropdownField("Options:", options, 0);
                
        dropdown.AddToClassList("dropdown-container");
        dropdown[0].AddToClassList("dropdown-list");
                
        box.Add(dropdown);
        box.Add(dynamicContainer);
        
        var subtitleCreation = CreateASubtitle();
        dynamicContainer.Add(subtitleCreation);
        subtitleCreation.style.display = DisplayStyle.Flex;
                
        dropdown.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == "Create a subtitle")
            {
                subtitleCreation.style.display = DisplayStyle.Flex;
            }
            else if (evt.newValue == "Add accessbility to existent subtitle")
            {
                subtitleCreation.style.display = DisplayStyle.None;
            }
        });

    }

    private VisualElement CreateASubtitle()
    {
        var subtitleCreationContainer = new VisualElement();
        subtitleCreationContainer.AddToClassList("subtitle-creation-container");
        
        //var toggleContainer = new VisualElement();
        //toggleContainer.AddToClassList("canvas-toggle-container");

        //var hasCanvasLabel = new Label("Is there a Canvas already created?");
        //hasCanvasLabel.AddToClassList("canvas-label");
        
        //var hasCanvasToggle = new Toggle();
        //hasCanvasToggle.AddToClassList("canvas-toggle");
        
        //toggleContainer.Add(hasCanvasLabel);
        //toggleContainer.Add(hasCanvasToggle);
        
        //subtitleCreationContainer.Add(toggleContainer);
        
        var canvasSelectionContainer = CreateCanvasSelection();
        subtitleCreationContainer.Add(canvasSelectionContainer);
        canvasSelectionContainer.style.display = DisplayStyle.None;

        var createSubtitlesButton = new Button() { text = "Create" };
        createSubtitlesButton.AddToClassList("create-subtitles-button");
        subtitleCreationContainer.Add(createSubtitlesButton);

        //hasCanvasToggle.RegisterValueChangedCallback(evt =>
        //{
            //if(evt.newValue) canvasSelectionContainer.style.display = DisplayStyle.Flex;
            //else canvasSelectionContainer.style.display = DisplayStyle.None;
        //});
        
        createSubtitlesButton.clicked += () =>
        {
            ACC_SubtitlesEditorWindow.ShowWindow();
        };

        return subtitleCreationContainer;
    }

    private VisualElement CreateCanvasSelection()
    {
        var canvasSelectionContainer = new VisualElement();
        canvasSelectionContainer.AddToClassList("canvas-selection-container");
        
        var canvasTitle = new Label("Select a Canvas: ");
        canvasTitle.AddToClassList("canvas-title");
        var canvasField = new ObjectField()
        {
            objectType = typeof(GameObject),
            allowSceneObjects = true
        };
        canvasField.AddToClassList("canvas-field");
        canvasSelectionContainer.Add(canvasTitle);
        canvasSelectionContainer.Add(canvasField);
        
        return canvasSelectionContainer;
    }
}
