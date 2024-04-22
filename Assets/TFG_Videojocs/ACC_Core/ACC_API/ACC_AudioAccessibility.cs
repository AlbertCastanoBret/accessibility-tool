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
    
    private ACC_SubtitlesManager accSubtitlesManager;
    private ACC_VisualNotificationManager accVisualNotificationManager;
    public ACC_AudioAccessibility()
    {
        accSubtitlesManager = ACC_PrefabHelper.InstantiatePrefabAsChild("Subtitles", ACC_AccessibilityManager.Instance.accCanvas).GetComponent<ACC_SubtitlesManager>();
        accVisualNotificationManager = ACC_PrefabHelper.InstantiatePrefabAsChild("VisualNotification", ACC_AccessibilityManager.Instance.accCanvas).GetComponent<ACC_VisualNotificationManager>();
    }
    
    /// <summary>
    /// Sets the state of a specified audio feature to either enabled or disabled.
    /// </summary>
    /// <param name="feature">The audio feature to modify. Use AudioFeatures enum to specify the feature.</param>
    /// <param name="state">A boolean value indicating whether the feature should be enabled (true) or disabled (false).</param>
    public void SetFeatureState(AudioFeatures feature, bool state)
    {
        audioFeatureStates[feature] = state;
        switch (feature)
        {
            case AudioFeatures.Subtitles:
                accSubtitlesManager.gameObject.SetActive(state);
                break;
            case AudioFeatures.VisualNotification:
                accVisualNotificationManager.gameObject.SetActive(state);
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
        accSubtitlesManager.SetTextFontColor(newColor);
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
        accSubtitlesManager.SetBackgroundColor(newColor);
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
        accSubtitlesManager.SetFontSize(newFontSize);
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
            accSubtitlesManager.SetTextFontColor(loadedFontColor);
        }
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
        {
            accSubtitlesManager.SetBackgroundColor(loadedBackgroundColor);
        }
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
        {
            accSubtitlesManager.SetFontSize((int) PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize));
        }
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
        accVisualNotificationManager.SetTextFontColor(newColor);
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
        accVisualNotificationManager.SetBackgroundColor(newColor);
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
        accVisualNotificationManager.SetFontSize(newFontSize);
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
            accVisualNotificationManager.SetTextFontColor(loadedFontColor);
        }
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
        {
            accVisualNotificationManager.SetBackgroundColor(loadedBackgroundColor);
        }
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize))
        {
            accVisualNotificationManager.SetFontSize((int) PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize));
        }
    }
    #endregion
}
