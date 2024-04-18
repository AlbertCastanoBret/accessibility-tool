using System;

namespace TFG_Videojocs.ACC_RemapControls
{
    [System.Serializable]
    public class ACC_BindingData
    {
        public string id;
        public string controlScheme;
        public string actionId;
        
        public ACC_BindingData(string guid, string controlScheme, string actionId)
        {
            id = guid;
            this.controlScheme = controlScheme;
            this.actionId = actionId;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ACC_BindingData)obj;

            return string.Equals(id, other.id, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(controlScheme, other.controlScheme, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(actionId, other.actionId, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ id.GetHashCode();
                hash = (hash * 16777619) ^ controlScheme.GetHashCode();
                hash = (hash * 16777619) ^ actionId.GetHashCode();
                return hash;
            }
        }
    }
}