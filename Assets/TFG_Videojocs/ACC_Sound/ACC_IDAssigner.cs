using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Core;
using UnityEditor;
using UnityEngine;

namespace TFG_Videojocs.ACC_Sound
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class ACC_IDAssigner
    {
        //private SortedDictionary<>
        private static List<GameObject> uniqueIDs = new();
        
        static ACC_IDAssigner()
        {
            EditorApplication.hierarchyChanged += AddUniqueIDComponent;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
        }
        
        private static void AddUniqueIDComponent()
        {
            uniqueIDs = new List<GameObject>();
            foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
            {
                if (obj.scene.IsValid() && obj.CompareTag("ACC_3DAudioSource"))
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(obj))
                    {
                        var idGenerator = obj.GetComponent<ACC_IDGenerator>() ?? obj.AddComponent<ACC_IDGenerator>();
                        idGenerator.SetUniqueID();
                        if (!uniqueIDs.Contains(obj))
                        {
                            uniqueIDs.Add(obj);
                        }
                    }
                }
            }
            
            var count = uniqueIDs.Count()-1;
            
            foreach (GameObject obj in uniqueIDs)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                var name = prefab.name;
                name = name.Replace("ACC_3DAS-", "");
                name = name.Replace("GO-", "");
                
                var idGenerator = obj.GetComponent<ACC_IDGenerator>();
                if (obj.name != name + "_" + count)
                {
                    idGenerator.SetOriginalName(name + "_" + count);
                }

                count--;
            }
        }
        
        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            var largeIcons = GetTextures("sv_label_", string.Empty, 0, 8);
            var icon = largeIcons[6];
            if (obj != null && obj.CompareTag("ACC_3DAudioSource"))
            {
                EditorGUIUtility.SetIconForObject(obj, icon.image as Texture2D);
            }
        }
        
        private static GUIContent[] GetTextures(string baseName, string postfix, int startIndex, int count)
        {
            GUIContent[] array = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postfix);
            }
            return array;
        }
        
        private static int CountCharacters(string s, char c)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    count++;
                }
            }
            return count;
        }
    }
#endif
}