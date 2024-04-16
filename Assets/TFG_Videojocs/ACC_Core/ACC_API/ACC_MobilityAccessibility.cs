using UnityEngine.InputSystem;

namespace TFG_Videojocs
{
    public enum MobilityFeatures
    {
        RemapControls
    }

    public class ACC_MobilityAccessibility
    {
        private PlayerInput playerInput;
        public ACC_MobilityAccessibility(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public void SetFeatureState(MobilityFeatures feature, bool state)
        {
            switch (feature)
            {
                case MobilityFeatures.RemapControls:
                    break;
            }
        }

        public void ChangeControlScheme(string controlScheme)
        {
            
        }

        public void ChangeBinding()
        {
            
        }
    }
}