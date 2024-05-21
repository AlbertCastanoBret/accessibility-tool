#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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

            if (manager.remapControlsMenus == null) manager.remapControlsMenus = new List<string>();
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
            EditorGUILayout.PropertyField(featureEnabledProperty, new GUIContent(featureEnabledProperty.displayName + " (Editor and Build)", "The value entered here will be used in the editor and in the build."));
            if (PlayerPrefs.HasKey(feature + "Enabled"))
            {
                if (GUILayout.Button("Reset Enable", new GUIStyle(GUI.skin.button)
                    {
                        fixedWidth = 118,
                        fixedHeight = 18,
                        fontStyle = FontStyle.Bold,
                        normal = {textColor = Color.white, background = MakeTex(120, 18, new Color(1, 0.5f, 0.5f))}
                    }))
                {
                    if (!string.IsNullOrEmpty(feature))
                    {
                        ResetEnable(feature);
                    }
                }
            }
            
            if (ExistsAnyKey(feature))
            {
                if (GUILayout.Button("Reset All Values", new GUIStyle(GUI.skin.button)
                    {
                        fixedWidth = 118,
                        fixedHeight = 18,
                        fontStyle = FontStyle.Bold,
                        normal = {textColor = Color.white, background = MakeTex(120, 18, Color.red)}
                    }))
                {
                    ResetValues(feature);
                }
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(showFeatureMenuProperty, new GUIContent(showFeatureMenuProperty.displayName + " (Only Editor)", "The value entered here will only be used in the editor."));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
            
            if (extraProperty != null)
            {
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
                    if (extraProperty.objectReferenceValue == null) return;
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

        private void ResetEnable(string feature)
        {
            switch (feature)
            {
                case "Subtitles":
                    if (ACC_AccessibilityManager.Instance != null) 
                        ACC_AccessibilityManager.Instance.AudioAccessibility.ResetFeatureState(AudioFeatures.Subtitles);
                    else
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled);
                    break;
                case "VisualNotification":
                    if (ACC_AccessibilityManager.Instance != null) 
                        ACC_AccessibilityManager.Instance.AudioAccessibility.ResetFeatureState(AudioFeatures.VisualNotification);
                    else
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationEnabled);
                    break;
                case "HighContrast":
                    if (ACC_AccessibilityManager.Instance != null) 
                        ACC_AccessibilityManager.Instance.VisualAccessibility.ResetFeatureState(VisualFeatures.HighContrast);
                    else
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled);
                    break;
                case "AudioManager":
                    if (ACC_AccessibilityManager.Instance != null) 
                        ACC_AccessibilityManager.Instance.MultifunctionalAccessibility.ResetFeatureState(MultifiunctionalFeatures.AudioManager);
                    else
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.AudioManagerEnabled);
                    break;
                case "RemapControls":
                    if (ACC_AccessibilityManager.Instance != null) 
                        ACC_AccessibilityManager.Instance.MobilityAccessibility.ResetFeatureState(MobilityFeatures.RemapControls);
                    else
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.RemapControlsEnabled);
                    break;
            }
        }

        private bool ExistsAnyKey(string feature)
        {
            switch (feature)
            {
                case "Subtitles":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled) || 
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorFontColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
                case "VisualNotification":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationEnabled) || 
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor) ||
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
                case "HighContrast":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled) || 
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration);
                case "RemapControls":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.RemapControlsEnabled);
                case "AudioManager":
                    return PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioManagerEnabled) || 
                           PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume);
                default:
                    return false;
            }
        }
        private void ResetValues(string feature)
        {
            switch (feature)
            {
                case "Subtitles":
                    if (ACC_AccessibilityManager.Instance != null)
                        ACC_AccessibilityManager.Instance.AudioAccessibility.ResetSubtitleSettings();
                    else
                    {
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorFontColor);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
                    }
                    break;
                case "VisualNotification":
                    if (ACC_AccessibilityManager.Instance != null)
                        ACC_AccessibilityManager.Instance.AudioAccessibility.ResetVisualNotificationSettings();
                    else
                    {
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationEnabled);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
                    }
                    break;
                case "HighContrast":
                    if (ACC_AccessibilityManager.Instance != null)
                        ACC_AccessibilityManager.Instance.VisualAccessibility.ResetHighContrastConfiguration();
                    else
                    {
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration);
                    }
                    break;
                case "RemapControls":
                    if (ACC_AccessibilityManager.Instance != null)
                        ACC_AccessibilityManager.Instance.MobilityAccessibility.ResetAllBindings();
                    else
                    {
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.RemapControlsEnabled);
                    }
                    break;
                case "AudioManager":
                    if (ACC_AccessibilityManager.Instance != null)
                        ACC_AccessibilityManager.Instance.MultifunctionalAccessibility.ResetFeatureState(MultifiunctionalFeatures.AudioManager);
                    else
                    {
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.AudioManagerEnabled);
                        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume);
                    }
                    break;
            }
        }
        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();

            return result;
        }
        private void OnRebindingMenuChanged(int newIndex)
        {
            if (newIndex < 0 || newIndex >= manager.remapControlsMenus.Count) return;
            manager.currentRemapControlsMenu = manager.remapControlsMenus[newIndex];
            if (Application.isPlaying)
            {
                manager.OnValidate();
            }
        }
    }
}
#endif