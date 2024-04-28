namespace TFG_Videojocs.ACC_HighContrast.Toilet
{
    public class ACC_HighContrastEditorWindowController:ACC_FloatingWindowController<ACC_HighContrastEditorWindow, ACC_HighContrastData>
    {
        public bool isPrevisualizing;
        protected override void RestoreFieldValues()
        {
            window.CreateTable();
            window.CreateSettingsContainer();
        }
    }
}