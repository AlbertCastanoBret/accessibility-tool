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
        
        toolbar.AddToClassList("my-toolbar");
        titleToolbar.AddToClassList("my-label");
        mainContainer.AddToClassList("main-container");
        sidebar.AddToClassList("my-box");
        audibleButton.AddToClassList("my-button");
        visualButton.AddToClassList("my-button");
        accessibilityContainer.AddToClassList("accessibility-container");
        accessibilityRow.AddToClassList("accessibility-row");
        
        rootVisualElement.styleSheets.Add(styleSheet);
        
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
                CreateSubtitlesBox(box);
                break;
        }
    }

    private void CreateSubtitlesBox(VisualElement box)
    {
        var dynamicContainer = new VisualElement();
        var options = new List<string> { "Create a subtitle", "Add accessbility to existent subtitle" };
        var dropdown = new DropdownField("Options", options, 0);
                
        dropdown.AddToClassList("dropdown-box");
                
        box.Add(dropdown);
        box.Add(dynamicContainer);
                
        dropdown.RegisterValueChangedCallback(evt =>
        {
            dynamicContainer.Clear();
            if (evt.newValue == "Create a subtitle")
            {
                var label = new Label("A");
                dynamicContainer.Add(label);
            }
            else if (evt.newValue == "Add accessbility to existent subtitle")
            {
                var label = new Label("B");
                dynamicContainer.Add(label);
            }
        });
                
        var label = new Label("A");
        dynamicContainer.Add(label);
    }
}
