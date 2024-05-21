using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    //Private serialized variables
    [SerializeField] private float marginFix;
    [SerializeField] private GameObject direction;
    [SerializeField] private bool invertControls;

    //Private variables
    private bool isOpen;
    private bool canMoveDoor;
    private Camera mainCamera;
    private GameObject parent;
    private Vector3 initialPosition;
    private float length;

    private void Start()
    {
        //Initialize variables
        canMoveDoor = false;
        isOpen = false;
        mainCamera = Camera.main;
        parent = transform.parent.gameObject;
        length = parent.transform.localScale.x - marginFix;
        initialPosition = parent.transform.position;
    }

    private void OnEnable()
    {
        InputManager.OnMouseInteraction += ActuateDrawer;
    }

    private void OnDisable()
    {
        InputManager.OnMouseInteraction -= ActuateDrawer;
    }

    private void ActuateDrawer(Vector2 value)
    {
        if (!canMoveDoor) return;

        //To fix the drawer position
        parent.transform.position = new Vector3(FixDrawerPosition(parent.transform.position.x), parent.transform.position.y, parent.transform.position.z);

        //Smoothly rotate the camera to look at the drawer knob
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCamera.transform.parent.transform.position);
        mainCamera.transform.parent.transform.rotation = Quaternion.Slerp(mainCamera.transform.parent.transform.rotation, targetRotation, 10f * Time.deltaTime);

        //Determine the direction of the force and apply it
        print(value.x);
        float forceDirection = DetermineForceDirection(value.x);

        //Move the drawer
        if(invertControls)
        {
            parent.GetComponent<Rigidbody>().AddForce(direction.transform.forward * -forceDirection);
        }
        else
        {
            parent.GetComponent<Rigidbody>().AddForce(direction.transform.forward * forceDirection);
        }

        if (parent.transform.position.x == initialPosition.x + length || parent.transform.position.x == initialPosition.x)
        {
            parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private float DetermineForceDirection(float inputValue)
    {
        float forceMultiplier = 5f;

        if (inputValue <= 1f && inputValue >= -1f)
        {
            return 0f;
        }
        else if (inputValue < 0)
        {
            return forceMultiplier;
        }
        else if (inputValue > 0)
        {
            return -forceMultiplier;
        }

        return 0f;
    }

    private float FixDrawerPosition(float positionZ)
    {
        
        if(!invertControls){
            if (positionZ > initialPosition.x + length)
            {
                return initialPosition.x + length;
            }
            else if (positionZ < initialPosition.x)
            {
                return initialPosition.x;
            }
        }
        else{
            if (positionZ < initialPosition.x - length)
            {
                return initialPosition.x - length;
            }
            else if (positionZ > initialPosition.x)
            {
                return initialPosition.x;
            }
        }
        return positionZ;
    }

    public bool CanMoveDoor
    {
        get => canMoveDoor;
        set => canMoveDoor = value;
    }
}
