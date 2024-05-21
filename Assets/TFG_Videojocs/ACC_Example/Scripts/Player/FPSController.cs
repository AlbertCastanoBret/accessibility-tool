using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class FPSController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private CharacterController characterController;
    private Vector3 gravitySpeed;

    [Header("Controller Values")]
    private float speed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float gravity;

    private bool isGrounded;
    private int shakeState;
    private bool isSprinting, isInspecting, isOpeningDoor, isClimbing;
    private GameObject flashlight;
    private Animator cameraAnimator;
    
    private Vector3 previousPosition, climbingDirection;
    private float elapsedTime;

    //private bool flashlightHasBattery, flashlightIsOn;
    private bool canUse;
    

    private void Start()
    {
        flashlight = GameObject.FindGameObjectWithTag("Flashlight").gameObject;
        cameraAnimator = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<Animator>();

        //Initialize the variables
        isOpeningDoor = false;
        isInspecting = false;
        speed = walkSpeed;
        shakeState = 0;
        isSprinting = false;
        canUse = true;
        //flashlightHasBattery = true;
        //flashlightIsOn = true;
        isClimbing = false;
        
        previousPosition = characterController.transform.position;
        elapsedTime = 0f;
    }

    private void Update()
    {
        //Check if the player is grounded
        isGrounded = characterController.isGrounded;
    }

    public void Move(Vector2 input)
    {
        float deltaTime = Time.deltaTime;
        
        if (!isInspecting && !isOpeningDoor)
        {
            Vector3 direction = Vector3.zero;
            direction.x = input.x;
            direction.z = input.y;
            direction = direction.normalized;
            characterController.Move(transform.TransformDirection(direction) * speed * Time.deltaTime);

            if (isGrounded && gravitySpeed.y < 0) gravitySpeed.y = 0;
            gravitySpeed.y += gravity * Time.deltaTime;
            characterController.Move(gravitySpeed * Time.deltaTime);
        }
        else if (isClimbing)
        {
            print(input.y);
            climbingDirection.y = input.y;
            climbingDirection = climbingDirection.normalized;
            characterController.Move(transform.TransformDirection(climbingDirection) * speed * Time.deltaTime);
        }

        Vector3 currentPosition = characterController.transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, previousPosition);
        float speedController = distanceMoved / deltaTime;
        cameraAnimator.SetFloat("Speed", speedController);
        previousPosition = currentPosition;
        
        if (speedController > 0f)
        {
            GetComponent<AudioSource>().enabled = true;
        }
        else
        {
            GetComponent<AudioSource>().enabled = false;
        }
    }

    public void OnSprint()
    {
        //speed = sprintSpeed;
        //isSprinting = true;
    }

    public void OnSprintEnd()
    {
        //speed = walkSpeed;
        //isSprinting = false;
    }

    public void OnFlashlightEnabled()
    {
        flashlight.GetComponent<AudioSource>().PlayOneShot(flashlight.GetComponent<AudioSource>().clip);
        if (canUse)
        {
            flashlight.GetComponent<Light>().enabled = !flashlight.GetComponent<Light>().enabled;
        }
    }

    public void StartFlashlightAnimation()
    {
        canUse = false;
    }
    
    public void EndFlashlightAnimation(bool lastAnimation)
    {
        if (lastAnimation)
        {
            canUse = false;
            flashlight.GetComponent<Light>().enabled = true;
        }
        else
        {
            canUse = true;
            flashlight.GetComponent<Light>().enabled = false;
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

    public void LoadData(GameData gameData)
    {
        characterController.enabled = false;
        Transform playerTransform = transform;
        playerTransform.position = new Vector3(gameData.position[0], gameData.position[1], gameData.position[2]);
        characterController.enabled = true;
    }

    public void SaveData(ref GameData gameData)
    {
        Vector3 position = transform.position;
        gameData.position[0] = position.x;
        gameData.position[1] = position.y;
        gameData.position[2] = position.z;
    }

    public void ChangeIsClimbing(Vector3 climbingDirection)
    {
        this.climbingDirection = climbingDirection;
        isClimbing = !isClimbing;
    }

    public void TeleportPlayer(float x, float y, float z)
    {
        characterController.enabled = false;
        
        Transform playerTransform = transform;
        Vector3 position = transform.position;
        position = new Vector3(x, y, z);
        playerTransform.position = position;
        
        characterController.enabled = true;
    }
}