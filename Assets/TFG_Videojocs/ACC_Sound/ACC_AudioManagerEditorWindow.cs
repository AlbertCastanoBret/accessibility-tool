using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEditor;
using UnityEditor.UIElements;
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
            
        var mainRow = uiElementFactory.CreateVisualElement("table-main-row");
        var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
        var arrowButton = uiElementFactory.CreateButton("\u25b6", "table-arrow-button");
        arrowButton.clicked += () => ToggleControlSchemeDisplay(arrowButton, row);
        var icon = uiElementFactory.CreateImage("table-cell-icon",
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Sound/d_AudioSource Icon.png"));
        var nameField = uiElementFactory.CreateTextField("table-cell", "", name, "table-cell-input",
            (value) => controller.currentData.audioSources.AddOrUpdate(index!=-1?index-1:currentRow, value));
        nameField.style.width = new StyleLength(Length.Percent(90));
            
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
        
        tableCell.Add(arrowButton);
        tableCell.Add(icon);
        tableCell.Add(nameField);
        mainRow.Add(tableCell);
        mainRow.Add(addButton);
        mainRow.Add(deleteButton);
        row.Add(mainRow);
        CreateSounds(row);
        if(index!=-1) tableScrollView.Insert(index, row);
        else tableScrollView.Add(row);
        rootVisualElement.schedule.Execute(() => { nameField[0].Focus(); }).StartingIn((long)0.001);
    }
    
    private void CreateSounds(VisualElement row)
    {
        var soundsContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
        soundsContainer.style.display = DisplayStyle.None;
        
        var soundTitleCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
        var soundTitle = uiElementFactory.CreateLabel("table-secondary-row-title", "Sounds");
        var addNewSoundButton = uiElementFactory.CreateButton("Add New Sound", "table-cell-button", () => CreateSound(row));
        addNewSoundButton.style.marginLeft = new StyleLength(Length.Auto());
        
        soundTitleCell.Add(soundTitle);
        soundTitleCell.Add(addNewSoundButton);
        soundsContainer.Add(soundTitleCell);
        row.Add(soundsContainer);

        foreach (var audioSource in controller.currentData.audioSources.Items)
        {
            foreach (var audioSource2 in controller.currentData.audioClips.Items)
            {
                if (audioSource.key == audioSource2.key)
                {
                    foreach (var audioClip in controller.currentData.audioClips.Items.Find(x => x.key == audioSource.key).value.Items)
                    {
                        CreateSound(row, audioClip.value.audioClip);
                    }
                }
            }
        }
        
    }

    private void CreateSound(VisualElement row, AudioClip audioClip = null)
    {
        var soundContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
        soundContainer.style.display = DisplayStyle.None;
        
        var soundCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
        var sound = (ObjectField) uiElementFactory.CreateObjectField("table-row-multi-input", "Audio Clip:", typeof(AudioClip),
            onValueChanged: (value) =>
            {
                if(value == null && controller.currentData.audioClips.Items.Exists(x=> x.key.Equals(tableScrollView.IndexOf(row) - 1)))
                {
                    controller.currentData.audioClips.Items.Find(x => x.key.Equals(tableScrollView.IndexOf(row) - 1)).value.Remove(row.IndexOf(soundContainer) - 2);
                    return;
                }
                if(value == null) return;
                
                var audioClip = (AudioClip) value;
                var accSound = new ACC_Sound(audioClip.name, audioClip);
                
                if(!controller.currentData.audioClips.Items.Exists(x=> x.key.Equals(tableScrollView.IndexOf(row) - 1)))
                {
                    controller.currentData.audioClips.AddOrUpdate(tableScrollView.IndexOf(row) - 1, new ACC_SerializableDictiornary<int, ACC_Sound>());
                }
                
                controller.currentData.audioClips.Items.Find(x => x.key.Equals(tableScrollView.IndexOf(row) - 1)).value.AddOrUpdate(row.IndexOf(soundContainer) - 2, accSound);
            });
        if(audioClip != null) sound.value = audioClip;
        
        soundCell.Add(sound);
        soundContainer.Add(soundCell);
        row.Add(soundContainer);
    }
    
    private void ToggleControlSchemeDisplay(Button arrowButton, VisualElement audioSource)
    {
        if (arrowButton.text == "\u25b6")
        {
            arrowButton.text = "\u25bc";
            for (int j = 1; j < audioSource.childCount; j++)
            {
                audioSource[j].style.display = DisplayStyle.Flex;
            }
        }
        else
        {
            arrowButton.text = "\u25b6";
            for (int j = 1; j < audioSource.childCount; j++)
            {
                audioSource[j].style.display = DisplayStyle.None;
            }
        }
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
