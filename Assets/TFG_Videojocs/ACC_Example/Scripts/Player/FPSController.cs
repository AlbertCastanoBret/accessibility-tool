using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    private Vector3 gravitySpeed;

    [Header("Controller Values")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float gravity;

    private bool isGrounded;
    private int shakeState;
    private bool isInspecting, isOpeningDoor;
    private GameObject flashlight;
    private Animator cameraAnimator;
    
    private Vector3 previousPosition, climbingDirection;
    private float elapsedTime;
    
    private bool canUse;
    private Vector2 inputMovement;
    

    private void Start()
    {
        flashlight = GameObject.FindGameObjectWithTag("Flashlight").gameObject;
        cameraAnimator = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<Animator>();
        
        isOpeningDoor = false;
        isInspecting = false;
        shakeState = 0;
        canUse = true;
        
        elapsedTime = 0f;
        previousPosition = characterController.transform.position;
    }

    private void Update()
    {
        if (isInspecting || isOpeningDoor) return;
        
        MoveCharacter();
        ApplyGravity();
        
        Vector3 currentPosition = characterController.transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, previousPosition);
        float speedController = distanceMoved / Time.deltaTime;
        cameraAnimator.SetFloat("Speed", speedController);
        previousPosition = currentPosition;
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = new Vector3(inputMovement.x, 0, inputMovement.y);
        moveDirection = transform.TransformDirection(moveDirection) * walkSpeed;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            gravitySpeed.y = 0;
        }
        else
        {
            gravitySpeed.y += gravity * Time.deltaTime;
        }

        characterController.Move(gravitySpeed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    public void OnFlashlightEnabled()
    {
        flashlight.GetComponent<AudioSource>().PlayOneShot(flashlight.GetComponent<AudioSource>().clip);
        if (canUse)
        {
            flashlight.GetComponent<Light>().enabled = !flashlight.GetComponent<Light>().enabled;
        }
    }

    public void SetIsInspecting(bool isInspecting)
    {
        this.isInspecting = isInspecting;
    }

    public bool GetIsInspecting()
    {
        return isInspecting;
    }

    public void SetIsOpeningDoor(bool isOpeningDoor)
    {
        this.isOpeningDoor = isOpeningDoor;
    }

    public bool GetIsOpeningDoor()
    {
        return isOpeningDoor;
    }
}