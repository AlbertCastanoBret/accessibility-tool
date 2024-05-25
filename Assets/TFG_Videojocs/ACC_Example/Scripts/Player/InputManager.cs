using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;
    private PlayerInput.UIActions uiActions;
    private FPSController fpsController;
    private PlayerLook playerLook;

    public delegate void InteractActionHandler();
    public static event InteractActionHandler OnInteraction;
    public static event InteractActionHandler OnLeaveInteraction;
    public static event InteractActionHandler OnInventory;
    public static event InteractActionHandler OnStartMouseInteraction;
    public static event InteractActionHandler OnCancelMouseInteraction;
    public static event InteractActionHandler OnArrowRight;
    public static event InteractActionHandler OnArrowLeft;
    public static event InteractActionHandler OnArrowUp;
    public static event InteractActionHandler OnArrowDown;
    public static event InteractActionHandler OnRead;
    public static event InteractActionHandler OnUse;
    public static event InteractActionHandler OnPauseMenu;
    
    public static event InteractActionHandler OnFlashlight;

    public delegate void MouseInteractionHandler(Vector2 value);
    public static event MouseInteractionHandler OnMouseInteraction;

    public static event MouseInteractionHandler OnMouseScroll;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInput = new PlayerInput();
        playerActions = playerInput.Player;
        uiActions = playerInput.UI;
        fpsController = GetComponent<FPSController>();
        playerLook = GetComponent<PlayerLook>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerActions.Sprint.performed += ctx => fpsController.OnSprint();
        playerActions.Sprint.canceled += ctx => fpsController.OnSprintEnd();

        playerActions.Interact.performed += ctx => OnInteraction?.Invoke();
        playerActions.LeaveInteract.performed += ctx => OnLeaveInteraction?.Invoke();

        playerActions.ArrowRight.performed += ctx => OnArrowRight?.Invoke();
        playerActions.ArrowLeft.performed += ctx => OnArrowLeft?.Invoke();
        
        playerActions.ArrowUp.performed += ctx => OnArrowUp?.Invoke();
        playerActions.ArrowDown.performed += ctx => OnArrowDown?.Invoke();

        playerActions.Inventory.performed += ctx => OnInventory?.Invoke();

        playerActions.Read.performed += ctx => OnRead?.Invoke();

        playerActions.Use.performed += ctx => OnUse?.Invoke();

        playerActions.Flashlight.performed += ctx => fpsController.OnFlashlightEnabled();
        
        playerActions.Flashlight.performed += ctx => OnFlashlight?.Invoke();
        
        playerActions.DoorClick.started += ctx => OnStartMouseInteraction?.Invoke();
        playerActions.DoorClick.canceled += ctx => OnCancelMouseInteraction?.Invoke();

        //uiActions.PauseMenu.performed += ctx => OnPauseMenu?.Invoke();
        uiActions.ScrollWheel.performed += ctx => OnMouseScroll?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        fpsController.Move(playerActions.Move.ReadValue<Vector2>());
        playerLook.Look(playerActions.Look.ReadValue<Vector2>()); //aixo estava a LateUpdate, per si dona algun problema tornar-ho a posar a LateUpdate
    }
    
    private void FixedUpdate()
    {
        OnMouseInteraction?.Invoke(playerActions.Look.ReadValue<Vector2>()); //aixo estava a LateUpdate, per si dona algun problema tornar-ho a posar a LateUpdate
    }

    public void ChangeStateActionMap()
    {
        if(playerInput.Player.enabled) playerInput.Player.Disable();
        else playerInput.Player.Enable();
    }
}
