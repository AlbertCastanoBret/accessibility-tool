using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_Utilities
{
    [CustomEditor(typeof(ACC_AccessibilityManager))]
    public class ACC_AccessibilityManagerEditor : Editor
    {
        private SerializedProperty subtitlesEnabledProperty;
        private SerializedProperty showSubtitlesMenuProperty;
        private SerializedProperty visualNotificationEnabledProperty;
        private SerializedProperty showVisualNotificationMenuProperty;
        
        private SerializedProperty highContrastEnabledProperty;
        private SerializedProperty showHighContrastMenuProperty;
        
        private SerializedProperty remapControlsEnabledProperty;
        private SerializedProperty showRemapControlsMenuProperty;
        private SerializedProperty remapControlsAssetProperty;
        
        private SerializedProperty audioManagerEnabledProperty;
        private SerializedProperty showAudioManagerMenuProperty;
        
        private string[] inputActionAssets = { "Default", "Custom1", "Custom2", "Custom3" };
        private int selectedAssetIndex = 0;

        private void OnEnable()
        {
            subtitlesEnabledProperty = serializedObject.FindProperty("subtitlesEnabled");
            showSubtitlesMenuProperty = serializedObject.FindProperty("showSubtitlesMenu");
            visualNotificationEnabledProperty = serializedObject.FindProperty("visualNotificationEnabled");
            showVisualNotificationMenuProperty = serializedObject.FindProperty("showVisualNotificationMenu");
            
            highContrastEnabledProperty = serializedObject.FindProperty("highContrastEnabled");
            showHighContrastMenuProperty = serializedObject.FindProperty("showHighContrastMenu");
            
            remapControlsEnabledProperty = serializedObject.FindProperty("remapControlsEnabled");
            showRemapControlsMenuProperty = serializedObject.FindProperty("showRemapControlsMenu");
            remapControlsAssetProperty = serializedObject.FindProperty("remapControlsAsset");
            
            audioManagerEnabledProperty = serializedObject.FindProperty("audioManagerEnabled");
            showAudioManagerMenuProperty = serializedObject.FindProperty("showAudioManagerMenu");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Accessibility", new GUIStyle(EditorStyles.boldLabel));
            GUILayout.EndHorizontal();
            
            DrawAccessibilityFeature(subtitlesEnabledProperty, showSubtitlesMenuProperty);
            DrawAccessibilityFeature(visualNotificationEnabledProperty, showVisualNotificationMenuProperty);
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Visual Accessibility", new GUIStyle(EditorStyles.boldLabel));
            GUILayout.EndHorizontal();
            
            DrawAccessibilityFeature(highContrastEnabledProperty, showHighContrastMenuProperty);
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mobility Accessibility", new GUIStyle(EditorStyles.boldLabel));
            GUILayout.EndHorizontal();
            
            DrawAccessibilityFeature(remapControlsEnabledProperty, showRemapControlsMenuProperty, remapControlsAssetProperty);
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Multifunctional Accessibility", new GUIStyle(EditorStyles.boldLabel));
            GUILayout.EndHorizontal();
            
            DrawAccessibilityFeature(audioManagerEnabledProperty, showAudioManagerMenuProperty);
            GUILayout.Space(10);
            
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawAccessibilityFeature(SerializedProperty featureEnabledProperty, SerializedProperty showFeatureMenuProperty, SerializedProperty extraProperty = null)
        {
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(featureEnabledProperty);
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 120,
            };
            if (GUILayout.Button("Delete Keys", style))
            {
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(showFeatureMenuProperty);
            GUILayout.EndHorizontal();
            
            if (extraProperty != null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(extraProperty);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                selectedAssetIndex = EditorGUILayout.Popup("Select Rebinding Menu", selectedAssetIndex, inputActionAssets);
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndVertical();
        }
    }
}