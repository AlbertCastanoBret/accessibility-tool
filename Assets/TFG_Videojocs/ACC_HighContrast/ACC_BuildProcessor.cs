#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_BuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
            MarkAllObjectsAsNonStaticInAllScenes();
        }
        
        private void MarkAllObjectsAsNonStaticInAllScenes()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            
            var currentScene = EditorSceneManager.GetActiveScene().path;
            
            foreach (var scenePath in scenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath);
                if (scene.isLoaded)
                {
                    MarkAllObjectsAsNonStatic();
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
            }
            
            if (!string.IsNullOrEmpty(currentScene))
            {
                EditorSceneManager.OpenScene(currentScene);
            }
        }
        
        private void MarkAllObjectsAsNonStatic()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.GetComponent<MeshRenderer>() != null)
                {
                    obj.isStatic = false;
                }
            }
        }
    }
}
#endif