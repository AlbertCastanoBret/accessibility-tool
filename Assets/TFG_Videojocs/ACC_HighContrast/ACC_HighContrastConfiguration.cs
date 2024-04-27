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
        public Color outlineColor;
        public float outlineThickness;
        
        public ACC_HighContrastConfiguration()
        {
            name = "New High Contrast Configuration";
            tag = "Untagged";
            color = Color.white;
            outlineColor = Color.white;
            outlineThickness = 0.6f;
        }
        
        public bool Equals(ACC_HighContrastConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return name == other.name && string.Equals(tag, other.tag) && color.Equals(other.color) && outlineColor.Equals(other.outlineColor) && outlineThickness.Equals(other.outlineThickness);
        }

        public object Clone()
        {
            return new ACC_HighContrastConfiguration
            {
                name = name,
                tag = tag,
                color = color,
                outlineColor = outlineColor,
                outlineThickness = outlineThickness
            };
        }
    }
}