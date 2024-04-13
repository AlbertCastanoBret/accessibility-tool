using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_VisualNotification
{
    public class ACC_VisualNotificationEditorWindowController: ACC_FloatingWindowController<ACC_VisualNotificationEditorWindow, ACC_VisualNotificationData>
    {
        protected override void RestoreFieldValues()
        {
            window.CreateSoundList();
            window.rootVisualElement.Query<TextField>(name: "option-input-name-0").First().value = currentData.name;
            window.rootVisualElement.Query<TextField>(name: "option-input-0").First().value = currentData.message;
            window.rootVisualElement.Query<DropdownField>(name: "option-input-1").First().value = currentData.horizontalAlignment;
            window.rootVisualElement.Query<DropdownField>(name: "option-input-2").First().value = currentData.verticalAlignment;
            window.rootVisualElement.Query<IntegerField>(name: "option-input-3").First().value = currentData.timeOnScreen;
            window.rootVisualElement.Query<ColorField>(name: "option-input-4").First().value = currentData.fontColor;
            window.rootVisualElement.Query<ColorField>(name: "option-input-5").First().value = currentData.backgroundColor;
            window.rootVisualElement.Query<SliderInt>(name: "multi-input-1-1").First().value = currentData.fontSize;
        }
    }
}