using System;
using System.Collections.Generic;
using TFG_Videojocs.ACC_RemapControls;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using ACC_UniqueInputControlSchemeData = TFG_Videojocs.ACC_RemapControls.ACC_UniqueInputControlSchemeData;

namespace TFG_Videojocs.ACC_Utilities
{
    public class ACC_AssetSaveProcessor : AssetPostprocessor 
    {
        private static List<ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>>> lastControlSchemesList =
            new List<ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>>>();
        private static List<ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>>> controlSchemesList =
            new List<ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>>>();
        
        public static List<ACC_KeyValuePairData<string, bool>> controlSchemesChanged = new List<ACC_KeyValuePairData<string, bool>>();
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.EndsWith(".inputactions"))
                {
                    /*InputActionAsset inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(asset);
                    if (inputActionAsset != null)
                    {
                        if(!controlSchemesChanged.Exists(x => x.key == inputActionAsset.name))
                        {
                            controlSchemesChanged.Add(new ACC_KeyValuePairData<string, bool>(inputActionAsset.name, false));
                        }
                        else
                        {
                            controlSchemesChanged.Find(x => x.key == inputActionAsset.name).value = false;
                        }

                        ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>> currentInputActionAsset;
                        ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>> lastCurrentInputActionAsset 
                            = lastControlSchemesList.Find(x => x.key == inputActionAsset.name);
                        //inputActionAsset.contro
                        
                        if(!controlSchemesList.Exists(x => x.key == inputActionAsset.name))
                        {
                            currentInputActionAsset = new ACC_KeyValuePairData<string, List<ACC_UniqueInputControlSchemeData>>(inputActionAsset.name, new List<ACC_UniqueInputControlSchemeData>());
                            controlSchemesList.Add(currentInputActionAsset);
                        }
                        else
                        {
                            currentInputActionAsset = controlSchemesList.Find(x => x.key == inputActionAsset.name);
                        }
                        
                        {
                            controlSchemesChanged.Find(x => x.key == inputActionAsset.name).value = true;
                        };
                        foreach (InputControlScheme controlScheme in inputActionAsset.controlSchemes)
                        {
                            ACC_UniqueInputControlSchemeData uniqueInputControlSchemeData = new ACC_UniqueInputControlSchemeData(controlScheme);
                            currentInputActionAsset.value.Add(uniqueInputControlSchemeData);    
                            
                        }
                    }*/
                }
            }
        }
    }
}