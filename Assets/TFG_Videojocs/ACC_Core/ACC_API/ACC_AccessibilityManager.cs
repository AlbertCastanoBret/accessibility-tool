using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using TFG_Videojocs.ACC_HighContrast;
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
    
    public class ACC_AccessibilityManager : MonoBehaviour
    {
        public static ACC_AccessibilityManager Instance { get; private set; } 
        internal GameObject accCanvas { get; private set; }
        
        [Header("Audio Accessibility")]
        [SerializeField] private bool subtitlesEnabled;
        [SerializeField] private bool visualNotificationEnabled;
        
        private ACC_AudioAccessibility accAudioAccessibility;
        
        [Header("Visual Accessibility")]
        [SerializeField] private bool highContrastEnabled;
        [HideInInspector] public bool shadersAdded, isPrevisualizing;
        
        private ACC_VisualAccessibility accVisualAccessibility;
        
        [Header("MobilityAccessibility")]
        [SerializeField] private bool remapControlsEnabled;
        
        private ACC_MobilityAccessibility accMobilityAccessibility;
            
        private bool sceneLoaded;
        
        #if UNITY_EDITOR
        ACC_AccessibilityManager()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        #endif

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                accCanvas = GameObject.Find("ACC_Canvas");
                accCanvas.transform.SetParent(transform);
                
                accAudioAccessibility = new ACC_AudioAccessibility();
                accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
                accAudioAccessibility.SetFeatureState(AudioFeatures.VisualNotification, visualNotificationEnabled);
                
                accVisualAccessibility = new ACC_VisualAccessibility();
                //  accVisualAccessibility.SetFeatureState(VisibilityFeatures.HighContrast, highContrastEnabled);
                
                accMobilityAccessibility = new ACC_MobilityAccessibility();
                accMobilityAccessibility.SetFeatureState(MobilityFeatures.RemapControls, remapControlsEnabled);
                
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            accAudioAccessibility.PlaySubtitle("A");
            ACC_AudioManager.Instance.PlaySFX("Alarm");
            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene()
        {
            yield return new WaitForSeconds(2);
            //accAudioAccessibility.ChangeSubtitleFontSize(20);
            //MobilityAccessibilityManager().ShowRemapControlsMenu("Gamepad");
            //yield return new WaitForSeconds(6);
            //LoadUserPreferences();
            //yield return new WaitForSeconds(6);
            //SceneManager.LoadScene(1);
            //yield return new WaitForSeconds(1);
            //AudioAccessibilityManager().PlaySubtitle("Ejemplo 2");
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && sceneLoaded)
            {
                accAudioAccessibility.SetFeatureState(AudioFeatures.Subtitles, subtitlesEnabled);
                accAudioAccessibility.SetFeatureState(AudioFeatures.VisualNotification, visualNotificationEnabled);
                accMobilityAccessibility.SetFeatureState(MobilityFeatures.RemapControls, remapControlsEnabled);
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

        private void Reset()
        {
            subtitlesEnabled = true;
            visualNotificationEnabled = true;
            remapControlsEnabled = true;
        }

        /// <summary>
        /// Retrieves the current instance of the audio accessibility manager.
        /// </summary>
        /// <returns>The active ACC_AudioAccessibility instance managing audio accessibility features.</returns>
        public ACC_AudioAccessibility AudioAccessibilityManager()
        {
            return accAudioAccessibility;
        }
        
        /// <summary>
        /// Retrieves the current instance of the mobility accessibility manager.
        /// </summary>
        /// <returns>The active ACC_MobilityAccessibility instance managing mobility accessibility features.</returns>
        public ACC_MobilityAccessibility MobilityAccessibilityManager()
        {
            return accMobilityAccessibility;
        }
        
        /// <summary>
        /// Loads user preferences for all accessibility modules.
        /// </summary>
        public void LoadAllUserPreferences()
        {
            accAudioAccessibility.LoadUserPreferences();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneLoaded = true;
        }
        private void OnSceneUnloading(Scene current)
        {
            sceneLoaded = false;
        }
        
        #if UNITY_EDITOR
        public void OnHierarchyChanged() 
        {
            Material highContrastColorMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/TFG_Videojocs/ACC_HighContrast/High-Contrast-Color.mat");
            Material highContrastOutlineMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/TFG_Videojocs/ACC_HighContrast/High-Contrast_Outline.mat");
            if (shadersAdded)
            {
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null && !AlreadyHasHighContrastColorMaterial(renderer))
                        {
                            var ambientOcclusionTexture = GetAmbientOcclusionTexture(renderer);
                            var materials = renderer.sharedMaterials;
                            var newMaterials = new Material[materials.Length + 1];
                            materials.CopyTo(newMaterials, 0);
                            newMaterials[materials.Length] = highContrastColorMaterial;
                            renderer.sharedMaterials = newMaterials;
                            renderer.sharedMaterials[^1].renderQueue = 50;
                            
                            if(ambientOcclusionTexture != null)
                            {
                                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propBlock, materials.Length);
                                propBlock.SetTexture("_AmbientOcclusion", ambientOcclusionTexture);
                                renderer.SetPropertyBlock(propBlock, materials.Length);
                            }
                        }
                        if(renderer != null && !AlreadyHasHighContrastOutlineMaterial(renderer))
                        {
                            var materials = renderer.sharedMaterials;
                            var newMaterials = new Material[materials.Length + 1];
                            materials.CopyTo(newMaterials, 0);
                            newMaterials[materials.Length] = highContrastOutlineMaterial;
                            renderer.sharedMaterials = newMaterials;
                            
                            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propBlock, materials.Length);
                            propBlock.SetFloat("_OutlineThickness", 0);
                            renderer.SetPropertyBlock(propBlock, materials.Length);
                        }
                    }
                }
            }
            else
            {
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null && AlreadyHasHighContrastOutlineMaterial(renderer))
                        {
                            var materials = renderer.sharedMaterials;
                            var newMaterials = new Material[materials.Length - 1];
                            int j = 0;
                            for (int i = 0; i < materials.Length; i++)
                            {
                                if (materials[i] != highContrastOutlineMaterial)
                                {
                                    newMaterials[j] = materials[i];
                                    j++;
                                }
                            }
                            renderer.sharedMaterials = newMaterials;
                        }
                        if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer))
                        {
                            var materials = renderer.sharedMaterials;
                            var newMaterials = new Material[materials.Length - 1];
                            int j = 0;
                            for (int i = 0; i < materials.Length; i++)
                            {
                                if (materials[i] != highContrastColorMaterial)
                                {
                                    newMaterials[j] = materials[i];
                                    j++;
                                }
                            }
                            renderer.sharedMaterials = newMaterials;
                        }
                    }
                }
            }
        }
        public void Previsualize(ACC_HighContrastData highContrastData=null)
        {
            if (shadersAdded)
            {
                isPrevisualizing = true;
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        ACC_SerializableDictiornary<int, ACC_HighContrastConfiguration>.ACC_KeyValuePair highContrastConfiguration = null;
                        if (highContrastData != null)
                        {
                            highContrastConfiguration = highContrastData.highContrastConfigurations.Items.Find(x => go.CompareTag(x.value.tag));
                        }
                        if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer) && highContrastConfiguration != null)
                        {
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = -50;
                            
                            MaterialPropertyBlock propColorBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propColorBlock, materials.Length - 2);
                            propColorBlock.SetColor("_Color", highContrastConfiguration.value.color);
                            renderer.SetPropertyBlock(propColorBlock, materials.Length - 2);
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetColor("_OutlineColor", highContrastConfiguration.value.outlineColor);
                            propOutlineBlock.SetFloat("_OutlineThickness", highContrastConfiguration.value.outlineThickness);
                            renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);
                        }
                        else if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer) &&
                                 highContrastConfiguration == null)
                        {
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = -50;
                            
                            MaterialPropertyBlock propColorBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propColorBlock, materials.Length - 2);
                            propColorBlock.SetColor("_Color", new Color(0.3679245f, 0.3679245f, 0.3679245f, 1));
                            renderer.SetPropertyBlock(propColorBlock, materials.Length - 2);
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetColor("_OutlineColor", Color.white);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0.6f);
                            renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);
                        }
                    }
                }
            }
        }
        public void StopPrevisualize()
        {
            if (shadersAdded)
            {
                isPrevisualizing = false;
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer))
                        {
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = 50;
                            
                            MaterialPropertyBlock propColorBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propColorBlock, materials.Length - 2);
                            propColorBlock.SetColor("_Color", new Color(0.3679245f, 0.3679245f, 0.3679245f, 1));
                            renderer.SetPropertyBlock(propColorBlock, materials.Length - 2);
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetColor("_OutlineColor", Color.white);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0);
                            renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);
                        }
                    }
                }
            }
        }
        private Texture GetAmbientOcclusionTexture(Renderer renderer)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material.HasProperty("_OcclusionMap") && material.GetTexture("_OcclusionMap") != null)
                    return material.GetTexture("_OcclusionMap");
            }
            return null;
        }
        private bool AlreadyHasHighContrastColorMaterial(Renderer renderer)
        {
            Material highContrastMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/TFG_Videojocs/ACC_HighContrast/High-Contrast-Color.mat");
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == highContrastMaterial)
                    return true;
            }
            return false;
        }
        private bool AlreadyHasHighContrastOutlineMaterial(Renderer renderer)
        {
            Material highContrastMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/TFG_Videojocs/ACC_HighContrast/High-Contrast_Outline.mat");
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == highContrastMaterial)
                    return true;
            }
            return false;
        }
        #endif
    }
}