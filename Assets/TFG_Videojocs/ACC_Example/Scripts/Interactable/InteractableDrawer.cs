using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDrawer : AbstractInteractable
{
    //Unity Events
    [SerializeField][HideInInspector] private UnityEvent<bool> onDrawerMoving;
    [SerializeField][HideInInspector] private UnityEvent<Vector2> lastCameraPosition;
    
    //Private Variables
    private Drawer drawer;
    private bool selectedDrawerKnob;

    private void Start()
    {
        //Initialize the private variables
        drawer = GetComponent<Drawer>();
        selectedDrawerKnob = false;
        
        //Add listeners to the Unity Events
        onDrawerMoving.AddListener(player.GetComponent<FPSController>().SetIsOpeningDoor);
        lastCameraPosition.AddListener(player.GetComponent<PlayerLook>().SetCameraTransform);
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
            if(isOver)
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
        InputManager.OnStartMouseInteraction += ActuateDrawer;
        InputManager.OnCancelMouseInteraction += StopActionDrawer;
    }

    private void OnDisable()
    {
        InputManager.OnStartMouseInteraction -= ActuateDrawer;
        InputManager.OnCancelMouseInteraction += StopActionDrawer;
    }

    private void ActuateDrawer()
    {
        if (isOver && !inventorySystem.IsInventoryOpen())
        {
            //If the player is looking at the drawer and the inventory is not open, start the drawer action
            onDrawerMoving.Invoke(true);
            drawer.CanMoveDrawer = true;
            selectedDrawerKnob = true;
        }
    }
    
    private void StopActionDrawer()
    {
        if (drawer.CanMoveDrawer)
        {
            //If the player can move the drawer, stop the drawer action
            Vector3 eulerAnglesParentObject = Camera.main.transform.parent.transform.eulerAngles;
            Vector2 lastPosition = new Vector2(eulerAnglesParentObject.y, -eulerAnglesParentObject.x);
            lastCameraPosition.Invoke(lastPosition);
            onDrawerMoving.Invoke(false);
            drawer.CanMoveDrawer = false;
            selectedDrawerKnob = false;
            if (isOver) isOver = false;
        }
    }
}


