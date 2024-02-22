using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs;
using UnityEngine;
using UnityEngine.UI;

public class ACC_AudioAccessibility
{
    public ACC_AudioAccessibility()
    {
        
    }

    public void CreateSubtitle()
    {
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();

        GameObject subtitleManager = new GameObject("SubtitleText");
        subtitleManager.transform.SetParent(canvasObject.transform, false);
        
        Text subtitleText = subtitleManager.AddComponent<Text>();
        ACC_SubtitlesManager accSubtitleManager = subtitleManager.AddComponent<ACC_SubtitlesManager>();
        accSubtitleManager.LoadSubtitles("JSONSubtitles");
        accSubtitleManager.EnableSubtitles();
        accSubtitleManager.subtitleText = subtitleText;
    }
    
    public void CreateSubtitle(GameObject canvas)
    {
        
    }
}
