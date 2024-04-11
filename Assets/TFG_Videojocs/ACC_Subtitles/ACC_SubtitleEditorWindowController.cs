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
using Object = UnityEngine.Object;

namespace TFG_Videojocs.ACC_Subtitles
{
    public class ACC_SubtitleEditorWindowController: ACC_FloatingWindowController<ACC_SubtitlesEditorWindow2>
    {
        public override void ConfigureJson()
        {
            ACC_SubtitleData subtitleData = new ACC_SubtitleData();
            var table = window.rootVisualElement.Query<VisualElement>(name: "table-0").First(); 
            for (int i = 1; i < table.childCount; i++)
            {
                var row = table[i];
                var subtitleElement = row.Query<TextField>().First();
                subtitleData.subtitleText.Add(new ACC_KeyValuePairData<int, string>(i, subtitleElement.value));

                var timeElement = row.Query<IntegerField>().First();
                subtitleData.timeText.Add(new ACC_KeyValuePairData<int, int>(i, timeElement.value));
            }
            
            subtitleData.name = window.rootVisualElement.Query<TextField>(name: "option-input-name-0").First().value;
            subtitleData.fontColor = window.rootVisualElement.Query<ColorField>(name: "option-input-0").First().value;
            subtitleData.backgroundColor = window.rootVisualElement.Query<ColorField>(name: "option-input-1").First().value;
            subtitleData.fontSize = window.rootVisualElement.Query<SliderInt>(name: "font-size-slider-0").First().value;
            
            ACC_JSONHelper.CreateJson(subtitleData, "/ACC_JSONSubtitle/");
            lastData = subtitleData;
            if (isEditing) oldName = subtitleData.name;
        }

        public override void LoadJson(string name)
        {
            ACC_SubtitleData subtitleData = ACC_JSONHelper.LoadJson<ACC_SubtitleData>("/ACC_JSONSubtitle/" + name);
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
            
            if (isEditing) oldName = subtitleData.name;
            
            for (int i = 0; i < subtitleData.subtitleText.Count; i++)
            {
                window.CreateRow(1, subtitleData.subtitleText[i].value, subtitleData.timeText[i].value);
            }
        }
    }
}