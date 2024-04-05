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
    }
}