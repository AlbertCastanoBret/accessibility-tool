using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_VisualNotificationEditorWindow : EditorWindow
{
    private DropdownField dropdownHorizontalAlignment, dropdownVerticalAlignment;
    private IntegerField timeOnScreen;
    
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_VisualNotificationEditorWindow>();
        window.titleContent = new GUIContent("Visual Notification Creation");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        //window.ShowModal();
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
        
        var createVisualNotificationButton = new Button() { text = "Create" };
        createVisualNotificationButton.AddToClassList("create-visual-notification-button");

        createVisualNotificationButton.clicked += () =>
        {
            CreateJson();
        };
        
        mainContainer.Add(dropdownHorizontalAlignment);
        mainContainer.Add(dropdownVerticalAlignment);
        mainContainer.Add(timeOnScreen);
        mainContainer.Add(createVisualNotificationButton);
        rootVisualElement.Add(mainContainer);
    }

    private void CreateJson()
    {
        ACC_VisualNotificationData accVisualNotificationData = new ACC_VisualNotificationData();
        accVisualNotificationData.horizontalAlignment = dropdownHorizontalAlignment.value;
        accVisualNotificationData.verticalAlignment = dropdownVerticalAlignment.value;
        accVisualNotificationData.timeOnScreen = timeOnScreen.value;
    }
}
