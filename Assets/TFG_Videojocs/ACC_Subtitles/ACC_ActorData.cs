using System;
using UnityEngine;

namespace TFG_Videojocs.ACC_Subtitles
{
    [System.Serializable]
    public class ACC_ActorData: ICloneable
    {
        public string actor;
        public Color color;

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            
            ACC_ActorData otherActor = (ACC_ActorData)other;
            return string.Equals(actor, otherActor.actor, StringComparison.OrdinalIgnoreCase)
                   && color.Equals(otherActor.color);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ actor.GetHashCode();
                hash = (hash * 16777619) ^ color.GetHashCode();
                return hash;
            }
        }
        
        public object Clone()
        {
            return new ACC_ActorData
            {
                actor = actor,
                color = color
            };
        }
    }
}