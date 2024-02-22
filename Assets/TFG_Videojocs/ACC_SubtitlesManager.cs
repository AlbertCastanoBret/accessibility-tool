using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ACC_SubtitlesManager : MonoBehaviour
{
    public Text subtitleText;
    public List<ACC_KeyValuePairData<int, string>> subtitleDictionary = new List<ACC_KeyValuePairData<int, string>>();
    public List<ACC_KeyValuePairData<int, int>> timeDictionary = new List<ACC_KeyValuePairData<int, int>>();
    [SerializeField] private bool subtitlesEnabled = false;
    
    private int currentIndex = 0;
    private float startTime;
    private float nextSubtitleTime;

    void Update()
    {
        if (subtitlesEnabled)
        {
            float currentTime = Time.time;
            if (currentTime >= nextSubtitleTime)
            {
                if (currentIndex < subtitleDictionary.Count)
                {
                    print("A");
                    subtitleText.text = subtitleDictionary[currentIndex].value;
                    startTime = currentTime;
                    nextSubtitleTime = startTime + timeDictionary[currentIndex].value;
                }
                else if (currentIndex >= subtitleDictionary.Count)
                {
                    subtitleText.text = "";
                }
                currentIndex++;
            }
        }
    }

    public void LoadSubtitles(string jsonFile)
    {
        subtitleDictionary = new List<ACC_KeyValuePairData<int, string>>();
        timeDictionary = new List<ACC_KeyValuePairData<int, int>>();
        string json = File.ReadAllText("Assets/TFG_Videojocs/" + jsonFile);
        ACC_SubtitleData subtitleData = JsonUtility.FromJson<ACC_SubtitleData>(json);
        for (int i = 0; i < subtitleData.subtitleText.Count; i++)
        {
            int time = subtitleData.timeText[i].value;
            string subtitle = subtitleData.subtitleText[i].value;
            subtitleDictionary.Add(new ACC_KeyValuePairData<int, string>(i, subtitle));
            timeDictionary.Add(new ACC_KeyValuePairData<int, int>(i, time));
        }
    }

    public void EnableSubtitles()
    {
        subtitlesEnabled = true;
    }

    public void DisableSubtitles()
    {
        subtitlesEnabled = false;
        subtitleText.text = "";
    }
}
