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
    private VisualElement table, tableContainer, tableScrollView, selectedItem = null;
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
        window.minSize = new Vector2(600, 530);
        window.maxSize = new Vector2(600, 530);
        
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
        
        // tableContainer = uiElementFactory.CreateVisualElement("container");
        // rootVisualElement.Add(tableContainer);
        
        //CreateTable2();
        CreateTable();
        if(!controller.isEditing) CreateRow(1, "","Hello", 1);
        CreateSettingsContainer();
        CreateBottomContainer();
        
        //controller.RestoreDataAfterCompilation();
    }

    // private void CreateTable2()
    // {
    //     tableContainer.Clear();
    //     
    //     var subtitlesTitle = uiElementFactory.CreateLabel("title", "Subtitles");
    //     tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view", table);
    //     
    //     var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
    //     containerTableTitle.style.width = new StyleLength(Length.Percent(100));
    //     var containerTableNameTitle = uiElementFactory.CreateLabel("table-title-name", "Subtitles");
    //     var containerTableTimeTitle = uiElementFactory.CreateLabel("table-title-time", "Time");
    //     
    //     containerTableTitle.Add(containerTableNameTitle);
    //     containerTableTitle.Add(containerTableTimeTitle);
    //     tableScrollView.Add(containerTableTitle);
    //     
    //     List<VisualElement> items = new List<VisualElement>(){CreateRow()};
    //     listView = new ListView(items, 24, CreateRow, BindItem);
    //     listView.AddToClassList("list-view");
    //     listView.selectionType = SelectionType.Single;
    //     listView.reorderable = true;
    //     listView.reorderMode = ListViewReorderMode.Animated;
    //     listView.showAddRemoveFooter = true;
    //     listView.unbindItem += UnbindItem;
    //     listView.selectionChanged += objects =>
    //     {
    //         if (!objects.Any()) return;
    //         selectedItem = objects.First() as VisualElement;
    //     };
    //     listView.Q<Button>(BaseListView.footerRemoveButtonName).clicked += () =>
    //     {
    //         Debug.Log(selectedItem);
    //         if (selectedItem != null)
    //         {
    //             Debug.Log("Removing item");
    //             int index = items.IndexOf(selectedItem);
    //             listView.RemoveAt(index);
    //             listView.RefreshItems();
    //             listView.Rebuild();
    //             selectedItem = null;
    //         }
    //     };
    //     
    //     tableScrollView.Add(listView);
    //     tableContainer.Add(subtitlesTitle);
    //     tableContainer.Add(tableScrollView);
    //     rootVisualElement.Add(tableContainer);
    // }

    // private VisualElement CreateRow()
    // {
    //     var newRow = uiElementFactory.CreateVisualElement("new-row");
    //     var subtitleField = uiElementFactory.CreateTextField("subtitles-new-cell", subClassList: "subtitles-input-cell");
    //     var timeField = uiElementFactory.CreateIntegerField("time-new-cell", subClassList: "time-input-cell");
    //     
    //     newRow.Add(subtitleField);
    //     newRow.Add(timeField);
    //     return newRow;
    // }
    //
    // private void BindItem(VisualElement row, int index)
    // {
    //     if (index == 0)
    //     {
    //         controller.currentData.subtitleText = new ACC_SerializableDictiornary<int, string>();
    //         controller.currentData.timeText = new ACC_SerializableDictiornary<int, int>();
    //     }
    //     
    //     var subtitleField = row.Q<TextField>();
    //     var timeField = row.Q<IntegerField>();
    //     
    //     controller.currentData.subtitleText.AddOrUpdate(index, subtitleField.value);
    //     controller.currentData.timeText.AddOrUpdate(index, timeField.value);
    //     
    //     subtitleField.RegisterValueChangedCallback(evt =>
    //     {
    //         controller.currentData.subtitleText.AddOrUpdate(index, evt.newValue);
    //     });
    //     
    //     timeField.RegisterValueChangedCallback(evt =>
    //     {
    //         controller.currentData.timeText.AddOrUpdate(index, evt.newValue);
    //     });
    // }
    //
    // private void UnbindItem(VisualElement row, int index)
    // {
    //     var subtitleField = row.Q<TextField>();
    //     var timeField = row.Q<IntegerField>();
    //     
    //     subtitleField.UnregisterValueChangedCallback(evt => { });
    //     
    //     timeField.UnregisterValueChangedCallback(evt => { });
    // }
    
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
    public void CreateRow(int numberOfRows, string actor, string subtitle, int time, int index = -1)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            var newRow = uiElementFactory.CreateVisualElement("new-row");
            if(index != -1) table.Insert(index, newRow);
            else table.Add(newRow);

            var contentRow = uiElementFactory.CreateVisualElement("content-row");
            var actorField = uiElementFactory.CreateTextField(value: actor, classList: "actor-new-cell", subClassList: "actor-input-cell",
                onValueChanged: value =>
                {
                    var currentRow = table.IndexOf(newRow)-1;
                    controller.currentData.actorText.AddOrUpdate(currentRow, value);
                });
            
            var subtitleField = uiElementFactory.CreateTextField(value: subtitle, classList: "subtitles-new-cell", subClassList: "subtitles-input-cell",
                onValueChanged: value =>
                {
                    var currentRow = table.IndexOf(newRow)-1;
                    controller.currentData.subtitleText.AddOrUpdate(currentRow, value);
                });
            var timeField = uiElementFactory.CreateIntegerField(value: time, classList: "time-new-cell", subClassList: "time-input-cell",
                onValueChanged: (value) =>
                {
                    var currentRow = table.IndexOf(newRow)-1;
                    controller.currentData.timeText.AddOrUpdate(currentRow, value);
                });
            var deleteButton = uiElementFactory.CreateButton("-", "delete-row-button", () =>
            {
                var currentRow = table.IndexOf(newRow)-1;
                table.Q(name: newRow.name).RemoveFromHierarchy();
                controller.currentData.actorText.Remove(currentRow);
                controller.currentData.subtitleText.Remove(currentRow);
                controller.currentData.timeText.Remove(currentRow);
                if (table.childCount > currentRow + 1)
                {
                    for (var j = currentRow + 1; j < table.childCount; j++)
                    {
                        controller.currentData.actorText.AddOrUpdate(j - 1, controller.currentData.actorText.Items.Find(x => x.key == j).value);
                        controller.currentData.subtitleText.AddOrUpdate(j - 1, controller.currentData.subtitleText.Items.Find(x => x.key == j).value);
                        controller.currentData.timeText.AddOrUpdate(j - 1, controller.currentData.timeText.Items.Find(x => x.key == j).value);
                        controller.currentData.actorText.Remove(j);
                        controller.currentData.subtitleText.Remove(j);
                        controller.currentData.timeText.Remove(j);
                    }
                }
            });
            var addRowButton = uiElementFactory.CreateButton("+", "add-row-button", () =>
            {
                var currentRow = table.IndexOf(newRow)-1;
                if (table.childCount - 1 > currentRow + 1)
                {
                    for (var j = table.childCount - 2; j > currentRow; j--)
                    {
                        controller.currentData.actorText.AddOrUpdate(j + 1, controller.currentData.actorText.Items.Find(x => x.key == j).value);
                        controller.currentData.subtitleText.AddOrUpdate(j + 1, controller.currentData.subtitleText.Items.Find(x => x.key == j).value);
                        controller.currentData.timeText.AddOrUpdate(j + 1, controller.currentData.timeText.Items.Find(x => x.key == j).value);
                    }
                }
                CreateRow(1, "", "Hello", 1, table.IndexOf(newRow)+1);
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
        var backgroundColorInput = uiElementFactory.CreateColorField("option-input", "Background color:", Color.white, subClassList: "option-input-label",
            onValueChanged: value => controller.currentData.backgroundColor = value);
        var fontSizeContainer =
            uiElementFactory.CreateSliderWithIntegerField("option-multi-input-last", "Font size:", 20, 100, 40,
                onValueChanged: value => controller.currentData.fontSize = value);
        
        settingsContainer.Add(settingsTitle);
        settingsContainer.Add(nameInput);
        settingsContainer.Add(fontColorInput);
        settingsContainer.Add(backgroundColorInput);
        settingsContainer.Add(fontSizeContainer);

        rootVisualElement.Add(settingsContainer);
    }
    private void CreateBottomContainer()
    {
        var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
        bottomContainer.style.marginTop = new StyleLength(Length.Auto());
        var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));

        var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
        var addSubtitlesLabel = uiElementFactory.CreateLabel("add-row-label", "Add subtitles:");
        
        var addSubtitle1 = uiElementFactory.CreateButton("+1", "add-row-button", () => CreateRow(1, "","Hello", 1));
        var addSubtitle5 = uiElementFactory.CreateButton("+5", "add-row-button", () => CreateRow(5, "", "Hello", 1));
        var addSubtitle10 = uiElementFactory.CreateButton("+10", "add-row-button", () => CreateRow(10, "", "Hello", 1));
        
        addSubtitlesContainer.Add(addSubtitlesLabel);
        addSubtitlesContainer.Add(addSubtitle1);
        addSubtitlesContainer.Add(addSubtitle5);
        addSubtitlesContainer.Add(addSubtitle10); 
        bottomContainer.Add(createSubtitleButton);
        bottomContainer.Add(addSubtitlesContainer);

        rootVisualElement.Add(bottomContainer);
    }
}
#endif
