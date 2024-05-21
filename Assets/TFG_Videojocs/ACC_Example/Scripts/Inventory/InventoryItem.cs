using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private InventoryItemData data;
    [SerializeField] private int stackSize;
    
    public InventoryItem(InventoryItemData data)
    {
        this.data = data;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }

    public int GetStack()
    {
        return stackSize;
    }
    
    public InventoryItemData GetData()
    {
        return data;
    }
}
