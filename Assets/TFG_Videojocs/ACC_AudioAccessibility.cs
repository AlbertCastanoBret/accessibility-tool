using System;
using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEngine;
using UnityEngine.UI;

public class ACC_AudioAccessibility
{
    private GameObject subtitleManager;
    private GameObject backgroundColor;
    public ACC_AudioAccessibility()
    {
        subtitleManager = GameObject.Find("SubtitleText");
        backgroundColor = GameObject.Find("BackgroundColor");
    }

    public void ChangeSubtitleFontSize(int newFontSize)
    {
        subtitleManager.GetComponent<Text>().fontSize = newFontSize;
    }

    public void ChangeSubtitleFontColor(Color newColor)
    {
        subtitleManager.GetComponent<Text>().color = new Color(newColor.r, newColor.g, newColor.b);
    }

    public void ChangeSubtitleBackgroundColor(Color newColor)
    {
        backgroundColor.GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b);
    }
    
    public void CreateSubtitle(GameObject canvas)
    {
        
    }

    public void EnableSubtitles(bool enabled)
    {
        subtitleManager.GetComponent<ACC_SubtitlesManager>().enabled = enabled;
    }
}
