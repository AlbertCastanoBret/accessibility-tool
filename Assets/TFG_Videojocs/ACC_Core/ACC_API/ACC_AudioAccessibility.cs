using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace ACC_API
{
    /// <summary>
    /// Enum representing different audio accessibility features.
    /// </summary>
    public enum AudioFeatures
    {
        /// <summary>
        /// Subtitles for audio content.
        /// </summary>
        Subtitles,

        /// <summary>
        /// Visual notifications for audio events.
        /// </summary>
        VisualNotification,
    }

    public class ACC_AudioAccessibility
    {
        private Dictionary<AudioFeatures, bool> audioFeatureStates = new Dictionary<AudioFeatures, bool>();

        private ACC_SubtitlesManager accSubtitlesManager;
        private ACC_VisualNotificationManager accVisualNotificationManager;

        internal ACC_AudioAccessibility()
        {
            accSubtitlesManager = ACC_PrefabHelper
                .InstantiatePrefabAsChild("Subtitles", ACC_AccessibilityManager.Instance.accCanvas)
                .GetComponent<ACC_SubtitlesManager>();
            accVisualNotificationManager = ACC_PrefabHelper
                .InstantiatePrefabAsChild("VisualNotification", ACC_AccessibilityManager.Instance.accCanvas)
                .GetComponent<ACC_VisualNotificationManager>();
        }

        internal void InitializeState(AudioFeatures feature, bool state)
        {
            switch (feature)
            {
                case AudioFeatures.Subtitles:
                    accSubtitlesManager.InitializeSubtitles(state);
                    break;
                case AudioFeatures.VisualNotification:
                    accVisualNotificationManager.InitializeVisualNotification(state);
                    break;
            }
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
                    accSubtitlesManager.SetSubtitles(state);
                    break;
                case AudioFeatures.VisualNotification:
                    accVisualNotificationManager.SetVisualNotification(state);
                    break;
            }
        }

        /// <summary>
        /// Retrieves the enabled state of the specified audio feature from player preferences.
        /// </summary>
        /// <param name="feature">An enum value representing the audio feature to check (e.g., Subtitles, VisualNotification).</param>
        /// <returns>
        /// <c>true</c> if the specified feature is enabled in player preferences; otherwise, <c>false</c>.
        /// </returns>
        public bool GetFeatureState(AudioFeatures feature)
        {
            switch (feature)
            {
                case AudioFeatures.Subtitles:
                    if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled))
                    {
                        return PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.SubtitlesEnabled) == 1;
                    }

                    return ACC_AccessibilityManager.Instance.subtitlesEnabled;
                case AudioFeatures.VisualNotification:
                    if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationEnabled))
                    {
                        return PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.VisualNotificationEnabled) == 1;
                    }

                    return ACC_AccessibilityManager.Instance.visualNotificationEnabled;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Resets the state of the specified audio feature to its default settings.
        /// </summary>
        /// <param name="feature">The audio feature to reset (e.g., subtitles, visual notifications).</param>
        public void ResetFeatureState(AudioFeatures feature)
        {
            switch (feature)
            {
                case AudioFeatures.Subtitles:
                    accSubtitlesManager.ResetSubtitlesState();
                    break;
                case AudioFeatures.VisualNotification:
                    accVisualNotificationManager.ResetVisualNotificationState();
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
        /// Enables the subtitles menu through the associated subtitles manager.
        /// </summary>
        public void EnableSubtitlesMenu()
        {
            ACC_AccessibilityManager.Instance.EnableCanvas();
            accSubtitlesManager.EnableSubtitlesMenu();
        }

        /// <summary>
        /// Disables the subtitles menu through the associated subtitles manager.
        /// </summary>
        public void DisableSubtitlesMenu()
        {
            ACC_AccessibilityManager.Instance.DisableCanvas();
            accSubtitlesManager.DisableSubtitlesMenu();
        }

        /// <summary>
        /// Plays the subtitle with the specified name after loading the user's subtitle preferences.
        /// </summary>
        /// <param name="name">The unique name identifier of the subtitle to be played. This name must match exactly with the subtitle file name created and configured in the accessibility creation window.</param>
        public void PlaySubtitle(string name)
        {
            //LoadUserPreferencesSubtitles();
            accSubtitlesManager.LoadSubtitles(name);
            accSubtitlesManager.PlaySubtitle();
        }

        /// <summary>
        /// Toggles the display of actors' names based on the provided boolean value and saves this setting.
        /// </summary>
        /// <param name="show">If true, enables the display of actors' names; if false, disables it.</param>
        public void ShowActorsName(bool show)
        {
            PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.ActorsNameEnabled, show ? 1 : 0);
            PlayerPrefs.Save();
            accSubtitlesManager.SetShowActorsName(show);
        }

        /// <summary>
        /// Retrieves the current setting for displaying actors' names.
        /// </summary>
        /// <returns>Returns true if the display of actors' names is enabled, otherwise returns false.</returns>
        public bool GetActorsNameEnabled()
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled))
            {
                return PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.ActorsNameEnabled) == 1;
            }

            return accSubtitlesManager.GetShowActorsName();
        }

        /// <summary>
        /// Resets the actor name setting to the loaded value if available,
        /// updates the corresponding UI elements, and deletes the relevant key from player preferences.
        /// </summary>
        public void ResetActorsName()
        {
            accSubtitlesManager.ResetActorsName();
        }

        /// <summary>
        /// Toggles the coloring of actors' names in subtitles based on the provided boolean value and saves this setting.
        /// </summary>
        /// <param name="show">If true, enables the coloring of actors' names; if false, disables it.</param>
        public void ShowActorsNameColors(bool show)
        {
            PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled, show ? 1 : 0);
            PlayerPrefs.Save();
            accSubtitlesManager.SetShowActorsNameColors(show);
        }

        /// <summary>
        /// Retrieves the current setting for coloring actors' names in subtitles.
        /// </summary>
        /// <returns>Returns true if the coloring of actors' names is enabled, otherwise returns false.</returns>
        public bool GetActorsNameColorsEnabled()
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled))
            {
                return PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled) == 1;
            }

            return accSubtitlesManager.GetShowActorsNameColors();
        }

        /// <summary>
        /// Resets the actor name colors setting to the loaded value if available,
        /// updates the relevant UI elements, and removes the corresponding key from player preferences.
        /// </summary>
        public void ResetActorsNameColors()
        {
            accSubtitlesManager.ResetActorsNameColors();
        }

        /// <summary>
        /// Changes the subtitle font color to the new specified color and saves this preference for future sessions.
        /// </summary>
        /// <param name="newColor">The new color to be applied to the subtitle text font. This color will also be saved in the user's preferences.</param>
        public void ChangeSubtitleFontColor(Color newColor)
        {
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor,
                ColorUtility.ToHtmlStringRGBA(newColor));
            PlayerPrefs.Save();
            accSubtitlesManager.SetTextFontColor(newColor);
        }

        /// <summary>
        /// Retrieves the preferred subtitle font color from user settings.
        /// </summary>
        /// <returns>The Color object for the subtitle text. Defaults to the subtitle manager's color if no specific preference is set.</returns>
        public Color GetSubtitleFontColor()
        {
            if (ColorUtility.TryParseHtmlString(
                    "#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor),
                    out Color loadedFontColor)
                && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
            {
                return loadedFontColor;
            }

            return accSubtitlesManager.GetTextFontColor();
        }

        /// <summary>
        /// Resets the text font color setting to the loaded value if available,
        /// updates the relevant UI elements, and removes the corresponding key from player preferences.
        /// </summary>
        public void ResetSubtitleFontColor()
        {
            accSubtitlesManager.ResetTextFontColor();
        }

        /// <summary>
        /// Changes the background color of subtitles to a new specified color and saves this preference for future use.
        /// </summary>
        /// <param name="newColor">The new color to apply to the subtitle background. This color will be saved in the user's preferences and applied immediately to the subtitle background.</param>
        public void ChangeSubtitleBackgroundColor(Color newColor)
        {
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor,
                ColorUtility.ToHtmlStringRGBA(newColor));
            PlayerPrefs.Save();
            accSubtitlesManager.SetBackgroundColor(newColor);
        }

        /// <summary>
        /// Retrieves the user's preferred subtitle background color from PlayerPrefs.
        /// </summary>
        /// <returns>The Color object representing the user's preferred subtitle background color. Returns a fully transparent white color if no preference is saved.</returns>
        public Color GetSubtitleBackgroundColor()
        {
            if (ColorUtility.TryParseHtmlString(
                    "#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor),
                    out Color loadedBackgroundColor)
                && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
            {
                return loadedBackgroundColor;
            }

            return accSubtitlesManager.GetBackgroundColor();
        }

        /// <summary>
        /// Resets the background color setting to the loaded value if available,
        /// updates the relevant UI elements, and removes the associated key from player preferences.
        /// </summary>
        public void ResetSubtitleBackgroundColor()
        {
            accSubtitlesManager.ResetBackgroundColor();
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
        /// Retrieves the user's preferred subtitle font size from settings.
        /// </summary>
        /// <returns>The font size for subtitles as a float. Returns the default size if no preference is set.</returns>
        public float GetSubtitleFontSize()
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
            {
                return PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
            }

            return accSubtitlesManager.GetFontSize();
        }

        /// <summary>
        /// Resets the font size setting to the loaded value if available,
        /// updates the relevant UI elements, and deletes the associated key from player preferences.
        /// </summary>
        public void ResetSubtitleFontSize()
        {
            accSubtitlesManager.ResetFontSize();
        }

        /// <summary>
        /// Loads the user's preferences for subtitles settings.
        /// </summary>
        public void LoadUserPreferencesSubtitles()
        {
            accSubtitlesManager.LoadSubtitlesSettings();
        }

        /// <summary>
        /// Resets all subtitle settings to the default or loaded values and updates the UI accordingly.
        /// Removes any previously stored subtitle-related preferences from player preferences.
        /// </summary>
        public void ResetSubtitleSettings()
        {
            accSubtitlesManager.ResetSubtitlesSettings();
        }

        #endregion

        #region VisualNotification

        /// <summary>
        /// Enables the visual notification menu through the associated visual notification manager.
        /// </summary>
        public void EnableVisualNotificationMenu()
        {
            ACC_AccessibilityManager.Instance.EnableCanvas();
            accVisualNotificationManager.EnableVisualNotificationMenu();
        }

        /// <summary>
        /// Disables the visual notification menu through the associated visual notification manager.
        /// </summary>
        public void DisableVisualNotificationMenu()
        {
            ACC_AccessibilityManager.Instance.DisableCanvas();
            accVisualNotificationManager.DisableVisualNotificationMenu();
        }

        /// <summary>
        /// Plays a visual notification based on a specified type.
        /// This function loads user preferences for visual notifications, then loads and plays the specified visual notification using the VisualNotificationManager.
        /// </summary>
        /// <param name="visualNotification">The identifier for the type of visual notification to be played.</param>
        public void PlayVisualNotification(string visualNotification)
        {
            LoadUserPreferencesVisualNotification();
            accVisualNotificationManager.LoadVisualNotification(visualNotification);
            accVisualNotificationManager.PlayVisualNotification();
        }

        /// <summary>
        /// Plays a visual notification using specified audio source and clip.
        /// This overloaded function first loads user preferences for visual notifications, then configures the VisualNotificationManager with a specific audio source and clip, and finally plays the visual notification.
        /// </summary>
        /// <param name="audioSource">The audio source to use for the visual notification.</param>
        /// <param name="audioClip">The audio clip to be played alongside the visual notification.</param>
        internal void PlayVisualNotification(string audioSource, string audioClip)
        {
            LoadUserPreferencesVisualNotification();
            accVisualNotificationManager.LoadVisualNotification(audioSource, audioClip);
            accVisualNotificationManager.PlayVisualNotification();
        }

        /// <summary>
        /// Adjusts the horizontal alignment of visual notifications based on the specified alignment parameter and saves the preference.
        /// </summary>
        /// <param name="alignment">An integer representing the desired horizontal alignment: 0 for Left, 1 for Center, and 2 for Right. Any other value logs an error.</param>
        public void ChangeVisualNotificationHorizontalAlignment(int alignment)
        {
            accVisualNotificationManager.SetHorizontalAlignment(alignment);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment, alignment switch
            {
                0 => "Left",
                1 => "Center",
                2 => "Right",
                _ => "Default"
            });
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

            return accVisualNotificationManager.GetCurrentHorizontalAlignment();
        }

        /// <summary>
        /// Resets the horizontal alignment setting to the loaded value if available,
        /// updates the relevant UI elements, and removes the corresponding key from player preferences.
        /// </summary>
        public void ResetVisualNotificationHorizontalAlignment()
        {
            accVisualNotificationManager.ResetHorizontalAlignment();
        }

        /// <summary>
        /// Adjusts the vertical alignment of visual notifications based on the specified alignment parameter and saves the preference.
        /// </summary>
        /// <param name="alignment">An integer representing the desired vertical alignment: 0 for Top, 1 for Center, and 2 for Down. Any other value results in an error.</param>
        public void ChangeVisualNotificationVerticalAlignment(int alignment)
        {
            accVisualNotificationManager.SetVerticalAlignment(alignment);
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment, alignment switch
            {
                0 => "Top",
                1 => "Center",
                2 => "Down",
                _ => "Default"
            });
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

            return accVisualNotificationManager.GetCurrentVerticalAlignment();
        }

        /// <summary>
        /// Resets the vertical alignment setting to the loaded value if available,
        /// updates the relevant UI elements, and removes the corresponding key from player preferences.
        /// </summary>
        public void ResetVisualNotificationVerticalAlignment()
        {
            accVisualNotificationManager.ResetVerticalAlignment();
        }

        /// <summary>
        /// Changes the duration for which visual notifications remain visible on the screen.
        /// </summary>
        /// <param name="newTime">The new time duration (in seconds) for visual notifications to be displayed.</param>
        public void ChangeVisualNotificationTimeOnScreen(int newTime)
        {
            PlayerPrefs.SetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen, newTime);
            PlayerPrefs.Save();
            accVisualNotificationManager.SetTimeOnScreen(newTime);
        }

        /// <summary>
        /// Retrieves the current duration time for which visual notifications are displayed on the screen.
        /// </summary>
        /// <returns>The duration time in seconds. Returns -1 if the setting has not been previously set.</returns>
        public float GetVisualNotificationTimeOnScreen()
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen))
            {
                return PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationTimeOnScreen);
            }

            return accVisualNotificationManager.GetTimeOnScreen();
        }

        /// <summary>
        /// Changes the font color of visual notifications to a new specified color and saves this preference for future use.
        /// </summary>
        /// <param name="newColor">The new color to apply to the visual notification font. This color will be saved in the user's preferences and applied immediately to the visual notification text.</param>
        public void ChangeVisualNotificationFontColor(Color newColor)
        {
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor,
                ColorUtility.ToHtmlStringRGBA(newColor));
            PlayerPrefs.Save();
            accVisualNotificationManager.SetTextFontColor(newColor);
        }

        /// <summary>
        /// Retrieves the user's preferred font color for visual notifications from PlayerPrefs.
        /// </summary>
        /// <returns>The Color object representing the user's preferred font color for visual notifications. Returns a default color of solid black if no preference has been saved.</returns>
        public Color GetVisualNotificationFontColor()
        {
            if (ColorUtility.TryParseHtmlString(
                    "#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor),
                    out Color loadedFontColor)
                && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor))
            {
                return loadedFontColor;
            }

            return accVisualNotificationManager.GetTextFontColor();
        }

        /// <summary>
        /// Resets the font size setting to the loaded value if available,
        /// updates the relevant UI elements, and deletes the associated key from player preferences.
        /// </summary>
        public void ResetVisualNotificationFontColor()
        {
            accVisualNotificationManager.ResetTextFontColor();
        }

        /// <summary>
        /// Changes the background color of visual notifications to a new specified color and saves this preference for future use.
        /// </summary>
        /// <param name="newColor">The new color to be applied to the background of visual notifications. This color will be saved in the user's preferences and applied immediately to the visual notification background.</param>
        public void ChangeVisualNotificationBackgroundColor(Color newColor)
        {
            PlayerPrefs.SetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor,
                ColorUtility.ToHtmlStringRGBA(newColor));
            PlayerPrefs.Save();
            accVisualNotificationManager.SetBackgroundColor(newColor);
        }

        /// <summary>
        /// Retrieves the user's preferred background color for visual notifications from PlayerPrefs.
        /// </summary>
        /// <returns>The Color object representing the user's preferred background color for visual notifications. Returns a default color of transparent white if no preference has been saved.</returns>
        public Color GetVisualNotificationBackgroundColor()
        {
            if (ColorUtility.TryParseHtmlString(
                    "#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor),
                    out Color loadedBackgroundColor)
                && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
            {
                return loadedBackgroundColor;
            }

            return accVisualNotificationManager.GetBackgroundColor();
        }

        /// <summary>
        /// Resets the background color setting to the loaded value if available,
        /// updates the relevant UI elements, and removes the associated key from player preferences.
        /// </summary>
        public void ResetVisualNotificationBackgroundColor()
        {
            accVisualNotificationManager.ResetBackgroundColor();
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

            return accVisualNotificationManager.GetFontSize();
        }

        /// <summary>
        /// Resets the font size setting to the loaded value if available,
        /// updates the relevant UI elements, and deletes the associated key from player preferences.
        /// </summary>
        public void ResetVisualNotificationFontSize()
        {
            accVisualNotificationManager.ResetTextFontSize();
        }

        /// <summary>
        /// Loads the user's preferences for visual notifications.
        /// </summary>
        public void LoadUserPreferencesVisualNotification()
        {
            accVisualNotificationManager.LoadVisualNotificationSettings();
        }

        /// <summary>
        /// Resets the visual notification settings to their default values by deleting specific PlayerPrefs keys related to visual notifications and updating the visual notification manager.
        /// </summary>
        /// <remarks>
        /// This function removes PlayerPrefs entries for the horizontal alignment, vertical alignment, font color, background color, and font size of visual notifications.
        /// After deleting these keys, it invokes accVisualNotificationManager.ResetVisualNotificationSettings() to reapply default settings.
        /// </remarks>
        public void ResetVisualNotificationSettings()
        {
            accVisualNotificationManager.ResetVisualNotificationSettings();
        }

        #endregion
    }
}
