using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Sound.ACC_Example;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ACC_AudioManagerEditorWindow : ACC_BaseFloatingWindow<ACC_AudioManagerEditorWindowController, ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    private VisualElement tableContainer, tableScrollView, settingsScrollView;
    public static void ShowWindow(string name)
    {
        CloseWindowIfExists<ACC_AudioManagerEditorWindow>();
        var window = GetWindow<ACC_AudioManagerEditorWindow>();
        window.titleContent = new GUIContent("Audio Manager");
        window.minSize = new Vector2(600, 660);
        window.maxSize = new Vector2(600, 660);
        
        window.controller.isEditing = true;
        window.controller.LoadOnlyEditableWindow("ACC_AudioManager");
        
        window.PositionWindowInBottomRight();
        window.SetFixedPosition();
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
            
        var highContrastTitle = uiElementFactory.CreateLabel("title", "Sounds:");
        tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
            
        var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
        var tableNameLabel = uiElementFactory.CreateLabel("table-title-name", "Audio Sources");
        
        tableContainer.Add(highContrastTitle);
        containerTableTitle.Add(tableNameLabel);
        tableScrollView.Add(containerTableTitle);
        
        for(int i=0; i<controller.currentData.audioSources.Items.Count; i++)
        {
            CreateAudioSource(controller.currentData.audioSources.Items.Find(x => x.key == i).value.name, i+1);
        }
        
        tableContainer.Add(tableScrollView);
    }
    private void CreateAudioSource(string name = "New Audio Source", int index=-1)
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
        icon.style.width = new StyleLength(16);
        icon.style.height = new StyleLength(16);
        var nameField = uiElementFactory.CreateTextField("table-cell", "", name, "table-cell-input",
            (value) =>
            {
                var currentRow = tableScrollView.IndexOf(row)-1;
                var volume = 0.5f;
                var isAudio3D = false;
                var sourceObjectGUID = "";
                var prefabGUID = "";

                if (controller.currentData.audioSources.Items.Exists(x => x.key == currentRow))
                {
                    volume = controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.volume;
                    isAudio3D = controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.is3D;
                    sourceObjectGUID = controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.sourceObjectGUID;
                    prefabGUID = controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.prefabGUID;
                }
                
                controller.currentData.audioSources.AddOrUpdate(currentRow, new ACC_AudioSourceData()
                {
                    name = value, volume = volume, is3D = isAudio3D, sourceObjectGUID = sourceObjectGUID, prefabGUID = prefabGUID
                });

                if (!controller.currentData.audioClips.Items.Exists(x =>
                        x.key.Equals(index != -1 ? index - 1 : currentRow)))
                {
                    controller.currentData.audioClips.AddOrUpdate(currentRow,
                        new ACC_SerializableDictiornary<int, string>());
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
            if (tableScrollView.childCount-1 > currentRow + 1)
            {
                for(var j = tableScrollView.childCount-2; j>currentRow; j--)
                {
                    controller.currentData.audioSources.AddOrUpdate(j+1, controller.currentData.audioSources.Items.Find(x => x.key == j).value);
                    controller.currentData.audioClips.AddOrUpdate(j+1, controller.currentData.audioClips.Items.Find(x => x.key == j).value);
                }
            } 
            CreateAudioSource(index: tableScrollView.IndexOf(row)+1);
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
                    controller.currentData.audioSources.AddOrUpdate(j-1, controller.currentData.audioSources.Items.Find(x => x.key == j).value);
                    controller.currentData.audioClips.AddOrUpdate(j-1, controller.currentData.audioClips.Items.Find(x => x.key == j).value);
                    controller.currentData.audioSources.Remove(j);
                    controller.currentData.audioClips.Remove(j);
                }
            }
            settingsScrollView.RemoveAt(currentRow);
        });
        
        CreateAudioSourceSetting(controller.currentData.audioSources.Items.Find(x => x.key == tableScrollView.IndexOf(row)-1).value.name, tableScrollView.IndexOf(row)-1);
        nameField.RegisterValueChangedCallback((evt) =>
        {
            var currentRow = tableScrollView.IndexOf(row)-1;
            settingsScrollView[currentRow].Q<Label>().text = evt.newValue;
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
                if (audioSource.key == audioSource2.key && tableScrollView.IndexOf(row) - 1 == audioSource.key)
                {
                    foreach (var sound in controller.currentData.audioClips.Items.Find(x => x.key == audioSource.key).value.Items)
                    {
                        CreateSound(row, sound.value);
                    }
                }
            }
        }
    }
    private void CreateSound(VisualElement row, string soundGuid = null, int index = -1)
    {
        var soundContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
        if(soundGuid != null) soundContainer.style.display =  DisplayStyle.None;
        if (index != -1) row.Insert(index, soundContainer);
        else row.Add(soundContainer);
        
        var soundCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
        var audioClipPath = AssetDatabase.GUIDToAssetPath(soundGuid);
        var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);
        
        var sound = (ObjectField)uiElementFactory.CreateObjectField("table-row-option-input", "Audio Clip:", typeof(AudioClip), audioClip,
            onValueChanged: value => {  
                var currentSoundRow = row.IndexOf(soundContainer) - 2;
                var assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                    
                if(!controller.currentData.audioClips.Items.Exists(x=> x.key.Equals(tableScrollView.IndexOf(row) - 1)))
                {
                    controller.currentData.audioClips.AddOrUpdate(tableScrollView.IndexOf(row) - 1, new ACC_SerializableDictiornary<int, string>());
                }
                    
                controller.currentData.audioClips.Items.Find(x => x.key.Equals(tableScrollView.IndexOf(row) - 1)).value.AddOrUpdate(currentSoundRow, assetGuid); 
            });
        sound.RegisterValueChangedCallback( evt =>
        {
            var currentRow = tableScrollView.IndexOf(row) - 1;
            var currentSoundRow = row.IndexOf(soundContainer) - 2;
            var assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));
            
            if(controller.currentData.audioClips.Items[currentRow].value.Items.Exists(x => x.key != currentSoundRow && x.value == assetGuid && x.value != ""))
            {
                EditorUtility.DisplayDialog("Error", "This audio clip is already in use", "Ok");
                sound.value = null;
            }
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
        settingsTitle.style.marginBottom = new Length(0);
        settingsScrollView = uiElementFactory.CreateScrollView("settings-scroll-view");
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(settingsScrollView);
        rootVisualElement.Add(settingsContainer);
    }
    private void CreateAudioSourceSetting(string name, int row)
    {
        var settingContainer = uiElementFactory.CreateVisualElement("setting-container");
        settingsScrollView.Insert(row, settingContainer);
        settingContainer.style.borderBottomWidth = new StyleFloat(1);
        settingContainer.style.borderBottomColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f, 1));
        settingContainer.style.paddingBottom = new StyleLength(6);
        settingContainer.style.paddingTop = new StyleLength(6);
        
        var label = uiElementFactory.CreateLabel("subtitle", name);
        label.style.color = new Color(0.9921569f, 0.7490196f, 0.03529412f, 1);
        
        var currentVolume = controller.currentData.audioSources.Items.Find(x => x.key == settingsScrollView.IndexOf(settingContainer)).value.volume;
        var sliderVolume = uiElementFactory.CreateSliderWithFloatField("option-multi-input", "Volume", 0, 1,
            currentVolume, onValueChanged: value =>
            {
                var currentRow = settingsScrollView.IndexOf(settingContainer);
                controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.volume = value;
            });
        
        var gameObjectGuid = controller.currentData.audioSources.Items.Find(x => x.key == settingsScrollView.IndexOf(settingContainer)).value.sourceObjectGUID;
        var gameObjectPath = AssetDatabase.GUIDToAssetPath(gameObjectGuid);
        var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(gameObjectPath);
        
        var gameObjectField = (ObjectField) uiElementFactory.CreateObjectField("option-input", "Game Object: ", typeof(GameObject), gameObject, onValueChanged:
            value =>
            {
                var currentRow = settingsScrollView.IndexOf(settingContainer);
                var gameObjectPath = AssetDatabase.GetAssetPath(value);
                var gameObjectGuid = AssetDatabase.AssetPathToGUID(gameObjectPath);
                controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.sourceObjectGUID = gameObjectGuid;
            });
        gameObjectField.allowSceneObjects = false;
        gameObjectField.style.display = DisplayStyle.None;
        gameObjectField[0].style.width = new Length(50, LengthUnit.Percent);
        
        var create3DAudio = controller.currentData.audioSources.Items.Find(x => x.key == settingsScrollView.IndexOf(settingContainer)).value.is3D;
        var audioToggle = (Toggle) uiElementFactory.CreateToggle("option-input", "3D Audio: ", create3DAudio, "option-input-label", onValueChanged: value =>
        {
            var currentRow = settingsScrollView.IndexOf(settingContainer);
            controller.currentData.audioSources.Items.Find(x => x.key == currentRow).value.is3D = value;
            if (value)
            {
                gameObjectField.style.display = DisplayStyle.Flex;
            }
            else
            {
                gameObjectField.style.display = DisplayStyle.None;
            }
        });
        
        
        // List<VisualElement> items = new List<VisualElement>(){Create3DAudioObjectField()};
        // ListView listView = new ListView(items, 50, Create3DAudioObjectField,
        //     (visualElement, i) => { });
        // listView.AddToClassList("list-view");
        // listView.style.display = DisplayStyle.None;
        // listView.selectionType = SelectionType.Single;
        // listView.reorderMode = ListViewReorderMode.Simple;
        // listView.showAddRemoveFooter = true;
        
        // audioToggle.RegisterValueChangedCallback(evt =>
        // {
        //     if (evt.newValue)
        //     {
        //         gameObjectField.style.display = DisplayStyle.Flex;
        //         //listView.style.display = DisplayStyle.Flex;
        //     }
        //     else
        //     {
        //         gameObjectField.style.display = DisplayStyle.None;
        //         //listView.style.display = DisplayStyle.None;
        //     }
        // });
        
        settingContainer.Add(label);
        settingContainer.Add(sliderVolume);
        settingContainer.Add(audioToggle);
        settingContainer.Add(gameObjectField);
        //settingContainer.Add(listView);
    }
    // private VisualElement Create3DAudioObjectField()
    // {
    //     var container = new VisualElement();
    //     container.style.borderBottomWidth = new StyleFloat(1);
    //     container.style.borderBottomColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f, 1));
    //     
    //     var gameObjectField = uiElementFactory.CreateObjectField("option-input", "Game Object: ", typeof(GameObject));
    //     gameObjectField[0].style.width = new Length(50, LengthUnit.Percent);
    //     
    //     var createPrefab = uiElementFactory.CreateToggle("option-input", "Create Prefab: ", false, "option-input-label");
    //     
    //     container.Add(gameObjectField);
    //     container.Add(createPrefab);
    //     return container;
    // }
    private void CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
        bottomContainer.style.marginTop = new StyleLength(Length.Auto());
        var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));
        
        var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
        var addSubtitle1 = uiElementFactory.CreateButton("Add New Audio Source", "add-row-button", () =>
        {
            CreateAudioSource();
        });
        
        addSubtitlesContainer.Add(addSubtitle1);
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(addSubtitlesContainer);

        rootVisualElement.Add(bottomContainer);
    }
    
}
