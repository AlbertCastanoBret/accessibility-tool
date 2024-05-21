using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathtubController : AbstractPuzzleController, I_InteractablePuzzleController
{
    [SerializeField] private Animation animation;
    [SerializeField] private GameObject drain;
    [SerializeField] private Material material;
    [SerializeField] private GameObject itemToCollect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    public bool IsOnTransition { get; set; }
    
    private void Start()
    {
        material.SetFloat("_Displacement", 0.06f);
        itemToCollect.GetComponent<BoxCollider>().enabled = false;
    }

    public bool CanOpenInventory(GameObject gameObject)
    {
        return false;
    }

    public void RemoveStopper()
    {
        animation.Play();
        audioSource.PlayOneShot(audioClip);
        StartCoroutine(ChangeStopperPropertySmoothly(0.15f, 1.5f, 0.06f, animation.clip.length-2f));
        Destroy(drain.transform.Find("Canvas")?.gameObject);
        //Destroy(drain.GetComponent<InteractablePuzzle>());
        drain.GetComponent<InteractablePuzzle>().enabled = false;
        drain.GetComponent<OutlineScript>().enabled = false;
    }

    private IEnumerator ChangeStopperPropertySmoothly(float targetValue, float firstDuration, float finalValue, float finalDuration)
    {
        float startTime = Time.time;
        float startValue = material.GetFloat("_Displacement");
        float elapsedTime = 0f;
        
        while (elapsedTime < firstDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = elapsedTime / firstDuration;
            float newValue = Mathf.Lerp(startValue, targetValue, t);
            material.SetFloat("_Displacement", newValue);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        
        startTime = Time.time;
        startValue = targetValue;
        elapsedTime = 0f;

        while (elapsedTime < finalDuration)
        {
            double tolerance = 1f;
            if (Math.Abs(elapsedTime - finalDuration / 1.1f) < tolerance)
            {
                if (itemToCollect != null)
                {
                    itemToCollect.GetComponent<BoxCollider>().enabled = true;
                }
            }
            elapsedTime = Time.time - startTime;
            float t = elapsedTime / finalDuration;
            float newValue = Mathf.Lerp(startValue, finalValue, t);
            material.SetFloat("_Displacement", newValue);
            yield return null;
        }
        
        material.SetFloat("_Displacement", finalValue);
    }

    public void RemoveObject(GameObject gameObject)
    {
        
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        
    }
}
