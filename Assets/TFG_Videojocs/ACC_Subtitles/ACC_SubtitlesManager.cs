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

public class ACC_SubtitlesManager : MonoBehaviour
{
    private TextMeshProUGUI subtitleText;
    private Image backgroundColor;
    private bool showActorsName;
    private Color? actorFontColor;
    
    private bool canPlaySubtitle;
    private int currentIndex;
    private float startTime;
    private float nextSubtitleTime;
    private ACC_SubtitleData loadedData;

    private void Awake()
    {
        subtitleText = GameObject.Find(gameObject.name + "/ACC_SubtitleText").GetComponent<TextMeshProUGUI>();
        backgroundColor = GameObject.Find(gameObject.name + "/ACC_SubtitleBackground").GetComponent<Image>();
        actorFontColor = null;
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
                        Color color;
                        if (actorFontColor != null)
                            color = (Color) actorFontColor;
                        else color = loadedData.actors.Items.Find(actor => actor.value.actor == loadedData.subtitles.Items[currentIndex].value.actor).value.color;
                        
                        var hexColor = color.ToHexString();
                        subtitleText.text = "<color=#" + hexColor + ">" + loadedData.subtitles.Items[currentIndex].value.actor + ": </color> " +  loadedData.subtitles.Items[currentIndex].value.subtitle;
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

    public void LoadSubtitles(string jsonFile)
    {
        loadedData = ACC_JSONHelper.LoadJson<ACC_SubtitleData>("ACC_Subtitles/" + jsonFile);
    }
    

    public void PlaySubtitle()
    {
        canPlaySubtitle = true;
        subtitleText.text = "";
        backgroundColor.gameObject.SetActive(true);
        currentIndex = 0;

        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorsNameEnabled))
        {
            showActorsName = loadedData.showActors;
        }
        
        if (!PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.ActorFontColor))
        {
            actorFontColor = loadedData.actors.Items[0].value.color;
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
        GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, subtitleText.preferredHeight);
        subtitleText.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(0, subtitleText.preferredHeight);
        backgroundColor.GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, subtitleText.preferredHeight);
    }
    
    public void SetShowActorsName(bool showActors)
    {
        showActorsName = showActors;
    }

    public void SetActorFontColor(Color color)
    {
        actorFontColor = new Color(color.r, color.g, color.b, color.a);
    }
    
    public void SetTextFontColor(Color color)
    {
        subtitleText.color = new Color(color.r, color.g, color.b, color.a);
    }
    
    public void SetBackgroundColor(Color color)
    {
        backgroundColor.color = new Color(color.r, color.g, color.b, color.a);
    }
    
    public void SetFontSize(int size)
    {
        subtitleText.fontSize = size;
    }

    public void ResetSubtitlesSettings()
    {
        actorFontColor = null;
        if (loadedData != null)
        {
            subtitleText.color = new Color(loadedData.fontColor.r, loadedData.fontColor.g,
                loadedData.fontColor.b, loadedData.fontColor.a);
            backgroundColor.color = new Color(loadedData.backgroundColor.r, loadedData.backgroundColor.g,
                loadedData.backgroundColor.b, loadedData.backgroundColor.a);
            subtitleText.fontSize = loadedData.fontSize;
        }
    }
}
