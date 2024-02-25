using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AudioFeatures
{
    Subtitles,
    VisualNotification,
    PimPam,
    Pum,
    Wow
}

public class ACC_AudioAccessibility
{
    private Dictionary<AudioFeatures, bool> audioFeatureStates = new Dictionary<AudioFeatures, bool>();
    
    private GameObject subtitleText, subtitleBackground;
    private ACC_SubtitlesManager accSubtitlesManager;

    private GameObject visualNotificationText,  visualNotificationBackground;
    private ACC_VisualNotificationManager accVisualNotificationManager;
    public ACC_AudioAccessibility()
    {
        CreateSubtitleManager();
        CreateVisualNotificationManager();
    }

    public void SetFeatureState(AudioFeatures feature, bool enable)
    {
        audioFeatureStates[feature] = enable;
        ApplyFeatureSettings(feature, enable);
    }
    
    private void ApplyFeatureSettings(AudioFeatures feature, bool enabled)
    {
        switch (feature)
        {
            case AudioFeatures.Subtitles:
                EnableSubtitles(enabled);
                break;
            case AudioFeatures.VisualNotification:
                EnableVisualNotification(enabled);
                break;
        }
    }

    public void ChangeSubtitleFontSize(int newFontSize)
    {
        subtitleText.GetComponent<Text>().fontSize = newFontSize;
    }

    public void ChangeSubtitleFontColor(Color newColor)
    {
        subtitleText.GetComponent<Text>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    public void ChangeSubtitleBackgroundColor(Color newColor)
    {
        subtitleBackground.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    public void PlaySubtitle(string name)
    {
        accSubtitlesManager.LoadSubtitles(name);
        accSubtitlesManager.PlaySubtitle();
    }
    
    private void EnableSubtitles(bool enabled)
    {
        subtitleText.GetComponent<TextMeshProUGUI>().enabled = enabled;
        subtitleBackground.GetComponent<Image>().enabled = enabled;
    }
    
    private void CreateSubtitleManager()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        var subtitleManager = new GameObject("ACC_SubtitleManager");
        subtitleManager.transform.SetParent(canvasObject.transform, false);

        RectTransform subtitleManagerTextRectTransform = subtitleManager.AddComponent<RectTransform>();
        subtitleManagerTextRectTransform.anchorMin = new Vector2(0.1f, 0);
        subtitleManagerTextRectTransform.anchorMax = new Vector2(0.9f, 0);
        subtitleManagerTextRectTransform.pivot = new Vector2(0.5f, 0);
        subtitleManagerTextRectTransform.anchoredPosition = new Vector2(0, 50);
        subtitleManagerTextRectTransform.sizeDelta = new Vector2(0, 40);
        
        subtitleBackground = new GameObject("ACC_Background");
        subtitleBackground.transform.SetParent(subtitleManager.transform, false);
        var backgroundColorImage = subtitleBackground.AddComponent<Image>();
        backgroundColorImage.color = new Color(0, 0, 0, 0);
        
        RectTransform backgroundTextRectTransform = subtitleBackground.GetComponent<RectTransform>();
        backgroundTextRectTransform.anchorMin = new Vector2(0, 0.5f);
        backgroundTextRectTransform.anchorMax = new Vector2(1, 0.5f);
        backgroundTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        backgroundTextRectTransform.anchoredPosition = new Vector2(0, 0);
        backgroundTextRectTransform.sizeDelta = new Vector2(0, 40);

        subtitleText = new GameObject("ACC_SubtitleText");
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
        
        accSubtitlesManager = subtitleManager.AddComponent<ACC_SubtitlesManager>();
    }

    public void PlayVisualNotification(ACC_Sound accSound)
    {
        accVisualNotificationManager.LoadVisualNotification(accSound);
        accVisualNotificationManager.PlayVisualNotification();
    }
    
    private void EnableVisualNotification(bool enabled)
    {
        visualNotificationText.GetComponent<TextMeshProUGUI>().enabled = enabled;
        visualNotificationBackground.GetComponent<Image>().enabled = enabled;
    }
    
    private void CreateVisualNotificationManager()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }
        
        var visualNotificationManager = new GameObject("ACC_VisualNotificationManager");
        visualNotificationManager.transform.SetParent(canvasObject.transform, false);

        RectTransform visualNotificationManagerTextRectTransform = visualNotificationManager.AddComponent<RectTransform>();
        visualNotificationManagerTextRectTransform.anchorMin = new Vector2(0.1f, 1);
        visualNotificationManagerTextRectTransform.anchorMax = new Vector2(0.5f, 1);
        visualNotificationManagerTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        visualNotificationManagerTextRectTransform.anchoredPosition = new Vector2(0, -100);
        visualNotificationManagerTextRectTransform.sizeDelta = new Vector2(0, 100);
        
        visualNotificationBackground = new GameObject("ACC_VisualNotificationBackground");
        visualNotificationBackground.transform.SetParent(visualNotificationManager.transform, false);
        var backgroundColorImage = visualNotificationBackground.AddComponent<Image>();
        backgroundColorImage.color = new Color(1, 1, 1, 0);
        
        RectTransform backgroundTextRectTransform = visualNotificationBackground.GetComponent<RectTransform>();
        backgroundTextRectTransform.anchorMin = new Vector2(0, 0.5f);
        backgroundTextRectTransform.anchorMax = new Vector2(1, 0.5f);
        backgroundTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        backgroundTextRectTransform.anchoredPosition = new Vector2(0, 0);
        backgroundTextRectTransform.sizeDelta = new Vector2(0, 100);

        visualNotificationText = new GameObject("ACC_VisualNotificationText");
        visualNotificationText.transform.SetParent(visualNotificationManager.transform, false);
        
        TextMeshProUGUI visualNotificationTextMeshProUGUI = visualNotificationText.AddComponent<TextMeshProUGUI>();
        visualNotificationTextMeshProUGUI.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        visualNotificationTextMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
        visualNotificationTextMeshProUGUI.enableWordWrapping = true;
        visualNotificationTextMeshProUGUI.color = new Color(1f, 0f, 0f, 0);
        
        RectTransform visualNotificationTextRectTransform = visualNotificationText.GetComponent<RectTransform>();
        visualNotificationTextRectTransform.anchorMin = new Vector2(0, 0.5f);
        visualNotificationTextRectTransform.anchorMax = new Vector2(1, 0.5f);
        visualNotificationTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        visualNotificationTextRectTransform.anchoredPosition = new Vector2(0, 0);
        visualNotificationTextRectTransform.sizeDelta = new Vector2(0, 100);

        accVisualNotificationManager = visualNotificationManager.AddComponent<ACC_VisualNotificationManager>();
    }
}
