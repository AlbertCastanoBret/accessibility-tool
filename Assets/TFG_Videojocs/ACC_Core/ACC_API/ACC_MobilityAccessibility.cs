using TFG_Videojocs.ACC_RemapControls;
using TFG_Videojocs.ACC_Utilities;
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
        private ACC_RemapControlsManager accRemapControlsManager;
        public ACC_MobilityAccessibility()
        {
            accRemapControlsManager = ACC_PrefabHelper.InstantiatePrefabAsChild(
                    "RemapControls", 
                    ACC_AccessibilityManager.Instance.accCanvas, 
                    ACC_AccessibilityManager.Instance.remapControlsAsset.name)
                .GetComponent<ACC_RemapControlsManager>();
        }

        internal void InitializeState(MobilityFeatures feature, bool state)
        {
            switch (feature)
            {
                case MobilityFeatures.RemapControls:
                    accRemapControlsManager.InitializeRemapControls(state);
                    break;
            }
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
                    accRemapControlsManager.SetRemapControls(state);
                    break;
            }
        }
        
        /// <summary>
        /// Displays the rebind menu for a specific device.
        /// </summary>
        /// <param name="device">The identifier for the device whose controls are to be rebound.</param>
        public void EnableRemapControlsMenu(string device)
        {
            accRemapControlsManager.EnableRemapControlsMenu(device);
        }
        
        /// <summary>
        /// Hides the remap controls menu.
        /// </summary>
        public void DisableRemapControlsMenu()
        {
            accRemapControlsManager.DisableRebindMenu();
        }
        
        /// <summary>
        /// Resets all bindings to their default settings using the AccRebindControlsManager.
        /// </summary>
        public void ResetAllBindings()
        {
            accRemapControlsManager.ResetAllBindings();
        }
        
        /// <summary>
        /// Resets all bindings associated with a specific control scheme to their default settings.
        /// </summary>
        /// <param name="controlScheme">The name of the control scheme to reset.</param>
        public void ResetControlSchemeBindings(string controlScheme)
        {
            accRemapControlsManager.ResetControlSchemeBindings(controlScheme);
        }

        public void ChangeControlScheme(string controlScheme)
        {
            
        }

        public void ChangeBinding()
        {
            
        }
    }
}