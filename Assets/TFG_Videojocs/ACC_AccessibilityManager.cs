using System.Collections.Generic;
using UnityEngine;

namespace TFG_Videojocs
{
    public enum AccessibilityFeature
    {
        Subtitles,
        HighContrast,
        TextSizeAdjustment,
        RemapControls,
        Audio3D,
        SimplifyTutorials,
        ContextualReminders
    }
    
    public enum AuditiveFeatures
    {
        Subtitles,
        VisualNotification,
        PimPam,
        Pum,
        Wow
    }
    
    public enum VisualFeatures
    {
        TextToVoice,
        Pim,
        Pam,
    }

    public class ACC_AccessibilityManager : MonoBehaviour
    {
        public static ACC_AccessibilityManager Instance { get; private set; }

        private Dictionary<AccessibilityFeature, bool> featureStates = new Dictionary<AccessibilityFeature, bool>();

        public ACC_AudioAccessibility accAudioAccessibility { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                accAudioAccessibility = new ACC_AudioAccessibility();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void EnableFeature(AccessibilityFeature feature)
        {
            if (featureStates.ContainsKey(feature))
            {
                featureStates[feature] = true;
            }
            else
            {
                featureStates.Add(feature, true);
            }
            ApplyFeatureSettings(feature, true);
        }

        public void DisableFeature(AccessibilityFeature feature)
        {
            if (featureStates.ContainsKey(feature))
            {
                featureStates[feature] = false;
                ApplyFeatureSettings(feature, false);
            }
        }

        public bool IsFeatureEnabled(AccessibilityFeature feature)
        {
            if (featureStates.ContainsKey(feature))
            {
                return featureStates[feature];
            }
            return false;
        }

        private void ApplyFeatureSettings(AccessibilityFeature feature, bool enabled)
        {
            switch (feature)
            {
                case AccessibilityFeature.Subtitles:
                    break;
                case AccessibilityFeature.HighContrast:
                    break;
            }
        }
    
        public void SetAccessibilityOption(string optionName, object value)
        {
        
        }
    }
}