using System;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

namespace TFG_Videojocs.ACC_HighContrast
{
    [System.Serializable]
    public class ACC_HighContrastConfiguration: ICloneable
    {
        public string name;
        public string tag;
        public Color color;
        public float colorMinDistance;
        public float colorMaxDistance;
        public Color outlineColor;
        public float outlineThickness;
        public float outlineFadeDistance;
        
        public ACC_HighContrastConfiguration()
        {
            name = "New High Contrast Configuration";
            tag = "Untagged";
            color = Color.white;
            colorMinDistance = 2f;
            colorMaxDistance = 6f;
            outlineColor = Color.white;
            outlineThickness = 0.6f;
            outlineFadeDistance = 5f;
        }
        
        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
                return false;

            ACC_HighContrastConfiguration otherConfiguration = (ACC_HighContrastConfiguration)other;
            return string.Equals(name, otherConfiguration.name, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(tag, otherConfiguration.tag, StringComparison.OrdinalIgnoreCase)
                   && color.Equals(otherConfiguration.color)
                   && colorMinDistance.Equals(otherConfiguration.colorMinDistance)
                   && colorMaxDistance.Equals(otherConfiguration.colorMaxDistance)
                   && outlineColor.Equals(otherConfiguration.outlineColor)
                   && outlineThickness.Equals(otherConfiguration.outlineThickness)
                   && outlineFadeDistance.Equals(otherConfiguration.outlineFadeDistance);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ name.GetHashCode();
                hash = (hash * 16777619) ^ tag.GetHashCode();
                hash = (hash * 16777619) ^ color.GetHashCode();
                hash = (hash * 16777619) ^ colorMinDistance.GetHashCode();
                hash = (hash * 16777619) ^ colorMaxDistance.GetHashCode();
                hash = (hash * 16777619) ^ outlineColor.GetHashCode();
                hash = (hash * 16777619) ^ outlineThickness.GetHashCode();
                hash = (hash * 16777619) ^ outlineFadeDistance.GetHashCode();
                return hash;
            }
        }

        public object Clone()
        {
            return new ACC_HighContrastConfiguration
            {
                name = name,
                tag = tag,
                color = color,
                colorMinDistance = colorMinDistance,
                colorMaxDistance = colorMaxDistance,
                outlineColor = outlineColor,
                outlineThickness = outlineThickness,
                outlineFadeDistance = outlineFadeDistance
            };
        }
    }
}