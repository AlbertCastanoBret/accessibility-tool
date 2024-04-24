using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_AudioManagerEditorWindow : ACC_BaseFloatingWindow<ACC_AudioManagerEditorWindowController, ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    private VisualElement tableContainer, tableScrollView;
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
        
        tableContainer = uiElementFactory.CreateVisualElement("container-2");
        rootVisualElement.Add(tableContainer);
        CreateTable();
        CreateBottomContainer();
        
        controller.RestoreDataAfterCompilation();
    }
    
    public void CreateTable()
    {
        tableContainer.Clear();
            
        var highContrastTitle = uiElementFactory.CreateLabel("title", "Audio Settings");
        tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
            
        var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
        var containerTableNameTitle = uiElementFactory.CreateLabel("table-title-name", "Audio Sources");
            
        containerTableTitle.Add(containerTableNameTitle);
        tableScrollView.Add(containerTableTitle);
        
        for(int i=0; i<controller.currentData.audioSources.Items.Count; i++)
        {
            AddNewAudioSource(controller.currentData.audioSources.Items.Find(x => x.key == i).value, i+1);
        }
            
        tableContainer.Add(highContrastTitle);
        tableContainer.Add(tableScrollView);
    }
    private void AddNewAudioSource(string name = "New Audio Source", int index=-1)
    {
        var currentRow = tableScrollView.childCount-1;
        var row = uiElementFactory.CreateVisualElement("table-row");
            
        var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
        var icon = uiElementFactory.CreateImage("table-cell-icon",
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Sound/d_AudioSource Icon.png"));
        var nameField = uiElementFactory.CreateTextField("table-cell", "", name, "table-cell-input",
            (value) => controller.currentData.audioSources.AddOrUpdate(index!=-1?index-1:currentRow, value));
        nameField.style.width = new StyleLength(Length.Percent(95));
            
        var addButton = uiElementFactory.CreateButton("+","table-add-button", () =>
        {
            var currentRow = tableScrollView.IndexOf(row)-1;
            if (tableScrollView.childCount - 1 > currentRow + 1)
            {
                for(var j = tableScrollView.childCount-2; j>currentRow; j--)
                {
                    controller.currentData.audioSources.AddOrUpdate(j+1, controller.currentData.audioSources.Items.Find(x => x.key == j).value);
                }
            } 
            AddNewAudioSource(index: tableScrollView.IndexOf(row)+1);
        });
        
        var deleteButton = uiElementFactory.CreateButton("-","table-delete-button", () =>
        {
            var currentRow = tableScrollView.IndexOf(row)-1;
            tableScrollView.Remove(row);
            controller.currentData.audioSources.Remove(currentRow);
            if (tableScrollView.childCount > currentRow + 1)
            {
                for (var j = currentRow + 1; j < tableScrollView.childCount; j++)
                {
                    controller.currentData.audioSources.AddOrUpdate(j - 1, controller.currentData.audioSources.Items.Find(x => x.key == j).value);
                    controller.currentData.audioSources.Remove(j);
                }
            }
        });
            
        tableCell.Add(icon);
        tableCell.Add(nameField);
        row.Add(tableCell);
        row.Add(addButton);
        row.Add(deleteButton);
        if(index!=-1) tableScrollView.Insert(index, row);
        else tableScrollView.Add(row);
        rootVisualElement.schedule.Execute(() => { nameField[0].Focus(); }).StartingIn((long)0.001);
    }
    private void CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
        bottomContainer.style.marginTop = new StyleLength(Length.Auto());
        var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));

        var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
        var addSubtitle1 = uiElementFactory.CreateButton("Add New Audio Source", "add-row-button", () => AddNewAudioSource());
        
        addSubtitlesContainer.Add(addSubtitle1);
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(addSubtitlesContainer);

        rootVisualElement.Add(bottomContainer);
    }
}
