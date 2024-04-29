using System;
using UnityEditor;
using UnityEngine;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastManager:MonoBehaviour
    {
        private ACC_HighContrastData loadedData;
        public void SetHighContrastMode(bool state)
        {
            if (state) EnableHighContrastMode();
            else DisableHighContrastMode();
        }
        
        private void EnableHighContrastMode()
        {
            if (ACC_AccessibilityManager.Instance.shadersAdded)
            {
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = -50;
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0.6f);
                        }
                    }
                }
            }
        }
        private void DisableHighContrastMode()
        {
            if (ACC_AccessibilityManager.Instance.shadersAdded)
            {
                GameObject[] goArray = FindObjectsOfType<GameObject>();
                foreach (GameObject go in goArray)
                {
                    if (go.activeInHierarchy)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = 50;
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0.6f);
                        }
                    }
                }
            }
        }
        public void ChangeHighContrastConfiguration(string configuration)
        {
            
        }
    }
}