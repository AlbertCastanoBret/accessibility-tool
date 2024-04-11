using System.Collections.Generic;

namespace TFG_Videojocs.ACC_Utilities
{
    [System.Serializable]
    public abstract class ACC_AbstractData
    {
        public string name;
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}