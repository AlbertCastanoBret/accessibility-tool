using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TFG_Videojocs.ACC_Sound
{
    [System.Serializable]
    public class ACC_Sound : IEquatable<ACC_Sound>, ICloneable
    {
        [ACC_ReadOnly] public int audioSourceKey;
        [ACC_ReadOnly] public string name;
        [ACC_ReadOnly] public string guid;
        //[HideInInspector] public string currentVisualNotificationData;

        public ACC_Sound() { }
        
        public ACC_Sound(int audioSourceKey, string name, string guid)
        {
            this.audioSourceKey = audioSourceKey;
            this.name = name;
            this.guid = guid;
            //currentVisualNotificationData = "";
        }

        public bool Equals(ACC_Sound other)
        {
            if (other == null) return false;
            return audioSourceKey == other.audioSourceKey
                   && name == other.name
                   && guid == other.guid;
            //&& currentVisualNotificationData == other.currentVisualNotificationData;
        }

        public object Clone()
        {
            ACC_Sound clone = new ACC_Sound
            {
                audioSourceKey = audioSourceKey,
                name = name,
                guid = guid
                //currentVisualNotificationData = currentVisualNotificationData
            };
            return clone;
        }
    }
}