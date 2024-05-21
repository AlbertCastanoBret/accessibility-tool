using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FuseBoxController : AbstractPuzzleController, I_InteractablePuzzleController
{
    //[SerializeField] private List<PuzzleObject> startingLevers = new List<PuzzleObject>();
    [SerializeField] private List<InventoryItemData> levers;
    [SerializeField] private List<GameObject> fuseParents;
    [SerializeField] private string firstRowOrder;
    [SerializeField] private List<GameObject> leversFirstRow;
    [SerializeField] private string secondRowOrder;
    [SerializeField] private List<GameObject> leversSecondRow;

    //Events fuses
    [SerializeField] private UnityEvent onFirstFuse;
    [SerializeField] private UnityEvent onSecondFuse;

    //Private variables
    private Dictionary<string, Fuse> dictionaryLevers;
    private bool isLeverOneEnabled;
    private bool isLeverTwoEnabled;
    
    public bool IsOnTransition { get; set; }
    private AudioSource audioSource;
    public struct Fuse
    {
        public bool hasLever, isEnabled;
        public string id;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //Initialize the dictionary
        dictionaryLevers = new Dictionary<string, Fuse>();

        //Adding the levers to the dictionary
        foreach (Transform child in transform)
        {
            dictionaryLevers.Add(child.GetSiblingIndex().ToString(), new Fuse(){hasLever = false, isEnabled = false, id = null});
        }
    }

    public void PutFuse(InventoryItemData leverObject, int index)
    {
        //If the id of the lever is the same as the id of the object, then put the fuse
        if (levers[index].id==leverObject.id)
        {
            Transform parent;
            if (!dictionaryLevers["0"].hasLever)
            {
                parent = fuseParents[0].transform;
            }
            else if(!dictionaryLevers["1"].hasLever)
            {
                parent = fuseParents[1].transform;
            }
            else
            {
                parent = fuseParents[2].transform;
            }
            
            dictionaryLevers[index.ToString()] = new Fuse() { hasLever = true, isEnabled = false , id = leverObject.id};
            inventorySystem.Remove(levers[index]);
            inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
            
            GameObject newLever = Instantiate(leverObject.prefab, parent);
            newLever.transform.localPosition = new Vector3(0,0, 0.0718f);
            newLever.transform.localEulerAngles = Vector3.zero;
            newLever.GetComponent<Collider>().enabled = false;
        }
    }

    public void EnableLever(GameObject gameObject)
    {
        //If the fuse is in the dictionary and it has a lever and it is not enabled, then enable it
        if (dictionaryLevers.TryGetValue(gameObject.transform.GetSiblingIndex().ToString(), out Fuse fuse))
        {
            if (fuse.hasLever && !fuse.isEnabled)
            {
                if (fuse.id == "10")
                {
                    if (CheckFuseRow(leversFirstRow, firstRowOrder))
                    {
                        fuse.isEnabled = true;
                        isLeverOneEnabled = true;
                        fuseParents[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("TurnOn");
                        audioSource.Play();
                        onFirstFuse.Invoke();
                    }
                    else
                    {
                        fuseParents[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("TurnOnOff");
                        audioSource.Play();
                        turnSmallFusesOff(leversFirstRow);
                    }
                }
                else if (fuse.id == "11")
                {
                    if (CheckFuseRow(leversSecondRow, secondRowOrder))
                    {
                        fuse.isEnabled = true;
                        isLeverTwoEnabled = true;
                        fuseParents[1].transform.GetChild(0).GetComponent<Animator>().SetTrigger("TurnOn");
                        audioSource.Play();
                        onSecondFuse.Invoke();
                    }
                    else
                    {
                        fuseParents[1].transform.GetChild(0).GetComponent<Animator>().SetTrigger("TurnOnOff");
                        audioSource.Play();
                        turnSmallFusesOff(leversSecondRow);
                    }
                }
            }
        }
    }

    public bool CanOpenInventory(GameObject gameObject)
    {
        //Check if the fuse is in the dictionary and it has a lever
        if (dictionaryLevers.TryGetValue(gameObject.transform.GetSiblingIndex().ToString(), out Fuse fuse))
        {
            return !fuse.hasLever;
        }
        return false;
    }

    private bool CheckFuseRow(List<GameObject> row, string order)
    {
        print(row.Count);
        for(int i=0; i<row.Count; i++)
        {
            if (row[i].transform.GetChild(0).GetComponent<SmallFuse>().GetValue() != int.Parse(order[i].ToString()))
            {
                return false;
            }
        }
        return true;
    }
    
    private void turnSmallFusesOff(List<GameObject> row)
    {
        for(int i=0; i<row.Count; i++)
        {
            if (row[i].transform.GetChild(0).GetComponent<SmallFuse>().GetValue() == 1f)
            {
                row[i].transform.GetChild(0).GetComponent<SmallFuse>().SetValue(0f);
                row[i].transform.GetChild(0).GetComponent<Animator>().SetTrigger("TurnOff");
            }
        }
    }
    
    public bool GetLeverOneEnabled()
    {
        return isLeverOneEnabled;
    }
    
    public bool GetLeverTwoEnabled()
    {
        return isLeverTwoEnabled;
    }
    
    public void RemoveObject(GameObject gameObject)
    {
        
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        
    }
}