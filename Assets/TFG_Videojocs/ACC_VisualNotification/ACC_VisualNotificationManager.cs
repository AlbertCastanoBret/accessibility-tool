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
        Debug.Log(fileName);
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
    }
}
