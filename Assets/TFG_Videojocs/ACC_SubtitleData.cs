using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ACC_SubtitleData
{
    public List<ACC_KeyValuePairData<int, string>> subtitleText;
    public List<ACC_KeyValuePairData<int, int>> timeText;
    public Color fontColor;
    public Color backgroundColor;
    public int fontSize;

    public ACC_SubtitleData()
    {
        subtitleText = new List<ACC_KeyValuePairData<int, string>>();
        timeText = new List<ACC_KeyValuePairData<int, int>>();
    }
}
