using TFG_Videojocs.ACC_HighContrast;
using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs
{
    public enum VisibilityFeatures
    {
        TextToVoice,
        HighContrast,
    }
    
    public class ACC_VisualAccessibility
    {
        private ACC_HighContrastManager accHighContrastManager;
        
        public ACC_VisualAccessibility()
        {
            accHighContrastManager = ACC_PrefabHelper.InstantiatePrefabAsChild("HighContrast", ACC_AccessibilityManager.Instance.gameObject).GetComponent<ACC_HighContrastManager>();
        }
        
        public void SetFeatureState(VisibilityFeatures feature, bool state)
        {
            switch (feature)
            {
                case VisibilityFeatures.HighContrast:
                    accHighContrastManager.gameObject.SetActive(state);
                    break;
            }
        }
        
        public void SetHighContrastMode(bool state)
        {
            accHighContrastManager.SetHighContrastMode(state);
        }
    }
}