using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastManager:MonoBehaviour
    {
        private ACC_HighContrastData loadedData;
        private bool isEnabled;
        public void SetHighContrastMode(bool state)
        {
            if (state) EnableHighContrastMode();
            else DisableHighContrastMode();
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
                        if (renderer != null)
                        {
                            var ambientOcclusionTexture = GetAmbientOcclusionTexture(renderer);
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = -50;
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0.6f);
                            
                            if(ambientOcclusionTexture != null)
                            {
                                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                                renderer.GetPropertyBlock(propBlock, materials.Length - 2);
                                propBlock.SetTexture("_AmbientOcclusion", ambientOcclusionTexture);
                                renderer.SetPropertyBlock(propBlock, materials.Length -2);
                            }
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
                        if (renderer != null)
                        {
                            var materials = renderer.sharedMaterials;
                            materials[^2].renderQueue = 50;
                            
                            MaterialPropertyBlock propOutlineBlock = new MaterialPropertyBlock();
                            renderer.GetPropertyBlock(propOutlineBlock, materials.Length - 1);
                            propOutlineBlock.SetFloat("_OutlineThickness", 0);
                        }
                    }
                }
            }
        }
        public void ChangeHighContrastConfiguration(string configuration)
        {
            loadedData = ACC_JSONHelper.LoadJson<ACC_HighContrastData>("ACC_HighContrast/" + configuration);
            if (loadedData != null && isEnabled)
            {
                if (ACC_AccessibilityManager.Instance.shadersAdded)
                {
                    GameObject[] goArray = FindObjectsOfType<GameObject>();
                    foreach (GameObject go in goArray)
                    {
                        if (go.activeInHierarchy)
                        {
                            Renderer renderer = go.GetComponent<Renderer>();
                            var highContrastConfiguration = loadedData.highContrastConfigurations.Items.Find(x => go.CompareTag(x.value.tag));
                            if (renderer != null && highContrastConfiguration != null)
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
                            else if (renderer != null && highContrastConfiguration == null)
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
        }
        public List<string> GetHighContrastConfigurations()
        {
            ACC_HighContrastData[] configurations = ACC_JSONHelper.LoadAllFiles<ACC_HighContrastData>("ACC_HighContrast");
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
                if (material.HasProperty("_OcclusionMap") && material.GetTexture("_OcclusionMap") != null)
                    return material.GetTexture("_OcclusionMap");
            }
            return null;
        }
    }
}