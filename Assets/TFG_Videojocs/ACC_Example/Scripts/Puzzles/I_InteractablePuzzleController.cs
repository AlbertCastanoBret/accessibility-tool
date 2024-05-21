using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_InteractablePuzzleController
{
    public bool CanOpenInventory(GameObject gameObject);

    public void RemoveObject(GameObject gameObject);
    
    public void RemoveObject(GameObject parent, GameObject gameObject);
    
    public bool IsOnTransition { get; set; }
}
