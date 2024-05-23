using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInteractable : MonoBehaviour
{
    protected bool isOver;
    [HideInInspector][SerializeField] protected GameObject player;
    [HideInInspector][SerializeField] protected GameObject canvas;
    [HideInInspector][SerializeField] protected InventorySystem inventorySystem;
    protected GameObject colliderInteractor;
    //private GameObject cursor;
    
    protected void Awake()
    {
        //Get the references to the player, the canvas and the inventory system
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = transform.Find("Canvas")?.gameObject;
        inventorySystem = player.transform.Find("Inventory")?.GetComponent<InventorySystem>();
        colliderInteractor = Camera.main.transform.Find("ColliderInteractor").gameObject;
        isOver = false;
        //cursor = GameObject.FindGameObjectWithTag("Cursor");
    }
    
    public void PassOver()
    {
        isOver = true;
        //cursor.SetActive(true);
    }

    public void StopPassOver()
    {
        isOver = false;
        //cursor.SetActive(false);
    }
}
