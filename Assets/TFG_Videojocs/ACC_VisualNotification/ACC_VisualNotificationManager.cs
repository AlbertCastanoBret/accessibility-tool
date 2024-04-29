using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ACC_VisualNotificationManager : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Image backgroundColor;
    
    private float startTime;
    private bool canPlaySubtitleNotification;
    private ACC_VisualNotificationData loadedData;
    
    private void Awake()
    {
        text = GameObject.Find(gameObject.name + "/ACC_VisualNotificationText").GetComponent<TextMeshProUGUI>();
        backgroundColor = GameObject.Find(gameObject.name + "/ACC_VisualNotificationBackground").GetComponent<Image>();
    }

    private void Update()
    {
        if (canPlaySubtitleNotification)
        {
            float currentTime = Time.time;
            float elapsedTime = currentTime - startTime;
            
            if (!(elapsedTime <= loadedData.timeOnScreen))
            {
                canPlaySubtitleNotification = false;
                text.text = "";
                backgroundColor.gameObject.SetActive(false);
                Resources.UnloadUnusedAssets();
            }
        }
    }

    public void LoadVisualNotification(ACC_Sound soundToMatch)
    {
        loadedData = ACC_JSONHelper.LoadJson<ACC_VisualNotificationData>("ACC_VisualNotification/A");
        Debug.Log(loadedData);
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
        
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        float horizontalAnchorMin = rectTransform.anchorMin.x;
        float horizontalAnchorMax = rectTransform.anchorMax.x;
        float verticalAnchorMin = rectTransform.anchorMin.y;
        float verticalAnchorMax = rectTransform.anchorMax.y;
        float posY = rectTransform.anchoredPosition.y;

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationHorizontalAlignment))
        {
            (horizontalAnchorMin, horizontalAnchorMax) = GetHorizontalAlignment();
        }

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.VisualNotificationVerticalAlignment))
        {
            (verticalAnchorMin, verticalAnchorMax, posY) = GetVerticalAlignment();
        }
        
        rectTransform.anchorMin = new Vector2(horizontalAnchorMin, verticalAnchorMin);
        rectTransform.anchorMax = new Vector2(horizontalAnchorMax, verticalAnchorMax);
        rectTransform.anchoredPosition = new Vector2(0, posY);
        
        UpdateSize();
    }
    
    public void UpdateSize()
    {
        GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, text.preferredHeight);
        text.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, text.preferredHeight);
        backgroundColor.GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, text.preferredHeight);
    }
    
    public void SetTextFontColor(Color color)
    {
        text.color = new Color(color.r, color.g, color.b, color.a);
    }
    
    public void SetBackgroundColor(Color color)
    {
        backgroundColor.color = new Color(color.r, color.g, color.b, color.a);
    }
    
    public void SetFontSize(int size)
    {
        text.fontSize = size;
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
    
}
