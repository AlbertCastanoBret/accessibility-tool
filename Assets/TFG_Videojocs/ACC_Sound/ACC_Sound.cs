using System;
using UnityEngine;

namespace TFG_Videojocs.ACC_Sound
{
    [System.Serializable]
    public class ACC_Sound : IEquatable<ACC_Sound>, ICloneable
    {
        [ACC_ReadOnly] public string name;
        public AudioClip audioClip;
        [HideInInspector] public string currentVisualNotificationData;

        public ACC_Sound()
        {
            name = "New Sound";
            audioClip = null;
            currentVisualNotificationData = "";
        }
        public ACC_Sound(string name, AudioClip audioClip)
        {
            this.name = name;
            this.audioClip = audioClip;
            currentVisualNotificationData = "";
        }

        public bool Equals(ACC_Sound other)
        {
            if (other == null) return false;
            return name == other.name;
        }

        public object Clone()
        {
            ACC_Sound clone = new ACC_Sound
            {
                name = name,
                audioClip = audioClip,
                currentVisualNotificationData = currentVisualNotificationData
            };
            return clone;
        }
    }
}