using System.Collections.Generic;
using UnityEngine;

namespace TFG_Videojocs.ACC_Utilities
{
    public enum ColorEnum
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        Orange,
        White
    }
    
    public class ACC_ColorManager
    {
        private static readonly Dictionary<ColorEnum, string> colorHexValues = new Dictionary<ColorEnum, string>
        {
            { ColorEnum.Red, "#FF0000" },
            { ColorEnum.Green, "#00FF00" },
            { ColorEnum.Blue, "#0000FF" },
            { ColorEnum.Yellow, "#FFFF00" },
            { ColorEnum.Purple, "#800080" },
            { ColorEnum.Orange, "#FFFF5E" },
            { ColorEnum.White, "#FFFFFF" }
        };
        
        public static Color ConvertHexToColor(ColorEnum colorEnum)
        {
            string hexValue = colorHexValues[colorEnum];
            
            if (ColorUtility.TryParseHtmlString(hexValue, out Color color))
            {
                return color;
            }
            Debug.LogError("El valor hexadecimal no es válido: " + hexValue);
            return Color.white;
        }
        
        public static string GetColorName(Color color)
        {
            foreach (var entry in colorHexValues)
            {
                if (ColorUtility.TryParseHtmlString(entry.Value, out Color dictColor) && dictColor == color)
                {
                    return entry.Key.ToString();
                }
            }
            
            return "Unknown";
        }
        
        public static Color ConvertTextToColor(string colorName)
        {
            if (System.Enum.TryParse(colorName, true, out ColorEnum colorEnum))
            {
                return ConvertHexToColor(colorEnum);
            }
            Debug.LogWarning($"Nombre de color no encontrado: {colorName}");
            return Color.white;
        }
    }
}