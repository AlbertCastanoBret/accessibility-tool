using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_AudioManagerEditorWindow : ACC_BaseFloatingWindow<ACC_AudioManagerEditorWindowController, ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_AudioManagerEditorWindow>();
        window.titleContent = new GUIContent("Audio Manager");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        
        window.controller.isEditing = true;
        window.controller.LoadOnlyEditableWindow("AudioManager");
    }
    
private new void CreateGUI()
    {
        base.CreateGUI();
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Sound/ACC_AudioManagerEditorWindowStyles.uss");
        //rootVisualElement.styleSheets.Add(styleSheet);
        
        
    }
}
