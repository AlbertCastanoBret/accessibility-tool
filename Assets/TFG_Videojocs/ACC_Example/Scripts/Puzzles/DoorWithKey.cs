using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorWithKey : AbstractPuzzleController, I_InteractablePuzzleController
{
    //Private Serialized Variables
    [SerializeField] private InventoryItemData inventoryItemData;
    [SerializeField] private GameObject mainKnob;
    [SerializeField] private GameObject otherKnob;
    [SerializeField] private GameObject mainAngleIndicator;
    
    //Private Variables
    private Vector3 doorTransformPosition;
    private Quaternion doorTransformRotation;
    
    public bool IsOnTransition { get; set; }

    private void Start()
    {
        doorTransformPosition = transform.position;
        doorTransformRotation = transform.rotation;
    }
    public void SetDoorOriginalTransform()
    {
        transform.position = doorTransformPosition;
        transform.rotation = doorTransformRotation;
        mainKnob.GetComponent<Door>().enabled = false;
        mainKnob.GetComponent<InteractableDoor>().enabled = false;
        otherKnob.GetComponent<InteractableDoor>().enabled = false;
        otherKnob.GetComponent<Door>().enabled = false;
    }

    public bool CanOpenInventory(GameObject gameObject)
    {
        return true;
    }

    public void DoorAction(InventoryItemData puzzleObject, int index)
        {
            //If the key id is correct, then the door can be open
            if (inventoryItemData.id == puzzleObject.id)
            {
                inventorySystem.Remove(inventoryItemData);
                Destroy(mainKnob.GetComponent<InteractablePuzzle>());
                InteractableDoor mainInteractableDoor = mainKnob.AddComponent<InteractableDoor>();
                mainInteractableDoor.SetAngleIndicator(mainAngleIndicator);
    
                //GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    
                if (mainKnob.GetComponent<PressE>() != null)
                {
                    mainKnob.GetComponent<PressE>().HideText();
                    Destroy(mainKnob.GetComponent<EventTriggerInteractor>());
                }
    
                mainKnob.GetComponent<Door>().enabled = true;
                Destroy(mainKnob.GetComponent<InteractablePuzzle>());
                otherKnob.GetComponent<InteractableDoor>().enabled = true;
                otherKnob.GetComponent<Door>().enabled = true;
            }
        }
    
    public void RemoveObject(GameObject gameObject)
    {
        
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        
    }
}
