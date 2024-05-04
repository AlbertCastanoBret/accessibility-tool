using System;

namespace TFG_Videojocs.ACC_Subtitles
{
    [System.Serializable]
    public class ACC_SubtitleRowData: IEquatable<ACC_SubtitleRowData>, ICloneable
    {
        public string actor;
        public string subtitle;
        public int time;
        
        public bool Equals(ACC_SubtitleRowData other)
        {
            if (other == null)
                return false;
            return actor == other.actor && subtitle == other.subtitle && time == other.time;
        }

        public object Clone()
        {
            return new ACC_SubtitleRowData
            {
                actor = actor,
                subtitle = subtitle,
                time = time
            };
        }
    }
}