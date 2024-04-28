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

namespace TFG_Videojocs
{
    [System.Serializable]
    public abstract class ACC_FloatingWindowController<TWindow, TData> where TWindow : EditorWindow where TData : ACC_AbstractData, ICloneable, new()
    {
        protected TWindow window;
        public ACC_UIElementFactory uiElementFactory;
        public TData currentData;
        public TData lastData;

        [SerializeField] public string oldName;
        [SerializeField] public bool isEditing, isClosing, isCreatingNewFileOnCreation, isOverWriting, isCreatingNewFileOnEdition, isRenamingFile, isDiscarting;
        
        public void Initialize(TWindow window)
        {
            this.window = window;
            uiElementFactory = new ACC_UIElementFactory();
            currentData = new TData();
            lastData = new TData();
            isClosing = false;
        }
        public virtual void LoadOnlyEditableWindow(string name, object objToInitialize = null)
        {
            currentData.name = name;
            
            var path = "/" + window.GetType().Name.Replace("EditorWindow", "") + "/";
            var exists = ACC_JSONHelper.FileNameAlreadyExists(path + name);
            
            if(exists) LoadJson(name);
            else
            {
                ConfigureJson();
            }
        }
        public virtual void ConfigureJson()
        {
            var path = "/" + window.GetType().Name.Replace("EditorWindow", "") + "/";
            ACC_JSONHelper.CreateJson(currentData, path);
            lastData = (TData)currentData.Clone(); 
            if (isEditing) oldName = currentData.name;
        }
        public virtual void LoadJson(string name)
        {
            var path = window.GetType().Name.Replace("EditorWindow", "") + "/";
            TData visualNotificationData = ACC_JSONHelper.LoadJson<TData>(path + name);

            if (isEditing) oldName = visualNotificationData.name;
            currentData = (TData) visualNotificationData.Clone();
            lastData = (TData) visualNotificationData.Clone();
            
            RestoreFieldValues();
        }
        
