using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
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
        private SerializedProperty remapControlsMenus;
        
        private SerializedProperty audioManagerEnabledProperty;
        private SerializedProperty showAudioManagerMenuProperty;

        private ACC_AccessibilityManager manager;
        private string[] inputActionAssets;
        private int selectedAssetIndex;
        private int previousSelectedAssetIndex;

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
            remapControlsMenus = serializedObject.FindProperty("remapControlsMenus");
            
            audioManagerEnabledProperty = serializedObject.FindProperty("audioManagerEnabled");
            showAudioManagerMenuProperty = serializedObject.FindProperty("showAudioManagerMenu");
            
            manager = (ACC_AccessibilityManager) target;

            var copiedArray = new string[manager.remapControlsMenus.Count];
            for (int i = 0; i < manager.remapControlsMenus.Count; i++)
            {
                copiedArray[i] = manager.remapControlsMenus[i];
            }
            
            inputActionAssets = copiedArray;
            selectedAssetIndex = 0;
            previousSelectedAssetIndex = selectedAssetIndex;
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
                // GUILayout.BeginHorizontal();
                // EditorGUILayout.PropertyField(remapControlsMenus);
                // GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                var copiedArray = new string[manager.remapControlsMenus.Count];
                for (int i = 0; i < manager.remapControlsMenus.Count; i++)
                {
                    copiedArray[i] = manager.remapControlsMenus[i];
                }
                inputActionAssets = copiedArray;
                
                selectedAssetIndex = EditorGUILayout.Popup("Select Rebinding Menu", selectedAssetIndex, inputActionAssets);
                GUILayout.EndHorizontal();
                
                OnRebindingMenuChanged(selectedAssetIndex);

                if (selectedAssetIndex != previousSelectedAssetIndex)
                {
                    OnRebindingMenuChanged(selectedAssetIndex);
                    previousSelectedAssetIndex = selectedAssetIndex;
                }
                
                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(extraProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    extraProperty.serializedObject.ApplyModifiedProperties();
                    var devices = ((InputActionAsset) extraProperty.objectReferenceValue).controlSchemes
                        .Select(scheme => 
                        {
                            return scheme.deviceRequirements
                                .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                                .Distinct()
                                .OrderBy(device => device)
                                .Aggregate((current, next) => current + ", " + next);
                        })
                        .Where(device => device != null)
                        .Distinct()
                        .ToList();
                    
                    manager.remapControlsMenus = new List<string>(devices);
                    remapControlsMenus.serializedObject.Update();
                    serializedObject.ApplyModifiedProperties();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        
        private void OnRebindingMenuChanged(int newIndex)
        {
            manager.currentRemapControlsMenu = manager.remapControlsMenus[newIndex];
            if (Application.isPlaying)
            {
                manager.OnValidate();
            }
        }
    }
}