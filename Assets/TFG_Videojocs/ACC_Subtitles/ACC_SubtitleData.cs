using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

[System.Serializable]
public class ACC_SubtitleData: ACC_AbstractData
{
    public ACC_SerializableDictiornary<int ,string> subtitleText = new();
    public ACC_SerializableDictiornary<int ,int> timeText = new();
    public Color fontColor;
    public Color backgroundColor;
    public int fontSize;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (ACC_SubtitleData)obj;

        bool subtitleTextEqual = subtitleText.Items.SequenceEqual(other.subtitleText.Items);
        bool timeTextEqual = timeText.Items.SequenceEqual(other.timeText.Items);

        return subtitleTextEqual
               && timeTextEqual
               && fontColor.Equals(other.fontColor)
               && backgroundColor.Equals(other.backgroundColor)
               && fontSize == other.fontSize;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ subtitleText.GetHashCode();
            hash = (hash * 16777619) ^ timeText.GetHashCode();
            hash = (hash * 16777619) ^ fontColor.GetHashCode();
            hash = (hash * 16777619) ^ backgroundColor.GetHashCode();
            hash = (hash * 16777619) ^ fontSize.GetHashCode();
            return hash;
        }
    }
}
