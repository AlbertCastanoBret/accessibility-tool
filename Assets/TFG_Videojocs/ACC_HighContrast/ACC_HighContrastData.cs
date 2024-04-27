using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Utilities;

namespace TFG_Videojocs.ACC_HighContrast
{
    [System.Serializable]
    public class ACC_HighContrastData:ACC_AbstractData
    {
        public ACC_SerializableDictiornary<int, ACC_HighContrastConfiguration> highContrastConfigurations = new ();

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            
            ACC_HighContrastData other = (ACC_HighContrastData)obj;
            bool highContrastConfigurationsEqual = highContrastConfigurations.Items.SequenceEqual(other.highContrastConfigurations.Items);
            
            return highContrastConfigurationsEqual;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var item in highContrastConfigurations.Items)
                {
                    hash = hash * 23 + item.GetHashCode();
                }
                return hash;
            }
        }

        public override object Clone()
        {
            ACC_HighContrastData clone = new ACC_HighContrastData
            {
                name = name,
                highContrastConfigurations = (ACC_SerializableDictiornary<int, ACC_HighContrastConfiguration>)highContrastConfigurations.Clone()
            };
            return clone;
        }
    }
}