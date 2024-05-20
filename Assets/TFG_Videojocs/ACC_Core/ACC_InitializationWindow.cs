#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TFG_Videojocs.ACC_RemapControls;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;

public class ACC_InitializationWindow : EditorWindow
{
    [MenuItem("Tools/ACC/Initialization Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_InitializationWindow>("Initialization Window");
        window.minSize = new Vector2(400, 600);
        window.maxSize = new Vector2(400, 600);
    }

    private void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Core/ACC_InitializationWindow.uss");

        var title = new Label("Accessibility Plugin");
        title.AddToClassList("title");
        
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Utilities/aiLogo.png");
        var image = new Image() {image = texture};
        image.AddToClassList("image");

        var text = new TextElement(){text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."};
        text.AddToClassList("text");
        
        var createButton = new Button() { text = "Create" };
        createButton.AddToClassList("button");
        createButton.clicked += () =>
        {
            CreateAccessibilityManager();
            Close();
        };
        
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.Add(title);
        rootVisualElement.Add(image);
        rootVisualElement.Add(text);
        rootVisualElement.Add(createButton);
        rootVisualElement.AddToClassList("root");
    }
    
    private static void CreateAccessibilityManager()
    {
        var accessibilityManager = GameObject.Find("ACC_AccessibilityManager");
        if (accessibilityManager) DestroyImmediate(accessibilityManager);
        accessibilityManager = new GameObject("ACC_AccessibilityManager");
        accessibilityManager.AddComponent<ACC_AccessibilityManager>();
    }
}
#endif
