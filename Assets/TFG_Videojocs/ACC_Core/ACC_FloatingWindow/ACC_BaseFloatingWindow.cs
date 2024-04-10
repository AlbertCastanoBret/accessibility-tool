using System;
using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TFG_Videojocs
{
    public abstract class ACC_BaseFloatingWindow<TController,TWindow>: EditorWindow where TController : ACC_FloatingWindowController<TWindow>, new() where TWindow : EditorWindow
    {
        protected TController controller;
        protected ACC_UIElementFactory uiElementFactory => controller.uiElementFactory;

        protected ACC_BaseFloatingWindow()
        {
            controller = new TController();
            controller.Initialize(this as TWindow);
        }

        private void CreateGUI()
        {
            rootVisualElement.name = "root-visual-element";
            ColorUtility.TryParseHtmlString("#4f4f4f", out var backgroundColor);
            rootVisualElement.style.backgroundColor = new StyleColor(backgroundColor);
        }

        public void CloneWindowAttributes<T>(T sourceWindow) where T : ACC_BaseFloatingWindow<TController, TWindow>
        {
            foreach (var item in sourceWindow.controller.uiElementFactory.nameCounters)
            {
                controller.uiElementFactory.nameCounters[item.Key] = item.Value;
            }
        }
    }
}