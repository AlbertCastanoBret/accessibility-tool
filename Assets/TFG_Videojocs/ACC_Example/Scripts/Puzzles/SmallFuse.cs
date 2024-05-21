using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallFuse : AbstractInteractable
{
    [SerializeField] private float idRow;
    private float value;
    [SerializeField] private FuseBoxController fuseBoxController;
    private AudioSource audioSource;
    void Start()
    {
        value = 0;
        audioSource = fuseBoxController.gameObject.GetComponent<AudioSource>();
    }
    
    private void OnEnable()
    {
        InputManager.OnInteraction += LeverAction;
    }

    private void OnDisable()
    {
        InputManager.OnInteraction -= LeverAction;
    }
    
    private void Update()
    {
        if (transform.parent.GetComponent<OutlineScript>() != null)
        {
            if(isOver)
            {
                transform.parent.GetComponent<OutlineScript>().enabled = true;
            }
            else
            {
                transform.parent.GetComponent<OutlineScript>().enabled = false;
            }
        }
    }

    public float GetValue()
    {
        return value;
    }
    
    public void SetValue(float newValue)
    {
        value = newValue;
    }

    private void LeverAction()
    {
        if (isOver && (!fuseBoxController.GetLeverOneEnabled() && idRow == 1f || !fuseBoxController.GetLeverTwoEnabled() && idRow == 2f))
        {
            if(value == 0)
            {
                value = 1f;
                GetComponent<Animator>().SetTrigger("TurnOn");
                audioSource.Play();
            }
            else
            {
                value = 0;
                GetComponent<Animator>().SetTrigger("TurnOff");
                audioSource.Play();
            }
        }
    }
}
