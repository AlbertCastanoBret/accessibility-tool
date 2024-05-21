using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableSlidingDoor : AbstractInteractable
{
    //Unity Events
    [SerializeField][HideInInspector] private UnityEvent<bool> onDoorMoving;
    [SerializeField][HideInInspector] private UnityEvent<Vector2> lastCameraPosition;
    
    //Private Variables
    private SlidingDoor door;
    private bool selectedDoorKnob;

    private void Start()
    {
        //Initialize the private variables
        door = GetComponent<SlidingDoor>();
        selectedDoorKnob = false;
        
        //Add listeners to the Unity Events
        onDoorMoving.AddListener(player.GetComponent<FPSController>().SetIsOpeningDoor);
        lastCameraPosition.AddListener(player.GetComponent<PlayerLook>().SetCameraTransform);
    }

    private void Update()
    {
        if (isOver)
        {
            //If the player is looking at the Door, show the canvas
            canvas.SetActive(true);
            canvas.transform.LookAt(player.transform);
        }
        else
        {
            //If the player is not looking at the Door, hide the canvas
            canvas.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        InputManager.OnStartMouseInteraction += ActuateDoor;
        InputManager.OnCancelMouseInteraction += StopActionDoor;
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
            //If the player is looking at the Door and the inventory is not open, start the Door action
            onDoorMoving.Invoke(true);
            door.CanMoveDoor = true;
            selectedDoorKnob = true;
        }
    }
    
    private void StopActionDoor()
    {
        if (door.CanMoveDoor)
        {
            //If the player can move the Door, stop the Door action
            Vector3 eulerAnglesParentObject = Camera.main.transform.parent.transform.eulerAngles;
            Vector2 lastPosition = new Vector2(eulerAnglesParentObject.y, -eulerAnglesParentObject.x);
            lastCameraPosition.Invoke(lastPosition);
            onDoorMoving.Invoke(false);
            door.CanMoveDoor = false;
            selectedDoorKnob = false;
            if (isOver) isOver = false;
        }
    }
}


