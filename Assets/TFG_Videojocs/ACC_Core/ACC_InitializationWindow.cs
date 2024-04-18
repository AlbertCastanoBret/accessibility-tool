#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TFG_Videojocs.ACC_RemapControls;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;

public class ACC_InitializationWindow : EditorWindow
{
    [MenuItem("Tools/ACC/Initialization Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<ACC_InitializationWindow>("Initialization Window");
        window.minSize = new Vector2(400, 600);
        window.maxSize = new Vector2(400, 600);
    }

    private void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TFG_Videojocs/ACC_Core/ACC_InitializationWindow.uss");

        var title = new Label("Accessibility Plugin");
        title.AddToClassList("title");
        
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TFG_Videojocs/ACC_Utilities/aiLogo.png");
        var image = new Image() {image = texture};
        image.AddToClassList("image");

        var text = new TextElement(){text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."};
        text.AddToClassList("text");
        
        var createButton = new Button() { text = "Create" };
        createButton.AddToClassList("button");
        createButton.clicked += () =>
        {
            CreateAccessibilityManager();
            CreateAudioManager();
            CreateCanvas();
            Close();
        };
        
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.Add(title);
        rootVisualElement.Add(image);
        rootVisualElement.Add(text);
        rootVisualElement.Add(createButton);
        rootVisualElement.AddToClassList("root");
    }
    
    private static void CreateAccessibilityManager()
    {
        var accessibilityManager = GameObject.Find("ACC_AccessibilityManager");
        if (accessibilityManager) DestroyImmediate(accessibilityManager);
        accessibilityManager = new GameObject("ACC_AccessibilityManager");
        accessibilityManager.AddComponent<ACC_AccessibilityManager>();
    }
    private static void CreateAudioManager()
    {
        var audioManager = GameObject.Find("ACC_AudioManager");
        if (audioManager) DestroyImmediate(audioManager);
        audioManager = new GameObject("ACC_AudioManager");
            
        var musicSource = new GameObject("ACC_MusicSource");
        musicSource.AddComponent<AudioSource>();
        musicSource.transform.SetParent(audioManager.transform);
            
        var sfxSource = new GameObject("ACC_SFXSource");
        sfxSource.AddComponent<AudioSource>();
        sfxSource.transform.SetParent(audioManager.transform);
            
        audioManager.AddComponent<ACC_AudioManager>();
    }
    private static void CreateCanvas()
    {
        var canvasObject = GameObject.Find("ACC_Canvas");
        if(canvasObject) DestroyImmediate(canvasObject);
        
        canvasObject = new GameObject("ACC_Canvas");
        canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasObject.AddComponent<GraphicRaycaster>();
            
        CreateRebindsControlManager(canvasObject);
        CreateSubtitleManager(canvasObject);
        CreateVisualNotificationManger(canvasObject);
    }
    private static void CreateRebindsControlManager(GameObject canvasObject)
    {
        var rebindControlsManager = new GameObject("ACC_RebindControlsManager");
        
        RectTransform rebindControlsManagerRectTransform = rebindControlsManager.AddComponent<RectTransform>();
        rebindControlsManagerRectTransform.SetParent(canvasObject.transform);
        rebindControlsManagerRectTransform.localPosition = Vector3.zero;
        rebindControlsManagerRectTransform.sizeDelta = new Vector2(0, 0);
        rebindControlsManagerRectTransform.anchorMin = new Vector2(0, 0);
        rebindControlsManagerRectTransform.anchorMax = new Vector2(1, 1);
        rebindControlsManagerRectTransform.pivot = new Vector2(0.5f, 0.5f);
    
        var rebindControlsManagerComponent = rebindControlsManager.AddComponent<ACC_RebindControlsManager>();
        rebindControlsManagerComponent.defaultMenu = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/ACC_DeviceManager.prefab");
        rebindControlsManagerComponent.rebindOverlay = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/WaitForInputPanel.prefab");
        rebindControlsManagerComponent.rebindUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/Rebinding UI/RebindUIPrefab.prefab");
    }
    private static void CreateSubtitleManager(GameObject canvasObject)
    {
        var subtitleManager = new GameObject("ACC_SubtitleManager");
        subtitleManager.transform.SetParent(canvasObject.transform, false);

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
    private static void CreateVisualNotificationManger(GameObject canvasObject)
    {
        var visualNotificationManager = new GameObject("ACC_VisualNotificationManager");
        visualNotificationManager.transform.SetParent(canvasObject.transform, false);

        RectTransform visualNotificationManagerTextRectTransform = visualNotificationManager.AddComponent<RectTransform>();
        visualNotificationManagerTextRectTransform.anchorMin = new Vector2(0.1f, 1);
        visualNotificationManagerTextRectTransform.anchorMax = new Vector2(0.5f, 1);
        visualNotificationManagerTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        visualNotificationManagerTextRectTransform.anchoredPosition = new Vector2(0, -100);
        visualNotificationManagerTextRectTransform.sizeDelta = new Vector2(0, 100);
        
        var visualNotificationBackground = new GameObject("ACC_VisualNotificationBackground");
        visualNotificationBackground.transform.SetParent(visualNotificationManager.transform, false);
        var backgroundColorImage = visualNotificationBackground.AddComponent<UnityEngine.UI.Image>();
        backgroundColorImage.color = new Color(1, 1, 1, 0);
        
        RectTransform backgroundTextRectTransform = visualNotificationBackground.GetComponent<RectTransform>();
        backgroundTextRectTransform.anchorMin = new Vector2(0, 0.5f);
        backgroundTextRectTransform.anchorMax = new Vector2(1, 0.5f);
        backgroundTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        backgroundTextRectTransform.anchoredPosition = new Vector2(0, 0);
        backgroundTextRectTransform.sizeDelta = new Vector2(0, 100);

        var visualNotificationText = new GameObject("ACC_VisualNotificationText");
        visualNotificationText.transform.SetParent(visualNotificationManager.transform, false);
        
        TextMeshProUGUI visualNotificationTextMeshProUGUI = visualNotificationText.AddComponent<TextMeshProUGUI>();
        visualNotificationTextMeshProUGUI.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        visualNotificationTextMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
        visualNotificationTextMeshProUGUI.enableWordWrapping = true;
        visualNotificationTextMeshProUGUI.color = new Color(1f, 0f, 0f, 0);
        //visualNotificationTextMeshProUGUI.enableAutoSizing = true;
        //visualNotificationTextMeshProUGUI.fontSizeMin = 10;
        //visualNotificationTextMeshProUGUI.fontSizeMax = 60;
        
        RectTransform visualNotificationTextRectTransform = visualNotificationText.GetComponent<RectTransform>();
        visualNotificationTextRectTransform.anchorMin = new Vector2(0, 0.5f);
        visualNotificationTextRectTransform.anchorMax = new Vector2(1, 0.5f);
        visualNotificationTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        visualNotificationTextRectTransform.anchoredPosition = new Vector2(0, 0);
        visualNotificationTextRectTransform.sizeDelta = new Vector2(0, 100);

        visualNotificationManager.AddComponent<ACC_VisualNotificationManager>();
    }
}
#endif
