using System;
using UnityEditor;
using UnityEngine;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastManager:MonoBehaviour
    {
        #if UNITY_EDITOR
        static ACC_HighContrastManager()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        
        private static void OnHierarchyChanged()
        {
            GameObject[] goArray = FindObjectsOfType<GameObject>();
            foreach (GameObject go in goArray)
            {
                if (go.activeInHierarchy)
                {
                    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                    if (meshRenderer != null && !AlreadyHasHighContrastMaterial(meshRenderer))
                    {
                        Material highContrastMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/TFG_Videojocs/ACC_HighContrast/High-Contrast-Color.mat");
                        var materials = meshRenderer.sharedMaterials;
                        var newMaterials = new Material[materials.Length + 1];
                        materials.CopyTo(newMaterials, 0);
                        newMaterials[materials.Length] = highContrastMaterial;
                        meshRenderer.sharedMaterials = newMaterials;
                    }
                }
            }
        }

        private static bool AlreadyHasHighContrastMaterial(MeshRenderer renderer)
        {
            Material highContrastMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/TFG_Videojocs/ACC_HighContrast/High-Contrast-Color.mat");
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