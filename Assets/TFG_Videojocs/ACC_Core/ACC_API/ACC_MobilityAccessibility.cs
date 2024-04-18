using TFG_Videojocs.ACC_RemapControls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TFG_Videojocs
{
    public enum MobilityFeatures
    {
        RemapControls
    }

    public class ACC_MobilityAccessibility
    {
        private ACC_RebindControlsManager accRebindControlsManager;
        public ACC_MobilityAccessibility()
        {
            accRebindControlsManager = GameObject.Find("ACC_RebindControlsManager").GetComponent<ACC_RebindControlsManager>();
        }

        public void SetFeatureState(MobilityFeatures feature, bool state)
        {
            switch (feature)
            {
                case MobilityFeatures.RemapControls:
                    break;
            }
        }
        
        /// <summary>
        /// Displays the rebind menu for a specific device.
        /// </summary>
        /// <param name="device">The identifier for the device whose controls are to be rebound.</param>
        public void ShowRemapControlsMenu(string device)
        {
            accRebindControlsManager.ShowRebindMenu(device);
        }
        
        /// <summary>
        /// Resets all bindings to their default settings using the AccRebindControlsManager.
        /// </summary>
        public void ResetAllBindings()
        {
            accRebindControlsManager.ResetAllBindings();
        }
        
        /// <summary>
        /// Resets all bindings associated with a specific control scheme to their default settings.
        /// </summary>
        /// <param name="controlScheme">The name of the control scheme to reset.</param>
        public void ResetControlSchemeBindings(string controlScheme)
        {
            accRebindControlsManager.ResetControlSchemeBindings(controlScheme);
        }

        public void ChangeControlScheme(string controlScheme)
        {
            
        }

        public void ChangeBinding()
        {
            
        }
    }
}