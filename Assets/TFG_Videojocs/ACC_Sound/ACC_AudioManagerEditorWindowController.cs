using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

public class ACC_AudioManagerEditorWindowController : ACC_FloatingWindowController<ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    public override void ConfigureJson()
    {
        base.ConfigureJson();
        ACC_PrefabHelper.CreatePrefab("Audio", oldName);
    }
    protected override void RestoreFieldValues()
    {
        window.CreateTable();
    }
}
