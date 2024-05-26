using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

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
        
        playerActions.DoorClick.started += ctx => OnStartMouseInteraction?.Invoke();
        playerActions.DoorClick.canceled += ctx => OnCancelMouseInteraction?.Invoke();

        uiActions.PauseMenu.performed += ctx => OnPauseMenu?.Invoke();
        uiActions.ScrollWheel.performed += ctx => OnMouseScroll?.Invoke(ctx.ReadValue<Vector2>());
    }

    public void OnInteract(InputAction.CallbackContext ctxt)
    { 
        OnInteraction?.Invoke();
    }
    
    public void OnLeaveInteract(InputAction.CallbackContext ctxt)
    {
        OnLeaveInteraction?.Invoke();
    }
    
    public void OnArrowRightPress(InputAction.CallbackContext ctxt)
    {
        OnArrowRight?.Invoke();
    }
    
    public void OnArrowLeftPress(InputAction.CallbackContext ctxt)
    {
        OnArrowLeft?.Invoke();
    }
    
    public void OnArrowUpPress(InputAction.CallbackContext ctxt)
    {
        OnArrowUp?.Invoke();
    }
    
    public void OnArrowDownPress(InputAction.CallbackContext ctxt)
    {
        OnArrowDown?.Invoke();
    }
    
    public void OnInventoryPress(InputAction.CallbackContext ctxt)
    {
        OnInventory?.Invoke();
    }
    
    public void OnReadPress(InputAction.CallbackContext ctxt)
    {
        OnRead?.Invoke();
    }
    
    public void OnUsePress(InputAction.CallbackContext ctxt)
    {
        OnUse?.Invoke();
    }
    
    public void OnFlashlightPress(InputAction.CallbackContext ctxt)
    {
        fpsController.OnFlashlightEnabled();
        OnFlashlight?.Invoke();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        playerLook.Look(playerActions.Look.ReadValue<Vector2>());
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
