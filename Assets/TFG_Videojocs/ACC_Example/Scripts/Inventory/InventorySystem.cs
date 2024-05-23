using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;
using UnityEngine.LowLevel;
using UnityEngine.Serialization;

public class InventorySystem : MonoBehaviour
{
    //Unity Events
    [HideInInspector][SerializeField] private UnityEvent<bool> onLookUp;
    [HideInInspector][SerializeField] private UnityEvent<int, int> updateCounter;
    [HideInInspector][SerializeField] private UnityEvent<bool> isReadable;
    [HideInInspector][SerializeField] private UnityEvent<string> changeText;
    [HideInInspector][SerializeField] private UnityEvent showText;
    [HideInInspector][SerializeField] private UnityEvent<bool> isUsable;
    [HideInInspector][SerializeField] private UnityEvent<bool, string, string> addDescription;

    //Private Serialized Variables
    [SerializeField] private List<InventoryItem> startingObject;
    [SerializeField] private List<InventoryItem> inventory;
    
    //Private Variables
    private Dictionary<InventoryItemData, InventoryItem> itemDictionary;
    private GameObject player;
    private GameObject focusedObject;
    private Transform mainCamera;
    private bool isInventoryOpen;
    private bool canRead;
    private bool canUse;
    private bool isNoteBookOpen;
    private HUD hud;
    private Interactor interactor;
    private int itemPosition;

    private void Start()
    {
        //Get references to the scene objects
        player = GameObject.FindGameObjectWithTag("Player");
        //hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUD>();
        interactor = GameObject.FindGameObjectWithTag("Interactor").GetComponent<Interactor>();
        mainCamera = Camera.main.transform;
        
        //Initialize the private variables
        canUse = false;
        isInventoryOpen = false;
        isNoteBookOpen = false;
        itemPosition = 0;
        inventory = new List<InventoryItem>();
        itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    
        //Add listeners to the Unity Events
        onLookUp.AddListener(player.GetComponent<FPSController>().SetIsInspecting);
        // onLookUp.AddListener(hud.ChangeStateInventoryMenu);
        // updateCounter.AddListener(hud.UpdateItemCounter);
        // isReadable.AddListener(hud.ChangeStateIsCanReadInv);
        // changeText.AddListener(hud.ChangeReadTextInv);
        // showText.AddListener(hud.ShowReadTextInv);
        // isUsable.AddListener(hud.ChangeStateUse);
        // addDescription.AddListener(hud.AddDescriptionInventory);
        
        //Add the starting objects to the inventory
        foreach (var item in startingObject)
        {
            inventory.Add(item);
        }
    }

    private void OnEnable()
    {
        InputManager.OnInventory += Inventory;
        InputManager.OnLeaveInteraction += CloseInventory;
        InputManager.OnMouseInteraction += RotateObject;
        InputManager.OnArrowRight += MoveRight;
        InputManager.OnArrowLeft += MoveLeft;
        InputManager.OnRead += Read;
        InputManager.OnUse += Use;
    }

