using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEngine;

public class ACC_AudioManagerEditorWindowController : ACC_FloatingWindowController<ACC_AudioManagerEditorWindow, ACC_AudioManagerData>
{
    protected override void RestoreFieldValues()
    {
        window.CreateTable();
    }
}
