using System.Collections.Generic;
using TFG_Videojocs.ACC_HighContrast;
using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs
{
    public enum VisibilityFeatures
    {
        HighContrast,
    }
    
    public class ACC_VisualAccessibility
    {
        private ACC_HighContrastManager accHighContrastManager;
        
        public ACC_VisualAccessibility()
        {
            accHighContrastManager = ACC_PrefabHelper.InstantiatePrefabAsChild("HighContrast", ACC_AccessibilityManager.Instance.gameObject).GetComponent<ACC_HighContrastManager>();
        }
        
        /// <summary>
        /// Sets the state of a specified visibility feature to either enabled or disabled.
        /// </summary>
        /// <param name="feature">The visibility feature to modify. Use VisibilityFeatures enum to specify the feature.</param>
        /// <param name="state">A boolean value indicating whether the feature should be enabled (true) or disabled (false).</param>
        public void SetFeatureState(VisibilityFeatures feature, bool state)
        {
            switch (feature)
            {
                case VisibilityFeatures.HighContrast:
                    accHighContrastManager.SetHighContrastMode(state);
                    break;
            }
        }

        /// <summary>
        /// Changes the high contrast configuration based on the provided JSON configuration file.
        /// </summary>
        /// <param name="configuration">The name of the configuration file within the 'ACC_HighContrast' directory.</param>
        public void ChangeHighContrastConfiguration(string configuration)
        {
            accHighContrastManager.ChangeHighContrastConfiguration(configuration);
        }
        
        /// <summary>
        /// Retrieves a list of available high contrast configurations.
        /// </summary>
        /// <returns>A list of strings representing the names of available high contrast configurations.</returns>
        /// <remarks>
        /// Delegates the call to the 'accHighContrastManager' to fetch the actual configurations.
        /// This method can be useful for displaying configuration options in a user interface or for debugging purposes.
        /// </remarks>
        public List<string> GetHighContrastConfigurations()
        {
            return accHighContrastManager.GetHighContrastConfigurations();
        }
    }
}