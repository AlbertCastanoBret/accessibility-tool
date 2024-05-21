using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AltarController : AbstractPuzzleController, I_InteractablePuzzleController
{
    [HideInInspector] [SerializeField] private UnityEvent<bool> onLookUp;
    [SerializeField] private InventoryItemData offering;
    [SerializeField] private Animation animation;
    [SerializeField] private UnityEvent OnEndAnimation;
    [SerializeField] private InputManager inputManager;
    
    public bool IsOnTransition { get; set; }

    private void Start()
    {
        onLookUp.AddListener(player.GetComponent<FPSController>().SetIsInspecting);
    }

    public void PutOffering(InventoryItemData inventoryItemData, int index)
    {
        if (offering.id == inventoryItemData.id)
        {
            GameObject newOffering = Instantiate(inventoryItemData.prefab, transform.GetChild(index).transform, true);
            newOffering.transform.localPosition = new Vector3(0, 0.6f, 0);
            
            inputManager.ChangeStateActionMap();
            animation.Play();
            StartCoroutine(ReturnToMainMenu(animation.clip.length));
        }
    }

    private IEnumerator ReturnToMainMenu(float animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        OnEndAnimation.Invoke();

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }

    public bool CanOpenInventory(GameObject gameObject)
    {
        return true;
    }
    
    public void RemoveObject(GameObject gameObject)
    {
        
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        
    }
}
