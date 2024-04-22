using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs.ACC_HighContrast
{
    public class ACC_HighContrastData:ACC_AbstractData
    {
        public List<string> highContrastSettings = new List<string>();

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override object Clone()
        {
            return "ACC_HighContrastData";
        }
    }
}