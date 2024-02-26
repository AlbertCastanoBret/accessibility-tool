using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEngine;

public class ACC_JSONHelper
{
    private static string basePath = "Assets/TFG_Videojocs/ACC_JSON";
    public delegate TResult GetCustomDelegate<in TData, out TResult>(TData data);
    public delegate List<TListItem> GetListDelegate<TListItem, in TData>(TData data);
    public delegate bool ItemMatchDelegate<in TListItem>(TListItem item, TListItem toMatch);

    public static void CreateJson(ACC_AbstractData abstractData, string folder)
    {
        string json = JsonUtility.ToJson(abstractData, true);
        File.WriteAllText(basePath + folder + abstractData.name + ".json", json);
        AssetDatabase.Refresh();
    }

    public static bool FileNameAlreadyExists(string filename)
    {
        string filePath = basePath + filename + ".json";
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

    public static string GetFileNameByListParameter<TData, TListItem>(
        string folder,
        GetListDelegate<TListItem, TData> getList,
        ItemMatchDelegate<TListItem> isMatch,
        TListItem toMatch)
    {
        string[] files = Directory.GetFiles(basePath + folder, "*.json");
        foreach (string filePath in files)
        {
            string json = File.ReadAllText(filePath);
            TData data = JsonUtility.FromJson<TData>(json);
            List<TListItem> list = getList(data);
            foreach (TListItem item in list)
            {
                if (isMatch(item, toMatch))
                {
                    return Path.GetFileName(filePath);
                }
            }
        }
        return null;
    }
    
    public static void RemoveItemFromListInFile<TData, TListItem>(
        string folder,
        GetListDelegate<TListItem, TData> getList,
        ItemMatchDelegate<TListItem> isMatch,
        TListItem toMatch) where TData : new()
    {
        string[] files = Directory.GetFiles(basePath + folder, "*.json");
        foreach (string filePath in files)
        {
            string json = File.ReadAllText(filePath);
            TData data = JsonUtility.FromJson<TData>(json);
            List<TListItem> list = getList(data);
            int matchIndex = list.FindIndex(item => isMatch(item, toMatch));

            if (matchIndex != -1)
            {
                list.RemoveAt(matchIndex);
                string modifiedJson = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePath, modifiedJson);
                break;
            }
        }
        AssetDatabase.Refresh();
    }

    public static TResult GetParamByFileName<TData, TResult>(GetCustomDelegate<TData, TResult> getCustom, string folder, string filename)
    {
        string json = File.ReadAllText(basePath + folder + filename + ".json");
        TData data = JsonUtility.FromJson<TData>(json);
        return getCustom(data);
    }

    public static List<TResult> GetFilesListByParam<TData, TResult>(string folder, GetCustomDelegate<TData, TResult> getCustom)
    {
        var options = new List<TResult>();
        string[] files = Directory.GetFiles(basePath + folder, "*.json");
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            TData data = JsonUtility.FromJson<TData>(json);
            TResult option = getCustom(data);
            options.Add(option);
        }
        return options;
    }

    public static void DeleteFile(string name)
    {
        string path = basePath + name + ".json";

        if (File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
