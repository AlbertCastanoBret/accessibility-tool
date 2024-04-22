using TFG_Videojocs.ACC_HighContrast.Toilet;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastEditorWindow:ACC_BaseFloatingWindow<ACC_HighContrastEditorWindowController, ACC_HighContrastEditorWindow, ACC_HighContrastData>
    {
        private VisualElement tableContainer;
        public static void ShowWindow(string name)
        {
            var window = GetWindow<ACC_HighContrastEditorWindow>();
            window.titleContent = new GUIContent("High Contrast Settings");
            window.minSize = new Vector2(600, 530);
            window.maxSize = new Vector2(600, 530);
            if (name != null)
            {
                window.controller.isEditing = true;
                window.controller.LoadJson(name);
            }
            //window.controller.lastData = window.controller.currentData.Clone() as ACC_HighContrastData;
        }
        
        private new void CreateGUI()
        {
            base.CreateGUI();
            //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_HighContrast/ACC_HighContrastWindowStyles.uss");
            //rootVisualElement.styleSheets.Add(styleSheet);
            
            CreateTable();
            CreateBottomContainer();
            
            controller.RestoreDataAfterCompilation();
        }

        private void CreateTable()
        {
            if(tableContainer == null) tableContainer = uiElementFactory.CreateVisualElement("container-2");
            else tableContainer.Clear();
            
            var highContrastTitle = uiElementFactory.CreateLabel("title", "High Contrast Settings");
            var tableScrollView = uiElementFactory.CreateScrollView("table-scroll-view");
            
            var controlSchemeTitle = uiElementFactory.CreateVisualElement("container-table-title");
            controlSchemeTitle.style.width = new StyleLength(Length.Percent(95));
            var controlSchemeNameTitle = uiElementFactory.CreateLabel("table-title-name", "Name");
            
            controlSchemeTitle.Add(controlSchemeNameTitle);
            tableScrollView.Add(controlSchemeTitle);
            
            for(int i=0; i<1; i++)
            {
                var row = uiElementFactory.CreateVisualElement("table-row");
                var name = uiElementFactory.CreateLabel("table-cell", "Hola");
                name.style.width = new StyleLength(Length.Percent(95));
                
                var deleteButton = uiElementFactory.CreateButton("-","table-delete-button");
                
                row.Add(name);
                row.Add(deleteButton);
                tableScrollView.Add(row);
            }
            
            tableContainer.Add(highContrastTitle);
            tableContainer.Add(tableScrollView);
            rootVisualElement.Add(tableContainer);
        }
        
        private void CreateBottomContainer()
        {
            var bottomContainer = uiElementFactory.CreateVisualElement("container-row");
            bottomContainer.style.marginTop = new StyleLength(Length.Auto());
            var createSubtitleButton = uiElementFactory.CreateButton("Save", "button", () => controller.HandleSave(this));

            var addSubtitlesContainer = uiElementFactory.CreateVisualElement("add-row-container");
            var addSubtitlesLabel = uiElementFactory.CreateLabel("add-row-label", "Add new configuration:");
        
            var addSubtitle1 = uiElementFactory.CreateButton("+1", "add-row-button");
            var addSubtitle5 = uiElementFactory.CreateButton("+5", "add-row-button");
            var addSubtitle10 = uiElementFactory.CreateButton("+10", "add-row-button");
        
            addSubtitlesContainer.Add(addSubtitlesLabel);
            addSubtitlesContainer.Add(addSubtitle1);
            addSubtitlesContainer.Add(addSubtitle5);
            addSubtitlesContainer.Add(addSubtitle10); 
            bottomContainer.Add(createSubtitleButton);
            bottomContainer.Add(addSubtitlesContainer);

            rootVisualElement.Add(bottomContainer);
        }
    }
}