    private void OnDisable()
    {
        InputManager.OnInventory -= Inventory;
        InputManager.OnLeaveInteraction -= CloseInventory;
        InputManager.OnMouseInteraction -= RotateObject;
        InputManager.OnArrowRight -= MoveRight;
        InputManager.OnArrowLeft -= MoveLeft;
        InputManager.OnRead -= Read;
        InputManager.OnUse -= Use;
    }

    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }

    private void Inventory()
    {
        //Checks if is interacting with door
        if ((interactor.GetInteractingObject() is InteractableDoor) && 
            ((InteractableDoor)interactor.GetInteractingObject()).GetSelectedDoorKnob()) return;
        
        //If the inventory is closed and the player is not inspecting an object, open the inventory
        if (!isInventoryOpen && !interactor.IsInspectingObject())
        {
            InventoryInspect(GetCurrentInventoryList());
        }
        //If the inventory is open and the player is not inspecting an object, close the inventory
        else if (isInventoryOpen && !interactor.IsInspectingObject())
        {
            InventoryStopInspect();
        }
    }

    private void CloseInventory()
    {
        //If the notebook is open, close it
        if (isNoteBookOpen)
        {
            isNoteBookOpen = false;
            itemPosition = 0;
            InventoryStopInspect();
            InventoryInspect(GetNonReadableItems());
        }
        //Close the inventory
        else
        {
            InventoryStopInspect();
        }
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        //Returns the InventoryItem if it exists in the inventory
        if(itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        //Adds the item to the inventory if it exists, otherwise creates a new InventoryItem
        if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            itemDictionary.Add(referenceData, newItem);
        }
        updateCounter.Invoke(itemPosition + 1, inventory.Count);
    }

    public void Remove(InventoryItemData referenceData)
    {
        //Removes the item from the inventory if it exists
        if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.GetStack() == 0)
            {
                inventory.Remove(value);
                itemDictionary.Remove(referenceData);
            }
        }
    }

    private void InventoryInspect(List<InventoryItem> inventoryList)
    {
        //Checks if the inventory is empty
        if (inventoryList.Count != 0)
        {
            //Instantiate the object to inspect and shows the counter
            addDescription.Invoke(true, inventoryList[itemPosition].GetData().name, inventoryList[itemPosition].GetData().description);
            updateCounter.Invoke(itemPosition + 1, inventoryList.Count);
            onLookUp.Invoke(true);
            focusedObject = Instantiate(inventoryList[itemPosition].GetData().prefab, mainCamera.position + mainCamera.forward * 
                inventoryList[itemPosition].GetData().prefab.GetComponent<InteractableItemObject>().GetDistance(), Quaternion.identity);
            
            focusedObject.layer = 9;
            foreach (Transform child in focusedObject.transform)
            {
                child.gameObject.layer = 9;
            }
            
            focusedObject.GetComponent<InteractableItemObject>().enabled = false;
            focusedObject.GetComponent<Collider>().enabled = false;
            focusedObject.transform.LookAt(mainCamera.transform.parent.transform);
            focusedObject.transform.Rotate(focusedObject.GetComponent<InteractableItemObject>().GetRotation());

            //Checks if the object is the notebook
            if (inventoryList[itemPosition].GetData().id == "6")
            {
                isReadable.Invoke(false);
                isUsable.Invoke(true);
                canUse = true;
                canRead = false;

            }
            //Checks if the object is readable
            else if(inventoryList[itemPosition].GetData().text != "")
            {
                isUsable.Invoke(false);
                isReadable.Invoke(true);
                changeText.Invoke(inventoryList[itemPosition].GetData().text);
                canRead = true;
                canUse = false;
            }
            else
            {
                isReadable.Invoke(false);
                isUsable.Invoke(false);
                canRead = false;
                canUse = false;
            }
            //Disables the canvas of the object
            if (focusedObject.transform.childCount > 0)
            {
                //focusedObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            
            //Sets the inventory as open
            isInventoryOpen = true;
        }
    }

    private void InventoryStopInspect()
    {
        //Destroys the object and leaves the inventory
        if (isInventoryOpen)
        {
            onLookUp.Invoke(false);
            Destroy(focusedObject);
            isInventoryOpen = false;
            if (isNoteBookOpen)
            {
                itemPosition= 0;
            }
            isNoteBookOpen = false;
        }
    }

    private void OpenNotebook()
    {
        //Checks if the inventory is open and opens the notebook
        if (isInventoryOpen)
        {
            onLookUp.Invoke(false);
            Destroy(focusedObject);
            InventoryInspect(GetCurrentInventoryList());
        }
    }

    private void RotateObject(Vector2 value)
    {
        if (isInventoryOpen && !focusedObject.GetComponent<InteractableItemObject>().IsAxisLocked())
        {
            //If the player is inspecting the object and the object is not readable, rotate the object in all axis
            float x = value.x * -0.15f;
            float y = value.y * 0.15f;
            
            Vector3 focusPosition = focusedObject.transform.position;
            
            focusedObject.transform.RotateAround(focusPosition, mainCamera.up, x);
            focusedObject.transform.RotateAround(focusPosition, mainCamera.right, y);
        }
        else if (isInventoryOpen && focusedObject.GetComponent<InteractableItemObject>().IsAxisLocked())
        {
            //If the player is inspecting the object and the object is readable, rotate the object in the x axis
            float x = value.x * -0.15f;

            focusedObject.transform.RotateAround(focusedObject.transform.position, mainCamera.transform.up, x);
        }
    }

    private void MoveRight()
    {
        if(isInventoryOpen && GetCurrentInventoryList().Count - 1 > itemPosition)
        {
            //Destroys the object and inspects the next object in the inventory
            itemPosition++;
            Destroy(focusedObject);
            InventoryInspect(GetCurrentInventoryList());
        } 
    }

    private void MoveLeft()
    {
        if(isInventoryOpen && itemPosition > 0)
        {
            //Destroys the object and inspects the previous object in the inventory
            itemPosition--;
            Destroy(focusedObject);
            InventoryInspect(GetCurrentInventoryList());
        }
    }

    private void Read()
    {
        //Checks if the object is readable and shows the text
        if (canRead)
        {
            showText.Invoke();
        }
    }

    private void Use()
    {
        //Checks if the object is the notebook and opens it
        if (isInventoryOpen && canUse && GetReadableItems().Count > 0)
        {
            isNoteBookOpen = true;
            itemPosition = 0;
            OpenNotebook();
        }
    }

    public List<InventoryItem> GetNonReadableItems()
    {
        //Returns the list of non readable items
        List<InventoryItem> items = new List<InventoryItem>();

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].GetData().text == "")
            {
                items.Add(inventory[i]);
            }
        }

        return items;
    }

    private List<InventoryItem> GetReadableItems()
    {
        //Returns the list of readable items
        List<InventoryItem> items = new List<InventoryItem>();
        
        for(int i=0; i<inventory.Count; i++)
        {
            if (inventory[i].GetData().text != "")
            {
                items.Add(inventory[i]);
            }
        }

        return items;
    }

    private List<InventoryItem> GetCurrentInventoryList()
    {
        //Returns the list of items depending on the state of the notebook
        if (isNoteBookOpen)
        {
            return GetReadableItems();
        }
        else
        {
            return GetNonReadableItems();
        }
    }

    public List<InventoryItem> GetInventory()
    {
        return inventory;
    }
    public int GetItemPosition()
    {
        return itemPosition;
    }
    public void SetItemPosition(int value)
    {
        itemPosition = value;
        if(itemPosition< 0)
        {
            itemPosition = 0;
        }
    }
}
