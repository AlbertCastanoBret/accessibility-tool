using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class InteractableItemObject : AbstractInteractable, I_InteractableInspectObject
{
    //Unity Events
    [SerializeField] private InventoryItemData referenceItem;
    [SerializeField][Range(0, 1)] private float distance = 0.75f;
    [SerializeField] private Vector3 rotation = new Vector3(0, 0, 0);
    [SerializeField] private bool lockAxis = false;
    [HideInInspector][SerializeField] private UnityEvent<InventoryItemData> addItem;
    [HideInInspector][SerializeField] private UnityEvent<bool> onLookUp;
    [HideInInspector][SerializeField] private UnityEvent<bool> isReadable;
    [HideInInspector][SerializeField] private UnityEvent<string> changeText;
    [HideInInspector][SerializeField] private UnityEvent showText;
    [HideInInspector][SerializeField] private UnityEvent<GameObject> onRemoveInteractableObject;
    [HideInInspector][SerializeField] private UnityEvent<bool, string, string> addDescription;
    [HideInInspector][SerializeField] private UnityEvent<GameObject, GameObject> OnRemoveFromPuzzle;

    //References to scene objects
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject startingPuzzleParent;
    private GameObject puzzleParent;
    private PlayerLook playerLook;
    private Transform mainCamera;
    private HUD hud;
    private Collider collider;

    //References to starting position and rotation of the object
    private Vector3 startingPosition;
    private Vector3 currentPosition;
    private Quaternion startingRotation;

    //Booleans to check if the object is in transition or if the player can read the object
    private bool transitionOver, canRead;

    //Property to check if the object is being inspected
    public bool IsInspecting { get; set; }

    private bool onPuzzle;
    [SerializeField] private bool audioNeeded;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private bool narratorNeeded;
    [SerializeField] private GameObject narratorClip;
    private AudioSource audioSource;
    private void Start()
    {
        if (audioNeeded)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        //Get references to the scene objects
        playerLook = player.GetComponent<PlayerLook>();
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUD>();
        mainCamera = Camera.main.transform;
        collider = GetComponent<Collider>();

        if (startingPuzzleParent != null)
        {
            onPuzzle = true;
            puzzleParent = startingPuzzleParent;
            OnRemoveFromPuzzle.AddListener(puzzleParent.transform.parent.GetComponent<I_InteractablePuzzleController>().RemoveObject);
        }

        //Initialize the private variables
        transitionOver = true;
        IsInspecting = false;

        startingPosition = transform.position;
        currentPosition = startingPosition;
        startingRotation = transform.rotation;

        //Add listeners to the Unity Events
        addItem.AddListener(inventorySystem.Add);
        onLookUp.AddListener(player.GetComponent<FPSController>().SetIsInspecting);
        onLookUp.AddListener(hud.ChangeStateInteractionMenu);
        isReadable.AddListener(hud.ChangeStateIsCanReadIns);
        changeText.AddListener(hud.ChangeReadTextIns);
        showText.AddListener(hud.ShowReadTextIns);
        onRemoveInteractableObject.AddListener(colliderInteractor.GetComponent<Interactor>().RemoveInteractableObject);
        addDescription.AddListener(hud.AddDescriptionInspector);
    }

    private void OnEnable()
    {
        InputManager.OnInteraction += Inspect;
        InputManager.OnInteraction += Store;
        InputManager.OnMouseInteraction += RotateObject;
        InputManager.OnLeaveInteraction += StopInspect;
        InputManager.OnRead += Read;
    }

    private void OnDisable()
    {
        InputManager.OnInteraction -= Inspect;
        InputManager.OnInteraction -= Store;
        InputManager.OnMouseInteraction -= RotateObject;
        InputManager.OnLeaveInteraction -= StopInspect;
        InputManager.OnRead -= Read;
    }

    private void Update()
    {
        if (canvas != null)
        {
            if (isOver && !IsInspecting)
            {
                //If the object is over and the player is not inspecting, show the canvas
                canvas.SetActive(true);
                canvas.transform.LookAt(playerLook.transform, new Vector3(0, 180, 0));
            }
            else if (IsInspecting && canvas.activeSelf || !IsInspecting && !isOver)
            {
                //If the player is inspecting the object, hide the canvas
                canvas.SetActive(false);
            }
        }
        if (GetComponent<OutlineScript>() != null)
        {
            if (isOver)
            {
                GetComponent<OutlineScript>().enabled = true;
            }
            else
            {
                GetComponent<OutlineScript>().enabled = false;
            }
        }
    }

    private void Store()
    {
        if (transitionOver && IsInspecting)
        {
            //If the player is inspecting the object and the transition is over
            //Add the item to the inventory and destroy the object
            addItem.Invoke(referenceItem);
            addDescription.Invoke(false, "", "");
            onLookUp.Invoke(false);
            IsInspecting = false;
            onRemoveInteractableObject.Invoke(gameObject);
            RemoveFromPuzzle();
            Destroy(gameObject);
        }
    }

    public void Inspect()
    {
        if (transitionOver && isOver && !IsInspecting && !inventorySystem.IsInventoryOpen())
        {
            if (puzzleParent == null ||
                puzzleParent.transform.parent.GetComponent<I_InteractablePuzzleController>().IsOnTransition == false)
            {
                //If the transition is over and object is over
                //The player is not inspecting and the inventory is not open, start the inspection
                onLookUp.Invoke(true);
                IsInspecting = true;
                if (canvas != null) canvas.SetActive(false);
                onRemoveInteractableObject.Invoke(gameObject);
                Select(mainCamera.transform.position + mainCamera.forward * distance, mainCamera);
                transform.Rotate(rotation);
                if (audioNeeded)
                {
                    audioSource.PlayOneShot(audioClip);
                }
                if (narratorNeeded)
                {
                    StartCoroutine(PlayNarrator());
                }
                StartCoroutine(Transition(0.5f, true, true));

                //Check if the object is readable
                if (referenceItem.text != "")
                {
                    isReadable.Invoke(true);
                }
                else
                {
                    isReadable.Invoke(false);
                }
            }
        }
    }

    private IEnumerator PlayNarrator()
    {
        yield return new WaitForSeconds(1);
        Instantiate(narratorClip).transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform);
    }

    public void StopInspect()
    {
        if (transitionOver && IsInspecting)
        {
            //If the transition is over and the player is inspecting, stop the inspection
            Deselect();
            if (audioNeeded)
            {
                audioSource.PlayOneShot(audioClip);
            }
            StartCoroutine(Transition(0.5f, false, false));
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
            changeText.Invoke("");
            canRead = false;
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
            if (parent != null)
            {
                transform.SetParent(parent.transform);
                gameObject.layer = 14;
            }
            onLookUp.Invoke(false);
            if (canvas != null) canvas.SetActive(true);
        }

        IsInspecting = inspecting;
        transform.position = currentPosition;
        transitionOver = true;
        if (entry)
        {
            addDescription.Invoke(true, referenceItem.name, referenceItem.description);
            if (referenceItem.text != "")
            {
                changeText.Invoke(referenceItem.text);
                canRead = true;
            }
        }
    }

    private void RotateObject(Vector2 value)
    {
        if (transitionOver && IsInspecting && !lockAxis)
        {
            //If the player is inspecting the object and the object is not readable, rotate the object in all axis
            float x = value.x * -0.15f;
            float y = value.y * 0.15f;

            Vector3 position = transform.position;

            transform.RotateAround(position, mainCamera.transform.up, x);
            transform.RotateAround(position, mainCamera.transform.right, y);
        }
        else if (transitionOver && IsInspecting && lockAxis)
        {
            //If the player is inspecting the object and the object is readable, rotate the object in the x axis
            float x = value.x * -0.15f;

            transform.RotateAround(transform.position, mainCamera.transform.up, x);
        }
    }

    private void Select(Vector3 focusPosition, Transform target)
    {
        startingPosition = transform.position;
        currentPosition = startingPosition;
        startingRotation = transform.rotation;

        //Move the object to the inspection position and look at the player
        currentPosition = focusPosition;
        transform.LookAt(target);
    }
    private void Deselect()
    {
        //Move the object to the starting position and rotation
        currentPosition = startingPosition;
        transform.rotation = startingRotation;
    }

    private void Read()
    {
        if (canRead)
        {
            //If the object is readable, show the text
            showText.Invoke();
        }
    }

    public InventoryItemData GetReferenceItem()
    {
        return referenceItem;
    }

    public Vector3 GetRotation()
    {
        return rotation;
    }

    public float GetDistance()
    {
        return distance;
    }

    public bool IsAxisLocked()
    {
        return lockAxis;
    }

    public void AddOnPuzzle(GameObject parent, I_InteractablePuzzleController interactablePuzzleController)
    {
        onPuzzle = true;
        OnRemoveFromPuzzle.AddListener(interactablePuzzleController.RemoveObject);
        this.parent = parent;
        puzzleParent = parent;

        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    private void RemoveFromPuzzle()
    {
        if (onPuzzle)
        {
            if (puzzleParent.transform.parent.GetComponent<I_InteractablePuzzleController>().IsOnTransition == false)
            {
                onPuzzle = false;
                OnRemoveFromPuzzle.Invoke(puzzleParent, gameObject);
            }
        }
    }
}
