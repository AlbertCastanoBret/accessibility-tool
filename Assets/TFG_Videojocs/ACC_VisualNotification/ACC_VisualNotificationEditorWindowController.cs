#if UNITY_EDITOR
using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_VisualNotification
{
    public class ACC_VisualNotificationEditorWindowController: ACC_FloatingWindowController<ACC_VisualNotificationEditorWindow, ACC_VisualNotificationData>
    {
        public override void ConfigureJson()
        {
            if(currentData.soundsList.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "You must add at least one sound to the visual notification.", "Ok");
                if(isCreatingNewFileOnCreation) isCreatingNewFileOnCreation = false;
                if(isClosing) Cancel(window);
                return;
            }
            
            Dictionary<string, List<ACC_Sound.ACC_Sound>> repeatedSounds = new Dictionary<string, List<ACC_Sound.ACC_Sound>>(); 
            for (int i = 0; i < currentData.soundsList.Count; i++)
            {
                string fileName = ACC_JSONHelper.GetFileNameByListParameter<ACC_VisualNotificationData, ACC_Sound.ACC_Sound>(
                    "ACC_VisualNotification/",
                    data => data.soundsList,
                    (itemInList, itemToMatch) => itemInList.name == itemToMatch.name,
                    currentData.soundsList[i]
                );
                if (fileName != null && fileName != oldName + ".json" || fileName != null && isCreatingNewFileOnEdition || fileName != null && isOverWriting)
                {
                    if (!repeatedSounds.ContainsKey(fileName))
                    {
                        repeatedSounds.Add(fileName, new List<ACC_Sound.ACC_Sound>());
                        repeatedSounds[fileName].Add(currentData.soundsList[i]);
                    }
                    else repeatedSounds[fileName].Add(currentData.soundsList[i]);
                }
            }
            
            isCreatingNewFileOnEdition = false;
            isOverWriting = false;
            if (repeatedSounds.Count > 0 && !isRenamingFile)
            {
                ShowDialogRepeatedSounds(repeatedSounds);
            }
            else
            {
                isRenamingFile = false;
                currentData.soundsList.ForEach(sound => sound.currentVisualNotificationData = currentData.name);
                base.ConfigureJson();
                ACC_PrefabHelper.CreatePrefab("VisualNotification");
            }
        }

        protected override void RestoreFieldValues()
        {
            window.CreateTable();
            window.rootVisualElement.Query<TextField>(name: "option-input-0").First().value = currentData.name;
            window.rootVisualElement.Query<TextField>(name: "option-input-1").First().value = currentData.message;
            window.rootVisualElement.Query<DropdownField>(name: "option-input-2").First().value = currentData.horizontalAlignment;
            window.rootVisualElement.Query<DropdownField>(name: "option-input-3").First().value = currentData.verticalAlignment;
            window.rootVisualElement.Query<IntegerField>(name: "option-input-4").First().value = currentData.timeOnScreen;
            window.rootVisualElement.Query<ColorField>(name: "option-input-5").First().value = currentData.fontColor;
            window.rootVisualElement.Query<ColorField>(name: "option-input-6").First().value = currentData.backgroundColor;
            window.rootVisualElement.Query<SliderInt>(name: "multi-input-1-1").First().value = currentData.fontSize;
        }

        private void ShowDialogRepeatedSounds(Dictionary<string, List<ACC_Sound.ACC_Sound>> repeatedSounds)
        {
            string sounds = string.Join(", ", repeatedSounds);
            int option = EditorUtility.DisplayDialogComplex(
                "Some sounds already have a visual notification.",
                "Sounds \"" + sounds +
                "\" already have been added to another visual notification. What would you like to do?",
                "Move sounds",
                "Cancel",
                ""
            );
            switch (option)
            {
                case 0:
                    foreach (KeyValuePair<string, List<ACC_Sound.ACC_Sound>> kvp in repeatedSounds)
                    {
                        foreach (ACC_Sound.ACC_Sound sound in kvp.Value)
                        {
                            ACC_JSONHelper.RemoveItemFromListInFile<ACC_VisualNotificationData, ACC_Sound.ACC_Sound>(
                                "ACC_VisualNotification",
                                data => data.soundsList,
                                (itemInList, itemToMatch) => itemInList.name == itemToMatch.name,
                                sound
                            );
                        }
                    }
                    base.ConfigureJson();
                    break;
                case 1:
                    if(isClosing) Cancel(window);
                    break;
            }
        }
    }
}
#endif