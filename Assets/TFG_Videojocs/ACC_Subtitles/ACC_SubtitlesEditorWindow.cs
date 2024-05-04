#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BgTools.Utils;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Subtitles;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACC_SubtitlesEditorWindow : ACC_BaseFloatingWindow<ACC_SubtitlesEditorWindowController, ACC_SubtitlesEditorWindow, ACC_SubtitleData>
{
    private VisualElement table, actorTableContainer, actorTableScrollView;
    //private VisualElement tableContainer, tableScrollView, selectedItem = null;
    private ListView listView;
    public static event WindowDelegate OnCloseWindow;
    
    private new void OnDestroy()
    {
        base.OnDestroy();
        OnCloseWindow?.Invoke("ACC_Subtitles/");
    }

    public static void ShowWindow(string name)
    {
        var window = GetWindow<ACC_SubtitlesEditorWindow>();
        if(window.controller.isEditing) return;
        if(window.controller.isCreatingNewFileOnCreation) return;
        
        window.titleContent = new GUIContent("Subtitle Creation");
        window.minSize = new Vector2(600, 550);
        window.maxSize = new Vector2(600, 780);
        
        if (name != null)
        {
            window.controller.isEditing = true;
            window.controller.LoadJson(name);
        }
        else
        {
            window.controller.isCreatingNewFileOnCreation = true;
            window.controller.lastData = window.controller.currentData.Clone() as ACC_SubtitleData;
        }
        
        window.PositionWindowInBottomRight();
        window.SetFixedPosition();
    }
    
    private new void CreateGUI()
    {
        base.CreateGUI();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Subtitles/ACC_SubtitlesEditorWindowStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        CreateTable();
        if(!controller.isEditing) CreateRow(1);
        CreateSettingsContainer();
        CreateBottomContainer();
        
        controller.RestoreDataAfterCompilation();
    }
    
    private void CreateTable()
    {
        var tableContainer = uiElementFactory.CreateVisualElement("container");
        var subtitlesTitle = uiElementFactory.CreateLabel("title", "Subtitles");
        var tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view", table);
        
        table = uiElementFactory.CreateVisualElement("subtitles-table");
        var mainRow = uiElementFactory.CreateVisualElement("main-row");
        var actor = uiElementFactory.CreateLabel("actor-cell", "Actor");
        var subtitles = uiElementFactory.CreateLabel("subtitles-cell", "Subtitles");
        var time = uiElementFactory.CreateLabel("time-cell", "Time");
        
        mainRow.Add(actor);
        mainRow.Add(subtitles);
        mainRow.Add(time);
        table.Add(mainRow);
        tableScrollView.Add(table);
        
        tableContainer.Add(subtitlesTitle);
        tableContainer.Add(tableScrollView);
        
        rootVisualElement.Add(tableContainer);
    }
    public void CreateRow(int numberOfRows, string actor = "New Actor", string subtitle = "Hello", int time = 1, int index = -1)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            var newRow = uiElementFactory.CreateVisualElement("new-row");
            if(index != -1) table.Insert(index, newRow);
            else table.Add(newRow);

            var contentRow = uiElementFactory.CreateVisualElement("content-row");
            var actorField = uiElementFactory.CreateDropdownField("actor-new-cell", options: new List<string>(){}, subClassList: "actor-input-cell", value: actor,
                onValueChanged: value =>
                {
                    var currentRow = table.IndexOf(newRow)-1;
                    var subtitle = "Hello";
                    var time = 1;
                    
                    if (controller.currentData.subtitles.Items.Exists(x => x.key == currentRow))
                    {
                        subtitle = controller.currentData.subtitles.Items.Find(x => x.key == currentRow).value.subtitle;
                        time = controller.currentData.subtitles.Items.Find(x => x.key == currentRow).value.time;
                    }
                    controller.currentData.subtitles.AddOrUpdate(currentRow, new ACC_SubtitleRowData
                    {
                        actor = value,
                        subtitle = subtitle,
                        time = time
                    });
                });
            
            var subtitleField = uiElementFactory.CreateTextField(value: subtitle, classList: "subtitles-new-cell", subClassList: "subtitles-input-cell",
                onValueChanged: value =>
                {
                    var currentRow = table.IndexOf(newRow)-1;
                    var actor = "New Actor";
                    var time = 1;
                    
                    if (controller.currentData.subtitles.Items.Exists(x => x.key == currentRow))
                    {
                        actor = controller.currentData.subtitles.Items.Find(x => x.key == currentRow).value.actor;
                        time = controller.currentData.subtitles.Items.Find(x => x.key == currentRow).value.time;
                    }
                    controller.currentData.subtitles.AddOrUpdate(currentRow, new ACC_SubtitleRowData
                    {
                        actor = actor,
                        subtitle = value,
                        time = time
                    });
                });
            var timeField = uiElementFactory.CreateIntegerField(value: time, classList: "time-new-cell", subClassList: "time-input-cell",
                onValueChanged: (value) =>
                {
                    var currentRow = table.IndexOf(newRow)-1;
                    var actor = "New Actor";
                    var subtitle = "Hello";
                    
                    if (controller.currentData.subtitles.Items.Exists(x => x.key == currentRow))
                    {
                        actor = controller.currentData.subtitles.Items.Find(x => x.key == currentRow).value.actor;
                        subtitle = controller.currentData.subtitles.Items.Find(x => x.key == currentRow).value.subtitle;
                    }
                    controller.currentData.subtitles.AddOrUpdate(currentRow, new ACC_SubtitleRowData
                    {
                        actor = actor,
                        subtitle = subtitle,
                        time = value
                    });
                });
            var addRowButton = uiElementFactory.CreateButton("+", "add-row-button", () =>
            {
                var currentRow = table.IndexOf(newRow)-1;
                if (table.childCount - 1 > currentRow + 1)
                {
                    for (var j = table.childCount - 2; j > currentRow; j--)
                    {
                        controller.currentData.subtitles.AddOrUpdate(j + 1, controller.currentData.subtitles.Items.Find(x => x.key == j).value);
                    }
                }
                CreateRow(1, index: table.IndexOf(newRow)+1);
            });
            var deleteButton = uiElementFactory.CreateButton("-", "delete-row-button", () =>
            {
                var currentRow = table.IndexOf(newRow)-1;
                table.Remove(newRow);
                controller.currentData.subtitles.Remove(currentRow);
                if (table.childCount > currentRow + 1)
                {
                    for (var j = currentRow + 1; j < table.childCount; j++)
                    {
                        controller.currentData.subtitles.AddOrUpdate(j - 1, controller.currentData.subtitles.Items.Find(x => x.key == j).value);
                        controller.currentData.subtitles.Remove(j);
                    }
                }
            });
            
            contentRow.Add(actorField);
            contentRow.Add(subtitleField);
            contentRow.Add(timeField);
            newRow.Add(contentRow);
            newRow.Add(addRowButton);
            newRow.Add(deleteButton);
            
            rootVisualElement.schedule.Execute(() => { subtitleField[0].Focus(); }).StartingIn((long)0.001);
        }
    }
    private void CreateSettingsContainer()
    {
        var settingsContainer = uiElementFactory.CreateVisualElement("container-2");
        
        var settingsTitle = uiElementFactory.CreateLabel("title", "Settings");
        var nameInput = uiElementFactory.CreateTextField( "option-input", "Name: ", "", "option-input-label", 
            value => controller.currentData.name = value);
        var fontColorInput = uiElementFactory.CreateColorField("option-input", "Font Color:", Color.black, subClassList: "option-input-label",
            onValueChanged: value => controller.currentData.fontColor = value);
        var backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background Color:", Color.white, subClassList: "option-input-label",
            onValueChanged: value => controller.currentData.backgroundColor = value);
        var fontSizeContainer =
            uiElementFactory.CreateSliderWithIntegerField("option-multi-input", "Font Size:", 20, 100, 40,
                onValueChanged: value => controller.currentData.fontSize = value);
        
        
        actorTableContainer = uiElementFactory.CreateVisualElement("container-2");
        CreateActors();
        
        var showActors = uiElementFactory.CreateToggle("option-input", "Show Actors:", false, "option-input-label",
            value =>
            {
                controller.currentData.showActors = value;
                actorTableContainer.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
                minSize = value ? new Vector2(600, 780) : new Vector2(600, 550);
                var addRowContainer = rootVisualElement.Q<VisualElement>("add-row-container-0");
                if (addRowContainer != null) addRowContainer.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
            });
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(nameInput);
        settingsContainer.Add(fontColorInput);
        settingsContainer.Add(backgroundColorInput);
        settingsContainer.Add(fontSizeContainer);
        settingsContainer.Add(showActors);

        rootVisualElement.Add(settingsContainer);
        rootVisualElement.Add(actorTableContainer);
    }
    public void CreateActors()
    {
        actorTableContainer.Clear();
        actorTableContainer.style.display = controller.currentData.showActors ? DisplayStyle.Flex : DisplayStyle.None;
        actorTableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
        actorTableScrollView.style.height = new Length(200, LengthUnit.Pixel);
        
        var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
        var tableNameLabel = uiElementFactory.CreateLabel("table-title-actor-name", "Actors");
        var tableColorLabel = uiElementFactory.CreateLabel("table-title-color", "Font Color");
        
        containerTableTitle.Add(tableNameLabel);
        containerTableTitle.Add(tableColorLabel);
        actorTableScrollView.Add(containerTableTitle);
        
        for (int i = 0; i < controller.currentData.actorText.Items.Count; i++)
        {
            CreateActor(controller.currentData.actorText.Items.Find(x => x.key == i).value,
                controller.currentData.actorColor.Items.Find(x => x.key == i).value, i+1);
        }
        
        actorTableContainer.Add(actorTableScrollView);
    }
    public void CreateActor(string name = "New Actor", Color color = default, int index = -1)
    {
        var row = uiElementFactory.CreateVisualElement("table-row");
        if(index != -1) actorTableScrollView.Insert(index, row);
        else actorTableScrollView.Add(row);
        
        var mainRow = uiElementFactory.CreateVisualElement("table-main-row");
        var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
        
        var actorField = uiElementFactory.CreateTextField(value: name, classList: "table-cell-actor-name", subClassList: "table-cell-input",
            onValueChanged: value =>
            {
                var currentRow = actorTableScrollView.IndexOf(row)-1;
                controller.currentData.actorText.AddOrUpdate(currentRow, value);
                var actors = rootVisualElement.Query<DropdownField>().Class("actor-new-cell").ToList();

                bool newActor = false;
                foreach (var actor in actors)
                {
                    if (currentRow >= 0 && currentRow < actor.choices.Count)
                    {
                        if (index != -1 && currentRow + 1 != actorTableScrollView.childCount-1)
                        {
                            newActor = true;
                            actor.choices.Insert(currentRow, value);
                        }
                        else
                        {
                            actor.choices[currentRow] = value;
                        }
                    }
                    else
                    {
                        actor.choices.Add(value);
                    }
                }
                if (newActor) index = -1;
            });
        
        var colorField = uiElementFactory.CreateColorField("table-cell-color", "", color, subClassList: "table-cell-input",
            onValueChanged: value =>
            {
                var currentRow = actorTableScrollView.IndexOf(row)-1;
                controller.currentData.actorColor.AddOrUpdate(currentRow, value);
            });

        var addButton = uiElementFactory.CreateButton("+", "add-row-button", 
            () =>
        {
            var currentRow = actorTableScrollView.IndexOf(row)-1;
            if (actorTableScrollView.childCount - 1 > currentRow + 1)
            {
                for (var j = actorTableScrollView.childCount - 2; j > currentRow; j--)
                {
                    controller.currentData.actorText.AddOrUpdate(j + 1, controller.currentData.actorText.Items.Find(x => x.key == j).value);
                    controller.currentData.actorColor.AddOrUpdate(j + 1, controller.currentData.actorColor.Items.Find(x => x.key == j).value);
                }
            }
            CreateActor(index: actorTableScrollView.IndexOf(row)+1);
        });
        
        var deleteButton = uiElementFactory.CreateButton("-", "delete-row-button", () =>
        {
            var currentRow = actorTableScrollView.IndexOf(row)-1;
            actorTableScrollView.Q(name: row.name).RemoveFromHierarchy();
            controller.currentData.actorText.Remove(currentRow);
            controller.currentData.actorColor.Remove(currentRow);
            if (actorTableScrollView.childCount > currentRow + 1)
            {
                for (var j = currentRow + 1; j < actorTableScrollView.childCount; j++)
                {
                    controller.currentData.actorText.AddOrUpdate(j - 1, controller.currentData.actorText.Items.Find(x => x.key == j).value);
                    controller.currentData.actorColor.AddOrUpdate(j - 1, controller.currentData.actorColor.Items.Find(x => x.key == j).value);
                    controller.currentData.actorText.Remove(j);
                    controller.currentData.actorColor.Remove(j);
                }
            }
            
            var actors = rootVisualElement.Query<DropdownField>().Class("actor-new-cell").ToList();
            foreach (var actor in actors)
            {
                if (currentRow >= 0 && currentRow < actor.choices.Count) actor.choices.RemoveAt(currentRow);
                if (actorTableScrollView.childCount == 1) actor.value = "No Actors";
            }
        });
        
        tableCell.Add(actorField);
        tableCell.Add(colorField);
        mainRow.Add(tableCell);
        mainRow.Add(addButton);
        mainRow.Add(deleteButton);
        row.Add(mainRow);
    }
    private void CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
        bottomContainer.style.marginTop = new StyleLength(Length.Auto());
        var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));

        var addActorContainer = uiElementFactory.CreateVisualElement("add-row-container");
        addActorContainer.style.marginRight = new Length(-200, LengthUnit.Pixel);
        addActorContainer.style.visibility = controller.currentData.showActors ? Visibility.Hidden : Visibility.Visible;
        var addActorLabel = uiElementFactory.CreateLabel("add-row-label", "Add actor:");
        
        var addActor1 = uiElementFactory.CreateButton("+1", "add-row-button", () => CreateActor());
        
        var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
        var addSubtitlesLabel = uiElementFactory.CreateLabel("add-row-label", "Add subtitles:");
        
        var addSubtitle1 = uiElementFactory.CreateButton("+1", "add-row-button", () => CreateRow(1));
        var addSubtitle5 = uiElementFactory.CreateButton("+5", "add-row-button", () => CreateRow(5));
        var addSubtitle10 = uiElementFactory.CreateButton("+10", "add-row-button", () => CreateRow(10));
        
        addActorContainer.Add(addActorLabel);
        addActorContainer.Add(addActor1);
        addSubtitlesContainer.Add(addSubtitlesLabel);
        addSubtitlesContainer.Add(addSubtitle1);
        addSubtitlesContainer.Add(addSubtitle5);
        addSubtitlesContainer.Add(addSubtitle10); 
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(addActorContainer);
        bottomContainer.Add(addSubtitlesContainer);

        rootVisualElement.Add(bottomContainer);
    }
}
#endif
