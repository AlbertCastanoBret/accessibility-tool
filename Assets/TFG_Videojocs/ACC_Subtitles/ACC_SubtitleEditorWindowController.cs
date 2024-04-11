using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TFG_Videojocs.ACC_Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;

namespace TFG_Videojocs.ACC_Subtitles
{
    public class ACC_SubtitleEditorWindowController: ACC_FloatingWindowController<ACC_SubtitlesEditorWindow, ACC_SubtitleData>
    {
        public override void LoadJson(string name)
        {
            ACC_SubtitleData subtitleData = ACC_JSONHelper.LoadJson<ACC_SubtitleData>("/ACC_JSONSubtitle/" + name);
            RestoreFieldValues(subtitleData);

            if (isEditing) oldName = subtitleData.name;
            currentData = subtitleData;
            lastData = subtitleData;
        }

        public override void RestoreDataAfterCompilation()
        {
            base.RestoreDataAfterCompilation();
            RestoreFieldValues(currentData);
            SessionState.EraseString( GetType() + "_tempData");
        }

        private void RestoreFieldValues(ACC_SubtitleData subtitleData)
        {
            window.rootVisualElement.Query<TextField>(name: "option-input-name-0").First().value = subtitleData.name;
            window.rootVisualElement.Query<ColorField>(name: "option-input-0").First().value = subtitleData.fontColor;
            window.rootVisualElement.Query<ColorField>(name: "option-input-1").First().value = subtitleData.backgroundColor;
            window.rootVisualElement.Query<SliderInt>(name: "slider-0").First().value = subtitleData.fontSize;
            
            var table = window.rootVisualElement.Query<VisualElement>(name: "table-0").First();
            List<VisualElement> rows = new List<VisualElement>();
            bool isFirstRow = true; 
            foreach (VisualElement child in table.Children())
            {
                if (!isFirstRow) rows.Add(child);
                isFirstRow = false;
            }
        
            foreach (VisualElement row in rows)
            {
                table.Remove(row);
            }
            
            for (int i = 0; i < subtitleData.subtitleText.Items.Count; i++)
            {
                window.CreateRow(1, subtitleData.subtitleText.Items[i].value, subtitleData.timeText.Items[i].value);
            }
        }
    }
}