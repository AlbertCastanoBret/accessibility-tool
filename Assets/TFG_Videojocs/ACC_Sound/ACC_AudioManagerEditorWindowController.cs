using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Utilities;
using UnityEditor;
using UnityEngine;

public class ACC_AudioManagerEditorWindowController : ACC_FloatingWindowController<ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    public override void ConfigureJson()
    {
        var audioSourceNames = currentData.audioSources.Items.Select(item => item.value.name);
        var duplicateNames = audioSourceNames.GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        var enumerable = duplicateNames as string[] ?? duplicateNames.ToArray();
        if (enumerable.Any())
        {
            EditorUtility.DisplayDialog("Error", "There are duplicate audio source names: " + string.Join(", ", enumerable), "Ok");
        }
        else
        {
            base.ConfigureJson();
            ACC_PrefabHelper.CreatePrefab("Audio", oldName);
            foreach (var audioSource in currentData.audioSources.Items)
            {
                if (audioSource.value.is3D)
                {
                    ACC_PrefabHelper.Create3DAudioSource(audioSource.value);
                }
            }
            ACC_PrefabHelper.CreatePrefab("Audio", oldName);
        }
    }
    
    protected override void RestoreFieldValues()
    {
        window.CreateTable();
    }
}
