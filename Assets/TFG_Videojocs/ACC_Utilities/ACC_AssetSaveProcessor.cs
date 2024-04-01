using System;
using System.Collections.Generic;
using TFG_Videojocs.ACC_RemapControls;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TFG_Videojocs.ACC_Utilities
{
    public class ACC_AssetSaveProcessor : AssetPostprocessor 
    {
        private static List<ACC_UniqueInputControlSchemeData> lastControlSchemesList;
        private static List<ACC_UniqueInputControlSchemeData> controlSchemesList;
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.EndsWith(".inputactions"))
                {
                    if(lastControlSchemesList == null)
                    {
                        lastControlSchemesList = new List<ACC_UniqueInputControlSchemeData>();
                    }
                    
                    if(controlSchemesList == null)
                    {
                        controlSchemesList = new List<ACC_UniqueInputControlSchemeData>();
                    }
                    
                    InputActionAsset inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(asset);
                    if (inputActionAsset != null)
                    {
                        foreach (InputControlScheme controlScheme in inputActionAsset.controlSchemes)
                        {
                            ACC_UniqueInputControlSchemeData controlSchemeData = new ACC_UniqueInputControlSchemeData(controlScheme);
                            if (!controlSchemesList.Exists(x => x.ControlScheme.name == controlScheme.name))
                            {
                                controlSchemesList.Add(controlSchemeData);
                            }
                        }
                        
                        if (lastControlSchemesList.Count != controlSchemesList.Count)
                        {
                            lastControlSchemesList = controlSchemesList;
                        }

                        foreach (var controlScheme in controlSchemesList)
                        {
                            Debug.Log(controlScheme.UniqueIdentifier + " " + controlScheme.ControlScheme.name);
                        }
                    }
                }
            }
        }
    }
}