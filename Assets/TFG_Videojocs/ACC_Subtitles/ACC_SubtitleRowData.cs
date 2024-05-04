using System;
using UnityEngine;

namespace TFG_Videojocs.ACC_Subtitles
{
    [System.Serializable]
    public class ACC_SubtitleRowData: ICloneable
    {
        public string actor;
        public string subtitle;
        public int time;
        
        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            
            ACC_SubtitleRowData otherRow = (ACC_SubtitleRowData)other;
            return string.Equals(actor, otherRow.actor, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(subtitle, otherRow.subtitle, StringComparison.OrdinalIgnoreCase)
                   && time == otherRow.time;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ actor.GetHashCode();
                hash = (hash * 16777619) ^ subtitle.GetHashCode();
                hash = (hash * 16777619) ^ time.GetHashCode();
                return hash;
            }
        }

        public object Clone()
        {
            return new ACC_SubtitleRowData
            {
                actor = actor,
                subtitle = subtitle,
                time = time
            };
        }
    }
}