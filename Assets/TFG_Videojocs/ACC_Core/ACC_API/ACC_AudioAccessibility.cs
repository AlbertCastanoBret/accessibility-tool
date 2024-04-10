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
    
    /// <summary>
    /// Sets the state of a specified audio feature to either enabled or disabled.
    /// </summary>
    /// <param name="feature">The audio feature to modify. Use AudioFeatures enum to specify the feature.</param>
    /// <param name="enable">A boolean value indicating whether the feature should be enabled (true) or disabled (false).</param>
    public void SetFeatureState(AudioFeatures feature, bool enable)
    {
        audioFeatureStates[feature] = enable;
        switch (feature)
        {
            case AudioFeatures.Subtitles:
                EnableSubtitles(enable);
                break;
            case AudioFeatures.VisualNotification:
                EnableVisualNotification(enable);
                break;
        }
    }
    
    /// <summary>
    /// Loads and applies the user's accessibility preferences related to audio features.
    /// </summary>
    public void LoadUserPreferences()
    {
        LoadUserPreferencesSubtitles();
        LoadUserPreferencesVisualNotification();
    }

    #region Subtitles
    
    /// <summary>
    /// Plays the subtitle with the specified name after loading the user's subtitle preferences.
    /// </summary>
    /// <param name="name">The unique name identifier of the subtitle to be played. This name must match exactly with the subtitle file name created and configured in the accessibility creation window.</param>
    public void PlaySubtitle(string name)
    {
        LoadUserPreferencesSubtitles();
        accSubtitlesManager.LoadSubtitles(name);
        accSubtitlesManager.PlaySubtitle();
    }

    /// <summary>
    /// Changes the subtitle font color to the new specified color and saves this preference for future sessions.
    /// </summary>
    /// <param name="newColor">The new color to be applied to the subtitle font. This color will also be saved in the user's preferences.</param>
    public void ChangeSubtitleFontColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        subtitleText.GetComponent<TextMeshProUGUI>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    /// <summary>
    /// Retrieves the user's preferred subtitle font color from PlayerPrefs.
    /// </summary>
    /// <returns>The Color object representing the user's preferred subtitle font color. Returns black with full opacity if no preference is saved.</returns>
    public Color GetSubtitleFontColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor), out Color loadedFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
        {
            return loadedFontColor;
        }
        return new Color(0,0,0,1);
    }

    /// <summary>
    /// Changes the background color of subtitles to a new specified color and saves this preference for future use.
    /// </summary>
    /// <param name="newColor">The new color to apply to the subtitle background. This color will be saved in the user's preferences and applied immediately to the subtitle background.</param>
    public void ChangeSubtitleBackgroundColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        subtitleBackground.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }
    
    /// <summary>
    /// Retrieves the user's preferred subtitle background color from PlayerPrefs.
    /// </summary>
    /// <returns>The Color object representing the user's preferred subtitle background color. Returns a fully transparent white color if no preference is saved.</returns>
    public Color GetSubtitleBackgroundColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
        {
            return loadedBackgroundColor;
        }
        return new Color(1,1,1,0);
    }
    
    /// <summary>
    /// Changes the font size of subtitles to a new specified size and saves this preference for future use.
    /// </summary>
    /// <param name="newFontSize">The new font size for subtitles. This value will be saved in the user's preferences and applied immediately to the subtitle text.</param>
    public void ChangeSubtitleFontSize(int newFontSize)
    {
        PlayerPrefs.SetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize, newFontSize);
        PlayerPrefs.Save();
        subtitleText.GetComponent<TextMeshProUGUI>().fontSize = newFontSize;
        accSubtitlesManager.UpdateSize();
    }

    /// <summary>
    /// Retrieves the user's preferred subtitle font size from PlayerPrefs.
    /// </summary>
    /// <returns>The font size of subtitles as a float. Returns a default size of 30 if no preference has been saved.</returns>
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
        
        subtitleBackground = new GameObject("ACC_SubtitleBackground");
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

    /// <summary>
    /// Plays a visual notification based on the provided ACC_Sound parameter, after loading the user's visual notification preferences.
    /// </summary>
    /// <param name="accSound">The ACC_Sound object that determines which visual notification to play. This should correspond to a specific sound or event for which a visual notification is configured.</param>
    public void PlayVisualNotification(ACC_Sound accSound)
    {
        LoadUserPreferencesVisualNotification();
        accVisualNotificationManager.LoadVisualNotification(accSound);
        accVisualNotificationManager.PlayVisualNotification();
    }

    /// <summary>
    /// Adjusts the horizontal alignment of visual notifications based on the specified alignment parameter and saves the preference.
    /// </summary>
    /// <param name="alignment">An integer representing the desired horizontal alignment: 0 for Left, 1 for Center, and 2 for Right. Any other value logs an error.</param>
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

    /// <summary>
    /// Retrieves the user's preferred horizontal alignment setting for visual notifications.
    /// </summary>
    /// <returns>A string representing the horizontal alignment preference. Defaults to "Left" if no preference has been saved.</returns>
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
    /// Adjusts the vertical alignment of visual notifications based on the specified alignment parameter and saves the preference.
    /// </summary>
    /// <param name="alignment">An integer representing the desired vertical alignment: 0 for Top, 1 for Center, and 2 for Down. Any other value results in an error.</param>
    public void ChangeVisualNotificationVerticalAlignment(int alignment)
    {
        if (alignment == 0)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 1);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.x, 1);
            accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, "Top");
            PlayerPrefs.Save();
        }
        else if (alignment == 1)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0.5f);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.x, 0.5f);
            accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, "Center");
            PlayerPrefs.Save();
        }
        else if (alignment == 2)
        {
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0);
            accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.x, 0);
            accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, "Down");
            PlayerPrefs.Save();
        }
        else Debug.LogError("Wrong parameter entered");
    }
    
    /// <summary>
    /// Retrieves the user's preferred vertical alignment setting for visual notifications.
    /// </summary>
    /// <returns>A string representing the vertical alignment preference for visual notifications. Defaults to "Top" if no preference has been previously saved.</returns>
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
    
    /// <summary>
    /// Changes the font color of visual notifications to a new specified color and saves this preference for future use.
    /// </summary>
    /// <param name="newColor">The new color to apply to the visual notification font. This color will be saved in the user's preferences and applied immediately to the visual notification text.</param>
    public void ChangeVisualNotificationFontColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        visualNotificationText.GetComponent<TextMeshProUGUI>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }

    /// <summary>
    /// Retrieves the user's preferred font color for visual notifications from PlayerPrefs.
    /// </summary>
    /// <returns>The Color object representing the user's preferred font color for visual notifications. Returns a default color of solid black if no preference has been saved.</returns>
    public Color GetVisualNotificationFontColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor), out Color loadedFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor))
        {
            return loadedFontColor;
        }
        return new Color(0,0,0,1);
    }
    
    /// <summary>
    /// Changes the background color of visual notifications to a new specified color and saves this preference for future use.
    /// </summary>
    /// <param name="newColor">The new color to be applied to the background of visual notifications. This color will be saved in the user's preferences and applied immediately to the visual notification background.</param>
    public void ChangeVisualNotificationBackgroundColor(Color newColor)
    {
        PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor, ColorUtility.ToHtmlStringRGBA(newColor));
        PlayerPrefs.Save();
        visualNotificationBackground.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, newColor.a);
    }
    
    /// <summary>
    /// Retrieves the user's preferred background color for visual notifications from PlayerPrefs.
    /// </summary>
    /// <returns>The Color object representing the user's preferred background color for visual notifications. Returns a default color of transparent white if no preference has been saved.</returns>
    public Color GetVisualNotificationBackgroundColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
        {
            return loadedBackgroundColor;
        }
        return new Color(1,1,1,0);
    }
    
    /// <summary>
    /// Changes the font size of visual notifications to a new specified size and saves this preference for future sessions.
    /// </summary>
    /// <param name="newFontSize">The new font size to be applied to visual notifications. This size will be saved in the user's preferences and applied immediately to the visual notification text.</param>
    public void ChangeVisualNotificationFontSize(int newFontSize)
    {
        PlayerPrefs.SetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize, newFontSize);
        PlayerPrefs.Save();
        visualNotificationText.GetComponent<TextMeshProUGUI>().fontSize = newFontSize;
        accVisualNotificationManager.UpdateSize();
    }
    
    /// <summary>
    /// Retrieves the user's preferred font size for visual notifications from PlayerPrefs.
    /// </summary>
    /// <returns>The font size of visual notifications as a float. Returns a default size of 30 if no preference has been saved.</returns>
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
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.x, 1);
                accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            }
            else if (verticalAlignment == "Center")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0.5f);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.x, 0.5f);
                accVisualNotificationManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
            else if (verticalAlignment == "Down")
            {
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMin = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMin.x, 0);
                accVisualNotificationManager.GetComponent<RectTransform>().anchorMax = new Vector2(
                    accVisualNotificationManager.GetComponent<RectTransform>().anchorMax.x, 0);
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
            visualNotificationBackground.GetComponent<Image>().color = new Color(loadedBackgroundColor.r, loadedBackgroundColor.g, loadedBackgroundColor.b, loadedBackgroundColor.a);
        }
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize))
        {
            visualNotificationText.GetComponent<TextMeshProUGUI>().fontSize = PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize);
        }
    }
    
    private void EnableVisualNotification(bool enabled)
    {
        accVisualNotificationManager.gameObject.SetActive(enabled);
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
