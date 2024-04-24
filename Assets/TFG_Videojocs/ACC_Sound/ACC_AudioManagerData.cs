using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

[System.Serializable]
public class ACC_AudioManagerData : ACC_AbstractData
{
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<ACC_Sound> audioClips = new List<ACC_Sound>();
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        ACC_AudioManagerData audioManagerData = obj as ACC_AudioManagerData;
        if (audioManagerData == null) return false;
        return audioSources == audioManagerData.audioSources && audioClips == audioManagerData.audioClips;
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
            audioSources = new List<AudioSource>(audioSources),
            audioClips = new List<ACC_Sound>(audioClips)
        };
        return clone;
    }
}
