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
        public ACC_ControlSchemeData loadedData;
        private Dictionary<GameObject, List<string>> controlSchemesOfEachDevice = new();
        private Dictionary<GameObject, string> currentControlSchemeOfEachDevice = new();

        private void Awake()
        {
            controlSchemesOfEachDevice = new();
            currentControlSchemeOfEachDevice = new();
            
            for(int i=0; i < gameObject.transform.childCount; i++)
            {
                var deviceManager = gameObject.transform.GetChild(i).gameObject;
                if (deviceManager.CompareTag("ACC_Prefab"))
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
        
        public void ShowRebindMenu(string device)
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
            if (!found) Debug.LogError("Device not found");
            
        }
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
        public void HideRebindMenu()
        {
            foreach (var deviceManager in controlSchemesOfEachDevice.Keys)
            {
                deviceManager.SetActive(false);
            }
        }
        
        private void PressLeftButton(GameObject deviceManager, GameObject currentControlScheme, GameObject rebindsScroll)
        {
            var currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if(currentRebindsList.CompareTag("ACC_ScrollList")) rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(false);
            
            currentControlScheme.GetComponent<TextMeshProUGUI>().text =
                controlSchemesOfEachDevice[deviceManager][(controlSchemesOfEachDevice[deviceManager].IndexOf(currentControlSchemeOfEachDevice[deviceManager]) - 1 + controlSchemesOfEachDevice[deviceManager].Count) % controlSchemesOfEachDevice[deviceManager].Count];
            currentControlSchemeOfEachDevice[deviceManager] = currentControlScheme.GetComponent<TextMeshProUGUI>().text;
            
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
            
            currentRebindsList = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject;
            if (currentRebindsList.CompareTag("ACC_ScrollList"))
            {
                rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).gameObject.SetActive(true);
                rebindsScroll.GetComponent<ScrollRect>().content = rebindsScroll.transform.Find(currentControlSchemeOfEachDevice[deviceManager]).GetComponent<RectTransform>();
            }
        }
    }
}