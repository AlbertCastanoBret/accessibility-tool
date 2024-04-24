using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_AudioManagerEditorWindow : ACC_BaseFloatingWindow<ACC_AudioManagerEditorWindowController, ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    private VisualElement tableContainer;
    private ReorderableList reorderableList;
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_AudioManagerEditorWindow>();
        window.titleContent = new GUIContent("Audio Manager");
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        
        window.controller.isEditing = true;
        window.controller.LoadOnlyEditableWindow("AudioManager");
        window.PositionWindowInBottomRight();
        window.SetFixedPosition();
    }
    
    private new void CreateGUI()
    {
        base.CreateGUI();

        CreateTable2();
        controller.RestoreDataAfterCompilation();
    }
    
    private void CreateTable()
    {
        if(tableContainer == null) tableContainer = uiElementFactory.CreateVisualElement("container-2");
        else tableContainer.Clear();
            
        var highContrastTitle = uiElementFactory.CreateLabel("title", "Audio Settings");
        var tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
            
        var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
        containerTableTitle.style.width = new StyleLength(Length.Percent(95));
        var containerTableNameTitle = uiElementFactory.CreateLabel("table-title-name", "Audio Sources");
            
        containerTableTitle.Add(containerTableNameTitle);
        tableScrollView.Add(containerTableTitle);
            
        for(int i=0; i<1; i++)
        {
            var row = uiElementFactory.CreateVisualElement("table-row");
            
            var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
            var icon = uiElementFactory.CreateImage("table-cell-icon",
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Sound/d_AudioSource Icon.png"));
            var name = uiElementFactory.CreateLabel("table-cell", "New Audio Source");
            name.style.width = new StyleLength(Length.Percent(95));
                
            var deleteButton = uiElementFactory.CreateButton("-","table-delete-button");
            
            tableCell.Add(icon);
            tableCell.Add(name);
            row.Add(tableCell);
            row.Add(deleteButton);
            tableScrollView.Add(row);
        }
            
        tableContainer.Add(highContrastTitle);
        tableContainer.Add(tableScrollView);
        rootVisualElement.Add(tableContainer);
    }
    
    private void CreateTable2()
    {
        if(tableContainer == null) tableContainer = uiElementFactory.CreateVisualElement("container-2");
        else tableContainer.Clear();
    
        var highContrastTitle = uiElementFactory.CreateLabel("title", "Audio Settings");
        var tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
    
        reorderableList = new ReorderableList(controller.currentData.audioClips, typeof(ACC_Sound), true, true, true, true);
        reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Audio Sources");
        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };
        
        var reorderableListContainer = new IMGUIContainer(() => reorderableList.DoLayoutList());
        
        tableScrollView.Add(reorderableListContainer);
        tableContainer.Add(highContrastTitle);
        tableContainer.Add(tableScrollView);
        rootVisualElement.Add(tableContainer);
    }
}
