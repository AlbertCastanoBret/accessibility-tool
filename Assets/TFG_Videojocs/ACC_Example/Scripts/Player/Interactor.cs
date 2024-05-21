using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private BoxCollider boxCollider;
    private AbstractInteractable currentObject=null;
    private I_InteractableInspectObject inspectObject=null;
    private bool isInspectingObject=false;

    private GameObject cameraRoot;
    private List<GameObject> interactableObjects = new List<GameObject>();
    
    RaycastHit hitInfo;
    private bool isInteracting = false;
    private void Awake()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("IgnoreTrigger"));
        cameraRoot = GameObject.FindGameObjectWithTag("CameraRoot");
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if(inspectObject!=null) isInspectingObject = inspectObject.IsInspecting;
        if (interactableObjects.Count > 1)
        {
            SetClosestObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out AbstractInteractable itemObject))
        {
            //print(other.gameObject.transform.parent.name);
            int layerMask = LayerMask.GetMask( "Wall", "Door", "Pomo", "Interactable");
            if(Physics.Raycast(cameraRoot.transform.position, other.transform.position - cameraRoot.transform.position, out hitInfo, 10,layerMask))
            {
                if (other.gameObject != hitInfo.collider.gameObject && hitInfo.collider.gameObject.layer == 11)
                {
                    return;
                }
            }
            interactableObjects.Add(other.gameObject);
            if (currentObject == null)
            {
                if(interactableObjects.Count < 2)itemObject.PassOver();
                currentObject = itemObject;
                if (currentObject is I_InteractableInspectObject) inspectObject = (I_InteractableInspectObject) currentObject;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out AbstractInteractable itemObject))
        {
            int layerMask = LayerMask.GetMask("Wall", "Door", "Pomo", "Interactable");
            if(Physics.Raycast(cameraRoot.transform.position, other.transform.position - cameraRoot.transform.position, out hitInfo, 10,layerMask))
            {
                if (other.gameObject != hitInfo.collider.gameObject && hitInfo.collider.gameObject.layer == 11)
                {
                    //if(interactableObjects.Contains(other.gameObject))interactableObjects.Remove(other.gameObject);
                    if (currentObject == other.gameObject.GetComponent<AbstractInteractable>())
                    {
                        if (other.gameObject.GetComponent<InteractableDoor>() != null && !isInteracting)
                        {
                            //itemObject.StopPassOver();
                            currentObject = null;
                        }
                        else if (!interactableObjects.Contains(other.gameObject))
                        {
                            interactableObjects.Add(other.gameObject);
                        }
                    }
                    return;
                }
            }
            if (!interactableObjects.Contains(other.gameObject))
            {
                interactableObjects.Add(other.gameObject);
            }
            if (currentObject == null || currentObject == itemObject)
            {
                if(interactableObjects.Count < 2)itemObject.PassOver();
                currentObject = itemObject;
                if (currentObject is I_InteractableInspectObject) inspectObject = (I_InteractableInspectObject) currentObject;
            }
        }
    }
    
    public void IsInteracting(bool state)
    {
        isInteracting = state;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out AbstractInteractable itemObject))
        {
            interactableObjects.Remove(other.gameObject);
            //print(other.gameObject.transform.parent.name);
            if (!(currentObject is InteractableDoor) || !((InteractableDoor)currentObject).GetSelectedDoorKnob())
            {
                itemObject.StopPassOver();
                currentObject = null;
                inspectObject = null;
            }
        }
    }

    public bool IsInspectingObject()
    {
        return isInspectingObject;
    }

    public AbstractInteractable GetInteractingObject()
    {
        return currentObject;
    }

    private void SetClosestObject()
    {
        for (int i = 0; i < interactableObjects.Count; i++)
        {
            if ((currentObject is InteractableDoor) && ((InteractableDoor)currentObject).GetSelectedDoorKnob()) return;
            //print(interactableObjects[i].transform.parent.name+". List amount: "+interactableObjects.Count);
            if (interactableObjects[i] != null && currentObject != null)
            {
                interactableObjects[i].GetComponent<AbstractInteractable>().StopPassOver();
                
                float dotProduct1 = Vector3.Dot((interactableObjects[i].transform.position - transform.position).normalized, cameraRoot.transform.forward);
                float dotProduct2 = Vector3.Dot((currentObject.transform.position - transform.position).normalized, cameraRoot.transform.forward);
                
                if (dotProduct1 > dotProduct2)
                {
                    currentObject.StopPassOver();
                    currentObject = interactableObjects[i].GetComponent<AbstractInteractable>();
                }
            }
        }
        if(currentObject!=null)currentObject.PassOver();
    }

    public void RemoveInteractableObject(GameObject interactableObject)
    {
        interactableObjects.Remove(interactableObject);
    }
}
