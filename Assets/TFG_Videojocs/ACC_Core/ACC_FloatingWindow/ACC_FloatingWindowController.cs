using System;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TFG_Videojocs
{
    public abstract class ACC_FloatingWindowController
    {
        protected EditorWindow window;
        public  ACC_UIElementFactory uiElementFactory{get; private set;}
        protected ACC_AbstractData lastData;
        protected bool isEditing, isClosing;
        
        public void Initialize(EditorWindow window)
        {
            this.window = window;
            uiElementFactory = new ACC_UIElementFactory();
        }
        
        public virtual void ConfigureJson()
        {
            
        }
        
        public virtual void HandleSave()
        {
            
        }

        public void Cancel<TController>(ACC_BaseFloatingWindow<TController> window) where TController : ACC_FloatingWindowController, new()
        {
            var newWindow = Object.Instantiate(window);
            newWindow.titleContent = new GUIContent(window.titleContent.text);
            newWindow.minSize = window.minSize;
            newWindow.maxSize = window.maxSize;
            newWindow.Show();
            
            newWindow.CloneWindowAttributes(window);
            SubtractRemainingVisualElements(window.rootVisualElement, newWindow.rootVisualElement);
            AddRemainingVisualElements(window.rootVisualElement, newWindow.rootVisualElement);
            UpdateVisualElementValues(window.rootVisualElement, newWindow.rootVisualElement);
        }

        public void ConfirmSaveChangesIfNeeded<TController>(string name, ACC_BaseFloatingWindow<TController> window) where TController : ACC_FloatingWindowController, new()
        {
            if (IsThereAnyChange())
            {
                var result = EditorUtility.DisplayDialogComplex("Current configuration has been modified",
                    $"Do you want to save the changes you made in:\n./ACC_JSONSubtitle/{name}.json\n\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
                switch (result)
                {
                    case 0:
                        isClosing = true;
                        HandleSave();
                        //OnCloseSubtitleWindow?.Invoke();
                        break;
                    case 1:
                        Cancel(window);
                        break;
                    case 2:
                        break;
                }
            }
        }

        private bool IsThereAnyChange()
        {
            return true;
        }
        
        private void SubtractRemainingVisualElements(VisualElement source, VisualElement target)
        {
            List<VisualElement> elementsToRemove = new List<VisualElement>();

            foreach (var targetChild in target.Children())
            {
                if (targetChild.name != null)
                {
                    var sourceChild = source.Q(name: targetChild.name);
                    if (sourceChild == null)
                    {
                        elementsToRemove.Add(targetChild);
                    }
                    else
                    {
                        SubtractRemainingVisualElements(sourceChild, targetChild);
                    }
                }
            }

            foreach (var element in elementsToRemove)
            {
                target.Remove(element);
            }
        }
        
        private void AddRemainingVisualElements(VisualElement source, VisualElement target)
        {
            foreach (var sourceChild in source.Children())
            {
                var targetChild = target.Q(name: sourceChild.name);
                if (targetChild == null)
                {
                    targetChild = CloneElement(sourceChild);
                    target.Add(targetChild);
                }
                AddRemainingVisualElements(sourceChild, targetChild);
            }
        }
        
        private void UpdateVisualElementValues(VisualElement source, VisualElement target)
        {
            var sourceChildren = source.Children();
            var targetChildren = target.Children();

            int count = Mathf.Min(sourceChildren.Count(), targetChildren.Count());

            for (int i = 0; i < count; i++)
            {
                var sourceChild = sourceChildren.ElementAt(i);
                var targetChild = targetChildren.ElementAt(i);
                
                if (sourceChild is TextField sourceTextField && targetChild is TextField targetTextField)
                {
                    targetTextField.value = sourceTextField.value;
                }
                else if (sourceChild is IntegerField sourceIntegerField && targetChild is IntegerField targetIntegerField)
                {
                    targetIntegerField.value = sourceIntegerField.value;
                }
                else if (sourceChild is ColorField sourceColorField && targetChild is ColorField targetColorField)
                {
                    targetColorField.value = sourceColorField.value;
                }
                else if (sourceChild is SliderInt sourceSliderInt && targetChild is SliderInt targetSliderInt)
                {
                    targetSliderInt.value = sourceSliderInt.value;
                }
                UpdateVisualElementValues(sourceChild, targetChild);
            }
        }
        
        private VisualElement CloneElement(VisualElement sourceElement)
        {
            if(sourceElement is Label sourceLabel)
            {
                var newElement = new Label
                {
                    name = sourceLabel.name,
                    text = sourceLabel.text
                };
                
                foreach (var c in sourceLabel.GetClasses().ToArray())
                {
                    newElement.AddToClassList(c);
                }
                return newElement;
            }
            
            if (sourceElement is TextField sourceTextField)
            {
                var newElement = new TextField
                {
                    name = sourceTextField.name,
                    value = sourceTextField.value
                };
                
                foreach (var c in sourceTextField.GetClasses().ToArray())
                {
                    newElement.AddToClassList(c);
                }
                
                foreach (var c in sourceTextField[0].GetClasses().ToArray())
                {
                    newElement[0].AddToClassList(c);
                }
                return newElement;
            }
            
            if (sourceElement is IntegerField integerField)
            {
                var newElement = new IntegerField
                {
                    name = integerField.name,
                    value = integerField.value
                };
                
                foreach (var c in integerField.GetClasses().ToArray())
                {
                    newElement.AddToClassList(c);
                }
                
                foreach (var c in integerField[0].GetClasses().ToArray())
                {
                    newElement[0].AddToClassList(c);
                }
                return newElement;
            }
            
            if (sourceElement is ColorField colorField)
            {
                var newElement = new ColorField
                {
                    name = colorField.name,
                    value = colorField.value
                };
                
                foreach (var c in sourceElement.GetClasses().ToArray())
                {
                    newElement.AddToClassList(c);
                }
                return newElement;
            }
            
            if (sourceElement is SliderInt sliderInt)
            {
                var newElement = new SliderInt(sliderInt.label, sliderInt.lowValue, sliderInt.highValue)
                {
                    name = sliderInt.name,
                    value = sliderInt.value
                };
                
                foreach (var c in sourceElement.GetClasses().ToArray())
                {
                    newElement.AddToClassList(c);
                }
                
                foreach (var c in sliderInt[0].GetClasses().ToArray())
                {
                    newElement[0].AddToClassList(c);
                }
                return newElement;
            }
            
            if (sourceElement is Button button)
            {
                var newElement = new Button
                {
                    name = button.name,
                    text = button.text
                };
                
                foreach (var c in button.GetClasses().ToArray())
                {
                    newElement.AddToClassList(c);
                }
                
                newElement.clickable.clicked += ACC_ButtonActions.CloneAction(new List<VisualElement> {newElement});
                
                return newElement;
            }
            
            if (sourceElement is VisualElement sourceVisualElement)
            {
                var newElement = new VisualElement
                {
                    name = sourceVisualElement.name
                };
                
                foreach (var child in sourceVisualElement.Children())
                {
                    var clonedChild = CloneElement(child);
                    newElement.Add(clonedChild);
                }
                
                if (sourceElement.name != "unity-drag-container"
                    && sourceElement.name != "unity-tracker"
                    && sourceElement.name != "unity-dragger-border"
                    && sourceElement.name != "unity-dragger")
                {
                    foreach (var c in sourceVisualElement.GetClasses().ToArray())
                    {
                        newElement.AddToClassList(c);
                    }
                }
                return newElement;
            }
            
            throw new NotImplementedException($"Cloning not implemented for type {sourceElement.GetType()}");
        }
    }
}