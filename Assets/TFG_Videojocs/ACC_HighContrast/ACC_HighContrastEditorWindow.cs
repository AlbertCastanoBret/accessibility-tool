#if UNITY_EDITOR
using System;
using System.Linq;
using ACC_API;
using TFG_Videojocs.ACC_HighContrast.Toilet;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastEditorWindow:ACC_BaseFloatingWindow<ACC_HighContrastEditorWindowController, ACC_HighContrastEditorWindow, ACC_HighContrastData>
    {
        private VisualElement tableContainer, tableScrollView, settingsContainer;
        private Button refreshButton;
        public static event WindowDelegate OnCloseWindow;
        private new void OnDestroy()
        {
            base.OnDestroy();
            OnCloseWindow?.Invoke("ACC_HighContrast/");

            if (controller.isCreatingNewFileOnCreation || controller.isEditing)
            {
                var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                accessibilityManager.StopPrevisualize();
                EditorUtility.SetDirty(accessibilityManager);
            }
        }
        public static void ShowWindow(string name)
        {
            var window = GetWindow<ACC_HighContrastEditorWindow>();
            if(window.controller.isEditing) return;
            if(window.controller.isCreatingNewFileOnCreation) return;
            window.titleContent = new GUIContent("High Contrast Settings");
            window.minSize = new Vector2(600, 430);
            window.maxSize = new Vector2(600, 430);
            if (name != null)
            {
                window.controller.isEditing = true;
                window.controller.LoadJson(name);
            }
            else
            {
                window.controller.isCreatingNewFileOnCreation = true;
                window.controller.lastData = window.controller.currentData.Clone() as ACC_HighContrastData;
            }
            
            window.PositionWindowInBottomRight();
            window.SetFixedPosition();
        }
        private new void CreateGUI()
        {
            base.CreateGUI();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_HighContrast/ACC_HighContrastEditorWIndowStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            
            tableContainer = uiElementFactory.CreateVisualElement("container");
            rootVisualElement.Add(tableContainer);
            CreateTable();
            
            settingsContainer = uiElementFactory.CreateVisualElement("container-2");
            rootVisualElement.Add(settingsContainer);
            CreateSettingsContainer();
            
            CreateBottomContainer();
            
            controller.RestoreDataAfterCompilation();
            
            if (!String.IsNullOrEmpty(EditorPrefs.GetString(typeof(ACC_HighContrastEditorWindow) + "Open")))
            {
                var name = EditorPrefs.GetString(typeof(ACC_HighContrastEditorWindow) + "Open");
                controller.isEditing = true;
                controller.LoadJson(name);
                EditorPrefs.SetString(typeof(ACC_HighContrastEditorWindow) + "Open", "");
            }
        }
        public void CreateTable()
        {
            tableContainer.Clear();
            
            var highContrastTitle = uiElementFactory.CreateLabel("title", "High Contrast Configurations");
            tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
            
            var containerTableTitle = uiElementFactory.CreateVisualElement("table-row-title");
            var containerTableNameTitle = uiElementFactory.CreateLabel("table-title-name", "High Contrast Configurations");
            
            containerTableTitle.Add(containerTableNameTitle);
            tableScrollView.Add(containerTableTitle);
            
            for(int i=0; i<controller.currentData.highContrastConfigurations.Items.Count; i++)
            {
                CreateHighContrastConfiguration(controller.currentData.highContrastConfigurations.Items[i].value.name);
            }
            
            tableContainer.Add(highContrastTitle);
            tableContainer.Add(tableScrollView);
        }
        private void CreateHighContrastConfiguration(string name = "New Contrast Configuration", int index=-1)
        {
            var row = uiElementFactory.CreateVisualElement("table-row");
            if(index != -1) tableScrollView.Insert(index, row);
            else tableScrollView.Add(row);
            
            var mainRow = uiElementFactory.CreateVisualElement("table-main-row");
            var tableCell = uiElementFactory.CreateVisualElement("table-row-content");
            var arrowButton = uiElementFactory.CreateButton("\u25b6", "table-arrow-button");
            arrowButton.clicked += () => ToggleControlSchemeDisplay(arrowButton, row);
            var nameField = uiElementFactory.CreateTextField("table-cell", "", name, "table-cell-input",
                value =>
                {
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    var name = "New High Contrast Configuration";
                    var tag = "Untagged";
                    var color = Color.white;
                    var colorMinDistance = 2f;
                    var colorMaxDistance = 6f;
                    var outlineColor = Color.white;
                    var outlineThickness = 0.6f;
                    var outlineFadeDistance = 5f;
                    
                    if(controller.currentData.highContrastConfigurations.Items.Exists(x => x.key == currentRow))
                    {
                        name = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.name;
                        tag = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.tag;
                        color = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.color;
                        colorMinDistance = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.colorMinDistance;
                        colorMaxDistance = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.colorMaxDistance;
                        outlineColor = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.outlineColor;
                        outlineThickness = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.outlineThickness;
                        outlineFadeDistance = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value.outlineFadeDistance;
                    }
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, new ACC_HighContrastConfiguration()
                    {
                        name = value,
                        tag = tag,
                        color = color,
                        colorMinDistance = colorMinDistance,
                        colorMaxDistance = colorMaxDistance,
                        outlineColor = outlineColor,
                        outlineThickness = outlineThickness,
                        outlineFadeDistance = outlineFadeDistance
                    });
                });
            
            nameField.style.width = new StyleLength(Length.Percent(90));

            var addButton = uiElementFactory.CreateButton("+", "table-add-button", () =>
            {
                var currentRow = tableScrollView.IndexOf(row)-1;
                if (tableScrollView.childCount - 1 > currentRow + 1)
                {
                    for(var j = tableScrollView.childCount-2; j>currentRow; j--)
                    {
                        controller.currentData.highContrastConfigurations.AddOrUpdate(j+1, controller.currentData.highContrastConfigurations.Items.Find(x => x.key == j).value);
                    }
                }
                CreateHighContrastConfiguration(index: tableScrollView.IndexOf(row)+1);
            });
            
            var deleteButton = uiElementFactory.CreateButton("-","table-delete-button", () =>
            {
                var currentRow = tableScrollView.IndexOf(row)-1;
                tableScrollView.Remove(row);
                controller.currentData.highContrastConfigurations.Remove(currentRow);
                if (tableScrollView.childCount > currentRow + 1)
                {
                    for (var j = currentRow + 1; j < tableScrollView.childCount; j++)
                    {
                        controller.currentData.highContrastConfigurations.AddOrUpdate(j-1, controller.currentData.highContrastConfigurations.Items.Find(x => x.key == j).value);
                        controller.currentData.highContrastConfigurations.Remove(j);
                    }
                }
            });
            
            tableCell.Add(arrowButton);
            tableCell.Add(nameField);
            mainRow.Add(tableCell);
            mainRow.Add(addButton);
            mainRow.Add(deleteButton);
            row.Add(mainRow);
            CreateHighContrastSettings(row);
            rootVisualElement.schedule.Execute(() => { nameField[0].Focus(); }).StartingIn((long)0.001);
        }
        private void CreateHighContrastSettings(VisualElement row)
        {
            var tagContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            tagContainer.style.display = DisplayStyle.None;
            
            var tagList = UnityEditorInternal.InternalEditorUtility.tags.ToList();
            var tagCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var tag = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.tag;
            var dropdownTag = (DropdownField)uiElementFactory.CreateDropdownField("table-row-option-input", "Tag:", tagList, tag, "table-row-option-input-label",
                onValueChanged: value =>
                {
                    var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                    newHighContrastData.tag = value;
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.Previsualize(controller.currentData);
                        EditorUtility.SetDirty(accessibilityManager);
                    }
                });
            
            var colorContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            colorContainer.style.display = DisplayStyle.None;
            
            var colorCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var color = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.color;
            var colorField = uiElementFactory.CreateColorField("table-row-option-input", "Color:", color,  "table-row-option-input-label",
                value =>
                {
                    var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                    newHighContrastData.color = new Color(value.r, value.g, value.b, value.a);
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.Previsualize(controller.currentData);
                        EditorUtility.SetDirty(accessibilityManager);
                    }
                });
            
            var minDistanceColorContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            minDistanceColorContainer.style.display = DisplayStyle.None;
            
            var minDistanceColorCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var minDistanceColor = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.colorMinDistance;
            var minDistanceColorField = uiElementFactory.CreateSliderWithFloatField("table-row-multi-input", "Min Distance Color:", 1, 10, minDistanceColor, onValueChanged:
                value =>
                {
                    var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                    newHighContrastData.colorMinDistance = value;
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.Previsualize(controller.currentData);
                        EditorUtility.SetDirty(accessibilityManager);
                    }
                });
            
            var maxDistanceColorContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            maxDistanceColorContainer.style.display = DisplayStyle.None;
            
            var maxDistanceColorCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var maxDistanceColor = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.colorMaxDistance;
            var maxDistanceColorField = uiElementFactory.CreateSliderWithFloatField("table-row-multi-input", "Max Distance Color:", 3, 50, maxDistanceColor, onValueChanged:
                value =>
                {
                    var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                    newHighContrastData.colorMaxDistance = value;
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.Previsualize(controller.currentData);
                        EditorUtility.SetDirty(accessibilityManager);
                    }
                });
                
            var outlineColorContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            outlineColorContainer.style.display = DisplayStyle.None;
            
            var outlineColorCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var outlineColor = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.outlineColor;
            var outlineColorField = uiElementFactory.CreateColorField("table-row-option-input", "Outline Color:", outlineColor, "table-row-option-input-label",
                value =>
                {
                    var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                    newHighContrastData.outlineColor = new Color(value.r, value.g, value.b, value.a);
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.Previsualize(controller.currentData);
                        EditorUtility.SetDirty(accessibilityManager);
                    }
                });
            
            var outlineThicknessContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            outlineThicknessContainer.style.display = DisplayStyle.None;
            
            var outlineThicknessCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var outlineThickness = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.outlineThickness;
            var outlineThicknessField =
                uiElementFactory.CreateSliderWithFloatField("table-row-multi-input", "Outline Thickness:", 0, 1, outlineThickness, onValueChanged:
                    value =>
                    {
                        var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                        var currentRow = tableScrollView.IndexOf(row)-1;
                        ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                        newHighContrastData.outlineThickness = value;
                        controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                        if (controller.isPrevisualizing)
                        {
                            accessibilityManager.Previsualize(controller.currentData);
                            EditorUtility.SetDirty(accessibilityManager);
                        }
                    });
            
            var outlineFadeDistanceContainer = uiElementFactory.CreateVisualElement("table-secondary-row");
            outlineFadeDistanceContainer.style.display = DisplayStyle.None;
            
            var outlineFadeDistanceCell = uiElementFactory.CreateVisualElement("table-secondary-row-content");
            var outlineFadeDistance = controller.currentData.highContrastConfigurations.Items
                .Find(x => x.key == tableScrollView.IndexOf(row) - 1).value.outlineFadeDistance;
            var outlineFadeDistanceField = uiElementFactory.CreateSliderWithFloatField("table-row-multi-input", "Outline Fade Distance:", 1, 50, outlineFadeDistance, onValueChanged:
                value =>
                {
                    var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                    var currentRow = tableScrollView.IndexOf(row)-1;
                    ACC_HighContrastConfiguration newHighContrastData = controller.currentData.highContrastConfigurations.Items.Find(x => x.key == currentRow).value;
                    newHighContrastData.outlineFadeDistance = value;
                    controller.currentData.highContrastConfigurations.AddOrUpdate(currentRow, newHighContrastData);
                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.Previsualize(controller.currentData);
                        EditorUtility.SetDirty(accessibilityManager);
                    }
                });
            
            tagCell.Add(dropdownTag);
            tagContainer.Add(tagCell);
            
            colorCell.Add(colorField);
            colorContainer.Add(colorCell);
            
            minDistanceColorCell.Add(minDistanceColorField);
            minDistanceColorContainer.Add(minDistanceColorCell);
            
            maxDistanceColorCell.Add(maxDistanceColorField);
            maxDistanceColorContainer.Add(maxDistanceColorCell);
            
            outlineColorCell.Add(outlineColorField);
            outlineColorContainer.Add(outlineColorCell);
            
            outlineThicknessCell.Add(outlineThicknessField);
            outlineThicknessContainer.Add(outlineThicknessCell);
            
            outlineFadeDistanceCell.Add(outlineFadeDistanceField);
            outlineFadeDistanceContainer.Add(outlineFadeDistanceCell);
            
            row.Add(tagContainer);
            row.Add(colorContainer);
            row.Add(minDistanceColorContainer);
            row.Add(maxDistanceColorContainer);
            row.Add(outlineColorContainer);
            row.Add(outlineThicknessContainer);
            row.Add(outlineFadeDistanceContainer);
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
        public void CreateSettingsContainer()
        {
            settingsContainer.Clear();
            var settingsTitle = uiElementFactory.CreateLabel("title", "Settings");
            
            var nameInput = uiElementFactory.CreateTextField( "option-input", "Name: ", controller.currentData.name, "option-input-label", 
                value => controller.currentData.name = value);

            var previsualizationToggle = uiElementFactory.CreateToggle("option-input", "Previsualize: ", false, "option-input-label",
                    value =>
                    {
                        var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                        controller.isPrevisualizing = value;
                        if (controller.isPrevisualizing)
                        {
                            if (controller.isCreatingNewFileOnCreation || controller.isEditing)
                            {
                                accessibilityManager.Previsualize(controller.currentData);
                                EditorUtility.SetDirty(accessibilityManager);
                                if(refreshButton!=null) refreshButton.style.display = DisplayStyle.Flex;
                            }
                        }
                        else
                        {
                            if (controller.isCreatingNewFileOnCreation || controller.isEditing)
                            {
                                accessibilityManager.StopPrevisualize();
                                EditorUtility.SetDirty(accessibilityManager);
                                if(refreshButton!=null) refreshButton.style.display = DisplayStyle.None;
                            }
                        }
                    });
            
            settingsContainer.Add(settingsTitle);
            settingsContainer.Add(nameInput);
            settingsContainer.Add(previsualizationToggle);
            
        }
        private void CreateBottomContainer()
        {
            var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
            bottomContainer.style.marginTop = new StyleLength(Length.Auto());
            var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () =>
            {
                var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                if (controller.isCreatingNewFileOnCreation || controller.isEditing)
                {
                    controller.HandleSave(this);
                    if (accessibilityManager.isPrevisualizing)
                    {
                        accessibilityManager.StopPrevisualize();
                        EditorUtility.SetDirty(accessibilityManager);
                        
                        rootVisualElement.Q<Toggle>().value = false;
                    }
                }
            });
            
            refreshButton = uiElementFactory.CreateButton("Refresh", "button", () =>
            {
                var accessibilityManager = FindObjectOfType<ACC_AccessibilityManager>();
                if (controller.isCreatingNewFileOnCreation || controller.isEditing)
                {
                    accessibilityManager.shadersAdded = false;
                    accessibilityManager.OnHierarchyChanged();
                    accessibilityManager.shadersAdded = true;
                    accessibilityManager.OnHierarchyChanged();

                    if (controller.isPrevisualizing)
                    {
                        accessibilityManager.StopPrevisualize();
                        accessibilityManager.Previsualize(controller.currentData);
                    }
                    EditorUtility.SetDirty(accessibilityManager);
                }
            });

            var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
            var addSubtitlesLabel = uiElementFactory.CreateLabel("add-row-label", "Add new configuration:");
        
            var addSubtitle1 = uiElementFactory.CreateButton("+1", "add-row-button", () => CreateHighContrastConfiguration());
            var addSubtitle5 = uiElementFactory.CreateButton("+5", "add-row-button", () => 
                { for (int i = 0; i < 5; i++) CreateHighContrastConfiguration(); });
            var addSubtitle10 = uiElementFactory.CreateButton("+10", "add-row-button", () => 
                { for (int i = 0; i < 10; i++) CreateHighContrastConfiguration(); });
        
            addSubtitlesContainer.Add(addSubtitlesLabel);
            addSubtitlesContainer.Add(addSubtitle1);
            addSubtitlesContainer.Add(addSubtitle5);
            addSubtitlesContainer.Add(addSubtitle10); 
            bottomContainer.Add(createSubtitleButton);
            bottomContainer.Add(refreshButton);
            bottomContainer.Add(addSubtitlesContainer);

            rootVisualElement.Add(bottomContainer);
        }
    }
}
#endif