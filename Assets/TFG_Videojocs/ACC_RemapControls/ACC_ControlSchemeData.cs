using System;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine.InputSystem;

namespace TFG_Videojocs.ACC_RemapControls
{
    [System.Serializable]
    public class ACC_ControlSchemeData: ACC_AbstractData
    {
        public InputActionAsset inputActionAsset;
        public ACC_SerializableDictiornary<string, bool> controlSchemesList;
        public ACC_SerializableDictiornary<ACC_BindingData, bool> bindingsList;
        //public List<ACC_KeyValuePair<string, bool>> controlSchemesList;
        //public List<ACC_KeyValuePair<ACC_BindingData, bool>> bindingsList;

        public ACC_ControlSchemeData()
        {
            //controlSchemesList = new List<ACC_KeyValuePair<string, bool>>();
            //bindingsList = new List<ACC_KeyValuePair<ACC_BindingData, bool>>();
            controlSchemesList = new ACC_SerializableDictiornary<string, bool>();
            bindingsList = new ACC_SerializableDictiornary<ACC_BindingData, bool>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ACC_ControlSchemeData)obj;

            bool controlSchemesEqual = controlSchemesList.Items.SequenceEqual(other.controlSchemesList.Items);
            bool bindingsEqual = bindingsList.Items.SequenceEqual(other.bindingsList.Items);

            return controlSchemesEqual && bindingsEqual;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ controlSchemesList.GetHashCode();
                hash = (hash * 16777619) ^ bindingsList.GetHashCode();
                return hash;
            }
        }
    }
}