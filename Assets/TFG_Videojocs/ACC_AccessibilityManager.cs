using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; // UnityEditor solo está disponible en el Editor de Unity.
#endif

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
        private ACC_AudioAccessibility accAudioAccessibility;
        //private Queue<Action> actionsToPerformOnLoad = new Queue<Action>();

        private Dictionary<AudioFeatures, bool> audioFeatureStates = new Dictionary<AudioFeatures, bool>();
        private Dictionary<VisualFeatures, bool> visualFeatureStates = new Dictionary<VisualFeatures, bool>();
        private Dictionary<AccessibilityFeature, bool> accessibilityFeatureStates = new Dictionary<AccessibilityFeature, bool>();
        
        [Header("Audio Accessibility")]
        [SerializeField] private bool subtitlesEnabled;
        private bool sceneLoaded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            AudioAccessibilityManager().PlaySubtitle("Ejemplo 2");
            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene()
        {
            yield return new WaitForSeconds(7);
            SceneManager.LoadScene("Scene2");
            yield return new WaitForSeconds(1);
            AudioAccessibilityManager().PlaySubtitle("Ejemplo");
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && sceneLoaded)
            {
                SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
            }
        }
        #endif

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloading;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloading;
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

        public ACC_AudioAccessibility AudioAccessibilityManager()
        {
            return accAudioAccessibility;
        }

        private void SetFeatureState<T>(Dictionary<T, bool> featureStates, T feature, bool enable) where T : Enum
        {
            featureStates[feature] = enable;
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
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            accAudioAccessibility = new ACC_AudioAccessibility();
            SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
            sceneLoaded = true;
            /*while (actionsToPerformOnLoad.Count > 0)
            {
                var action = actionsToPerformOnLoad.Dequeue();
                action();
            }*/
        }
        
        /*private void EnqueueAction(Action action)
        {
            if (sceneLoaded) action();
            else actionsToPerformOnLoad.Enqueue(action);
        }*/
        
        private void OnSceneUnloading(Scene current)
        {
            sceneLoaded = false;
        }
    }
}