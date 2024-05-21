using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerExit : GameEvent
{
    [SerializeField] private bool canBeRepeated;
    [SerializeField] private UnityEvent onTriggerExit;
    [SerializeField] private UnityEvent onTriggerEnter;

    private void OnEnable()
    {
        hasFinished = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasFinished == false && other.gameObject.CompareTag("Player"))
        {
            onTriggerExit.Invoke();
            if(!canBeRepeated) hasFinished = true;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (hasFinished == false && other.gameObject.CompareTag("Player"))
        {
            onTriggerEnter.Invoke();
            if(!canBeRepeated) hasFinished = true;
        }
    }
}
