using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    //Private serialized variables
    [SerializeField] private float marginFix;
    [SerializeField] private GameObject direction;

    //Private variables
    private bool isOpen;
    private bool canMoveDrawer;
    private Camera mainCamera;
    private GameObject parent;
    private Vector3 initialPosition;
    private float length;
    [Header("Configuració de l'empenta del calaixó")]
    [SerializeField] private DoorOpenDirection doorOpenDirection;
    [SerializeField] private bool invertKinematics;
    [SerializeField] private DrawerAxis drawerAxis;


    private void Start()
    {
        //Initialize variables
        canMoveDrawer = false;
        isOpen = false;
        mainCamera = Camera.main;
        parent = transform.parent.gameObject;
        if (drawerAxis == DrawerAxis.Eix_X_Positiva)
        {
            length = parent.transform.localScale.x - marginFix;
        }
        else if (drawerAxis == DrawerAxis.Eix_Y_Positiva)
        {
            length = parent.transform.localScale.y - marginFix;
        }
        else if (drawerAxis == DrawerAxis.Eix_Z_Positiva)
        {
            length = parent.transform.localScale.z - marginFix;
        }
        else if (drawerAxis == DrawerAxis.Eix_X_Negativa)
        {
            length = parent.transform.localScale.x + marginFix;
        }
        else if (drawerAxis == DrawerAxis.Eix_Y_Negativa)
        {
            length = parent.transform.localScale.y + marginFix;
        }
        else if (drawerAxis == DrawerAxis.Eix_Z_Negativa)
        {
            length = parent.transform.localScale.z + marginFix;
        }
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
        if (!canMoveDrawer) return;

        //To fix the drawer position



        //Smoothly rotate the camera to look at the drawer knob
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCamera.transform.parent.transform.position);
        mainCamera.transform.parent.transform.rotation = Quaternion.Slerp(mainCamera.transform.parent.transform.rotation, targetRotation, 10f * Time.deltaTime);

        //Determine the direction of the force and apply it
        float forceDirection = DetermineForceDirection(value.y);
        if (invertKinematics)
        {
            forceDirection = -forceDirection;
        }
        if (doorOpenDirection == DoorOpenDirection.Forward)
            parent.GetComponent<Rigidbody>().AddForce(parent.transform.forward * forceDirection);
        else if (doorOpenDirection == DoorOpenDirection.Up)
            parent.GetComponent<Rigidbody>().AddForce(parent.transform.up * forceDirection);
        else if (doorOpenDirection == DoorOpenDirection.Right)
            parent.GetComponent<Rigidbody>().AddForce(parent.transform.right * forceDirection);


        if (drawerAxis == DrawerAxis.Eix_X_Positiva)
        {
            parent.transform.position = new Vector3(FixDrawerPosition(parent.transform.position.x), parent.transform.position.y, parent.transform.position.z);
            if (parent.transform.position.x == initialPosition.x + length || parent.transform.position.x == initialPosition.x)
            {
                parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Y_Positiva)
        {
            parent.transform.position = new Vector3(parent.transform.position.x, FixDrawerPosition(parent.transform.position.y), parent.transform.position.z);
            if (parent.transform.position.y == initialPosition.y + length || parent.transform.position.y == initialPosition.y)
            {
                parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Z_Positiva)
        {
            parent.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, FixDrawerPosition(parent.transform.position.z));
            if (parent.transform.position.z == initialPosition.z + length || parent.transform.position.z == initialPosition.z)
            {
                parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_X_Negativa)
        {
            parent.transform.position = new Vector3(FixDrawerPosition(parent.transform.position.x), parent.transform.position.y, parent.transform.position.z);
            if (parent.transform.position.x == initialPosition.x - length || parent.transform.position.x == initialPosition.x)
            {
                parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Y_Negativa)
        {
            parent.transform.position = new Vector3(parent.transform.position.x, FixDrawerPosition(parent.transform.position.y), parent.transform.position.z);
            if (parent.transform.position.y == initialPosition.y - length || parent.transform.position.y == initialPosition.y)
            {
                parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Z_Negativa)
        {
            parent.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, FixDrawerPosition(parent.transform.position.z));
            if (parent.transform.position.z == initialPosition.z - length || parent.transform.position.z == initialPosition.z)
            {
                parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
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
        if (drawerAxis == DrawerAxis.Eix_Z_Positiva)
        {
            if (positionZ < initialPosition.z)
            {
                return initialPosition.z;
            }
            else if (positionZ > initialPosition.z + length)
            {
                return initialPosition.z + length;
            }
            else
            {
                return positionZ;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Y_Positiva)
        {
            if (positionZ < initialPosition.y)
            {
                return initialPosition.y;
            }
            else if (positionZ > initialPosition.y + length)
            {
                return initialPosition.y + length;
            }
            else
            {
                return positionZ;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_X_Positiva)
        {
            if (positionZ < initialPosition.x)
            {
                return initialPosition.x;
            }
            else if (positionZ > initialPosition.x + length)
            {
                return initialPosition.x + length;
            }
            else
            {
                return positionZ;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Z_Negativa)
        {
            if (positionZ > initialPosition.z)
            {
                return initialPosition.z;
            }
            else if (positionZ < initialPosition.z - length)
            {
                return initialPosition.z - length;
            }
            else
            {
                return positionZ;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_Y_Negativa)
        {
            if (positionZ > initialPosition.y)
            {
                return initialPosition.y;
            }
            else if (positionZ < initialPosition.y - length)
            {
                return initialPosition.y - length;
            }
            else
            {
                return positionZ;
            }
        }
        else if (drawerAxis == DrawerAxis.Eix_X_Negativa)
        {
            if (positionZ > initialPosition.x)
            {
                return initialPosition.x;
            }
            else if (positionZ < initialPosition.x - length)
            {
                return initialPosition.x - length;
            }
            else
            {
                return positionZ;
            }
        }
        else
        {
            return positionZ;
        }
    }

    public bool CanMoveDrawer
    {
        get => canMoveDrawer;
        set => canMoveDrawer = value;
    }
}

enum DrawerAxis
{
    Eix_X_Positiva,
    Eix_X_Negativa,
    Eix_Y_Positiva,
    Eix_Y_Negativa,
    Eix_Z_Positiva,
    Eix_Z_Negativa
}
