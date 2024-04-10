using System;
using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TFG_Videojocs
{
    public abstract class ACC_BaseFloatingWindow<TController>: EditorWindow where TController : ACC_FloatingWindowController, new()
    {
        protected TController controller;
        protected ACC_UIElementFactory uiElementFactory => controller.uiElementFactory;
        
        protected virtual void OnEnable()
        {
            controller = new TController();
            controller.Initialize(this);
        }
        
        public void CloneWindowAttributes<T>(T sourceWindow) where T : ACC_BaseFloatingWindow<TController>
        {
            foreach (var item in sourceWindow.controller.uiElementFactory.nameCounters)
            {
                controller.uiElementFactory.nameCounters[item.Key] = item.Value;
            }
        }
    }
}