using System;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs.ACC_RemapControls;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TFG_Videojocs.ACC_Utilities
{
    public class ACC_AssetSaveProcessor : AssetPostprocessor 
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.EndsWith(".inputactions"))
                {

                    string filename = Path.GetFileNameWithoutExtension(asset);
                    ACC_JSONHelper.DeleteFile("/ACC_JSONRemapControls/" + filename);
                }
            }
        }
    }
}