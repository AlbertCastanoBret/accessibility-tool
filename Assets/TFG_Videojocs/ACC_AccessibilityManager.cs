using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public enum AudioFeatures
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
        public ACC_AudioAccessibility accAudioAccessibility { get; set; }

        private Dictionary<AudioFeatures, bool> audioFeatureStates = new Dictionary<AudioFeatures, bool>();
        private Dictionary<VisualFeatures, bool> visualFeatureStates = new Dictionary<VisualFeatures, bool>();
        private Dictionary<AccessibilityFeature, bool> accessibilityFeatureStates = new Dictionary<AccessibilityFeature, bool>();
        
        [Header("Audio Accessibility")]
        [SerializeField] private bool subtitlesEnabled;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                accAudioAccessibility = new ACC_AudioAccessibility();
                SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void SetFeatureState(AudioFeatures feature, bool enable)
        {
            SetFeatureState(audioFeatureStates, feature, enable);
        }

        public void SetFeatureState(VisualFeatures feature, bool enable)
        {
            SetFeatureState(visualFeatureStates, feature, enable);
        }

        public void SetFeatureState(AccessibilityFeature feature, bool enable)
        {
            SetFeatureState(accessibilityFeatureStates, feature, enable);
        }

        private void SetFeatureState<T>(Dictionary<T, bool> featureStates, T feature, bool enable) where T : Enum
        {
            if (featureStates.ContainsKey(feature))
            {
                featureStates[feature] = enable;
            }
            else
            {
                featureStates.Add(feature, enable);
            }
            ApplyFeatureSettings(feature, enable);
        }

        private void ApplyFeatureSettings<T>(T feature, bool enabled) where T : Enum
        {
            switch (feature)
            {
                case AudioFeatures.Subtitles:
                    accAudioAccessibility.EnableSubtitles(enabled);
                    break;
            }
        }
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            accAudioAccessibility = new ACC_AudioAccessibility();
        }
    }
}