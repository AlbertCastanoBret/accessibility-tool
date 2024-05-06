using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFG_Videojocs;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class ACC_SubtitlesManager : MonoBehaviour
{
    private TextMeshProUGUI subtitleText;
    private Image backgroundColor;
    private bool showActorsName, showActorNameColors;
    private GameObject subtitleSettings, subtitlesToggle;
    // private Color? actorFontColor;
    
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
                                        subtitlesToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.
                                                SetFeatureState(AudioFeatures.Subtitles, value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ShowActors")
                                    {
                                        var toggle = settingsOption.Find("Toggle");
                                        toggle.GetComponent<Toggle>().isOn = !PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled) || PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.ActorsNameEnabled) == 1;
                                        if (loadedData != null) toggle.GetComponent<Toggle>().isOn = loadedData.showActors;
                                        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowActorsName(value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ShowActorsColors")
                                    {
                                        var toggle = settingsOption.Find("Toggle");
                                        toggle.GetComponent<Toggle>().isOn = !PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled) || PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled) == 1;
                                        if (loadedData != null) toggle.GetComponent<Toggle>().isOn = loadedData.showActorsColors;
                                        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowActorsNameColors(value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ColorSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor), out Color loadedFontColor)
                                            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
                                        {
                                            dropdown.GetComponent<TMP_Dropdown>().value = loadedFontColor == Color.red ? 1 : loadedFontColor == Color.green ? 2 : loadedFontColor == Color.blue ? 3 : 4;
                                        }
                                        else
                                        {
                                            dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                        }
                                        
                                        dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((value) =>
                                        {
                                            Color color = default; 
                                            switch (value)
                                            {
                                                case 0:
                                                    color = loadedData != null ? new Color(loadedData.fontColor.r, loadedData.fontColor.g, loadedData.fontColor.b, loadedData.fontColor.a) : Color.black;
                                                    subtitleText.color = color;
                                                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
                                                    return;
                                                case 1:
                                                    color = Color.red;
                                                    break;
                                                case 2:
                                                    color = Color.green;
                                                    break;
                                                case 3:
                                                    color = Color.blue;
                                                    break;
                                                case 4:
                                                    color = new Color(1, 0.37f, 0, 1);
                                                    break;
                                            }
                                            
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeSubtitleFontColor(color);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_BackgroundColor")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color loadedBackgroundColor)
                                                 && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
                                        {
                                            dropdown.GetComponent<TMP_Dropdown>().value = loadedBackgroundColor == Color.white ? 1 : loadedBackgroundColor == Color.red ? 2 : loadedBackgroundColor ==  Color.green ? 3 : 4;
                                        }
                                        else
                                        {
                                            dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                        }
                                        dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((value) =>
                                        {
                                            Color color = default; 
                                            switch (value)
                                            {
                                                case 0:
                                                    color = loadedData != null ? new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g, loadedData.backgroundColor.b, loadedData.backgroundColor.a) : Color.black;
                                                    backgroundColor.color = color;
                                                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor);
                                                    return;
                                                case 1:
                                                    color = Color.white;
                                                    break;
                                                case 2:
                                                    color = Color.red;
                                                    break;
                                                case 3:
                                                    color = Color.green;
                                                    break;
                                            }
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeSubtitleBackgroundColor(color);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_FontSizeSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
                                        {
                                            double TOLERANCE = 0.00001f;
                                            dropdown.GetComponent<TMP_Dropdown>().value = Math.Abs(PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize) - 20) < TOLERANCE ? 1 : 
                                                Math.Abs(PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize) - 50) < TOLERANCE ? 2 : 3;
                                        }
                                        else
                                        {
                                            dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                        }
                                        
                                        dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((value) =>
                                        {
                                            int size = 0;
                                            switch (value)
                                            {
                                                case 0:
                                                    size = loadedData?.fontSize ?? 50;
                                                    subtitleText.fontSize = size;
                                                       PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
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
                    subtitleText.text = "";
                    backgroundColor.gameObject.SetActive(false);
                    loadedData = null;
                    Resources.UnloadUnusedAssets();
                }
                currentIndex++;
            }
        }
    }
    
    public void SetSubtitles(bool state)
    {
        subtitleText.gameObject.SetActive(state);
        backgroundColor.gameObject.SetActive(state);
        if (state)
        {
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
            {
                ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor), out Color fontColor);
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
    public void SetShowActorsNameColors(bool showActorNameColors)
    {
        this.showActorNameColors = showActorNameColors;
    }
    public bool GetShowActorsNameColors()
    {
        return showActorNameColors;
    }
    public void SetTextFontColor(Color color)
    {
        subtitleText.color = new Color(color.r, color.g, color.b, color.a);
    }
    public Color GetTextFontColor()
    {
        return subtitleText.color;
    }
    public void SetBackgroundColor(Color color)
    {
        backgroundColor.color = new Color(color.r, color.g, color.b, color.a);
    }
    public Color GetBackgroundColor()
    {
        return backgroundColor.color;
    }
    public void SetFontSize(int size)
    {
        subtitleText.fontSize = size;
    }
    public float GetFontSize()
    {
        return subtitleText.fontSize;
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
    }
}
