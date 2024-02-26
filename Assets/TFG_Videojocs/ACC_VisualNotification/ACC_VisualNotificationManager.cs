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
    
    private string currentText;
    private float startTime;
    private bool canPlaySubtitleNotification;
    private ACC_VisualNotificationData loadedData;
    
    private void Awake()
    {
        text = GameObject.Find("ACC_VisualNotificationManager/ACC_VisualNotificationText").GetComponent<TextMeshProUGUI>();
        backgroundColor = GameObject.Find("ACC_VisualNotificationManager/ACC_VisualNotificationBackground").GetComponent<Image>();
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
                backgroundColor.color = new Color(0, 0, 0, 0);
            }
        }
    }

    public void LoadVisualNotification(ACC_Sound soundToMatch)
    {
        string fileName = ACC_JSONHelper.GetFileNameByListParameter<ACC_VisualNotificationData, ACC_Sound>(
            "/ACC_JSONVisualNotification/",
            data => data.soundsList,
            (itemInList, itemToMatch) => itemInList.name == itemToMatch.name,
            soundToMatch
        );
        
        string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_JSONVisualNotification/" + fileName);
        loadedData = JsonUtility.FromJson<ACC_VisualNotificationData>(json);
    }

    public void PlayVisualNotification()
    {
        canPlaySubtitleNotification = true;
        text.text = loadedData.message;
        startTime = Time.time;
        
        text.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
            loadedData.fontColor.b, loadedData.fontColor.a);
        backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
            loadedData.backgroundColor.b, loadedData.backgroundColor.a);
        text.fontSize = loadedData.fontSize;
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        (float horizontalAnchorMin, float horizontalAnchorMax, float verticalAnchorMin, float verticalAnchorMax, float posY) = GetAlignment();
        
        rectTransform.anchorMin = new Vector2(horizontalAnchorMin, verticalAnchorMin);
        rectTransform.anchorMax = new Vector2(horizontalAnchorMax, verticalAnchorMax);
        rectTransform.anchoredPosition = new Vector2(0, posY);
        
        GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, text.preferredHeight);
        text.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, text.preferredHeight);
        backgroundColor.GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, text.preferredHeight);
    }

    private (float, float, float, float, float) GetAlignment()
    {
        float horizontalAnchorMin = 0, horizontalAnchorMax = 0, verticalAnchorMin = 0, verticalAnchorMax = 0, posY = 0;
        if (loadedData.horizontalAlignment == "Left")
        {
            horizontalAnchorMin = 0.1f;
            horizontalAnchorMax = 0.5f;
        }
        else if (loadedData.horizontalAlignment == "Center")
        {
            horizontalAnchorMin = 0.3f;
            horizontalAnchorMax = 0.7f;
        }
        else if (loadedData.horizontalAlignment == "Right")
        {
            horizontalAnchorMin = 0.5f;
            horizontalAnchorMax = 0.9f;
        }
        
        if (loadedData.verticalAlignment == "Top")
        {
            verticalAnchorMin = 1;
            verticalAnchorMax = 1;
            posY = -100;
        }
        else if (loadedData.verticalAlignment == "Center")
        {
            verticalAnchorMin = 0.5f;
            verticalAnchorMax = 0.5f;
            posY = 0;
        }
        else if (loadedData.verticalAlignment == "Down")
        {
            verticalAnchorMin = 0;
            verticalAnchorMax = 0;
            posY = 100;
        }

        return (horizontalAnchorMin, horizontalAnchorMax, verticalAnchorMin, verticalAnchorMax, posY);
    }
}
