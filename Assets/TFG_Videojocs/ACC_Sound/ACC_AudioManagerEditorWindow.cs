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
        window.minSize = new Vector2(600, 560);
        window.maxSize = new Vector2(600, 560);
        
        window.controller.isEditing = true;
        window.controller.LoadOnlyEditableWindow("AudioManager");
    }
    
    private new void CreateGUI()
    {
        base.CreateGUI();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Sound/ACC_AudioManagerEditorWindowStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        tableContainer = uiElementFactory.CreateVisualElement("container");
        rootVisualElement.Add(tableContainer);
        CreateTable();
        CreateSettingsContainer();
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
        var row = uiElementFactory.CreateVisualElement("table-row");
        if(index!=-1) tableScrollView.Insert(index, row);
        else tableScrollView.Add(row);
            
        var mainRow = uiElementFactory.CreateVisualElement("table-main-row");
        var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
        var arrowButton = uiElementFactory.CreateButton("\u25b6", "table-arrow-button");
        arrowButton.clicked += () => ToggleControlSchemeDisplay(arrowButton, row);
        var icon = uiElementFactory.CreateImage("table-cell-icon",
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Sound/d_AudioSource Icon.png"));
        var nameField = uiElementFactory.CreateTextField("table-cell", "", name, "table-cell-input",
            (value) =>
            {
                var currentRow = tableScrollView.IndexOf(row)-1;
                controller.currentData.audioSources.AddOrUpdate(currentRow, value);

                if (!controller.currentData.audioClips.Items.Exists(x =>
                        x.key.Equals(index != -1 ? index - 1 : currentRow)))
                {
                    controller.currentData.audioClips.AddOrUpdate(currentRow,
                        new ACC_SerializableDictiornary<int, ACC_Sound>());
                }

                else
                {
                    controller.currentData.audioClips.AddOrUpdate(currentRow, 
                        controller.currentData.audioClips.Items.Find(x => x.key == currentRow).value);
                }
                
            });
        nameField.style.width = new StyleLength(Length.Percent(90));
            
        var addButton = uiElementFactory.CreateButton("+","table-add-button", () =>
        {
            var currentRow = tableScrollView.IndexOf(row)-1;
            if (tableScrollView.childCount - 1 > currentRow + 1)
            {
                for(var j = tableScrollView.childCount-2; j>currentRow; j--)
                {
                    controller.currentData.audioSources.AddOrUpdate(j+1, controller.currentData.audioSources.Items.Find(x => x.key == j).value);
                    controller.currentData.audioClips.AddOrUpdate(j+1, controller.currentData.audioClips.Items.Find(x => x.key == j).value);
                }
            } 
            AddNewAudioSource(index: tableScrollView.IndexOf(row)+1);
        });
        
        var deleteButton = uiElementFactory.CreateButton("-","table-delete-button", () =>
        {
            var currentRow = tableScrollView.IndexOf(row)-1;
            tableScrollView.Remove(row);
            controller.currentData.audioSources.Remove(currentRow);
            controller.currentData.audioClips.Remove(currentRow);
            if (tableScrollView.childCount > currentRow + 1)
            {
                for (var j = currentRow + 1; j < tableScrollView.childCount; j++)
                {
                    controller.currentData.audioSources.AddOrUpdate(j - 1, controller.currentData.audioSources.Items.Find(x => x.key == j).value);
                    controller.currentData.audioClips.AddOrUpdate(j - 1, controller.currentData.audioClips.Items.Find(x => x.key == j).value);
                    controller.currentData.audioSources.Remove(j);
                    controller.currentData.audioClips.Remove(j);
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
                if (audioSource.key == audioSource2.key && tableScrollView.IndexOf(row)-1 == audioSource.key)
                {
                    foreach (var accSound in controller.currentData.audioClips.Items.Find(x => x.key == audioSource.key).value.Items)
                    {
                        CreateSound(row, accSound.value);
                    }
                }
            }
        }
        
    }
    private void CreateSound(VisualElement row, ACC_Sound accSound = null, int index = -1)
    {
        var soundContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
        if(accSound != null) soundContainer.style.display =  DisplayStyle.None;
        if (index != -1) row.Insert(index, soundContainer);
        else row.Add(soundContainer);

        if (accSound != null)
        {
            // var audioClipPath = AssetDatabase.GetAssetPath(accSound.audioClip);
            // Debug.Log(AssetDatabase.AssetPathToGUID(audioClipPath));
        }
        
        var soundCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
        var sound = (ObjectField)uiElementFactory.CreateObjectField("table-row-multi-input", "Audio Clip:", typeof(AudioClip));
        if(accSound != null) sound.value = accSound.audioClip;
        sound.RegisterValueChangedCallback((ChangeEvent<UnityEngine.Object> changeEvent) =>
        {
            var currentSoundRow = row.IndexOf(soundContainer) - 2;
            var newValue = changeEvent.newValue;
            
            var audioClip = newValue as AudioClip;
            ACC_Sound accSound;

            if(audioClip != null)
            {
                accSound = new ACC_Sound(audioClip.name, audioClip);
            }
            else
            {
                accSound = new ACC_Sound("Default Name", null);
            }
                
            if(!controller.currentData.audioClips.Items.Exists(x=> x.key.Equals(tableScrollView.IndexOf(row) - 1)))
            {
                controller.currentData.audioClips.AddOrUpdate(tableScrollView.IndexOf(row) - 1, new ACC_SerializableDictiornary<int, ACC_Sound>());
            }
                
            controller.currentData.audioClips.Items.Find(x => x.key.Equals(tableScrollView.IndexOf(row) - 1)).value.AddOrUpdate(currentSoundRow, accSound);
        });
        
        var addButton = uiElementFactory.CreateButton("+", "table-add-button", () =>
        {
            var currentRow = tableScrollView.IndexOf(row) - 1;
            var currentSoundRow = row.IndexOf(soundContainer) - 2;
            if (row.childCount - 2 > currentSoundRow + 1)
            {
                for (var j = row.childCount - 3; j > currentSoundRow; j--)
                {
                    controller.currentData.audioClips.Items.Find(x => x.key.Equals(currentRow)).
                        value.AddOrUpdate(j + 1, controller.currentData.audioClips.Items.
                            Find(x => x.key.Equals(currentRow)).value.Items.Find(x => x.key == j).value);
                }
            }
            CreateSound(row, index: row.IndexOf(soundContainer) + 1);
        });
        var deleteButton = uiElementFactory.CreateButton("-", "table-delete-button", () =>
        {
            var currentRow = tableScrollView.IndexOf(row) - 1;
            var currentSoundRow = row.IndexOf(soundContainer) - 2;
            row.Remove(soundContainer);
            controller.currentData.audioClips.Items.Find(x => x.key.Equals(currentRow)).value.Remove(currentSoundRow);
            
            if (row.childCount > currentSoundRow + 2)
            {
                for (var j = currentSoundRow + 2; j < row.childCount; j++)
                {
                    controller.currentData.audioClips.Items.Find(x => x.key.Equals(currentRow)).
                        value.AddOrUpdate(j - 2, controller.currentData.audioClips.Items.
                            Find(x => x.key.Equals(currentRow)).value.Items.Find(x => x.key == j-1).value);
                    controller.currentData.audioClips.Items.Find(x => x.key.Equals(currentRow)).value.Remove(j-1);
                }
            }
        });
        
        soundCell.Add(sound);
        soundContainer.Add(soundCell);
        soundContainer.Add(addButton);
        soundContainer.Add(deleteButton);
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
    private void CreateSettingsContainer()
    {
        var settingsContainer = uiElementFactory.CreateVisualElement("container-2");
        var settingsTitle = uiElementFactory.CreateLabel("title", "Settings");
        var settingsScrollView = uiElementFactory.CreateScrollView("settings-scroll-view");

        var sliderVolume = uiElementFactory.CreateSliderWithFloatField("option-multi-input", "Volume", 0, 1,
            0.5f);
        settingsScrollView.Add(sliderVolume);
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(settingsScrollView);
        rootVisualElement.Add(settingsContainer);
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
