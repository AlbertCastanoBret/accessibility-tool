using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDoor : AbstractInteractable
{
    //Unity Events
    [SerializeField][HideInInspector] private UnityEvent<bool> onDoorMoving;
    [SerializeField][HideInInspector] private UnityEvent<Vector2> lastCameraPosition;
    [SerializeField][HideInInspector] private UnityEvent<bool> isOpeningDoor;

    //Private variables
    private Door door;
    private bool selectedDoorKnob;
    [SerializeField] private GameObject angleIndicator;

    private void Start()
    {
        //Initialize the private variables
        door = GetComponent<Door>();
        selectedDoorKnob = false;

        //Add listeners to the Unity Events
        onDoorMoving = new UnityEvent<bool>();
        lastCameraPosition = new UnityEvent<Vector2>();
        isOpeningDoor = new UnityEvent<bool>();
        onDoorMoving.AddListener(player.GetComponent<FPSController>().SetIsOpeningDoor);
        lastCameraPosition.AddListener(player.GetComponent<PlayerLook>().SetCameraTransform);
        isOpeningDoor.AddListener(GameObject.FindGameObjectWithTag("Interactor").GetComponent<Interactor>().IsInteracting);
    }

    private void Update()
    {
        if (canvas != null)
        {
            if (isOver)
            {
                //If the player is looking at the door, show the canvas
                canvas.SetActive(true);
                canvas.transform.LookAt(player.transform);
            }
            else
            {
                //If the player is not looking at the door, hide the canvas
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }

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
        InputManager.OnStartMouseInteraction += ActuateDoor;
        InputManager.OnCancelMouseInteraction += StopActionDoor;
        //Initialize the private variables
        door = GetComponent<Door>();
        selectedDoorKnob = false;

        //Add listeners to the Unity Events
        onDoorMoving = new UnityEvent<bool>();
        lastCameraPosition = new UnityEvent<Vector2>();
        onDoorMoving.AddListener(player.GetComponent<FPSController>().SetIsOpeningDoor);
        lastCameraPosition.AddListener(player.GetComponent<PlayerLook>().SetCameraTransform);
    }

    private void OnDisable()
    {
        InputManager.OnStartMouseInteraction -= ActuateDoor;
        InputManager.OnCancelMouseInteraction += StopActionDoor;
    }

    private void ActuateDoor()
    {
        if (isOver && !inventorySystem.IsInventoryOpen())
        {
            //If the player is looking at the door and the inventory is not open, start the door action
            float angle = Vector3.Angle(angleIndicator.transform.forward, player.transform.forward);
            door.SetAngle(angle);
            if (gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
            onDoorMoving.Invoke(true);
            door.SetCanMoveDoor(true);
            isOpeningDoor.Invoke(true);
            selectedDoorKnob = true;
        }
    }

    private void StopActionDoor()
    {
        if (door.CanMoveDoor())
        {
            //If the player can move the door, stop the door action
            Vector3 eulerAnglesParentObject = Camera.main.transform.parent.transform.eulerAngles;
            Vector2 lastPosition = new Vector2(eulerAnglesParentObject.y, -eulerAnglesParentObject.x);
            lastCameraPosition.Invoke(lastPosition);
            onDoorMoving.Invoke(false);
            door.SetCanMoveDoor(false);
            isOpeningDoor.Invoke(false);
            selectedDoorKnob = false;
            if (isOver) isOver = false;
        }
    }

    public bool GetSelectedDoorKnob()
    {
        return selectedDoorKnob;
    }

    public void SetAngleIndicator(GameObject angleIndicator)
    {
        if (this.angleIndicator == null) this.angleIndicator = angleIndicator;
    }
}


