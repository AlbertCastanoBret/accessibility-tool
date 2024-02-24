using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class ACC_Sound
{
    [ACC_ReadOnly] public string Name;
    public AudioClip AudioClip;
    //public bool needsVisualNotification;
}
