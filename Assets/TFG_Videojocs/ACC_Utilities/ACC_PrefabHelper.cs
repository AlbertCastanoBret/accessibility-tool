using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TFG_Videojocs.ACC_Utilities
{
    public class ACC_PrefabHelper
    {
        public static void CreatePrefab(string feature)
        {
            GameObject gameObject = CreateGameObject(feature);
            var folder = "ACC_ " + feature + "/";
            var name = "ACC_" + feature + "Manager.prefab";
            
            Directory.CreateDirectory("Assets/Resources/ACC_Prefabs/" + folder);
            
            var prefabPath = "Assets/Resources/ACC_Prefabs/" + folder + name;
            
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
            
            GameObject.DestroyImmediate(gameObject);
        }
        
        public static GameObject InstantiatePrefabAsChild(string feature, GameObject parent)
        {
            var folder = "ACC_ " + feature + "/";
            var name = "ACC_" + feature + "Manager";
            var prefabPath = "ACC_Prefabs/" + folder + name;
            Debug.Log(prefabPath);
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError("No se pudo cargar el prefab en: " + prefabPath);
                return null;
            }

            return GameObject.Instantiate(prefab, parent.transform);
        }

        private static GameObject CreateGameObject(string feature)
        {
            string name = "ACC_" + feature + "Manager";
            GameObject a = new GameObject(name);
            
            switch (feature)
            {
                case "Subtitles":
                    CreateSubtitleManager(a);
                    break;
            }
            return a;
        }
        
        private static void CreateSubtitleManager(GameObject subtitleManager)
        {
            RectTransform subtitleManagerTextRectTransform = subtitleManager.AddComponent<RectTransform>();
            subtitleManagerTextRectTransform.anchorMin = new Vector2(0.1f, 0);
            subtitleManagerTextRectTransform.anchorMax = new Vector2(0.9f, 0);
            subtitleManagerTextRectTransform.pivot = new Vector2(0.5f, 0);
            subtitleManagerTextRectTransform.anchoredPosition = new Vector2(0, 50);
            subtitleManagerTextRectTransform.sizeDelta = new Vector2(0, 40);
            
            var subtitleBackground = new GameObject("ACC_SubtitleBackground");
            subtitleBackground.transform.SetParent(subtitleManager.transform, false);
            var backgroundColorImage = subtitleBackground.AddComponent<UnityEngine.UI.Image>();
            backgroundColorImage.color = new Color(0, 0, 0, 0);
            
            RectTransform backgroundTextRectTransform = subtitleBackground.GetComponent<RectTransform>();
            backgroundTextRectTransform.anchorMin = new Vector2(0, 0.5f);
            backgroundTextRectTransform.anchorMax = new Vector2(1, 0.5f);
            backgroundTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            backgroundTextRectTransform.anchoredPosition = new Vector2(0, 0);
            backgroundTextRectTransform.sizeDelta = new Vector2(0, 40);

            var subtitleText = new GameObject("ACC_SubtitleText");
            subtitleText.transform.SetParent(subtitleManager.transform, false);

            TextMeshProUGUI subtitleTextMeshProUGUI = subtitleText.AddComponent<TextMeshProUGUI>();
            subtitleTextMeshProUGUI.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            subtitleTextMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
            subtitleTextMeshProUGUI.enableWordWrapping = true;
            subtitleTextMeshProUGUI.color = new Color(1f, 0f, 0f, 1);

            RectTransform subtitleTextRectTransform = subtitleText.GetComponent<RectTransform>();
            subtitleTextRectTransform.anchorMin = new Vector2(0, 0.5f);
            subtitleTextRectTransform.anchorMax = new Vector2(1, 0.5f);
            subtitleTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            subtitleTextRectTransform.anchoredPosition = new Vector2(0, 0);
            subtitleTextRectTransform.sizeDelta = new Vector2(0, 40);
            
            subtitleManager.AddComponent<ACC_SubtitlesManager>();
        }
    }
}