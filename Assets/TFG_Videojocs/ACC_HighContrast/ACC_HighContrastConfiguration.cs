using System;
using UnityEngine;

namespace TFG_Videojocs.ACC_HighContrast
{
    [System.Serializable]
    public class ACC_HighContrastConfiguration: IEquatable<ACC_HighContrastConfiguration>, ICloneable
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
        
        public bool Equals(ACC_HighContrastConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return 
                name == other.name 
                && string.Equals(tag, other.tag) 
                && color.Equals(other.color) 
                && colorMinDistance.Equals(other.colorMinDistance)
                && colorMaxDistance.Equals(other.colorMaxDistance)
                && outlineColor.Equals(other.outlineColor) 
                && outlineThickness.Equals(other.outlineThickness)
                && outlineFadeDistance.Equals(other.outlineFadeDistance);
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