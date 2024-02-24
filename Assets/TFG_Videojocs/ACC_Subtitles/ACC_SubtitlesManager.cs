using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ACC_SubtitlesManager : MonoBehaviour
{
    [HideInInspector] private TextMeshProUGUI subtitleText { get; set; }
    [HideInInspector] private Image backgroundColor { get; set; }
    private List<ACC_KeyValuePairData<int, string>> subtitleDictionary = new List<ACC_KeyValuePairData<int, string>>();
    private List<ACC_KeyValuePairData<int, int>> timeDictionary = new List<ACC_KeyValuePairData<int, int>>();
    
    private bool canPlaySubtitle = false;
    private int currentIndex = 0;
    private float startTime;
    private float nextSubtitleTime;

    private void Awake()
    {
        subtitleText = GameObject.Find("ACC_SubtitleManager/ACC_SubtitleText").GetComponent<TextMeshProUGUI>();
        backgroundColor = GameObject.Find("ACC_SubtitleManager/ACC_Background").GetComponent<Image>();
    }

    void Update()
    {
        if (canPlaySubtitle)
        {
            float currentTime = Time.time;
            if (currentTime >= nextSubtitleTime)
            {
                if (currentIndex < subtitleDictionary.Count)
                {
                    subtitleText.text = subtitleDictionary[currentIndex].value;
                    startTime = currentTime;
                    nextSubtitleTime = startTime + timeDictionary[currentIndex].value;
                    GetComponent<RectTransform>().sizeDelta =
                        new Vector2(0, subtitleText.preferredHeight);
                    subtitleText.GetComponent<RectTransform>().sizeDelta = 
                        new Vector2(0, subtitleText.preferredHeight);
                    backgroundColor.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(0, subtitleText.preferredHeight);
                }
                else if (currentIndex >= subtitleDictionary.Count)
                {
                    currentIndex = -1;
                    canPlaySubtitle = false;
                    subtitleText.text = "";
                    backgroundColor.color = new Color(0, 0, 0, 0);
                }
                currentIndex++;
            }
        }
    }

    public void LoadSubtitles(string jsonFile)
    {
        subtitleDictionary = new List<ACC_KeyValuePairData<int, string>>();
        timeDictionary = new List<ACC_KeyValuePairData<int, int>>();
        string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSONSubtitle/" + jsonFile + ".json");
        ACC_SubtitleData subtitleData = JsonUtility.FromJson<ACC_SubtitleData>(json);
        for (int i = 0; i < subtitleData.subtitleText.Count; i++)
        {
            int time = subtitleData.timeText[i].value;
            string subtitle = subtitleData.subtitleText[i].value;
            subtitleDictionary.Add(new ACC_KeyValuePairData<int, string>(i, subtitle));
            timeDictionary.Add(new ACC_KeyValuePairData<int, int>(i, time));
        }

        subtitleText.color = new Color(subtitleData.fontColor.r, subtitleData.fontColor.g,
            subtitleData.fontColor.b, subtitleData.fontColor.a);
        backgroundColor.color = new Color(subtitleData.backgroundColor.r, subtitleData.backgroundColor.g,
            subtitleData.backgroundColor.b, subtitleData.backgroundColor.a);
        subtitleText.fontSize = subtitleData.fontSize;
    }

    public void PlaySubtitle()
    {
        canPlaySubtitle = true;
        subtitleText.text = "";
        currentIndex = 0;
    }

    /*public void EndSubtitle()
    {
        canPlaySubtitle = false;
        subtitleText.text = "";
        currentIndex = 0;
    }*/
}
