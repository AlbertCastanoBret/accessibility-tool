using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

[System.Serializable]
public class ACC_VisualNotificationData: ACC_AbstractData
{
    public List<ACC_Sound> soundsList;
    public string message;
    public Color fontColor;
    public Color backgroundColor;
    public int fontSize;
    public string horizontalAlignment;
    public string verticalAlignment;
    public int timeOnScreen;

    public ACC_VisualNotificationData()
    {
        soundsList = new List<ACC_Sound>();
    }
}
