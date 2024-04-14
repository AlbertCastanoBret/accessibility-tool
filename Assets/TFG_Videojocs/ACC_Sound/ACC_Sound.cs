using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ACC_Sound : IEquatable<ACC_Sound>, ICloneable
{
    [ACC_ReadOnly] public string name;
    public AudioClip audioClip;

    public ACC_Sound(string name, AudioClip audioClip)
    {
        this.name = name;
        this.audioClip = audioClip;
    }

    public bool Equals(ACC_Sound other)
    {
        if (other == null) return false;
        return name == other.name;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
