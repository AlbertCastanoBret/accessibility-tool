#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TFG_Videojocs.ACC_Core
{
    [InitializeOnLoad]
    public class ACC_TagLayerSetup
    {
        private const string InitializedKey = "SetupPipelineAndTagsInitialized";

        static ACC_TagLayerSetup()
        {
            if (EditorPrefs.GetBool(InitializedKey, false)) return;
            SetupTagsAndLayers();
            SetupRenderPipeline();
            EditorPrefs.SetBool(InitializedKey, true);
        }

        static void SetupTagsAndLayers()
        {
            AddTag("ACC_Scroll");
            AddTag("ACC_ScrollList");
            AddTag("ACC_AudioSource");
            AddTag("ACC_Prefab");
            AddTag("ACC_SubtitlesManager");
            AddTag("ACC_SubtitlesBackground");
            AddTag("ACC_SubtitleText");
            AddTag("ACC_Button");
            AddTag("ACC_VisualNotificationManager");
            AddTag("ACC_VisualNotificationBackground");
            AddTag("ACC_VisualNotificationText");
            AddTag("ACC_HighContrastManager");
            AddTag("ACC_AudioManager");
            AddTag("ACC_AudioSources");
            AddTag("ACC_RemapControlsManager");
            AddTag("ACC_WaitForInputPanel");
            AddTag("ACC_3DAudioSource");
            AddTag("Inventory");
            AddTag("CameraRoot");
            AddTag("Interactor");
            AddTag("Flashlight");
            AddTag("Cursor");
            AddTag("UI");
            AddTag("SFXAudioSource");
            AddTag("ButtonsContainer");
            AddTag("TV");
            AddTag("Item");
            AddTag("Door");
            
            AddLayer("HighContrast");
            AddLayer("FirstRender");
            AddLayer("IgnoreTrigger");
            AddLayer("FloorAndCeiling");
            AddLayer("Wall");
            AddLayer("Door");
            AddLayer("Glass");
            AddLayer("Cursor");
            AddLayer("InventoryRender");
            AddLayer("Pomo");
        }

        static void AddTag(string tag)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag)) { return; }
            }

            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }

        static void AddLayer(string layer)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            for (int i = 0; i < layersProp.arraySize; i++)
            {
                SerializedProperty l = layersProp.GetArrayElementAtIndex(i);
                if (l.stringValue.Equals(layer)) { return; }
            }

            for (int j = 8; j < layersProp.arraySize; j++)
            {
                SerializedProperty newLayerProp = layersProp.GetArrayElementAtIndex(j);
                if (string.IsNullOrEmpty(newLayerProp.stringValue))
                {
                    newLayerProp.stringValue = layer;
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }
            Debug.LogError("Maximum limit of layers reached. Layer not added: " + layer);
        }
        
        static void SetupRenderPipeline()
        {
            var pipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>("Assets/TFG_Videojocs/ACC_Example/Renderers/URP-High Fidelity.asset");

            if (pipelineAsset == null)
            {
                return;
            }
            
            GraphicsSettings.renderPipelineAsset = pipelineAsset;
            
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipelineAsset;
            }
        }
    }
}
#endif