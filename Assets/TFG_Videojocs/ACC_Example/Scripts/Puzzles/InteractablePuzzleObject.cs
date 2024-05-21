using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractablePuzzleObject : AbstractInteractable, I_InteractableInspectObject
{
    [HideInInspector][SerializeField] private UnityEvent<bool> onLookUp;
    [HideInInspector][SerializeField] private UnityEvent<GameObject> onRemoveInteractableObject;
    
    [HideInInspector][SerializeField] private UnityEvent<bool> usableInventory;
    [HideInInspector][SerializeField] private UnityEvent<bool, string, string> addDescription;
    
    //References to scene objects
    [SerializeField] private InventoryItemData referenceItem;
    [SerializeField] private GameObject parent;
    [SerializeField] [Range(0, 1)] private float distance;
    private Camera mainCamera;
    private PlayerLook playerLook;
    private I_InspectPuzzleObjectController puzzleObjectController;
    private Collider collider;
    private HUD hud;
    private Vector3 startingPosition, currentPosition;
    private bool transitionOver;
    private Quaternion startingRotation;
    
    public bool IsInspecting { get; set; }

    private void Start()
    {
        transitionOver = true;
        
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUD>();
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = transform.Find("Canvas")?.gameObject;
        inventorySystem = player.transform.Find("Inventory")?.GetComponent<InventorySystem>();
        onLookUp.AddListener(player.GetComponent<FPSController>().SetIsInspecting);
        puzzleObjectController = GetComponent<I_InspectPuzzleObjectController>();
        collider = GetComponent<BoxCollider>();
        
        onRemoveInteractableObject.AddListener(colliderInteractor.GetComponent<Interactor>().RemoveInteractableObject);
        usableInventory.AddListener(hud.ChangeStateUseInventory);
        addDescription.AddListener(hud.AddDescriptionPuzzle);
    }

    private void Update()
    {
        if (canvas != null)
        {
           if(isOver && !IsInspecting)
           {
               canvas.SetActive(true); 
               canvas.transform.LookAt(playerLook.transform, new Vector3(0,180,0));
           }
           else if (IsInspecting && canvas.activeSelf || !IsInspecting && !isOver)
           { 
               canvas.SetActive(false);
           } 
        }
        if (GetComponent<OutlineScript>() != null)
        {
            if(isOver && !IsInspecting)
            {
                GetComponent<OutlineScript>().enabled = true;
            }
            else if (IsInspecting && GetComponent<OutlineScript>().enabled || !IsInspecting && !isOver)
            {
                GetComponent<OutlineScript>().enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        transitionOver = false;
        IsInspecting = false;
        mainCamera = Camera.main;
        playerLook = GameObject.Find("Player").GetComponent<PlayerLook>();
        
        startingPosition = transform.position;
        currentPosition = startingPosition;
        startingRotation = transform.rotation;
        
        InputManager.OnInteraction += Inspect;
        InputManager.OnLeaveInteraction += StopInspect;
        InputManager.OnUse += CompletePuzzle;
    }
    
    private void OnDisable()
    {
        InputManager.OnInteraction -= Inspect;
        InputManager.OnLeaveInteraction -= StopInspect;
        InputManager.OnUse -= CompletePuzzle;
    }
    
    public void Inspect()
    {
        if (transitionOver && isOver && !IsInspecting && !inventorySystem.IsInventoryOpen())
        {
            onLookUp.Invoke(true);
            addDescription.Invoke(false, "", "");
            usableInventory.Invoke(true);
            onRemoveInteractableObject.Invoke(gameObject);
            Select(mainCamera.transform.position + mainCamera.transform.forward * distance, mainCamera.transform);
            IsInspecting = true;
            StartCoroutine(Transition(0.5f, true, true));
            puzzleObjectController.Inspect(true);
        }
    }
    
    public void StopInspect()
    {
        if(transitionOver && IsInspecting)
        {
            Deselect();
            IsInspecting = false;
            puzzleObjectController.Inspect(false);
            StartCoroutine(Transition(0.5f, false, false));
        }
    }

    private void CompletePuzzle()
    {
        puzzleObjectController.CheckPuzzle();
        if (((AbstractPuzzleController)puzzleObjectController).IsCompleted())
        {
            IsInspecting = false;
            usableInventory.Invoke(false);
            addDescription.Invoke(false, "", "");
            puzzleObjectController.Inspect(false);
            onLookUp.Invoke(false);
            gameObject.SetActive(false);
        }
    }
    
    private IEnumerator Transition(float duration, bool entry, bool inspecting)
    {
        //Enable and disable the item collider
        if (entry)
        {
            collider.enabled = false;
            transform.SetParent(null);
            gameObject.layer = 9;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 9;
            }
        }
        else
        {
            addDescription.Invoke(false, "", "");
            gameObject.layer = 0;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 0;
            }
        }

        //Coroutine to move the object to the inspection position
        transitionOver = false;
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Slerp(initialPosition, currentPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //Enable the item collider
        if (!entry)
        {
            collider.enabled = true;
            if(parent != null)transform.SetParent(parent.transform);
            onLookUp.Invoke(false);
            usableInventory.Invoke(false);
        }
        
        IsInspecting = inspecting;
        transform.position = currentPosition;
        transitionOver = true;
        if (entry)
        {
            addDescription.Invoke(true, referenceItem.name, referenceItem.description);
        }
    }
    
    public void Select(Vector3 focusPosition, Transform target)
    {
        currentPosition = focusPosition;
        transform.LookAt(target);
    }
    public void Deselect()
    {
        currentPosition = startingPosition;
        transform.rotation = startingRotation;
    }
}
