using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

namespace TFG_Videojocs.ACC_RemapControls
{
    public class ACC_RebindControlsManager:MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Editor Only")]
        public GameObject defaultMenu;
        public GameObject rebindUIPrefab;
        public GameObject rebindOverlay;
        private TextMeshProUGUI rebindPrompt;
#endif
        [Header("Runtime Only")]
        [SerializeField] private ACC_ControlSchemeData loadedData;
        private Dictionary<GameObject, List<string>> controlSchemesOfEachDevice = new();
        private Dictionary<GameObject, string> currentControlSchemeOfEachDevice = new();

        private void Awake()
        {
            controlSchemesOfEachDevice = new();
            currentControlSchemeOfEachDevice = new();
            
            for(int i=0; i < gameObject.transform.childCount; i++)
            {
                var deviceManager = gameObject.transform.GetChild(i).gameObject;
                if (deviceManager.CompareTag("DeviceManager"))
                {
                    var controlSchemes = loadedData.inputActionAsset.controlSchemes
                        .Where(scheme => String.Join(", ", scheme.deviceRequirements
                            .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                            .Distinct()
                            .OrderBy(d => d)) == gameObject.transform.GetChild(i).name)
                        .Select(scheme => scheme.name)
                        .ToList();
                    
                    controlSchemesOfEachDevice.Add(deviceManager, controlSchemes.Count > 0 ? controlSchemes : new List<string> { "Default" });
                    currentControlSchemeOfEachDevice.Add(deviceManager, controlSchemesOfEachDevice[deviceManager][0]);

                    deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent")
                        .Find("LeftArrow").GetComponent<Button>().onClick.AddListener(() =>
                            PressLeftButton(deviceManager,
                                deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent")
                                    .Find("CurrentControlScheme").gameObject,
                            deviceManager.transform.Find("RebindsScroll").gameObject));
                    
                    deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent")
                        .Find("RightArrow").GetComponent<Button>().onClick.AddListener(() => 
                            PressRightButton(deviceManager,
                                 deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent")
                                    .Find("CurrentControlScheme").gameObject,
                                 deviceManager.transform.Find("RebindsScroll").gameObject));
                    
                    deviceManager.transform.Find("ResetAllButton").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ResetControlSchemeBindings(currentControlSchemeOfEachDevice[deviceManager]);
                    });
                }
            }    
        }

