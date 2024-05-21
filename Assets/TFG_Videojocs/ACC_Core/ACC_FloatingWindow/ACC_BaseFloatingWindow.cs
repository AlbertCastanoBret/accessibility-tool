#if UNITY_EDITOR
using System;
using System.Reflection;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFG_Videojocs
{
    public abstract class ACC_BaseFloatingWindow<TController,TWindow, TData>: EditorWindow where TController : ACC_FloatingWindowController<TWindow, TData>, new() where TWindow : EditorWindow where TData : ACC_AbstractData, new()
    {
        public TController controller { get; private set; }
        protected ACC_UIElementFactory uiElementFactory => controller.uiElementFactory;
        public delegate void WindowDelegate(string directory);
        
        private Vector2 fixedPosition = new Vector2(100, 100);
        private bool positionSet = false;
        
        protected void OnEnable()
        {
            controller = new TController();
            controller.Initialize(this as TWindow);
            CompilationPipeline.compilationStarted += controller.SerializeDataForCompilation;
            EditorApplication.playModeStateChanged += controller.OnPlayModeStateChanged;
            EditorApplication.quitting -= OnEditorQuitting;
            EditorApplication.quitting += OnEditorQuitting;
        }
        private void OnDisable()
        {
            CompilationPipeline.compilationStarted -= controller.SerializeDataForCompilation;
        }
        private void OnEditorQuitting()
        {
            var key = GetType() + "Open";
            EditorPrefs.SetString(key, controller.oldName);
        }
        protected void OnDestroy()
        {
            controller.ConfirmSaveChangesIfNeeded(this);
        }
        protected void CreateGUI()
        {
            rootVisualElement.Clear();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Core/ACC_FloatingWindow/ACC_BaseFloatingWindowStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            rootVisualElement.name = "root-visual-element";
            ColorUtility.TryParseHtmlString("#4f4f4f", out var backgroundColor);
            rootVisualElement.style.backgroundColor = new StyleColor(backgroundColor);
            rootVisualElement.AddToClassList("main-container");
        }
        private void Update()
        {
            if (positionSet && (Math.Abs(position.x - fixedPosition.x) > 0.1f || Math.Abs(position.y - fixedPosition.y) > 0.1f))
            {
                PositionWindowInBottomRight();
            }
        }
        
        public static void CloseWindowIfExists<T>() where T : EditorWindow
        {
            T window = (T)GetWindow(typeof(T), false);
            ACC_BaseFloatingWindow<TController,TWindow, TData> floatingWindow = window as ACC_BaseFloatingWindow<TController, TWindow, TData>;
            if (floatingWindow != null)
            {
                floatingWindow.controller.isDiscarting = true;
                floatingWindow.Close();
            }
        }
        protected void PositionWindowInBottomRight()
        {
            var mainWindow = GetEditorMainWindowPos();
            var x = mainWindow.x + mainWindow.width - minSize.x;
            var y = mainWindow.y + mainWindow.height - minSize.y;

            position = new Rect(x-10, y-50, minSize.x, minSize.y);
        }
        private Rect GetEditorMainWindowPos()
        {
            var containerWinType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
            var showModeField = containerWinType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);

            foreach (var win in windows)
            {
                var showmode = (int)showModeField.GetValue(win);
                if (showmode == 4)
                {
                    var position = (Rect)positionProperty.GetValue(win);
                    return position;
                }
            }
            throw new InvalidOperationException("Cannot find main editor window.");
        }
        protected void SetFixedPosition()
        {
            fixedPosition = new Vector2(position.x, position.y);
            positionSet = true;
        }
        public void CloneWindowAttributes<T>(T sourceWindow) where T : ACC_BaseFloatingWindow<TController, TWindow, TData>
        {
            var type = GetType();
            var fields = type.GetFields();
            
            /*foreach (var field in fields)
            {
                field.SetValue(this, field.GetValue(sourceWindow));
            }*/
            
            controller.CloneControllerAttributes(sourceWindow.controller);
        }
    }
}
#endif