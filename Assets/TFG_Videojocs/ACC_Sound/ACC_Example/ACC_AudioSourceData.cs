using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs.ACC_Sound.ACC_Example
{
    [System.Serializable]
    public class ACC_AudioSourceData:ACC_AbstractData
    {
        public float volume;
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            ACC_AudioSourceData other = (ACC_AudioSourceData)obj;
            return string.Equals(name, other.name, System.StringComparison.OrdinalIgnoreCase)
                   && volume.Equals(other.volume);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ volume.GetHashCode();
                return hash;
            }
        }

        public override object Clone()
        {
            ACC_AudioSourceData clone = new ACC_AudioSourceData
            {
                name = name,
                volume = volume
            };
            return clone;
        }
    }
}