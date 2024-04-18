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
            accRebindControlsManager = ACC_AccessibilityManager.Instance.accCanvas.transform.Find("ACC_RebindControlsManager").GetComponent<ACC_RebindControlsManager>();
        }

        /// <summary>
        /// Sets the state of a specified mobility feature to either enabled or disabled.
        /// </summary>
        /// <param name="feature">The mobility feature to modify. Use MobilityFeatures enum to specify the feature.</param>
        /// <param name="state">A boolean value indicating whether the feature should be enabled (true) or disabled (false).</param>
        public void SetFeatureState(MobilityFeatures feature, bool state)
        {
            switch (feature)
            {
                case MobilityFeatures.RemapControls:
                    accRebindControlsManager.gameObject.SetActive(state);
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
        /// Hides the remap controls menu.
        /// </summary>
        public void HideRemapControlsMenu()
        {
            accRebindControlsManager.HideRebindMenu();
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