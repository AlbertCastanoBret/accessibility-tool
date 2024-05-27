using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool frontKnob;
    private float torqueMultiplier;
    private bool isOpen;
    private bool canMoveDoor;
    private float angle;
    private Camera mainCamera;
    [Header("Configuració de l'empenta de la porta")]
    [SerializeField] private DoorOpenDirection doorOpenDirection;
    [SerializeField] private AudioClip[] doorOpenSounds = new AudioClip[0];
    [Space(15)]
    [Header("Configuració de la freqüència d'emissió dels sons del portell")]
    [SerializeField]
    private float frequenciaSons = 2.5f;
    [Space(15)]
    [Header("Configuració de la freqüència d'emissió dels sons del portell")]
    [SerializeField]
    private AudioClip doorCloseSound;
    private Vector3 actualRotation;
    private Vector3 startingRotation;
    private Vector3 lastRotation;
    private Rigidbody parent;
    [SerializeField]
    private AudioSource audioSource;
    private bool hasPlayedSound;
    [SerializeField]
    private AudioSource openCloseSound;
    private void Start()
    {
        parent = transform.parent.GetComponent<Rigidbody>();
        //Initialize variables
        canMoveDoor = false;
        isOpen = false;
        mainCamera = Camera.main;
        torqueMultiplier = 3.7f;
        lastRotation = parent.transform.eulerAngles;
        startingRotation = parent.transform.eulerAngles;
        hasPlayedSound = false;
        //iterate for each file of the path folder
    }

    private void OnEnable()
    {
        InputManager.OnMouseInteraction += ActuateDoor;
    }

    private void OnDisable()
    {
        InputManager.OnMouseInteraction -= ActuateDoor;
    }

    private void ActuateDoor(Vector2 value)
    {
        if (!canMoveDoor) return;

        //Smoothly rotate the camera to look at the door knob
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCamera.transform.parent.transform.position);
        mainCamera.transform.parent.transform.rotation = Quaternion.Slerp(mainCamera.transform.parent.transform.rotation, targetRotation, 10f * Time.deltaTime);

        //Get door rigidbody


        //Determine the direction of the torque and apply it
        float torqueDirection = DetermineTorqueDirection(value.x);

        //Move the door
        if (doorOpenDirection == DoorOpenDirection.Forward)
            parent.AddTorque(parent.transform.forward * torqueDirection);
        else if (doorOpenDirection == DoorOpenDirection.Up)
            parent.AddTorque(parent.transform.up * torqueDirection);
        else if (doorOpenDirection == DoorOpenDirection.Right)
            parent.AddTorque(parent.transform.right * torqueDirection);

        actualRotation = parent.transform.eulerAngles;

        if (Mathf.Abs(actualRotation.y - lastRotation.y) >= (frequenciaSons + UnityEngine.Random.Range(0, 2.5f)))
        {
            lastRotation = actualRotation;
            if (audioSource != null)
            {
                audioSource.PlayOneShot(doorOpenSounds[UnityEngine.Random.Range(0, doorOpenSounds.Length)]);
            }

        }

        //check if actualrotation.y is similar to startingrotation.y
        if ((Mathf.Abs(actualRotation.y - startingRotation.y) <= 0.5f) && hasPlayedSound)
        {
            hasPlayedSound = false;
            if (openCloseSound != null)
            {
                openCloseSound.PlayOneShot(doorCloseSound);
            }

        }
        else if ((Mathf.Abs(actualRotation.y - startingRotation.y) > 0.5f) && !hasPlayedSound)
        {
            hasPlayedSound = true;
            if (openCloseSound != null)
            {
                openCloseSound.PlayOneShot(doorCloseSound);
            }
        }


    }

    private float DetermineTorqueDirection(float inputValue)
    {
        //If the input value is 0, return 0
        if (Mathf.Approximately(inputValue, 0f))
        {
            return 0f;
        }

        if (frontKnob)
        {
            if (angle > 125)
            {
                return inputValue < 0 ? torqueMultiplier : -torqueMultiplier;
            }
            else
            {
                return inputValue < 0 ? -torqueMultiplier : torqueMultiplier;
            }
        }
        else
        {
            if (angle > 55)
            {
                return inputValue < 0 ? torqueMultiplier : -torqueMultiplier;
            }
            else
            {
                return inputValue < 0 ? -torqueMultiplier : torqueMultiplier;
            }
        }
    }

    public bool CanMoveDoor()
    {
        return canMoveDoor;
    }

    public void SetCanMoveDoor(bool canMoveDoor)
    {
        this.canMoveDoor = canMoveDoor;
    }

    public void SetAngle(float angle)
    {
        this.angle = angle;
    }

    public void SetKnob(bool frontKnob)
    {
        this.frontKnob = frontKnob;
    }
}

enum DoorOpenDirection
{
    Forward,
    Up,
    Right
}