using System;
using System.Collections;
using System.Collections.Generic;
using ACC_API;
using UnityEngine;
using UnityEngine.Events;

public class SwitchLight : AbstractInteractable
{
    [SerializeField] List<GameObject> lights;
    [SerializeField] private bool isOn;

    private void Update()
    {
        if (GetComponent<OutlineScript>() != null)
        {
            if (isOver)
            {
                GetComponent<OutlineScript>().enabled = true;
            }
            else
            {
                GetComponent<OutlineScript>().enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        InputManager.OnInteraction += LeverAction;
    }

    private void OnDisable()
    {
        InputManager.OnInteraction -= LeverAction;
    }

    private void LeverAction()
    {
        if (isOver)
        {
            if (isOn)
            {
                ACC_AccessibilityManager.Instance.MultifunctionalAccessibility.PlaySound("SFX", "Off");
                GetComponent<Animator>().SetBool("IsOn", false);
                SetLightsOff();
                isOn = false;
            }
            else
            {
                ACC_AccessibilityManager.Instance.MultifunctionalAccessibility.PlaySound("SFX", "On");
                GetComponent<Animator>().SetBool("IsOn", true);
                SetLightsOn();
                isOn = true;
            }
        }
    }

    public void SetLightsOn()
    {
        
        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
    }

    public void SetLightsOff()
    {
        
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }
    }
}
