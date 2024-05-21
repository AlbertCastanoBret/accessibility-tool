using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData 
{
    public float[] position;
    public float[] rotation;

    public GameData()
    {
        position = new float[3]{0, 0, 0};
        rotation = new float[2]{0, 0};
    }
}
