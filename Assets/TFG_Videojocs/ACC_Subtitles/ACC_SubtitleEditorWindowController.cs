#if UNITY_EDITOR
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
    public class
        ACC_SubtitleEditorWindowController : ACC_FloatingWindowController<ACC_SubtitlesEditorWindow, ACC_SubtitleData>
    {
        public override void ConfigureJson()
        {
            base.ConfigureJson();
            ACC_PrefabHelper.CreatePrefab("Subtitles");
        }
        
        protected override void RestoreFieldValues()
        {
            window.rootVisualElement.Query<TextField>(name: "option-input-name-0").First().value = currentData.name;
            window.rootVisualElement.Query<ColorField>(name: "option-input-0").First().value = currentData.fontColor;
            window.rootVisualElement.Query<ColorField>(name: "option-input-1").First().value =
                currentData.backgroundColor;
            window.rootVisualElement.Query<SliderInt>(name: "multi-input-1-0").First().value = currentData.fontSize;

            var table = window.rootVisualElement.Query<VisualElement>(name: "subtitles-table-0").First();
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

            for (int i = 0; i < currentData.subtitleText.Items.Count; i++)
            {
                window.CreateRow(1, currentData.subtitleText.Items[i].value, currentData.timeText.Items[i].value);
            }
        }
    }
}
#endif