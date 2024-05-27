using System;
using System.Collections;
using System.Collections.Generic;
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
                //audioSource.PlayOneShot(on);
                GetComponent<Animator>().SetBool("IsOn", true);
                isOn = false;
                SetLightsOff();
            }
            else
            {
                //audioSource.PlayOneShot(off);
                GetComponent<Animator>().SetBool("IsOn", false);
                isOn = true;
                SetLightsOn();
            }
        }
    }

    public void SetLightsOn()
    {

        isOn = true;
        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
    }

    public void SetLightsOff()
    {

        isOn = false;
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }
    }
}