#if UNITY_EDITOR
        
        public void CreateRebindControlsManager(string jsonFile)
        {
            string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_ControlSchemesConfiguration/" + jsonFile + ".json");
            var loadedData = JsonUtility.FromJson<ACC_ControlSchemeData>(json);
            
            this.loadedData = loadedData;
            
            if (Application.isPlaying)
            {
                Debug.LogError("DestroyAllChildrenInEditor should not be called during play mode.");
                return;
            }
            
            ResetRebindControlsManager();
            SetControlSchemes(loadedData);
        }
        private void ResetRebindControlsManager()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            
            var waitForInputPanel = Instantiate(rebindOverlay, transform, true);
            waitForInputPanel.transform.localScale = new Vector3(1, 1, 1);
            waitForInputPanel.transform.localPosition = new Vector3(0, 0, 0);
            waitForInputPanel.transform.SetParent(transform);
            
            rebindPrompt = waitForInputPanel.transform.Find("WaitForInputText").GetComponent<TextMeshProUGUI>();
        }
        private void SetControlSchemes(ACC_ControlSchemeData loadedData)
        {
            var devices = loadedData.inputActionAsset.controlSchemes
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

            bool first = true;
            
            foreach (var device in devices)
            {
                var deviceManager = Instantiate(defaultMenu, transform, true);
                deviceManager.name = device;
                deviceManager.transform.localScale = new Vector3(1, 1, 1);
                deviceManager.transform.localPosition = new Vector3(0, 0, 0);

                deviceManager.SetActive(first);
                first = false;
                
                var controlSchemes = loadedData.inputActionAsset.controlSchemes
                    .Where(scheme => String.Join(", ", scheme.deviceRequirements
                        .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                        .Distinct()
                        .OrderBy(d => d)) == device)
                    .Select(scheme => scheme.name)
                    .ToList();
                
                deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent").Find("CurrentControlScheme").GetComponent<TextMeshProUGUI>().text =
                    controlSchemes[0];
                
                SetBindings(loadedData, deviceManager, controlSchemes);
            }
        }
        
        private void SetBindings(ACC_ControlSchemeData loadedData, GameObject deviceManager, List<string> controlSchemes)
        {
            bool first = true;
            foreach (var controlScheme in controlSchemes)
            {
                var rebindingsScroll = deviceManager.transform.Find("RebindsScroll");
                
                var controlSchemeParent = new GameObject
                {
                    name = controlScheme,
                    tag = "RebindsList"
                };
                var rectTransform = controlSchemeParent.AddComponent<RectTransform>();
                
                rectTransform.SetParent(rebindingsScroll);
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                
                if (first)
                {
                    rebindingsScroll.GetComponent<ScrollRect>().content = rectTransform;
                    first = false;
                }
                else controlSchemeParent.SetActive(false);
                
                var verticalLayoutGroup = controlSchemeParent.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.spacing = 0;
                
                var contentSizeFitter = controlSchemeParent.AddComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                
                foreach (var binding in loadedData.bindingsList.Items)
                {
                    if (controlScheme == binding.key.controlScheme)
                    {
                        var uiPrefab = Instantiate(rebindUIPrefab, controlSchemeParent.transform, true);
                        uiPrefab.name = binding.key.id;
                        uiPrefab.transform.localScale = new Vector3(1, 1, 1);
                        
                        var rebindUI = uiPrefab.GetComponent<RebindActionUI>();
                        
                        InputAction action = loadedData.inputActionAsset.FindAction(binding.key.actionId);
                        InputActionReference actionReference = ScriptableObject.CreateInstance<InputActionReference>();
                        actionReference.Set(action);
                        rebindUI.actionReference = actionReference;
                        rebindUI.bindingId = binding.key.id;
                        rebindUI.rebindOverlay = rebindOverlay;
                        rebindUI.rebindPrompt = rebindPrompt;
                        if (!binding.value || loadedData.controlSchemesList.Items.Find(scheme => scheme.key == controlScheme).value == false)
                        {
                            var button = uiPrefab.transform.Find("TriggerRebindButton");
                            button.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                            button.GetComponent<Button>().enabled = false;
                            
                            var resetButton = uiPrefab.transform.Find("ResetToDefaultButton");
                            resetButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                            resetButton.GetComponent<Button>().enabled = false;
                        }
                    }
                }
            }
        }
        
#endif

        public void ResetAllBindings()
        {
            foreach (InputActionMap map in loadedData.inputActionAsset.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
        }
        
        public void ResetControlSchemeBindings(string controlScheme)
        {
            foreach (InputActionMap map in loadedData.inputActionAsset.actionMaps)
            {
                foreach (InputAction action in map.actions)
                {
                    action.RemoveBindingOverride(InputBinding.MaskByGroup(controlScheme));
                }
            }
        }
        
        private void PressLeftButton(GameObject deviceManager, GameObject currentControlScheme, GameObject rebindsScroll)
        {
            var currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if(currentRebindsList.CompareTag("RebindsList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(false);
            
            currentControlScheme.GetComponent<TextMeshProUGUI>().text =
                controlSchemesOfEachDevice[deviceManager][(controlSchemesOfEachDevice[deviceManager].IndexOf(currentControlSchemeOfEachDevice[deviceManager]) - 1 + controlSchemesOfEachDevice[deviceManager].Count) % controlSchemesOfEachDevice[deviceManager].Count];
            currentControlSchemeOfEachDevice[deviceManager] = currentControlScheme.GetComponent<TextMeshProUGUI>().text;
            
            currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if(currentRebindsList.CompareTag("RebindsList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(true);
        }

        private void PressRightButton(GameObject deviceManager, GameObject currentControlScheme, GameObject rebindsScroll)
        {
            var currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if(currentRebindsList.CompareTag("RebindsList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(false);
            
            currentControlScheme.GetComponent<TextMeshProUGUI>().text =
                controlSchemesOfEachDevice[deviceManager][(controlSchemesOfEachDevice[deviceManager].IndexOf(currentControlSchemeOfEachDevice[deviceManager]) + 1) % controlSchemesOfEachDevice[deviceManager].Count];
            currentControlSchemeOfEachDevice[deviceManager] = currentControlScheme.GetComponent<TextMeshProUGUI>().text;
            
            currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if(currentRebindsList.CompareTag("RebindsList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(true);
        }
    }
}