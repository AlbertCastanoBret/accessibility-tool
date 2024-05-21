using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPuzzleController : MonoBehaviour
{
    [HideInInspector][SerializeField] protected GameObject player;
    [HideInInspector][SerializeField] protected InventorySystem inventorySystem;
    protected bool completed;
    void Awake()
    {
        //Get the references to the player, the canvas and the inventory system
        player = GameObject.FindGameObjectWithTag("Player");
        inventorySystem = player.transform.Find("Inventory").GetComponent<InventorySystem>();
        
        //Initialize the protected variables
        completed = false;
    }
    
    public bool IsCompleted()
    {
        return completed;
    }
}
