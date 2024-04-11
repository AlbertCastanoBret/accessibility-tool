using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (ACC_VisualNotificationData)obj;

        bool soundsEqual = soundsList.SequenceEqual(other.soundsList);

        return soundsEqual
               && message.Equals(other.message)
               && fontColor.Equals(other.fontColor)
               && backgroundColor.Equals(other.backgroundColor)
               && fontSize == other.fontSize
               && horizontalAlignment.Equals(other.horizontalAlignment)
               && verticalAlignment.Equals(other.verticalAlignment)
               && timeOnScreen == other.timeOnScreen;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ soundsList.GetHashCode();
            hash = (hash * 16777619) ^ message.GetHashCode();
            hash = (hash * 16777619) ^ fontColor.GetHashCode();
            hash = (hash * 16777619) ^ backgroundColor.GetHashCode();
            hash = (hash * 16777619) ^ fontSize.GetHashCode();
            hash = (hash * 16777619) ^ horizontalAlignment.GetHashCode();
            hash = (hash * 16777619) ^ verticalAlignment.GetHashCode();
            hash = (hash * 16777619) ^ timeOnScreen.GetHashCode();
            return hash;
        }
    }
}
