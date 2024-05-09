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
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                fixedHeight = 22,
            };
            
            serializedObject.Update();
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Accessibility", titleStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            DrawAccessibilityFeature(subtitlesEnabledProperty, showSubtitlesMenuProperty, "Subtitles");
            DrawAccessibilityFeature(visualNotificationEnabledProperty, showVisualNotificationMenuProperty, "VisualNotification");
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Visual Accessibility", titleStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            DrawAccessibilityFeature(highContrastEnabledProperty, showHighContrastMenuProperty, "HighContrast");
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mobility Accessibility", titleStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            DrawAccessibilityFeature(remapControlsEnabledProperty, showRemapControlsMenuProperty, "RemapControls", remapControlsAssetProperty);
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Multifunctional Accessibility", titleStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            DrawAccessibilityFeature(audioManagerEnabledProperty, showAudioManagerMenuProperty, "AudioManager");
            GUILayout.Space(10);
            
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawAccessibilityFeature(SerializedProperty featureEnabledProperty, SerializedProperty showFeatureMenuProperty, string feature="", SerializedProperty extraProperty = null)
        {
            GUIStyle subTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 20,
                normal = {textColor = new Color(0.7f,0.7f,0.7f,1)}
            };
            
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(feature, subTitleStyle);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(featureEnabledProperty);
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 120,
            };
            if (PlayerPrefs.HasKey(feature + "Enabled"))
            {
                if (GUILayout.Button("Delete Key", style))
                {
                    if (!string.IsNullOrEmpty(feature))
                    {
                        PlayerPrefs.DeleteKey(feature + "Enabled");
                    }
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            Rect totalRect = EditorGUILayout.GetControlRect();
            
            float halfWidth = totalRect.width / 2;
            
            Rect labelRect = new Rect(totalRect.x, totalRect.y, halfWidth, totalRect.height);
            Rect buttonRect = new Rect(totalRect.x + halfWidth, totalRect.y, halfWidth, totalRect.height);
            
            EditorGUI.LabelField(labelRect, "Reset values");
            if (ExistsAnyKey(feature))
            {
                if (GUI.Button(buttonRect, "Reset"))
                {
                    ResetValues(feature);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(showFeatureMenuProperty);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
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
            GUILayout.Space(5);
        }

        private bool ExistsAnyKey(string feature)
        {
            switch (feature)
            {
                case "Subtitles":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorFontColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
                case "VisualNotification":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
                case "HighContrast":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration);
                case "RemapControls":
                    return false;
                case "AudioManager":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume);
                default:
                    return false;
            }
        }
        
        private void ResetValues(string feature)
        {
            switch (feature)
            {
                case "Subtitles":
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorFontColor);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
                    break;
                case "VisualNotification":
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor);
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
                    break;
                case "HighContrast":
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration);
                    break;
                case "RemapControls":
                    break;
                case "AudioManager":
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume);
                    break;
            }
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