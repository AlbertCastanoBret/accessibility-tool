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
    private GameObject subtitleText;
    private GameObject backgroundColor;
    private ACC_SubtitlesManager accSubtitlesManager;
    public ACC_AudioAccessibility()
    {
        CreateSubtitleManager();
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
        backgroundColor.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    public void PlaySubtitle(string name)
    {
        accSubtitlesManager.LoadSubtitles(name);
        accSubtitlesManager.PlaySubtitle();
    }
    
    private void EnableSubtitles(bool enabled)
    {
        subtitleText.GetComponent<TextMeshProUGUI>().enabled = enabled;
        backgroundColor.GetComponent<Image>().enabled = enabled;
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
        
        backgroundColor = new GameObject("ACC_BackgroundColor");
        backgroundColor.transform.SetParent(canvasObject.transform, false);
        var backgroundColorImage = backgroundColor.AddComponent<Image>();
        backgroundColorImage.color = new Color(0, 0, 0, 0);
        
        RectTransform backgroundColorTextRectTransform = backgroundColor.GetComponent<RectTransform>();
        backgroundColorTextRectTransform.anchorMin = new Vector2(0.1f, 0);
        backgroundColorTextRectTransform.anchorMax = new Vector2(0.9f, 0);
        backgroundColorTextRectTransform.pivot = new Vector2(0.5f, 0);
        backgroundColorTextRectTransform.anchoredPosition = new Vector2(0, 50);
        backgroundColorTextRectTransform.sizeDelta = new Vector2(0, 40);

        subtitleText = new GameObject("ACC_SubtitleText");
        subtitleText.transform.SetParent(canvasObject.transform, false);
        
        TextMeshProUGUI subtitleTextMeshProUGUI = this.subtitleText.AddComponent<TextMeshProUGUI>();
        subtitleTextMeshProUGUI.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        subtitleTextMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
        subtitleTextMeshProUGUI.enableWordWrapping = true;
        subtitleTextMeshProUGUI.color = new Color(1f, 0f, 0f, 1);
        
        RectTransform subtitleTextRectTransform = this.subtitleText.GetComponent<RectTransform>();
        subtitleTextRectTransform.anchorMin = new Vector2(0.1f, 0);
        subtitleTextRectTransform.anchorMax = new Vector2(0.9f, 0);
        subtitleTextRectTransform.pivot = new Vector2(0.5f, 0);
        subtitleTextRectTransform.anchoredPosition = new Vector2(0, 50);
        subtitleTextRectTransform.sizeDelta = new Vector2(0, 40);
        
        accSubtitlesManager = subtitleText.AddComponent<ACC_SubtitlesManager>(); 
    }
}
