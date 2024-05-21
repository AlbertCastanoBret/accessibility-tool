using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class GameEvent : MonoBehaviour
{
    public bool hasFinished;

    private void Start()
    {
        hasFinished = false;
    }
}
