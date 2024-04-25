using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class ACC_AudioManagerData : ACC_AbstractData
{
    public ACC_SerializableDictiornary<int, string> audioSources = new ACC_SerializableDictiornary<int, string>();
    public ACC_SerializableDictiornary<int, ACC_SerializableDictiornary<int, string>> audioClips = new ACC_SerializableDictiornary<int,  ACC_SerializableDictiornary<int, string>>();
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        ACC_AudioManagerData other = (ACC_AudioManagerData)obj;
        bool audioSourcesEqual = audioSources.Items.SequenceEqual(other.audioSources.Items);
        bool audioClipsEqual = audioClips.Items.SequenceEqual(other.audioClips.Items);
        
        return string.Equals(name, other.name, StringComparison.OrdinalIgnoreCase)
               && audioSourcesEqual
               && audioClipsEqual;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ audioSources.GetHashCode();
            hash = (hash * 16777619) ^ audioClips.GetHashCode();
            return hash;
        }
    }

    public override object Clone()
    {
        ACC_AudioManagerData clone = new ACC_AudioManagerData
        {
            name = name,
            audioSources = (ACC_SerializableDictiornary<int, string>)audioSources.Clone(),
            audioClips = (ACC_SerializableDictiornary<int, ACC_SerializableDictiornary<int, string>>)audioClips.Clone()
        };
        return clone;
    }
}
