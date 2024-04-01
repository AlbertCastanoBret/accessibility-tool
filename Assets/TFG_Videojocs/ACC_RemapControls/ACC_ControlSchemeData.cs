using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine.InputSystem;

namespace TFG_Videojocs.ACC_RemapControls
{
    [System.Serializable]
    public class ACC_ControlSchemeData: ACC_AbstractData
    {
        public InputActionAsset inputActionAsset;
        public List<ACC_KeyValuePairData<string, bool>> controlSchemesList;

        public ACC_ControlSchemeData()
        {
            controlSchemesList = new List<ACC_KeyValuePairData<string, bool>>();
            
            
        }
    }
}