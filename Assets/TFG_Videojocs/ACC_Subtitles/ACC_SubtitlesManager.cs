using ACC_API;
using TFG_Videojocs.ACC_Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class ACC_SubtitlesManager : MonoBehaviour
{
    private TextMeshProUGUI subtitleText;
    private Image backgroundColor;
    private bool showActorsName, showActorNameColors, menuEnabled;
    private GameObject subtitleSettings, subtitlesToggle, subtitleScrollList;
    
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
                                subtitleScrollList = scrollComponent.gameObject;
                                foreach (Transform settingsOption in scrollComponent)
                                {
                                    if (settingsOption.name == "ACC_SubtitlesEnable")
                                    {
                                        subtitlesToggle = settingsOption.Find("Toggle").gameObject;
                                        subtitlesToggle.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DisableSubtitlesMenu();
        }
    }
    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ACC_Prefab"))
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
                                        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowActorsName(value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ShowActorsColors")
                                    {
                                        var toggle = settingsOption.Find("Toggle");
                                        toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                                        {
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowActorsNameColors(value);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ColorSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
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
                    backgroundColor.enabled = false;
                    loadedData = null;
                    Resources.UnloadUnusedAssets();
                }
                currentIndex++;
            }
        }
    }
    private void OnToggleValueChanged(bool value)
    {
        ACC_AccessibilityManager.Instance.AudioAccessibility.
            SetFeatureState(AudioFeatures.Subtitles, value);
    }
    
    public void InitializeSubtitles(bool state)
    {
        subtitleText.gameObject.SetActive(state);
        backgroundColor.gameObject.SetActive(state);
        if (canPlaySubtitle && state) backgroundColor.enabled = true;
        else backgroundColor.enabled = false;
        
        if (state && loadedData != null && !menuEnabled)
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
            subtitlesToggle.GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleValueChanged);
            subtitlesToggle.GetComponent<Toggle>().isOn = state;
            subtitlesToggle.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
        }
        ACC_AccessibilityManager.Instance.subtitlesEnabled = state;
    }
    public void SetSubtitles(bool state)
    {
        subtitleText.gameObject.SetActive(state);
        backgroundColor.gameObject.SetActive(state);
        if (canPlaySubtitle && state) backgroundColor.enabled = true;
        else backgroundColor.enabled = false;
        
        if (state && !menuEnabled)
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
    public void ResetSubtitlesState()
    {
        if (subtitlesToggle != null)
        {
            subtitlesToggle.GetComponent<Toggle>().isOn = false;
        }
        
        ACC_AccessibilityManager.Instance.subtitlesEnabled = false;
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled);
        PlayerPrefs.Save();
    }
    public void EnableSubtitlesMenu()
    {
        if (subtitleSettings != null) subtitleSettings.SetActive(true);
    }
    public void DisableSubtitlesMenu()
    {
        if (subtitleSettings != null) subtitleSettings.SetActive(false);
    }
    public void HideSubtitles(bool state)
    {
        if (state)
        {
            menuEnabled = true;
            subtitleText.gameObject.SetActive(false);
            backgroundColor.gameObject.SetActive(false);
            subtitleText.enabled = false;
            backgroundColor.enabled = false;
        }
        else
        {
            menuEnabled = false;
            var subtitlesEnabled = ACC_AccessibilityManager.Instance.subtitlesEnabled;
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled))
            {
                subtitlesEnabled = PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.SubtitlesEnabled) == 1;
            }
            InitializeSubtitles(subtitlesEnabled);
            subtitleText.enabled = true;
        }
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
        backgroundColor.enabled = true;

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
        LoadSubtitlesSettings();
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
        #if UNITY_EDITOR
        ACC_AccessibilityManager.Instance.OnValidate();
        #endif
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

        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_ShowActors")
                {
                    var toggle = settingsOption.Find("Toggle");
                    toggle.GetComponent<Toggle>().isOn = loadedData == null || loadedData.showActors;
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
        
        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_ShowActorsColors")
                {
                    var toggle = settingsOption.Find("Toggle");
                    toggle.GetComponent<Toggle>().isOn = loadedData == null || loadedData.showActorsColors;
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

        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_ColorSelector")
                {
                    var dropdown = settingsOption.Find("Dropdown");
                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                }
            }
        }
        
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
        PlayerPrefs.Save();
    }
    public void SetBackgroundColor(Color color)
    {
        backgroundColor.color = new Color(color.r, color.g, color.b, color.a);
        if (menuEnabled) backgroundColor.enabled = false;
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
        
        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_BackgroundColor")
                {
                    var dropdown = settingsOption.Find("Dropdown");
                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
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
        
        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_FontSizeSelector")
                {
                    var dropdown = settingsOption.Find("Dropdown");
                    dropdown.GetComponent<TMP_Dropdown>().value = 0;
                }
            }
        }
        
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
        PlayerPrefs.Save();
    }
    public void LoadSubtitlesSettings()
    {
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled))
        {
            SetSubtitles(PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.SubtitlesEnabled) == 1);
        }
        else
        {
            InitializeSubtitles(ACC_AccessibilityManager.Instance.subtitlesEnabled);
        }
        
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled))
        {
            SetShowActorsName(PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.ActorsNameEnabled) == 1);
        }
        
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled))
        {
            SetShowActorsNameColors(PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled) == 1);
        }
        
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleFontColor), out Color loadedTextFontColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor))
        {
            SetTextFontColor(loadedTextFontColor);
        }
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor), out Color loadedBackgroundColor)
            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor))
        {
            SetBackgroundColor(loadedBackgroundColor);
        }
        if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize))
        {
            SetFontSize((int) PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.SubtitleFontSize));
        }

        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_SubtitlesEnable")
                {
                    var toggle = settingsOption.Find("Toggle");
                    toggle.GetComponent<Toggle>().isOn = ACC_AccessibilityManager.Instance.AudioAccessibility.GetFeatureState(AudioFeatures.Subtitles);
                }
                if (settingsOption.name == "ACC_ShowActors")
                {
                    var toggle = settingsOption.Find("Toggle");
                    toggle.GetComponent<Toggle>().isOn = ACC_AccessibilityManager.Instance.AudioAccessibility.GetActorsNameEnabled();
                }
                if (settingsOption.name == "ACC_ShowActorsColors")
                {
                    var toggle = settingsOption.Find("Toggle");
                    toggle.GetComponent<Toggle>().isOn = ACC_AccessibilityManager.Instance.AudioAccessibility.GetActorsNameColorsEnabled();
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
                }
            }
        }
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

        if (subtitleScrollList != null)
        {
            foreach (Transform settingsOption in subtitleScrollList.transform)
            {
                if (settingsOption.name == "ACC_SubtitlesEnable")
                {
                    var toggle = settingsOption.Find("Toggle");
                    toggle.GetComponent<Toggle>().isOn = false;
                }
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
        
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitlesEnabled);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorsNameColorsEnabled);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.ActorFontColor);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontColor);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleBackgroundColor);
        PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.SubtitleFontSize);
        PlayerPrefs.Save();
    }
}
