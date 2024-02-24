using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEngine;

public class ACC_JSONHelper
{
    private static string basePath = "Assets/TFG_Videojocs/ACC_JSON";

    public static void CreateJson(ACC_AbstractData abstractData, string folder)
    {
        string json = JsonUtility.ToJson(abstractData, true);
        File.WriteAllText(basePath + folder + abstractData.name + ".json", json);
        AssetDatabase.Refresh();
    }
    
    public static bool FileNameAlreadyExists(string filename)
    {
        string filePath = basePath + filename + ".json";
        Debug.Log(filePath);
        return File.Exists(filePath);
    }
    
    public static void RenameFile(string oldName, string newName)
    {
        string oldPath = basePath + oldName + ".json";
        string newPath = basePath + newName + ".json";
        
        if (File.Exists(oldPath))
        {
            if (File.Exists(newPath))
            {
                EditorUtility.DisplayDialog("Filename already exists.", 
                    $"Cannot rename the file to '{newName}' because a file with that name already exists.", "OK");
            }
            else
            {
                AssetDatabase.MoveAsset(oldPath, newPath);
                AssetDatabase.Refresh();
            }
        }
        else
        {
            Debug.LogError($"The file {oldName}.json does not exist.");
        }
    }
}
