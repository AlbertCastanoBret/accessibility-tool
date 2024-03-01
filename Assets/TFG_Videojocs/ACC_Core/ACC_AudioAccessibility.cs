using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public enum AudioFeatures
{
    Subtitles,
    VisualNotification,
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

    public void LoadUserPreferences()
    {
        LoadUserPreferencesSubtitles();
        LoadUserPreferencesVisualNotification();
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

    #region Subtitles

    public void ChangeSubtitleFontColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        subtitleText.GetComponent<TextMeshProUGUI>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    public Color GetSubtitleFontColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor), out Color loadedFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
        {
            return loadedFontColor;
        }
        return new Color(0,0,0,1);
    }

    public void ChangeSubtitleBackgroundColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        subtitleBackground.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }
    
    public Color GetSubtitleBackgroundColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
        {
            return loadedBackgroundColor;
        }
        return new Color(1,1,1,0);
    }
    
    public void ChangeSubtitleFontSize(int newFontSize)
    {
        PlayerPrefs.SetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize, newFontSize);
        PlayerPrefs.Save();
        subtitleText.GetComponent<TextMeshProUGUI>().fontSize = newFontSize;
        accSubtitlesManager.UpdateSize();
    }

    public float GetSubtitleFontSize()
    {
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
        {
            return PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
        }
        return 30;
    }

    private void LoadUserPreferencesSubtitles()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor), out Color loadedFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
        {
            subtitleText.GetComponent<TextMeshProUGUI>().color = new Color(loadedFontColor.r, loadedFontColor.g, loadedFontColor.b, loadedFontColor.a);
        }
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
        {
            subtitleBackground.GetComponent<Image>().color = new Color(loadedBackgroundColor.r, loadedBackgroundColor.g, loadedBackgroundColor.b, loadedBackgroundColor.a);
        }
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
        {
            subtitleText.GetComponent<TextMeshProUGUI>().fontSize = PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
        }
    }
    
    public void PlaySubtitle(string name)
    {
        LoadUserPreferencesSubtitles();
        accSubtitlesManager.LoadSubtitles(name);
        accSubtitlesManager.PlaySubtitle();
    }
    
    private void EnableSubtitles(bool enabled)
    {
        accSubtitlesManager.gameObject.SetActive(enabled);
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
    
    #endregion

    #region VisualNotification

        public void PlayVisualNotification(ACC_Sound accSound)
    {
        accVisualNotificationManager.LoadVisualNotification(accSound);
        accVisualNotificationManager.PlayVisualNotification();
    }

    /// <summary>
    /// Change horizontal alignment for visual notifications
    /// </summary>
    /// <param name="alignment">0 = Left, 1 = Center, 2 = Right</param>
    public void ChangeVisualNotificationHorizontalAlignment(int alignment)
    {
        if (alignment == 0)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f,
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.y);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment, "Left");
            PlayerPrefs.Save();
        }
        else if (alignment == 1)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f,
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.y);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f,
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment, "Center");
            PlayerPrefs.Save();
        }
        else if (alignment == 2)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.y);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f,
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment, "Right");
            PlayerPrefs.Save();
        }
        else Debug.LogError("Wrong parameter entered");
    }

    public string GetVisualNotificationHorizontalAlignment()
    {
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment))
        {
            return PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment);
        }
        return "Left";
        //return "Value not saved";
    }
    
    /// <summary>
    /// Change vertical alignment for visual notifications
    /// </summary>
    /// <param name="alignment">0 = Top, 1 = Center, 2 = Down</param>
    public void ChangeVisualNotificationVerticalAlignment(int alignment)
    {
        if (alignment == 0)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 1);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y, 1);
            accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, "Top");
            PlayerPrefs.Save();
        }
        else if (alignment == 1)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0.5f);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y, 0.5f);
            accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, "Center");
            PlayerPrefs.Save();
        }
        else if (alignment == 2)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y, 0);
            accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, "Down");
            PlayerPrefs.Save();
        }
        else Debug.LogError("Wrong parameter entered");
    }
    
    public string GetVisualNotificationVerticalAlignment()
    {
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment))
        {
            return PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment);
        }
        return "Top";
    }
    
    public void ChangeVisualNotificationTimeOnScreen(int newTime)
    {
        
    }
    
    public void ChangeVisualNotificationFontColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        visualNotificationText.GetComponent<TextMeshProUGUI>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    public Color GetVisualNotificationFontColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor), out Color loadedFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor))
        {
            return loadedFontColor;
        }
        return new Color(0,0,0,1);
    }
    
    public void ChangeVisualNotificationBackgroundColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        visualNotificationBackground.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }
    
    public Color GetVisualNotificationBackgroundColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
        {
            return loadedBackgroundColor;
        }
        return new Color(1,1,1,0);
    }
    
    public void ChangeVisualNotificationFontSize(int newFontSize)
    {
        PlayerPrefs.SetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize, newFontSize);
        PlayerPrefs.Save();
        visualNotificationText.GetComponent<TextMeshProUGUI>().fontSize = newFontSize;
        accVisualNotificationManager.UpdateSize();
    }
    
    public float GetVisualNotificationFontSize()
    {
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize))
        {
            return PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
        }
        return 30;
    }
    
    private void LoadUserPreferencesVisualNotification()
    {
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment))
        {
            string horizontalAlignment =
                PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment);
            if (horizontalAlignment == "Left")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f,
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.y);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y);
            }
            else if (horizontalAlignment == "Center")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f,
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.y);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f,
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y);
            }
            else if (horizontalAlignment == "Right")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.y);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f,
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y);
            }
        }

        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment))
        {
            string verticalAlignment =
                PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment);
            if (verticalAlignment == "Top")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 1);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y, 1);
                accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            }
            else if (verticalAlignment == "Center")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0.5f);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y, 0.5f);
                accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
            else if (verticalAlignment == "Down")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.y, 0);
                accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            }
        }
        
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor), out Color loadedFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor))
        {
            visualNotificationText.GetComponent<TextMeshProUGUI>().color = new Color(loadedFontColor.r, loadedFontColor.g, loadedFontColor.b, loadedFontColor.a);
        }
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
        {
            visualNotificationBackground.GetComponent<TextMeshProUGUI>().color = new Color(loadedBackgroundColor.r, loadedBackgroundColor.g, loadedBackgroundColor.b, loadedBackgroundColor.a);
        }
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize))
        {
            visualNotificationText.GetComponent<TextMeshProUGUI>().fontSize = PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
        }
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
        //visualNotificationTextMeshProUGUI.enableAutoSizing = true;
        //visualNotificationTextMeshProUGUI.fontSizeMin = 10;
        //visualNotificationTextMeshProUGUI.fontSizeMax = 60;
        
        RectTransform visualNotificationTextRectTransform = visualNotificationText.GetComponent<RectTransform>();
        visualNotificationTextRectTransform.anchorMin = new Vector2(0, 0.5f);
        visualNotificationTextRectTransform.anchorMax = new Vector2(1, 0.5f);
        visualNotificationTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        visualNotificationTextRectTransform.anchoredPosition = new Vector2(0, 0);
        visualNotificationTextRectTransform.sizeDelta = new Vector2(0, 100);

        accVisualNotificationManager = visualNotificationManager.AddComponent<ACC_VisualNotificationManager>();
    }

    #endregion
}
