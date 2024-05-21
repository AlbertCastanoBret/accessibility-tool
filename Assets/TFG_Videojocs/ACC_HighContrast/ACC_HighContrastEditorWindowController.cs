#if UNITY_EDITOR
using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs.ACC_HighContrast.Toilet
{
    public class ACC_HighContrastEditorWindowController:ACC_FloatingWindowController<ACC_HighContrastEditorWindow, ACC_HighContrastData>
    {
        public bool isPrevisualizing;
        public override void ConfigureJson()
        {
            base.ConfigureJson();
            ACC_PrefabHelper.CreatePrefab("HighContrast");
        }

        protected override void RestoreFieldValues()
        {
            window.CreateTable();
            window.CreateSettingsContainer();
        }
    }
}
#endif