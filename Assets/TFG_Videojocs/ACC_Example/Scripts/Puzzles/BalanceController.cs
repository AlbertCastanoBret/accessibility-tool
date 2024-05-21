using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BalanceController : AbstractPuzzleController, I_InteractablePuzzleController
{
    [SerializeField] private int numberOfHands=3;
    [SerializeField] private int numberOfFingers=4;
    [SerializeField] private GameObject bar;
    [SerializeField] private UnityEvent OnStartLightAnimation;
    private Position[,] handPositions;
    private Position[,] fingerPositions;
    private Position[] bottle;
    private float weightLeft, weigthRight;
    private float tolerance = 0.00001f;
    
    public bool IsOnTransition { get; set; }

    private void Start()
    {
        weightLeft = 0;
        weigthRight = 0;
        IsOnTransition = false;
        
        handPositions = new Position[2, numberOfHands];
        fingerPositions = new Position[2, numberOfFingers];
        bottle = new Position[2];

        for (int j = 0; j < 2; j++)
        {
            float multiplier =  numberOfHands / 2;
            if (numberOfHands % 2 == 0)
            {
                for (int i = 0; i < numberOfHands; i++)
                {
                    handPositions[j, i].isOcuppied = false;
                    handPositions[j, i].multiplier = multiplier - 0.5f;
                    multiplier--;
                }
            }

            else
            {
                for (int i = 0; i < numberOfHands; i++)
                {
                    handPositions[j, i].isOcuppied = false;
                    handPositions[j, i].multiplier = multiplier;
                    multiplier--;
                }
            }
            
            multiplier = numberOfFingers/2;
            if (numberOfFingers % 2 == 0)
            {
                for (int i = 0; i < numberOfFingers; i++)
                {
                    fingerPositions[j, i].isOcuppied = false;
                    fingerPositions[j, i].multiplier = multiplier - 0.5f;
                    multiplier--;
                }
            }

            else
            {
                for (int i = 0; i < numberOfFingers; i++)
                {
                    fingerPositions[j, i].isOcuppied = false;
                    fingerPositions[j, i].multiplier = multiplier;
                    multiplier--;
                }
            }
        }
        
        for (int i = 0; i < bottle.Length; i++)
        {
            bottle[i].isOcuppied = false;
            bottle[i].multiplier = 1;
        }
        
    }

    public struct Position
    {
        public bool isOcuppied;
        public float multiplier;
        public InventoryItemData inventoryItemData;
    }
    
    public void PutObject(InventoryItemData inventoryItemData, int index)
    {
        Collider collider = transform.GetChild(index).GetComponent<Collider>();
        if (inventoryItemData.name == "Hand" && CanBePlaced(index, "Hand") && IsOnTransition == false)
        {
            for (int i = 0; i < numberOfHands; i++)
            {
                if (handPositions[index, i].isOcuppied == false)
                {
                    handPositions[index, i].isOcuppied = true;
                    handPositions[index, i].inventoryItemData = inventoryItemData;
                    inventorySystem.Remove(inventoryItemData);
                    print(inventorySystem.GetItemPosition());
                    GameObject newObject = Instantiate(inventoryItemData.prefab, transform.GetChild(index), true);
                    newObject.transform.localPosition = new Vector3((collider.bounds.size.x/numberOfHands * handPositions[index, i].multiplier), collider.bounds.size.y/2, -collider.bounds.size.z/4);
                    newObject.GetComponent<InteractableItemObject>().AddOnPuzzle(transform.GetChild(index).gameObject, this);
                    StartCoroutine(Transition(inventoryItemData.kg, index==0? 1:-1));
                    break;
                }
            }
        }
        
        else if (inventoryItemData.name == "Finger" && CanBePlaced(index, "Finger") && IsOnTransition == false)
        {
            for (int i = 0; i < numberOfFingers; i++)
            {
                if (fingerPositions[index, i].isOcuppied == false)
                {
                    fingerPositions[index, i].isOcuppied = true;
                    fingerPositions[index, i].inventoryItemData = inventoryItemData;
                    inventorySystem.Remove(inventoryItemData);
                    inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
                    GameObject newObject = Instantiate(inventoryItemData.prefab, transform.GetChild(index), true);
                    newObject.transform.localPosition =
                        new Vector3((collider.bounds.size.x/2/numberOfFingers * fingerPositions[index, i].multiplier) - collider.bounds.size.x/4,
                            collider.bounds.size.y/2, collider.bounds.size.z/9);
                    newObject.GetComponent<InteractableItemObject>().AddOnPuzzle(transform.GetChild(index).gameObject, this);
                    StartCoroutine(Transition(inventoryItemData.kg, index==0? 1:-1));
                    break;
                }
            }
        }
        
        else if (inventoryItemData.name == "Bottle" && CanBePlaced(index, "Bottle") && IsOnTransition == false)
        {
            if (bottle[index].isOcuppied == false)
            { 
                bottle[index].isOcuppied = true;
                bottle[index].inventoryItemData = inventoryItemData;
                inventorySystem.Remove(inventoryItemData);
                inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
                GameObject newObject = Instantiate(inventoryItemData.prefab, transform.GetChild(index), true);
                newObject.transform.localPosition = new Vector3(collider.bounds.size.x/4,
                            collider.bounds.size.y/2, collider.bounds.size.z/8);
                newObject.GetComponent<InteractableItemObject>().AddOnPuzzle(transform.GetChild(index).gameObject, this);
                StartCoroutine(Transition(inventoryItemData.kg, index==0? 1:-1));
            }
        }
    }

    private bool CanBePlaced(int index, string piece)
    {
        if (piece == "Hand")
        {
            for (int j = 0; j < numberOfHands; j++)
            {
                if (!handPositions[index, j].isOcuppied) return true;
            }
        }
        else if (piece == "Finger")
        {
            for (int j = 0; j < numberOfFingers; j++)
            {
                if (!fingerPositions[index, j].isOcuppied) return true;
            }
        }
        else if (piece == "Bottle")
        {
            return !bottle[index].isOcuppied;
        }
        return false;
    }

    private bool CheckWin()
    {
        return Mathf.Abs(bar.transform.localRotation.z) < tolerance && bottle.Any(position => position.isOcuppied);
    }
    
    public void RemoveObject(GameObject gameObject)
    {
        
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        InventoryItemData reference = gameObject.GetComponent<InteractableItemObject>().GetReferenceItem();
        if (reference.name == "Hand")
        {
            for (int i = 0; i < numberOfHands; i++)
            {
                if (handPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].inventoryItemData != null)
                {
                    if (reference.id == handPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].inventoryItemData.id)
                    {
                        handPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].isOcuppied = false;
                        handPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].inventoryItemData = null;
                        StartCoroutine(Transition(-reference.kg, parent.transform.GetSiblingIndex()==0? 1: -1));
                    }
                }
            }
        }
        
        else if (reference.name == "Finger")
        {
            for (int i = 0; i < numberOfFingers; i++)
            {
                if (fingerPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].inventoryItemData != null)
                {
                    if (reference.id == fingerPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].inventoryItemData.id)
                    {
                        fingerPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].isOcuppied = false;
                        fingerPositions[parent.transform.GetSiblingIndex()==0? 0: 1, i].inventoryItemData = null;
                        StartCoroutine(Transition(-reference.kg, parent.transform.GetSiblingIndex()==0? 1: -1));
                    }
                }
            }
        }
        
        else if (reference.name == "Bottle")
        {
            if (bottle[parent.transform.GetSiblingIndex()==0? 0: 1].inventoryItemData != null)
            {
                if (reference.id == bottle[parent.transform.GetSiblingIndex()==0? 0: 1].inventoryItemData.id)
                {
                    bottle[parent.transform.GetSiblingIndex()==0? 0: 1].isOcuppied = false;
                    bottle[parent.transform.GetSiblingIndex()==0? 0: 1].inventoryItemData = null;
                    StartCoroutine(Transition(-reference.kg, parent.transform.GetSiblingIndex()==0? 1: -1));
                }
            }
            
        }
    }

    public bool CanOpenInventory(GameObject gameObject)
    {
        return true;
    }

    private IEnumerator Transition(float mass, int side)
    {
        IsOnTransition = true;
        Vector3 initialRotation = bar.transform.eulerAngles;
        Vector3 finalRotation;
        
        finalRotation = new Vector3(bar.transform.eulerAngles.x, bar.transform.eulerAngles.y, bar.transform.eulerAngles.z + mass * 200 * side);

        float passedTime = 0f;

        while (passedTime < 1f)
        {
            Vector3 rotacionActual = Vector3.Slerp(initialRotation, finalRotation, passedTime);
            
            bar.transform.eulerAngles = rotacionActual;
            
            passedTime += Time.deltaTime;

            yield return null;
        }

        bar.transform.eulerAngles = finalRotation;
        IsOnTransition = false;
        if (CheckWin())
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Collider>().enabled = false;
                child.GetComponent<InteractablePuzzle>().enabled = false;
                Destroy(child.GetComponent<OutlineScript>());
                foreach (Transform item in child)
                {
                    if(item.TryGetComponent(out Collider collider))
                    {
                        collider.enabled = false;
                        collider.gameObject.GetComponent<InteractableItemObject>().enabled = false;
                        Destroy(collider.gameObject.GetComponent<OutlineScript>());
                    };
                }
            }
            OnStartLightAnimation.Invoke();
        }
    }
}
