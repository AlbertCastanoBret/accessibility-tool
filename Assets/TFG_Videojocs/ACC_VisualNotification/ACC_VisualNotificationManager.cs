using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFG_Videojocs;
using TFG_Videojocs.ACC_Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ACC_VisualNotificationManager : MonoBehaviour
{
    
    private TextMeshProUGUI text;
    private Image backgroundColor;
    private int timeOnScreen;
    private GameObject visualNotificationSettings;
    
    private float startTime;
    private bool canPlaySubtitleNotification;
    private ACC_VisualNotificationData loadedData;
    
    private void Awake()
    {
        timeOnScreen = -1;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ACC_VisualNotificationText")) text = child.GetComponent<TextMeshProUGUI>();
            
            if (child.CompareTag("ACC_VisualNotificationBackground")) backgroundColor = child.GetComponent<Image>();
            if (child.CompareTag("ACC_Prefab"))
            {
                visualNotificationSettings = child.gameObject;
                foreach (Transform settingComponent in visualNotificationSettings.transform)
                {
                    if (settingComponent.CompareTag("ACC_Scroll"))
                    {
                        foreach (Transform scrollComponent in settingComponent)
                        {
                            if (scrollComponent.CompareTag("ACC_ScrollList"))
                            {
                                foreach (Transform settingsOption in scrollComponent)
                                {
                                    if (settingsOption.name == "ACC_HorizontalAlignment")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                        var key = PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment) ? PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment) : null;
                                        dropdown.value = key switch
                                        {
                                            "Default" => 0,
                                            "Left" => 1,
                                            "Center" => 2,
                                            "Right" => 3,
                                            _ => 0
                                        };
                                        dropdown.onValueChanged.AddListener((value) =>
                                        {
                                            var newValue = -1;
                                            switch (value)
                                            {
                                                case 0:
                                                    newValue = -1;
                                                    if (loadedData != null)
                                                    {
                                                        newValue = loadedData.horizontalAlignment switch
                                                        {
                                                            "Left" => 0,
                                                            "Center" => 1,
                                                            "Right" => 2,
                                                            _ => -1
                                                        };
                                                    }
                                                    break;
                                                case 1:
                                                    newValue = 0;
                                                    break;
                                                case 2:
                                                    newValue = 1;
                                                    break;
                                                case 3:
                                                    newValue = 2;
                                                    break;
                                            }
                                            
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeVisualNotificationHorizontalAlignment(newValue);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_VerticalAlignment")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                        var key = PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment) ? PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment) : null;
                                        dropdown.value = key switch
                                        {
                                            "Default" => 0,
                                            "Top" => 1,
                                            "Center" => 2,
                                            "Down" => 3,
                                            _ => 0
                                        };
                                        dropdown.onValueChanged.AddListener((value) =>
                                        {
                                            var newValue = -1;
                                            switch (value)
                                            {
                                                case 0:
                                                    newValue = -1;
                                                    if (loadedData != null)
                                                    {
                                                        newValue = loadedData.verticalAlignment switch
                                                        {
                                                            "Top" => 0,
                                                            "Center" => 1,
                                                            "Down" => 2,
                                                            _ => -1
                                                        };
                                                    }
                                                    break;
                                                case 1:
                                                    newValue = 0;
                                                    break;
                                                case 2:
                                                    newValue = 1;
                                                    break;
                                                case 3:
                                                    newValue = 2;
                                                    break;
                                            }
                                            
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeVisualNotificationVerticalAlignment(newValue);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_ColorSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        var key = PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor) ? PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor) : null;
                                        if (key == "Default") dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                        else if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor), out Color loadedFontColor)
                                            && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor))
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
                                                    break;
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
                                            
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeVisualNotificationFontColor(color);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_BackgroundColor")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        var key = PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor) ? PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor) : null;
                                        if (key == "Default") dropdown.GetComponent<TMP_Dropdown>().value = 0;
                                        else if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor), out Color loadedBackgroundColor)
                                                 && PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
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
                                                    color = loadedData != null ? new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g, loadedData.backgroundColor.b, loadedData.backgroundColor.a) : 
                                                            new Color(0,0,0,0);
                                                    break;
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
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeVisualNotificationBackgroundColor(color);
                                        });
                                    }
                                    if (settingsOption.name == "ACC_FontSizeSelector")
                                    {
                                        var dropdown = settingsOption.Find("Dropdown");
                                        var key = PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize) ? PlayerPrefs.GetString(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize) : null;
                                        if (key == "Default") dropdown.GetComponent<TMP_Dropdown>().value = 1;
                                        else if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize))
                                        {
                                            double TOLERANCE = 0.00001f;
                                            dropdown.GetComponent<TMP_Dropdown>().value = Math.Abs(PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize) - 20) < TOLERANCE ? 1 : 
                                                Math.Abs(PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize) - 50) < TOLERANCE ? 2 : 3;
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
                                                    break;
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
                                            ACC_AccessibilityManager.Instance.AudioAccessibility.ChangeVisualNotificationFontSize(size);
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
                            ACC_AccessibilityManager.Instance.AudioAccessibility.ResetVisualNotificationSettings();
                        });
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (canPlaySubtitleNotification)
        {
            float currentTime = Time.time;
            float elapsedTime = currentTime - startTime;
            
            if (!(elapsedTime <= loadedData.timeOnScreen) && timeOnScreen == -1)
            {
                canPlaySubtitleNotification = false;
                text.text = "";
                backgroundColor.gameObject.SetActive(false);
                Resources.UnloadUnusedAssets();
            }
            else if (!(elapsedTime <= timeOnScreen) && timeOnScreen != -1)
            {
                canPlaySubtitleNotification = false;
                text.text = "";
                backgroundColor.gameObject.SetActive(false);
                Resources.UnloadUnusedAssets();
            }
        }
    }

    public void LoadVisualNotification(string jsonFile)
    {
        loadedData = ACC_JSONHelper.LoadJson<ACC_VisualNotificationData>("ACC_VisualNotification/" + jsonFile);
    }
    public void LoadVisualNotification(string audioSource, string audioClip)
    {
        var allFiles = ACC_JSONHelper.LoadAllFiles<ACC_VisualNotificationData>("ACC_VisualNotification");
        foreach (var file in allFiles)
        {
            var audioSourceGameObject = FindGameObject(audioSource, "ACC_AudioSource");
            var audioSourceIndex = audioSourceGameObject.transform.GetSiblingIndex();
            
            if(file.soundsList.Exists(x => x.audioSourceKey == audioSourceIndex && x.name == audioClip))
            {
                loadedData = file;
                break;
            }
        }
    }
    public void PlayVisualNotification()
    {
        canPlaySubtitleNotification = true;
        text.text = loadedData.message;
        backgroundColor.gameObject.SetActive(true);
        startTime = Time.time;
        
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontColor))
        {
            text.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
                loadedData.fontColor.b, loadedData.fontColor.a);
        }

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationBackgroundColor))
        {
            backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                loadedData.backgroundColor.b, loadedData.backgroundColor.a);
        }

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationFontSize))
        {
            text.fontSize = loadedData.fontSize;
        }
        
        RectTransform textRectTransform = text.GetComponent<RectTransform>();
        float horizontalAnchorMin = textRectTransform.anchorMin.x;
        float horizontalAnchorMax = textRectTransform.anchorMax.x;
        float verticalAnchorMin = textRectTransform.anchorMin.y;
        float verticalAnchorMax = textRectTransform.anchorMax.y;
        float posY = textRectTransform.anchoredPosition.y;
       
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment))
        {
            (horizontalAnchorMin, horizontalAnchorMax) = GetHorizontalAlignment();
        }
       
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment))
        {
            (verticalAnchorMin, verticalAnchorMax, posY) = GetVerticalAlignment();
        }
       
        textRectTransform.anchorMin = new Vector2(horizontalAnchorMin, verticalAnchorMin);
        textRectTransform.anchorMax = new Vector2(horizontalAnchorMax, verticalAnchorMax);
        textRectTransform.anchoredPosition = new Vector2(0, posY);
        
        RectTransform backgroundRectTransform = backgroundColor.GetComponent<RectTransform>();
        horizontalAnchorMin = backgroundRectTransform.anchorMin.x;
        horizontalAnchorMax = backgroundRectTransform.anchorMax.x;
        verticalAnchorMin = backgroundRectTransform.anchorMin.y;
        verticalAnchorMax = backgroundRectTransform.anchorMax.y;
        posY = backgroundRectTransform.anchoredPosition.y;
       
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment))
        {
            (horizontalAnchorMin, horizontalAnchorMax) = GetHorizontalAlignment();
        }
       
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment))
        {
            (verticalAnchorMin, verticalAnchorMax, posY) = GetVerticalAlignment();
        }
       
        backgroundRectTransform.anchorMin = new Vector2(horizontalAnchorMin, verticalAnchorMin);
        backgroundRectTransform.anchorMax = new Vector2(horizontalAnchorMax, verticalAnchorMax);
        backgroundRectTransform.anchoredPosition = new Vector2(0, posY);
        
        UpdateSize();
    }
    public void UpdateSize()
    {
        text.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, text.preferredHeight);
        backgroundColor.GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, text.preferredHeight);
    }
    public void SetTimeOnScreen(int time)
    {
        timeOnScreen = time;
    }
    public float GetTimeOnScreen()
    {
        return timeOnScreen;
    }
    public void SetTextFontColor(Color color)
    {
        text.color = new Color(color.r, color.g, color.b, color.a);
    }
    public Color GetTextFontColor()
    {
        return text.color;
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
        text.fontSize = size;
    }
    public float GetFontSize()
    {
        return text.fontSize;
    }
    public void SetHorizontalAlignment(int alignment)
    {
        switch (alignment)
        {
            case 0:
                text.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, text.GetComponent<RectTransform>().anchorMin.y);
                text.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, text.GetComponent<RectTransform>().anchorMax.y);
                backgroundColor.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, backgroundColor.GetComponent<RectTransform>().anchorMin.y);
                backgroundColor.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, backgroundColor.GetComponent<RectTransform>().anchorMax.y);
                break;
            case 1:
                text.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f, text.GetComponent<RectTransform>().anchorMin.y);
                text.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f, text.GetComponent<RectTransform>().anchorMax.y);
                backgroundColor.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f, backgroundColor.GetComponent<RectTransform>().anchorMin.y);
                backgroundColor.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f, backgroundColor.GetComponent<RectTransform>().anchorMax.y);
                break;
            case 2:
                text.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, text.GetComponent<RectTransform>().anchorMin.y);
                text.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, text.GetComponent<RectTransform>().anchorMax.y);
                backgroundColor.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, backgroundColor.GetComponent<RectTransform>().anchorMin.y);
                backgroundColor.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, backgroundColor.GetComponent<RectTransform>().anchorMax.y);
                break;
        }
    }
    public string GetCurrentHorizontalAlignment()
    {
        var TOLERANCE = 0.0001f;
        if (Math.Abs(text.GetComponent<RectTransform>().anchorMin.x - 0.1f) < TOLERANCE 
            && Math.Abs(text.GetComponent<RectTransform>().anchorMax.x - 0.5f) < TOLERANCE)
        {
            return "Left";
        }
        if (Math.Abs(text.GetComponent<RectTransform>().anchorMin.x - 0.3f) < TOLERANCE 
            && Math.Abs(text.GetComponent<RectTransform>().anchorMax.x - 0.7f) < TOLERANCE)
        {
            return "Center";
        }
        if (Math.Abs(text.GetComponent<RectTransform>().anchorMin.x - 0.5f) < TOLERANCE 
            && Math.Abs(text.GetComponent<RectTransform>().anchorMax.x - 0.9f) < TOLERANCE)
        {
            return "Right";
        }

        return "Default";
    }
    public void SetVerticalAlignment(int alignment)
    {
        switch (alignment)
        {
            case 0:
                text.GetComponent<RectTransform>().anchorMin = new Vector2(text.GetComponent<RectTransform>().anchorMin.x, 1);
                text.GetComponent<RectTransform>().anchorMax = new Vector2(text.GetComponent<RectTransform>().anchorMax.x, 1);
                text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
                backgroundColor.GetComponent<RectTransform>().anchorMin = new Vector2(backgroundColor.GetComponent<RectTransform>().anchorMin.x, 1);
                backgroundColor.GetComponent<RectTransform>().anchorMax = new Vector2(backgroundColor.GetComponent<RectTransform>().anchorMax.x, 1);
                backgroundColor.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
                break;
            case 1:
                text.GetComponent<RectTransform>().anchorMin = new Vector2(text.GetComponent<RectTransform>().anchorMin.x, 0.5f);
                text.GetComponent<RectTransform>().anchorMax = new Vector2(text.GetComponent<RectTransform>().anchorMax.x, 0.5f);
                text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                backgroundColor.GetComponent<RectTransform>().anchorMin = new Vector2(backgroundColor.GetComponent<RectTransform>().anchorMin.x, 0.5f);
                backgroundColor.GetComponent<RectTransform>().anchorMax = new Vector2(backgroundColor.GetComponent<RectTransform>().anchorMax.x, 0.5f);
                backgroundColor.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                break;
            case 2:
                text.GetComponent<RectTransform>().anchorMin = new Vector2(text.GetComponent<RectTransform>().anchorMin.x, 0);
                text.GetComponent<RectTransform>().anchorMax = new Vector2(text.GetComponent<RectTransform>().anchorMax.x, 0);
                text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
                backgroundColor.GetComponent<RectTransform>().anchorMin = new Vector2(backgroundColor.GetComponent<RectTransform>().anchorMin.x, 0);
                backgroundColor.GetComponent<RectTransform>().anchorMax = new Vector2(backgroundColor.GetComponent<RectTransform>().anchorMax.x, 0);
                backgroundColor.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
                break;
        }
    }
    public string GetCurrentVerticalAlignment()
    {
        var TOLERANCE = 0.0001f;
        if (Math.Abs(text.GetComponent<RectTransform>().anchorMin.y - 1) 
            < TOLERANCE && Math.Abs(text.GetComponent<RectTransform>().anchorMax.y - 1) < TOLERANCE)
        {
            return "Top";
        }
        if (Math.Abs(text.GetComponent<RectTransform>().anchorMin.y - 0.5f) 
            < TOLERANCE && Math.Abs(text.GetComponent<RectTransform>().anchorMax.y - 0.5f) < TOLERANCE)
        {
            return "Center";
        }
        if (Math.Abs(text.GetComponent<RectTransform>().anchorMin.y - 0) 
            < TOLERANCE && Math.Abs(text.GetComponent<RectTransform>().anchorMax.y - 0) < TOLERANCE)
        {
            return "Down";
        }

        return "Default";
    }
    public void ResetVisualNotificationSettings()
    {
        timeOnScreen = -1;
        if (loadedData != null)
        {
            text.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
                loadedData.fontColor.b, loadedData.fontColor.a);
            backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                loadedData.backgroundColor.b, loadedData.backgroundColor.a);
            text.fontSize = loadedData.fontSize;
        }

        if (visualNotificationSettings != null)
        {
            foreach (Transform settingComponent in visualNotificationSettings.transform)
            {
                if (settingComponent.CompareTag("ACC_Scroll"))
                {
                    foreach (Transform scrollComponent in settingComponent)
                    {
                        if (scrollComponent.CompareTag("ACC_ScrollList"))
                        {
                            foreach (Transform settingsOption in scrollComponent)
                            {
                                if (settingsOption.name == "ACC_HorizontalAlignment")
                                {
                                    var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                    dropdown.value = 0;
                                }
                                if (settingsOption.name == "ACC_VerticalAlignment")
                                {
                                    var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                    dropdown.value = 0;
                                }
                                if (settingsOption.name == "ACC_ColorSelector")
                                {
                                    var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                    dropdown.value = 0;
                                }
                                if (settingsOption.name == "ACC_BackgroundColor")
                                {
                                    var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                    dropdown.value = 0;
                                }
                                if (settingsOption.name == "ACC_FontSizeSelector")
                                {
                                    var dropdown = settingsOption.Find("Dropdown").GetComponent<TMP_Dropdown>();
                                    dropdown.value = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private (float horizontalAnchorMin, float horizontalAnchorMax) GetHorizontalAlignment()
    {
        float horizontalAnchorMin = 0, horizontalAnchorMax = 0;

        switch (loadedData.horizontalAlignment)
        {
            case "Left":
                horizontalAnchorMin = 0.1f;
                horizontalAnchorMax = 0.5f;
                break;
            case "Center":
                horizontalAnchorMin = 0.3f;
                horizontalAnchorMax = 0.7f;
                break;
            case "Right":
                horizontalAnchorMin = 0.5f;
                horizontalAnchorMax = 0.9f;
                break;
        }

        return (horizontalAnchorMin, horizontalAnchorMax);
    }
    private (float verticalAnchorMin, float verticalAnchorMax, float posY) GetVerticalAlignment()
    {
        float verticalAnchorMin = 0, verticalAnchorMax = 0, posY = 0;

        switch (loadedData.verticalAlignment)
        {
            case "Top":
                verticalAnchorMin = 1;
                verticalAnchorMax = 1;
                posY = -100;
                break;
            case "Center":
                verticalAnchorMin = 0.5f;
                verticalAnchorMax = 0.5f;
                posY = 0;
                break;
            case "Down":
                verticalAnchorMin = 0;
                verticalAnchorMax = 0;
                posY = 100;
                break;
        }

        return (verticalAnchorMin, verticalAnchorMax, posY);
    }
    private GameObject FindGameObject(string name, string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in gameObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }
}
