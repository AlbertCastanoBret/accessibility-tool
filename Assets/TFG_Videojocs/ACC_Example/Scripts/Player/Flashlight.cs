using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flashlight : MonoBehaviour
{
    [HideInInspector][SerializeField] private UnityEvent<bool> hasBatteryEvent;
    [SerializeField] private List<GameObject> ticks;

    private List<GameObject> batteries;
    private bool hasBattery;
    private GameObject flashlight;
    
    private float startingTime;
    private float flashlightTime = 400000000f;
    private float remainingTime;
    private bool canUse;
    
    public Coroutine FlashlightCoroutine;
    void Awake()
    {
        hasBattery = true;
        batteries = new List<GameObject>();
        remainingTime = 0;
        
        flashlight = GameObject.FindGameObjectWithTag("Flashlight").gameObject;
        //hasBatteryEvent.AddListener(GetComponent<FPSController>().SetFlashlightBattery);

        startingTime = Time.time;
        StartCoroutineWithParameter(startingTime);
    }
    
    private void OnEnable()
    {
        InputManager.OnFlashlight += ReloadBattery;
    }
    
    private void OnDisable()
    {
        InputManager.OnFlashlight -= ReloadBattery;
    }
    public void StartCoroutineWithParameter(float time)
    {
        startingTime = time;
        FlashlightCoroutine = StartCoroutine(FlashlightTime());
    }
    IEnumerator FlashlightTime()
    {
        if (remainingTime == 0)
        {
            yield return new WaitForSeconds(flashlightTime/4);
            ticks[0].SetActive(false);
            yield return new WaitForSeconds(flashlightTime/4);
            ticks[1].SetActive(false);
            yield return new WaitForSeconds(flashlightTime/4);
            ticks[2].SetActive(false);
            yield return new WaitForSeconds(flashlightTime/4);
            ticks[3].SetActive(false);
        }
        else
        {
            List<GameObject> activeTicks = GetActiveTicks();
            for (int i = 0; i < activeTicks.Count; i++)
            {
                yield return new WaitForSeconds((flashlightTime/4) - (((flashlightTime/4)*activeTicks.Count-i) - remainingTime));
                SetRemainingTime();
                activeTicks[i].SetActive(false);
            }
        }
        
        hasBattery = false;
        hasBatteryEvent.Invoke(false);
        flashlight.GetComponent<Light>().enabled = false;
        remainingTime = 0;
    }

    private List<GameObject> GetActiveTicks()
    {
        List<GameObject> activeTicks = new List<GameObject>();
        
        for (int i = 0; i < ticks.Count; i++)
        {
            if (ticks[i].activeSelf)
            {
                activeTicks.Add(ticks[i]);     
                //print(ticks[i].name);
            }    
        }
        return activeTicks;
    }

    public void SetRemainingTime()
    {
        if (remainingTime == 0)
        {
            remainingTime = flashlightTime - (Time.time - startingTime);
        }
        else
        {
            remainingTime -= (Time.time - startingTime);
        }
        //print(remainingTime);
        if(remainingTime < 0) remainingTime = 0;
    }

    private void ReloadBattery()
    {
        if (!hasBattery && batteries.Count > 0)
        {
            hasBattery = true;
            hasBatteryEvent.Invoke(true);
            flashlight.GetComponent<Light>().enabled = true;
            batteries.RemoveAt(batteries.Count - 1);
            
            //Reset the timer
            startingTime = Time.time;
            FlashlightCoroutine = StartCoroutine(FlashlightTime());
        }
    }

    public void AddBattery(GameObject battery)
    {
        batteries.Add(battery);
    }
}
