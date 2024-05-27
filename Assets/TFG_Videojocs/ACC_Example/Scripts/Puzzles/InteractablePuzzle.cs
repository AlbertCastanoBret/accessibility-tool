using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class InteractablePuzzle : AbstractInteractable, I_InteractableInspectObject
{
    //Unity Events Serialized
    [SerializeField] private UnityEvent<InventoryItemData, int> onUsePuzzleObject;
    [SerializeField] private UnityEvent<GameObject> onInteractionPuzzleObject;
    
    //Unity Events
    [HideInInspector][SerializeField] private UnityEvent<bool> usableInventory;
    [HideInInspector][SerializeField] private UnityEvent<int, int> updateCounter;
    [HideInInspector][SerializeField] private UnityEvent<bool> onLookUp;
    [HideInInspector][SerializeField] private UnityEvent<bool, string, string> addDescription;
    
    //Private variables
    private I_InteractablePuzzleController puzzleController;
    private GameObject focusedObject;
    private Transform mainCamera;
    private HUD hud;
    private PlayerLook playerLook;
    private int itemPosition;
    public bool IsInspecting { get; set; }

    private void Start()
    {
        //Get the references from the scene
        playerLook = player.GetComponent<PlayerLook>();
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUD>();
        puzzleController = transform.parent.GetComponent<I_InteractablePuzzleController>();
        mainCamera = Camera.main.transform;
        
        //Initialize the variables
        IsInspecting = false;
        itemPosition = inventorySystem.GetItemPosition();
        
        //Add the listeners to the events
        onLookUp.AddListener(player.GetComponent<FPSController>().SetIsInspecting);
        usableInventory.AddListener(hud.ChangeStateUseInventory);
        updateCounter.AddListener(hud.UpdateItemCounterPuzzle);
        addDescription.AddListener(hud.AddDescriptionPuzzle);
    }
    
    private void Update()
    {
        if (canvas != null)
        {
            if(isOver && !IsInspecting)
            {
                //If the object is over and the player is not inspecting, show the canvas
                canvas.SetActive(true);
                canvas.transform.LookAt(playerLook.transform, new Vector3(0,180,0));
            }
            else if (IsInspecting && canvas.activeSelf || !IsInspecting && !isOver)
            {
                //If the player is inspecting the object, hide the canvas
                canvas.SetActive(false);
            }
        }

        if (GetComponent<OutlineScript>() != null)
        {
            if(isOver && !IsInspecting)
            {
                GetComponent<OutlineScript>().enabled = true;
            }
            else if (IsInspecting && GetComponent<OutlineScript>().enabled || !IsInspecting && !isOver)
            {
                GetComponent<OutlineScript>().enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        InputManager.OnInteraction += DoPuzzleAction;
        InputManager.OnUse += Use;
        InputManager.OnMouseInteraction += RotateObject;
        InputManager.OnLeaveInteraction += StopInspect;
        InputManager.OnArrowRight += MoveRight;
        InputManager.OnArrowLeft += MoveLeft;
    }

    private void OnDisable()
    {
        InputManager.OnInteraction -= DoPuzzleAction;
        InputManager.OnUse -= Use;
        InputManager.OnMouseInteraction -= RotateObject;
        InputManager.OnLeaveInteraction -= StopInspect;
        InputManager.OnArrowRight -= MoveRight;
        InputManager.OnArrowLeft -= MoveLeft;
    }

    private void DoPuzzleAction()
    {
        if (isOver && !IsInspecting && !inventorySystem.IsInventoryOpen())
        {
            if (puzzleController.CanOpenInventory(gameObject))
            {
                usableInventory.Invoke(true);
                itemPosition = inventorySystem.GetItemPosition();
                Inspect();
            }
            else onInteractionPuzzleObject.Invoke(gameObject);
        }
    }
    
    private void Use()
    {
        if (IsInspecting)
        {
            onUsePuzzleObject.Invoke(focusedObject.GetComponent<InteractableItemObject>().GetReferenceItem(), transform.GetSiblingIndex());
            itemPosition--;
            if (itemPosition < 0) itemPosition = 0;
            StopInspect();
        }
    }

    public void Inspect()
    {
        List<InventoryItem> nonReadableItems = inventorySystem.GetNonReadableItems();
        if (nonReadableItems.Count != 0)
        {
            if(canvas != null)canvas.SetActive(false);
            addDescription.Invoke(true, nonReadableItems[itemPosition].GetData().name, nonReadableItems[itemPosition].GetData().description);
            updateCounter.Invoke(itemPosition + 1, nonReadableItems.Count);
            onLookUp.Invoke(true);
            focusedObject = Instantiate(nonReadableItems[itemPosition].GetData().prefab, mainCamera.position + mainCamera.forward * 
                nonReadableItems[itemPosition].GetData().prefab.GetComponent<InteractableItemObject>().GetDistance(), Quaternion.identity);
            focusedObject.layer = 9;
            foreach (Transform child in focusedObject.transform)
            {
                child.gameObject.layer = 9;
            }
            focusedObject.GetComponent<InteractableItemObject>().enabled = false;
            focusedObject.GetComponent<Collider>().enabled = false;
            focusedObject.transform.LookAt(mainCamera.transform);
            focusedObject.transform.Rotate(focusedObject.GetComponent<InteractableItemObject>().GetRotation());

            if (focusedObject.transform.childCount > 0)
            {
               //focusedObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false); 
            }
            
            IsInspecting = true;
        }
    }
    
    public void StopInspect()
    {
        if (IsInspecting)
        {
            if(canvas != null)canvas.SetActive(true);
            usableInventory.Invoke(false);
            onLookUp.Invoke(false);
            Destroy(focusedObject);
            IsInspecting = false;
        }
    }
    
    private void RotateObject(Vector2 value)
    {
        if (IsInspecting && !focusedObject.GetComponent<InteractableItemObject>().IsAxisLocked())
        {
            float x = value.x * -0.15f;
            float y = value.y * 0.15f;
            
            Vector3 focusPosition = focusedObject.transform.position;
            
            focusedObject.transform.RotateAround(focusPosition, mainCamera.up, x);
            focusedObject.transform.RotateAround(focusPosition, mainCamera.right, y);
        }
        else if (IsInspecting && focusedObject.GetComponent<InteractableItemObject>().IsAxisLocked())
        {
            //If the player is inspecting the object and the object is readable, rotate the object in the x axis
            float x = value.x * -0.15f;

            focusedObject.transform.RotateAround(focusedObject.transform.position, mainCamera.transform.up, x);
        }
    }
    
    private void MoveRight()
    {
        if(IsInspecting && inventorySystem.GetNonReadableItems().Count - 1 > itemPosition)
        {
            itemPosition++;
            inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()+1);
            Destroy(focusedObject);
            //inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
            Inspect();
        } 
    }

    private void MoveLeft()
    {
        if(IsInspecting && itemPosition > 0)
        {
            itemPosition--;
            inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
            Destroy(focusedObject);
            //inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
            Inspect();
        }
    }
}
