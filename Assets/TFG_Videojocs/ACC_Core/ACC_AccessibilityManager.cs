using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    
    public enum VisualFeatures
    {
        TextToVoice,
    }

    public class ACC_AccessibilityManager : MonoBehaviour
    {
        public static ACC_AccessibilityManager Instance { get; private set; }
        private ACC_AudioAccessibility accAudioAccessibility;

        //private Queue<Action> pendingActions = new Queue<Action>();
        
        [Header("Audio Accessibility")]
        [SerializeField] private bool subtitlesEnabled;
        [SerializeField] private bool visualNotificationEnabled;
            
        private bool sceneLoaded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                accAudioAccessibility = new ACC_AudioAccessibility();
                accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
                accAudioAccessibility.SetFeatureState(AudioFeatures.VisualNotification, visualNotificationEnabled);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            AudioAccessibilityManager().PlaySubtitle("Ejemplo");
            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene()
        {
            yield return new WaitForSeconds(6);
            //LoadUserPreferences();
            //yield return new WaitForSeconds(6);
            //SceneManager.LoadScene(1);
            //yield return new WaitForSeconds(1);
            AudioAccessibilityManager().PlaySubtitle("Ejemplo 2");
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && sceneLoaded)
            {
                accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
                accAudioAccessibility.SetFeatureState(AudioFeatures.VisualNotification, visualNotificationEnabled);
            }
        }

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

        public ACC_AudioAccessibility AudioAccessibilityManager()
        {
            return accAudioAccessibility;
        }

        public void LoadUserPreferences()
        {
            accAudioAccessibility.LoadUserPreferences();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (accAudioAccessibility == null)
            {
                accAudioAccessibility = new ACC_AudioAccessibility();
                accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
                accAudioAccessibility.SetFeatureState(AudioFeatures.VisualNotification, visualNotificationEnabled);
            }
            sceneLoaded = true;
            /*while (pendingActions.Count > 0)
            {
                var action = pendingActions.Dequeue();
                action();
            }*/
        }
        
        private void OnSceneUnloading(Scene current)
        {
            sceneLoaded = false;
            accAudioAccessibility = null;
        }

        public bool IsSceneLoaded()
        {
            return sceneLoaded;
        }
    }
}