        protected abstract void RestoreFieldValues();
        public virtual void HandleSave<TController>(ACC_BaseFloatingWindow<TController, TWindow, TData> window) where TController : ACC_FloatingWindowController<TWindow, TData>, new()
        {
            var name = currentData.name;
            if (name.Length > 0)
            {
                var path = "/" + window.GetType().Name.Replace("EditorWindow", "") + "/";
                var fileExists = ACC_JSONHelper.FileNameAlreadyExists(path + name);
                
                if (!fileExists && !isEditing || fileExists && isEditing && string.Equals(name, oldName, StringComparison.OrdinalIgnoreCase))
                {
                    ConfigureJson();
                }
                else if(fileExists && !isEditing || fileExists && isEditing && !string.Equals(name, oldName, StringComparison.OrdinalIgnoreCase))
                {
                    int option = EditorUtility.DisplayDialogComplex(
                        "File name already exists",
                        "The name \"" + name + "\" already exists. What would you like to do?",
                        "Overwrite",
                        "Cancel",
                        ""
                    );
                    switch (option)
                    {
                        case 0:
                            isOverWriting = true;
                            ConfigureJson();
                            break;
                        case 1:
                            if(isClosing) Cancel(window);
                            break;
                    }
                }
                else if(!fileExists && isEditing)
                {
                    int option = EditorUtility.DisplayDialogComplex(
                        "Name Change Detected",
                        $"The name has been changed to \"{name}\". What would you like to do?",
                        "Create New File", 
                        "Cancel",
                        "Rename Existing File"
                    );
                    switch (option)
                    {
                        case 0:
                            isCreatingNewFileOnEdition = true;
                            ConfigureJson();
                            break;
                        case 1:
                            if(isClosing) Cancel(window);
                            break;
                        case 2:
                            isRenamingFile = true;
                            ACC_JSONHelper.RenameFile(path + oldName, path + name);
                            ConfigureJson();
                            break;
                    }
                }
                if (isCreatingNewFileOnCreation && !isEditing &&!isClosing) window.Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Required field", "Please, introduce a name before saving.", "OK");
                if(isClosing) Cancel(window);
            }
        }
        public void Cancel<TController>(ACC_BaseFloatingWindow<TController, TWindow, TData> window) where TController : ACC_FloatingWindowController<TWindow, TData>, new()
        {
            var newWindow = Object.Instantiate(window);
            newWindow.titleContent = new GUIContent(window.titleContent.text);
            newWindow.minSize = window.minSize;
            newWindow.maxSize = window.maxSize;
            newWindow.Show();
            
            newWindow.CloneWindowAttributes(window);
            newWindow.controller.RestoreFieldValues();
        }
        public void ConfirmSaveChangesIfNeeded<TController>(ACC_BaseFloatingWindow<TController, TWindow, TData> window) where TController : ACC_FloatingWindowController<TWindow, TData>, new()
        {   
            if(isDiscarting) return;
            if (IsThereAnyChange())
            {
                var path = "./" + window.GetType().Name.Replace("EditorWindow", "") + "/";
                var result = EditorUtility.DisplayDialogComplex("Current configuration has been modified",
                    $"Do you want to save the changes you made in:\n{path}{oldName}.json\n\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
                switch (result)
                {
                    case 0:
                        isClosing = true;
                        HandleSave(window);
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
        
        public void CloneControllerAttributes<TController>(TController sourceController) where TController : ACC_FloatingWindowController<TWindow, TData>
        {
            foreach (var item in sourceController.uiElementFactory.nameCounters.Items)
            {
                uiElementFactory.nameCounters.AddOrUpdate(item.key, item.value);
            }
            currentData = (TData)sourceController.currentData.Clone();
            lastData = new TData();
            lastData = (TData)sourceController.lastData.Clone();
            oldName = sourceController.oldName;
            isEditing = sourceController.isEditing;
            isClosing = sourceController.isClosing;
            isCreatingNewFileOnCreation = sourceController.isCreatingNewFileOnCreation;
            isOverWriting = sourceController.isOverWriting;
            isCreatingNewFileOnEdition = sourceController.isCreatingNewFileOnEdition;
            isRenamingFile = sourceController.isRenamingFile;
        }
        public void SerializeDataForCompilation(object obj)
        {
            var container = new ACC_PreCompilationDataStorage();
            var type = GetType();
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                if (value != null)
                {
                    var valueAsString = value is bool or int or float or string ? value.ToString() : JsonUtility.ToJson(value);
                    container.keyValuePairs.AddOrUpdate(field.Name, valueAsString);
                }
            }

            var json = JsonUtility.ToJson(container);
            SessionState.SetString(type + "_tempData", json);
        }
        public void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SerializeDataForCompilation(null);
            }
        }
        public virtual void RestoreDataAfterCompilation()
        {
            var type = GetType();
            var json = SessionState.GetString(type + "_tempData", "");
            if (json.Length > 0)
            {
                var container = JsonUtility.FromJson<ACC_PreCompilationDataStorage>(json);
                var fields = type.GetFields();
        
                foreach (var field in fields)
                {
                    var serializedValue = container.keyValuePairs.Items.FirstOrDefault(item => item.key == field.Name)?.value;
                    if (serializedValue != null)
                    {
                        object value = IsJson(serializedValue) ? 
                            JsonUtility.FromJson(serializedValue, field.FieldType) : 
                            Convert.ChangeType(serializedValue, field.FieldType);
                        field.SetValue(this, value);
                    }
                }
            }
            RestoreFieldValues();
            SessionState.EraseString(GetType() + "_tempData");
        }
        private bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}") || input.StartsWith("[") && input.EndsWith("]");
        }
        private bool IsThereAnyChange()
        {
            if(currentData.Equals(lastData)) return false;
            return true;
        }
    }
}
#endif