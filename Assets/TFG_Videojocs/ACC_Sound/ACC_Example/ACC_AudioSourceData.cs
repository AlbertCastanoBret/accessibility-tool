using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

namespace TFG_Videojocs.ACC_Sound.ACC_Example
{
    [System.Serializable]
    public class ACC_AudioSourceData:ACC_AbstractData
    {
        public float volume;
        public bool is3D;
        public string sourceObjectGUID;
        public string prefabGUID;
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            
            ACC_AudioSourceData other = (ACC_AudioSourceData)obj;
            return string.Equals(name, other.name, System.StringComparison.OrdinalIgnoreCase)
                   && volume.Equals(other.volume)
                   && is3D.Equals(other.is3D)
                   && string.Equals(sourceObjectGUID, other.sourceObjectGUID, System.StringComparison.OrdinalIgnoreCase)
                   && string.Equals(prefabGUID, other.prefabGUID, System.StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ volume.GetHashCode();
                hash = (hash * 16777619) ^ is3D.GetHashCode();
                hash = (hash * 16777619) ^ sourceObjectGUID.GetHashCode();
                hash = (hash * 16777619) ^ prefabGUID.GetHashCode();
                return hash;
            }
        }

        public override object Clone()
        {
            ACC_AudioSourceData clone = new ACC_AudioSourceData
            {
                name = name,
                volume = volume,
                is3D = is3D,
                sourceObjectGUID = sourceObjectGUID,
                prefabGUID = prefabGUID
            };
            return clone;
        }
    }
}