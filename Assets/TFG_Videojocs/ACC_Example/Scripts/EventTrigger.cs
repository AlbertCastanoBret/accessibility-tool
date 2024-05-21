using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventTrigger : GameEvent
{
    [SerializeField] private bool canBeRepeated;
    [SerializeField] private UnityEvent onTriggerEnter;

    private void OnEnable()
    {
        hasFinished = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasFinished == false && other.gameObject.CompareTag("Player"))
        {
            onTriggerEnter.Invoke();
            if(!canBeRepeated) hasFinished = true;
        }
    }
}
