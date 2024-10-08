using System.Collections.Generic;
using ACC_API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastManager : MonoBehaviour
    {
        private ACC_HighContrastData loadedData;
        private bool isEnabled;
        private GameObject highContrastSettings, highContrastToggle, highContrastScrollList;
        private Material highContrastColorMaterial, highContrastOutlineMaterial;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("ACC_Prefab"))
                {
                    highContrastSettings = child.gameObject;
                    foreach (Transform settingComponent in highContrastSettings.transform)
                    {
                        if (settingComponent.CompareTag("ACC_Scroll"))
                        {
                            foreach (Transform scrollComponent in settingComponent)
                            {
                                if (scrollComponent.CompareTag("ACC_ScrollList"))
                                {
                                    highContrastScrollList = scrollComponent.gameObject;
                                    foreach (Transform settingsOption in scrollComponent)
                                    {
                                        if (settingsOption.name == "ACC_HighContrastEnable")
                                        {
                                            highContrastToggle = settingsOption.Find("Toggle").gameObject;
                                            var toggleComponent = highContrastToggle.GetComponent<Toggle>();
                                            toggleComponent.onValueChanged.AddListener(OnToggleValueChanged);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DisableHighContrastMenu();
        }
        private void Start()
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("ACC_Prefab"))
                {
                    foreach (Transform settingComponent in highContrastSettings.transform)
                    {
                        if (settingComponent.CompareTag("ACC_Scroll"))
                        {
                            foreach (Transform scrollComponent in settingComponent)
                            {
                                if (scrollComponent.CompareTag("ACC_ScrollList"))
                                {
                                    foreach (Transform settingsOption in scrollComponent)
                                    {
                                        if (settingsOption.name == "ACC_HighContrastSelector")
                                        {
                                            var allFiles = ACC_AccessibilityManager.Instance.VisualAccessibility
                                                .GetHighContrastConfigurations();
                                            var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                            dropdown.ClearOptions();
                                            dropdown.AddOptions(allFiles);
                                            dropdown.onValueChanged.AddListener((value) =>
                                            {
                                                ACC_AccessibilityManager.Instance.VisualAccessibility
                                                    .ChangeHighContrastConfiguration(allFiles[value]);
                                            });
                                            ACC_AccessibilityManager.Instance.VisualAccessibility.LoadUserPreferencesHighContrast();
                                        }
                                    }
                                }
                            }
                        }
                        if (settingComponent.name == "ACC_ResetButton")
                        {
                            var button = settingComponent.GetComponent<Button>();
                            button.onClick.AddListener(() =>
                            {
                                ACC_AccessibilityManager.Instance.VisualAccessibility
                                    .ResetHighContrastConfiguration();
                            });
                        }
                    }
                }
            }
        }
        private void OnToggleValueChanged(bool value)
        {
            ACC_AccessibilityManager.Instance.VisualAccessibility.
                SetFeatureState(VisualFeatures.HighContrast, value);
        }
        
        public void InitializeHighContrastMode(bool state)
        {
            if (state) EnableHighContrastMode();
            else DisableHighContrastMode();

            if (highContrastToggle != null)
            {
                highContrastToggle.GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleValueChanged);
                highContrastToggle.GetComponent<Toggle>().isOn = state;
                highContrastToggle.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
            }
            ACC_AccessibilityManager.Instance.highContrastEnabled = state;
            highContrastColorMaterial = ACC_AccessibilityManager.Instance.highContrastColorMaterial;
            highContrastOutlineMaterial = ACC_AccessibilityManager.Instance.highContrastOutlineMaterial;
            //ACC_AccessibilityManager.Instance.canChangeShaders = false;
        }
        public void SetHighContrastMode(bool state)
        {
            if (state) EnableHighContrastMode();
            else DisableHighContrastMode();

            PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.HighContrastEnabled, state ? 1 : 0);
            PlayerPrefs.Save();

            if (highContrastToggle != null)
                highContrastToggle.GetComponent<Toggle>().isOn =
                    PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled) &&
                    PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.HighContrastEnabled) == 1;
            ACC_AccessibilityManager.Instance.highContrastEnabled = state;
        }
        public void ResetHighContrastMode()
        {
            DisableHighContrastMode();
            if (highContrastToggle != null)
                highContrastToggle.GetComponent<Toggle>().isOn = false;
            ACC_AccessibilityManager.Instance.highContrastEnabled = false;
            PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled);
            PlayerPrefs.Save();
        }
        
        public void EnableHighContrastMenu()
        {
            if (highContrastSettings != null) highContrastSettings.SetActive(true);
        }
        public void DisableHighContrastMenu()
        {
            if (highContrastSettings != null) highContrastSettings.SetActive(false);
        }
        private void EnableHighContrastMode()
        {
            if (ACC_AccessibilityManager.Instance.shadersAdded)
            {
                isEnabled = true;
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer))
                        {
                            var ambientOcclusionTexture = GetAmbientOcclusionTexture(renderer);
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = -50;

                            MaterialPropertyBlock propColorBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propColorBlock, materials.Length - 2);
                            propColorBlock.SetColor("_Color", new Color(0.3679245f, 0.3679245f, 0.3679245f, 1));
                            propColorBlock.SetFloat("_MinDistance", 2);
                            propColorBlock.SetFloat("_MaxDistance", 6);
                            renderer.SetPropertyBlock(propColorBlock, materials.Length - 2);

                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetColor("_OutlineColor", Color.white);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0.6f);
                            propOutlineBlock.SetFloat("_FadeDistance", 5);
                            renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);

                            if (ambientOcclusionTexture != null)
                            {
                                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propBlock, materials.Length - 2);
                                propBlock.SetTexture("_AmbientOcclusion", ambientOcclusionTexture);
                                renderer.SetPropertyBlock(propBlock, materials.Length - 2);
                            }
                        }
                    }
                }

                if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration))
                {
                    ChangeHighContrastConfiguration(
                        PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.HighContrastConfiguration));
                }
                else if (highContrastScrollList != null)
                {
                    foreach (Transform settingsOption in highContrastScrollList.transform)
                    {
                        if (settingsOption.name == "ACC_HighContrastSelector")
                        {
                            var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                            dropdown.value = 0;
                            var allFiles = ACC_AccessibilityManager.Instance.VisualAccessibility
                                .GetHighContrastConfigurations();
                            if (allFiles.Count > 0) ChangeHighContrastConfiguration(allFiles[0]);
                        }
                    }
                }
            }
        }
        private void DisableHighContrastMode()
        {
            if (ACC_AccessibilityManager.Instance.shadersAdded)
            {
                isEnabled = false;
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

                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0);
                            renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);
                        }
                    }
                }
            }
        }
        public void ChangeHighContrastConfiguration(string configuration)
        {
            loadedData = ACC_JSONHelper.LoadJson<ACC_HighContrastData>("ACC_HighContrast/" + configuration);
            if (loadedData != null)
            {
                PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.HighContrastConfiguration, configuration);
                PlayerPrefs.Save();
            }
            if (loadedData != null && isEnabled)
            {
                if (ACC_AccessibilityManager.Instance.shadersAdded)
                {
                    GameObject[] goArray = FindObjectsOfType<GameObject>();
                    foreach (GameObject go in goArray)
                    {
                        if (go.activeInHierarchy)
                        {
                            go.isStatic = false;
                            Renderer renderer = go.GetComponent<Renderer>();
                            var highContrastConfiguration =
                                loadedData.highContrastConfigurations.Items.Find(x => go.CompareTag(x.value.tag));
                            if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer) && highContrastConfiguration != null)
                            {
                                var materials = renderer.sharedMaterials;
                                materials[^2].renderQueue = -50;

                                MaterialPropertyBlock propColorBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propColorBlock, materials.Length - 2);
                                propColorBlock.SetColor("_Color", highContrastConfiguration.value.color);
                                propColorBlock.SetFloat("_MinDistance", highContrastConfiguration.value.colorMinDistance);
                                propColorBlock.SetFloat("_MaxDistance", highContrastConfiguration.value.colorMaxDistance);
                                renderer.SetPropertyBlock(propColorBlock, materials.Length - 2);

                                MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                                propOutlineBlock.SetColor("_OutlineColor",
                                    highContrastConfiguration.value.outlineColor);
                                propOutlineBlock.SetFloat("_OutlineThickness",
                                    highContrastConfiguration.value.outlineThickness);
                                propOutlineBlock.SetFloat("_FadeDistance",
                                    highContrastConfiguration.value.outlineFadeDistance);
                                renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            }
                            else if (renderer != null && AlreadyHasHighContrastColorMaterial(renderer) && highContrastConfiguration == null)
                            {
                                var materials = renderer.sharedMaterials;
                                materials[^2].renderQueue = -50;

                                MaterialPropertyBlock propColorBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propColorBlock, materials.Length - 2);
                                propColorBlock.SetColor("_Color", new Color(0.3679245f, 0.3679245f, 0.3679245f, 1));
                                propColorBlock.SetFloat("_MinDistance", 2);
                                propColorBlock.SetFloat("_MaxDistance", 6);
                                renderer.SetPropertyBlock(propColorBlock, materials.Length - 2);

                                MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                                propOutlineBlock.SetColor("_OutlineColor", Color.white);
                                propOutlineBlock.SetFloat("_OutlineThickness", 0.6f);
                                propOutlineBlock.SetFloat("_FadeDistance", 5);
                                renderer.SetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            }
                        }
                    }
                }
            }
        }
        private bool AlreadyHasHighContrastColorMaterial(Renderer renderer)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == highContrastColorMaterial)
                    return true;
            }
            return false;
        }
        public List<string> GetHighContrastConfigurations()
        {
            ACC_HighContrastData[] configurations =
                ACC_JSONHelper.LoadAllFiles<ACC_HighContrastData>("ACC_HighContrast");
            List<string> configurationNames = new();
            foreach (ACC_HighContrastData configuration in configurations)
            {
                configurationNames.Add(configuration.name);
            }

            return configurationNames;
        }
        private Texture GetAmbientOcclusionTexture(Renderer renderer)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material == null) return null;
                if (material.HasProperty("_OcclusionMap") && material.GetTexture("_OcclusionMap") != null)
                    return material.GetTexture("_OcclusionMap");
            }

            return null;
        }
        public void LoadHighContrastSettings()
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled))
            {
                SetHighContrastMode(PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.HighContrastEnabled) == 1);
            }
            else
            {
                InitializeHighContrastMode(ACC_AccessibilityManager.Instance.highContrastEnabled);
            }
            
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration))
            {
                ChangeHighContrastConfiguration(PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.HighContrastConfiguration));
            }

            if (highContrastScrollList != null)
            {
                foreach (Transform settingsOption in highContrastScrollList.transform)
                {
                    if (settingsOption.name == "ACC_HighContrastEnable")
                    {
                        var toggle = settingsOption.Find("Toggle").GetComponent<Toggle>();
                        var toggleComponent = toggle.GetComponent<Toggle>();
                        toggleComponent.isOn = ACC_AccessibilityManager.Instance.VisualAccessibility.GetFeatureState(VisualFeatures.HighContrast);
                    }
                    if (settingsOption.name == "ACC_HighContrastSelector")
                    {
                        var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                        var allFiles = ACC_AccessibilityManager.Instance.VisualAccessibility.GetHighContrastConfigurations();
                        var key = ACC_AccessibilityManager.Instance.VisualAccessibility.GetCurrentHighContrastConfiguration();
                        if (key != default)
                        {
                            var index = allFiles.FindIndex(x => x == key);
                            dropdown.value = index;
                        }
                        else dropdown.value = 0;
                    }
                }
            }
        }
        public void ResetHighContrastConfiguration()
        {
            DisableHighContrastMode();
            
            if (highContrastScrollList != null)
            {
                foreach (Transform settingsOption in highContrastScrollList.transform)
                {
                    if (settingsOption.name == "ACC_HighContrastEnable")
                    {
                        var toggleComponent = highContrastToggle.GetComponent<Toggle>();
                        toggleComponent.isOn = false;
                    }
                                    
                    if (settingsOption.name == "ACC_HighContrastSelector")
                    {
                        var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                        dropdown.value = 0;
                        var allFiles = ACC_AccessibilityManager.Instance.VisualAccessibility
                            .GetHighContrastConfigurations();
                        if (allFiles.Count > 0) ChangeHighContrastConfiguration(allFiles[0]);
                    }
                }
            }
            
            PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastEnabled);
            PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.HighContrastConfiguration);
            PlayerPrefs.Save();
        }
    }
}