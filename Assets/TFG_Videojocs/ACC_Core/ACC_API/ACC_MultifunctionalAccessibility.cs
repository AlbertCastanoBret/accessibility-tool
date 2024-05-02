using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs
{
    public enum MultifiunctionalFeatures
    {
        AudioManager
    }
    
    public class ACC_MultifunctionalAccessibility
    {
        private ACC_AudioManager accAudioManager;
        public ACC_MultifunctionalAccessibility()
        {
            accAudioManager = ACC_PrefabHelper.InstantiatePrefabAsChild("Audio", ACC_AccessibilityManager.Instance.gameObject).GetComponent<ACC_AudioManager>();
        }
    }
}