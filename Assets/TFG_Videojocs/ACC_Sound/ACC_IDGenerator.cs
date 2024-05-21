using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;


namespace TFG_Videojocs.ACC_Sound
{
    public class ACC_IDGenerator:MonoBehaviour
    {
        [SerializeField] [ACC_ReadOnly] private string originalName;
        [SerializeField] [ACC_ReadOnly] private string uniqueID;
        
        private void Awake()
        {
            gameObject.name = originalName;
        }
        
        private void Update()
        {
            if (gameObject.name != originalName)
            {
                gameObject.name = originalName;
            }
        }

    #if UNITY_EDITOR

        public void SetUniqueID()
        {
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = Guid.NewGuid().ToString();
            }
        }
        
        public void SetOriginalName(string name)
        {
            if (!string.Equals(originalName, name, StringComparison.Ordinal)) originalName = name;
            if(!string.Equals(this.name, originalName, StringComparison.Ordinal)) this.name = originalName;
        }
    #endif
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ACC_IDGenerator))]
    public class NonEditableNameEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("The name of this GameObject cannot be changed.", MessageType.Info);
            DrawDefaultInspector();
        }
    }
    #endif
}
