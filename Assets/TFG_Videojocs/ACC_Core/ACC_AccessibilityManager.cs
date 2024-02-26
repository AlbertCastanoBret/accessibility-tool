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
        Pim,
        Pam,
    }

    public class ACC_AccessibilityManager : MonoBehaviour
    {
        public static ACC_AccessibilityManager Instance { get; private set; }
        private ACC_AudioAccessibility accAudioAccessibility;
        [Range(0, 10)]
        public float mySliderValue = 5;
        //private Queue<Action> actionsToPerformOnLoad = new Queue<Action>();
        
        [Header("Audio Accessibility")]
        [SerializeField] private bool subtitlesEnabled;
        [SerializeField] private bool visualNotificationEnabled;
            
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
            //StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene()
        {
            yield return new WaitForSeconds(7);
            SceneManager.LoadScene("Scene2");
            yield return new WaitForSeconds(1);
            AudioAccessibilityManager().PlaySubtitle("Ejemplo");
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && sceneLoaded)
            {
                accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
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
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            accAudioAccessibility = new ACC_AudioAccessibility();
            accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
            accAudioAccessibility.SetFeatureState(AudioFeatures.VisualNotification, visualNotificationEnabled);
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