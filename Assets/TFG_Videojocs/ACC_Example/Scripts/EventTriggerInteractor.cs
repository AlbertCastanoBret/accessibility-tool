using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerInteractor : GameEvent
{
    [SerializeField] private bool canBeRepeated;
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

    private void OnEnable()
    {
        hasFinished = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasFinished == false && other.gameObject.CompareTag("Interactor"))
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasFinished == false && other.gameObject.CompareTag("Interactor"))
        {
            onTriggerExit.Invoke();
            if(!canBeRepeated) hasFinished = true;
        }
    }
}
