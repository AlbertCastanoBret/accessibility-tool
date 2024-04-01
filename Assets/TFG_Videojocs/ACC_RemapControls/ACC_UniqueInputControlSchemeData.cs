using System;
using UnityEngine.InputSystem;

namespace TFG_Videojocs.ACC_RemapControls
{
    public class ACC_UniqueInputControlSchemeData
    {
        public Guid UniqueIdentifier { get; set; }
        public InputControlScheme ControlScheme { get; set; }

        public ACC_UniqueInputControlSchemeData(InputControlScheme controlScheme)
        {
            UniqueIdentifier = Guid.NewGuid();
            ControlScheme = controlScheme;
        }
    }
}