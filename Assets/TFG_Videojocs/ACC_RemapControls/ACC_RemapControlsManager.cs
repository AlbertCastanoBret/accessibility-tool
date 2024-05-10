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
    public class ACC_RemapControlsManager:MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Editor Only")]
        public GameObject defaultMenu;
        public GameObject rebindUIPrefab;
        public GameObject rebindOverlay;
#endif
        [Header("Runtime Only")]
        private Dictionary<GameObject, List<string>> controlSchemesOfEachDevice = new();
        private Dictionary<GameObject, string> currentControlSchemeOfEachDevice = new();
        private bool isEnabled;

        private void Awake()
        {
            controlSchemesOfEachDevice = new();
            currentControlSchemeOfEachDevice = new();
            
            for(int i=0; i < gameObject.transform.childCount; i++)
            {
                var deviceManager = gameObject.transform.GetChild(i).gameObject;
                if (deviceManager.CompareTag("ACC_Prefab"))
                {
                    var controlSchemes = ACC_AccessibilityManager.Instance.remapControlsAsset.controlSchemes
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
                            deviceManager.transform.Find("ACC_RebindsScroll").gameObject));
                    
                    deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent")
                        .Find("RightArrow").GetComponent<Button>().onClick.AddListener(() => 
                            PressRightButton(deviceManager,
                                 deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent")
                                    .Find("CurrentControlScheme").gameObject,
                                 deviceManager.transform.Find("ACC_RebindsScroll").gameObject));
                    
                    deviceManager.transform.Find("ResetAllButton").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ResetControlSchemeBindings(currentControlSchemeOfEachDevice[deviceManager]);
                    });
                }
            }    
        }

        public void InitializeRemapControls(bool state)
        {
            if (state) isEnabled = true;
            else
            {
                ResetAllBindings();
                isEnabled = false;
            }
            
            ACC_AccessibilityManager.Instance.remapControlsEnabled = state;
        }
        public void SetRemapControls(bool state)
        {
            if (state) isEnabled = true;
            else
            {
                ResetAllBindings();
                isEnabled = false;
            }
            
            ACC_AccessibilityManager.Instance.remapControlsEnabled = state;
            PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.RemapControlsEnabled, state ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ResetRemapControlsState()
        {
            ResetAllBindings();
            isEnabled = false;
            
            ACC_AccessibilityManager.Instance.remapControlsEnabled = false;
            PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.RemapControlsEnabled, 0);
            PlayerPrefs.Save();
        }
        public void EnableRemapControlsMenu(string device)
        {
            bool found = false;
            foreach (var deviceManager in controlSchemesOfEachDevice.Keys)
            {
                if (deviceManager.name == device)
                {
                    found = true;
                    deviceManager.SetActive(true);
                }
                else deviceManager.SetActive(false);
            }
            if (!found) Debug.LogError("Device " + device + " not found");
        }
        public void DisableRebindMenu()
        {
            foreach (var deviceManager in controlSchemesOfEachDevice.Keys)
            {
                deviceManager.SetActive(false);
            }
        }
        public void ChangeBinding(string actionName, int bindingIndex, InputBinding newBinding)
        {
            if (!isEnabled) return;
            
            InputAction action = ACC_AccessibilityManager.Instance.remapControlsAsset.FindAction(actionName);
            if (action == null)
            {
                Debug.LogError($"Action '{actionName}' not found in the Input Action Asset.");
                return;
            }
            
            if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            {
                Debug.LogError($"Binding index '{bindingIndex}' is invalid for action '{actionName}'.");
                return;
            }
            
            action.ApplyBindingOverride(bindingIndex, newBinding);
        }
        public void ResetBinding(string actionName, int bindingIndex)
        {
            if (!isEnabled) return;
            
            InputAction action = ACC_AccessibilityManager.Instance.remapControlsAsset.FindAction(actionName);
            if (action == null)
            {
                Debug.LogError($"Action '{actionName}' not found in the Input Action Asset.");
                return;
            }
            
            if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            {
                Debug.LogError($"Binding index '{bindingIndex}' is invalid for action '{actionName}'.");
                return;
            }
            
            action.RemoveBindingOverride(bindingIndex);
        }
        public void ChangeControlScheme(string controlScheme)
        {
            UnityEngine.InputSystem.PlayerInput[] allPlayerInputs = FindObjectsOfType<UnityEngine.InputSystem.PlayerInput>();
            foreach (var playerInput in allPlayerInputs)
            {
                if (playerInput.actions != ACC_AccessibilityManager.Instance.remapControlsAsset)
                {
                    playerInput.actions = ACC_AccessibilityManager.Instance.remapControlsAsset;
                }

                playerInput.defaultControlScheme = controlScheme;
                playerInput.enabled = false;
                playerInput.enabled = true;
            }
        }
        public void ResetAllBindings()
        {
            if (!isEnabled) return;
            foreach (InputActionMap map in ACC_AccessibilityManager.Instance.remapControlsAsset.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
        }
        public void LoadRemapControlsSettings()
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.RemapControlsEnabled))
            {
                SetRemapControls(PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.RemapControlsEnabled) == 1);
            }
        }
        public void ResetControlSchemeBindings(string controlScheme)
        {
            if (!isEnabled) return;
            foreach (InputActionMap map in ACC_AccessibilityManager.Instance.remapControlsAsset.actionMaps)
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
            if(currentRebindsList.CompareTag("ACC_ScrollList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(false);
            
            currentControlScheme.GetComponent<TextMeshProUGUI>().text =
                controlSchemesOfEachDevice[deviceManager][(controlSchemesOfEachDevice[deviceManager].IndexOf(currentControlSchemeOfEachDevice[deviceManager]) - 1 + controlSchemesOfEachDevice[deviceManager].Count) % controlSchemesOfEachDevice[deviceManager].Count];
            currentControlSchemeOfEachDevice[deviceManager] = currentControlScheme.GetComponent<TextMeshProUGUI>().text;
            
            ChangeControlScheme(currentControlScheme.GetComponent<TextMeshProUGUI>().text);
            
            currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if (currentRebindsList.CompareTag("ACC_ScrollList"))
            {
                rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(true);
                rebindsScroll.GetComponent<ScrollRect>().content = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).GetComponent<RectTransform>();
            }
        }
        private void PressRightButton(GameObject deviceManager, GameObject currentControlScheme, GameObject rebindsScroll)
        {
            var currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if(currentRebindsList.CompareTag("ACC_ScrollList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(false);
            
            currentControlScheme.GetComponent<TextMeshProUGUI>().text =
                controlSchemesOfEachDevice[deviceManager][(controlSchemesOfEachDevice[deviceManager].IndexOf(currentControlSchemeOfEachDevice[deviceManager]) + 1) % controlSchemesOfEachDevice[deviceManager].Count];
            currentControlSchemeOfEachDevice[deviceManager] = currentControlScheme.GetComponent<TextMeshProUGUI>().text;
            
            ChangeControlScheme(currentControlScheme.GetComponent<TextMeshProUGUI>().text);
            
            currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if (currentRebindsList.CompareTag("ACC_ScrollList"))
            {
                rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(true);
                rebindsScroll.GetComponent<ScrollRect>().content = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).GetComponent<RectTransform>();
            }
        }
    }
}