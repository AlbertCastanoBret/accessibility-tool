using System.Collections;
using System.Collections.Generic;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

public class ACC_AudioManagerData : ACC_AbstractData
{
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
        return MemberwiseClone();
    }
}
