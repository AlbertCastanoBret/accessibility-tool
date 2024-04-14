using System;

namespace TFG_Videojocs.ACC_RemapControls
{
    [System.Serializable]
    public class ACC_BindingData
    {
        public string id;
        public string controlScheme;
        
        public ACC_BindingData(string guid, string controlScheme)
        {
            id = guid;
            this.controlScheme = controlScheme;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            ACC_BindingData other = (ACC_BindingData) obj;
            return id == other.id && controlScheme == other.controlScheme;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ id.GetHashCode();
                hash = (hash * 16777619) ^ controlScheme.GetHashCode();
                return hash;
            }
        }
    }
}