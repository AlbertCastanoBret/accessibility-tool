using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Subtitles;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

[System.Serializable]
public class ACC_SubtitleData: ACC_AbstractData
{
    public ACC_SerializableDictiornary<int, ACC_SubtitleRowData> subtitles = new();
    public ACC_SerializableDictiornary<int, ACC_ActorData> actors = new();
    public Color fontColor;
    public Color backgroundColor;
    public int fontSize;
    public bool showActors;
    public bool showActorsColors;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (ACC_SubtitleData)obj;
        
        bool subtitlesEqual = subtitles.Items.SequenceEqual(other.subtitles.Items);
        bool actorsEqual = actors.Items.SequenceEqual(other.actors.Items);
        
        return string.Equals(name,other.name, StringComparison.OrdinalIgnoreCase)
               && subtitlesEqual
               && actorsEqual
               && fontColor.Equals(other.fontColor)
               && backgroundColor.Equals(other.backgroundColor)
               && fontSize == other.fontSize
               && showActors == other.showActors
               && showActorsColors == other.showActorsColors;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ name.GetHashCode();
            hash = (hash * 16777619) ^ subtitles.GetHashCode();
            hash = (hash * 16777619) ^ actors.GetHashCode();
            hash = (hash * 16777619) ^ fontColor.GetHashCode();
            hash = (hash * 16777619) ^ backgroundColor.GetHashCode();
            hash = (hash * 16777619) ^ fontSize.GetHashCode();
            hash = (hash * 16777619) ^ showActors.GetHashCode();
            hash = (hash * 16777619) ^ showActorsColors.GetHashCode();
            return hash;
        }
    }

    public override object Clone()
    {
        ACC_SubtitleData clone = new ACC_SubtitleData
        {
            name = name,
            subtitles = (ACC_SerializableDictiornary<int, ACC_SubtitleRowData>) subtitles.Clone(),
            actors = (ACC_SerializableDictiornary<int, ACC_ActorData>) actors.Clone(),
            fontColor = fontColor,
            backgroundColor = backgroundColor,
            fontSize = fontSize,
            showActors = showActors,
            showActorsColors = showActorsColors
        };

        return clone;
    }
}
