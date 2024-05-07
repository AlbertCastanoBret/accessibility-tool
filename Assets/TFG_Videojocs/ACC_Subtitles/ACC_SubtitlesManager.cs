using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public enum FontColor
{
    Red = 1,
    Green = 2,
    Blue = 3,
    Orange = 4
}

public class ACC_SubtitlesManager : MonoBehaviour
{
    private TextMeshProUGUI subtitleText;
    private Image backgroundColor;
    private bool showActorsName, showActorNameColors;
    private GameObject subtitleSettings, subtitlesToggle;
    
    private bool canPlaySubtitle;
    private int currentIndex;
    private float startTime;
    private float nextSubtitleTime;
    private ACC_SubtitleData loadedData;
    
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ACC_SubtitleText")) subtitleText = child.GetComponent<TextMeshProUGUI>();
            if (child.CompareTag("ACC_SubtitleBackground")) backgroundColor = child.GetComponent<Image>();
        }
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ACC_Prefab"))
            {
                subtitleSettings = child.gameObject;
                foreach (Transform settingComponent in subtitleSettings.transform)
                {
                    if (settingComponent.CompareTag("ACC_Scroll"))
                    {
                        foreach (Transform scrollComponent in settingComponent)
                        {
                            if (scrollComponent.CompareTag("ACC_ScrollList"))
                            {
                                foreach (Transform settingsOption in scrollComponent)
                                {
                                    if (settingsOption.name == "ACC_SubtitlesEnable")
                                    {
                                        subtitlesToggle = settingsOption.Find("Toggle").gameObject;
                                        subtitlesToggle.GetComponent<Toggle>().isOn =
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.GetFeatureState(AudioFeatures.Subtitles);
                                        subtitlesToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.
                                                SetFeatureState(AudioFeatures.Subtitles, value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ShowActors")
                                    {
                                        var toggle = settingsOption.Find("Toggle");
                                        toggle.GetComponent<Toggle>().isOn = ACC_AccessibilityManager.Instance
                                            .AudioAccessibility.GetActorsNameEnabled();
                                        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowActorsName(value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ShowActorsColors")
                                    {
                                        var toggle = settingsOption.Find("Toggle");
                                        toggle.GetComponent<Toggle>().isOn = ACC_AccessibilityManager.Instance
                                            .AudioAccessibility.GetActorsNameColorsEnabled();
                                        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowActorsNameColors(value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ColorSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        var color = ACC_AccessibilityManager.Instance.AudioAccessibility.GetSubtitleFontColor();
                                        var colorName = ACC_ColorManager.GetColorName(color);
                                        dropdown.GetComponent<TMP_Dropdown>().value = colorName switch
                                        {
                                            "Unknown" => 0,
                                            "Red" => 1,
                                            "Green" => 2,
                                            "Blue" => 3,
                                            _ => 0
                                        };
                                        dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((value) =>
                                        {
                                            Color color = default; 
                                            var text = dropdown.GetComponent<TMP_Dropdown>().options[value].text;

                                            if (value == 0)
                                            {
                                                ACC_AccessibilityManager.Instance.AudioAccessibility.ResetSubtitleFontColor();
                                                return;
                                            }
                                            color = ACC_ColorManager.ConvertTextToColor(text);
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeSubtitleFontColor(color);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_BackgroundColor")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        var color = ACC_AccessibilityManager.Instance.AudioAccessibility.GetSubtitleBackgroundColor();
                                        var colorName = ACC_ColorManager.GetColorName(color);
                                        dropdown.GetComponent<TMP_Dropdown>().value = colorName switch
                                        {
                                            "Unknown" => 0,
                                            "White" => 1,
                                            "Red" => 2,
                                            "Green" => 3,
                                            _ => 0
                                        };
                                        dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((value) =>
                                        {
                                            Color color = default; 
                                            var text = dropdown.GetComponent<TMP_Dropdown>().options[value].text;

                                            if (value == 0)
                                            {
                                                ACC_AccessibilityManager.Instance.AudioAccessibility.ResetSubtitleBackgroundColor();
                                                return;
                                            }
                                            color = ACC_ColorManager.ConvertTextToColor(text);
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeSubtitleBackgroundColor(color);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_FontSizeSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        var fontSize = ACC_AccessibilityManager.Instance.AudioAccessibility.GetSubtitleFontSize();
                                        dropdown.GetComponent<TMP_Dropdown>().value = fontSize switch
                                        {
                                            20 => 1,
                                            50 => 2,
                                            80 => 3,
                                            _ => 0
                                        };
                                        dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((value) =>
                                        {
                                            int size = 0;
                                            switch (value)
                                            {
                                                case 0: 
                                                    ACC_AccessibilityManager.Instance.AudioAccessibility.ResetSubtitleFontSize();
                                                    return;
                                                case 1:
                                                    size = 20;
                                                    break;
                                                case 2:
                                                    size = 50;
                                                    break;
                                                case 3:
                                                    size = 80;
                                                    break;
                                            }
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeSubtitleFontSize(size);
                                        });
                                    }
                                }
                            }
                        }
                    }
                    if(settingComponent.name == "ACC_ResetButton")
                    {
                        var button = settingComponent.GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
                            ACC_AccessibilityManager.Instance.AudioAccessibility.ResetSubtitleSettings();
                        });
                    }
                }
            }
        }
    }

    void Update()
    {
        if (canPlaySubtitle)
        {
            float currentTime = Time.time;
            if (currentTime >= nextSubtitleTime)
            {
                if (currentIndex < loadedData.subtitles.Items.Count)
                {
                    if (showActorsName)
                    {
                        Color color = loadedData.actors.Items.Find(actor => actor.value.actor == loadedData.subtitles.Items[currentIndex].value.actor).value.color;
                        
                        if (showActorNameColors)
                        {
                            var hexColor = color.ToHexString();
                            subtitleText.text = "<color=#" + hexColor + ">" + loadedData.subtitles.Items[currentIndex].value.actor + ": </color> " +  loadedData.subtitles.Items[currentIndex].value.subtitle;
                        }
                        else
                        {
                            subtitleText.text = loadedData.subtitles.Items[currentIndex].value.actor + ": " + loadedData.subtitles.Items[currentIndex].value.subtitle;
                        }
                    }
                    else
                        subtitleText.text = loadedData.subtitles.Items[currentIndex].value.subtitle;
                    startTime = currentTime;
                    nextSubtitleTime = startTime + loadedData.subtitles.Items[currentIndex].value.time;
                    UpdateSize();
                }
                else if (currentIndex >= loadedData.subtitles.Items.Count)
                {
                    currentIndex = -1;
                    canPlaySubtitle = false;
                    showActorsName = false;
                    showActorNameColors = false;
                    subtitleText.text = "";
                    backgroundColor.gameObject.SetActive(false);
                    loadedData = null;
                    Resources.UnloadUnusedAssets();
                }
                currentIndex++;
            }
        }
    }
    
    #if UNITY_EDITOR
    public void InitializeSubtitles(bool state)
    {
        subtitleText.gameObject.SetActive(state);
        backgroundColor.gameObject.SetActive(state);
        if (state && loadedData != null)
        { 
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
            {
                ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color fontColor);
                backgroundColor.color = new Color(fontColor.r, fontColor.g, fontColor.b, fontColor.a);
            }
            else if (loadedData != null)
            {
                backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                    loadedData.backgroundColor.b, loadedData.backgroundColor.a);
            }
        }
        else
        {
            backgroundColor.color = new Color(0, 0, 0, 0);
        }

        if (subtitlesToggle != null)
        {
            subtitlesToggle.GetComponent<Toggle>().isOn = state;
            PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled);
        }
        ACC_AccessibilityManager.Instance.subtitlesEnabled = state;
    }
    #endif
    
    public void SetSubtitles(bool state)
    {
        subtitleText.gameObject.SetActive(state);
        backgroundColor.gameObject.SetActive(state);
        if (state)
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
            {
                ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color fontColor);
                backgroundColor.color = new Color(fontColor.r, fontColor.g, fontColor.b, fontColor.a);
            }
            else if (loadedData != null)
            {
                backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                    loadedData.backgroundColor.b, loadedData.backgroundColor.a);
            }
        }
        else
        {
            backgroundColor.color = new Color(0, 0, 0, 0);
        }
        
        PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.SubtitlesEnabled, state ? 1 : 0);
        PlayerPrefs.Save();
        
        if (subtitlesToggle != null) subtitlesToggle.GetComponent<Toggle>().isOn = state;
        ACC_AccessibilityManager.Instance.subtitlesEnabled = state;
    }
    public void LoadSubtitles(string jsonFile)
    {
        loadedData = ACC_JSONHelper.LoadJson<ACC_SubtitleData>("ACC_Subtitles/" + jsonFile);
    }
    public void PlaySubtitle()
    {
        canPlaySubtitle = true;
        subtitleText.text = "";
        currentIndex = 0;

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled))
        {
            showActorsName = loadedData.showActors;
        }
        
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled))
        {
            showActorNameColors = loadedData.showActorsColors;
        }
        
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
        {
            subtitleText.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
                loadedData.fontColor.b, loadedData.fontColor.a);
        }

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
        {
            backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                loadedData.backgroundColor.b, loadedData.backgroundColor.a);
        }

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
        {
            subtitleText.fontSize = loadedData.fontSize;
        }
    }
    public void UpdateSize()
    {
        subtitleText.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, subtitleText.preferredHeight);
        backgroundColor.GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, subtitleText.preferredHeight);
    }
    public void SetShowActorsName(bool showActorsName)
    {
        this.showActorsName = showActorsName;
    }
    public bool GetShowActorsName()
    {
        return showActorsName;
    }
    public void ResetActorsName()
    {
        if (loadedData != null)
        {
            showActorsName = loadedData.showActors;
        }
        
        if (subtitleSettings != null)
        {
            foreach (Transform settingComponent in subtitleSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_ShowActors")
                                {
                                    var toggle = settingsOption.Find("Toggle");
                                    toggle.GetComponent<Toggle>().isOn = loadedData == null || loadedData.showActors;
                                }
                            }
                        }
                    }
                }
            }
        }
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled);
        PlayerPrefs.Save();
    }
    public void SetShowActorsNameColors(bool showActorNameColors)
    {
        this.showActorNameColors = showActorNameColors;
    }
    public bool GetShowActorsNameColors()
    {
        return showActorNameColors;
    }
    public void ResetActorsNameColors()
    {
        if (loadedData != null)
        {
            showActorNameColors = loadedData.showActorsColors;
        }
        
        if (subtitleSettings != null)
        {
            foreach (Transform settingComponent in subtitleSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_ShowActorsColors")
                                {
                                    var toggle = settingsOption.Find("Toggle");
                                    toggle.GetComponent<Toggle>().isOn = loadedData == null || loadedData.showActorsColors;
                                }
                            }
                        }
                    }
                }
            }
        }
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled);
        PlayerPrefs.Save();
    }
    public void SetTextFontColor(Color color)
    {
        subtitleText.color = new Color(color.r, color.g, color.b, color.a);
    }
    public Color GetTextFontColor()
    {
        return subtitleText.color;
    }
    public void ResetTextFontColor()
    {
        if (loadedData != null)
        {
            subtitleText.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
                loadedData.fontColor.b, loadedData.fontColor.a);
        }
        
        if (subtitleSettings != null)
        {
            foreach (Transform settingComponent in subtitleSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_ColorSelector")
                                {
                                    var dropdown = settingsOption.Find("Dropdown");
                                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
        PlayerPrefs.Save();
    }
    public void SetBackgroundColor(Color color)
    {
        backgroundColor.color = new Color(color.r, color.g, color.b, color.a);
    }
    public Color GetBackgroundColor()
    {
        return backgroundColor.color;
    }
    public void ResetBackgroundColor()
    {
        if (loadedData != null)
        {
            backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                loadedData.backgroundColor.b, loadedData.backgroundColor.a);
        }
        
        if (subtitleSettings != null)
        {
            foreach (Transform settingComponent in subtitleSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_BackgroundColor")
                                {
                                    var dropdown = settingsOption.Find("Dropdown");
                                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor);
        PlayerPrefs.Save();
    }
    public void SetFontSize(int size)
    {
        subtitleText.fontSize = size;
    }
    public float GetFontSize()
    {
        return subtitleText.fontSize;
    }
    public void ResetFontSize()
    {
        if (loadedData != null)
        {
            subtitleText.fontSize = loadedData.fontSize;
        }
        
        if (subtitleSettings != null)
        {
            foreach (Transform settingComponent in subtitleSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_FontSizeSelector")
                                {
                                    var dropdown = settingsOption.Find("Dropdown");
                                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
        PlayerPrefs.Save();
    }
    public void ResetSubtitlesSettings()
    {
        if (loadedData != null)
        {
            showActorsName = loadedData.showActors;
            showActorNameColors = loadedData.showActorsColors;
            subtitleText.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
                loadedData.fontColor.b, loadedData.fontColor.a);
            backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                loadedData.backgroundColor.b, loadedData.backgroundColor.a);
            subtitleText.fontSize = loadedData.fontSize;
        }

        if (subtitleSettings != null)
        {
            foreach (Transform settingComponent in subtitleSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_ShowActors")
                                {
                                    var toggle = settingsOption.Find("Toggle");
                                    toggle.GetComponent<Toggle>().isOn = loadedData == null || loadedData.showActors;
                                }
                                
                                if (settingsOption.name == "ACC_ShowActorsColors")
                                {
                                    var toggle = settingsOption.Find("Toggle");
                                    toggle.GetComponent<Toggle>().isOn = loadedData == null || loadedData.showActorsColors;
                                }

                                if (settingsOption.name == "ACC_ColorSelector")
                                {
                                    var dropdown = settingsOption.Find("Dropdown");
                                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                }
                                    
                                if (settingsOption.name == "ACC_BackgroundColor")
                                {
                                    var dropdown = settingsOption.Find("Dropdown");
                                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                }

                                if (settingsOption.name == "ACC_FontSizeSelector")
                                {
                                    var dropdown = settingsOption.Find("Dropdown");
                                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorFontColor);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
        PlayerPrefs.Save();
    }
}
