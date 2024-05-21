using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerIconChanger : MonoBehaviour
{
    private Image controlImage;
    [SerializeField] private Sprite keyboardIcon;
    [SerializeField] private Sprite gamepadIcon;

    public enum ControllerType
    {
        KeyboardMouse,
        Gamepad
    }
    private void Start()
    {
        controlImage = GetComponent<Image>();
    }

    private ControllerType lastUsedControllerType;

    void Update()
    {
        if(Gamepad.current != null)
        {
            Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
            Vector2 rightStick = Gamepad.current.rightStick.ReadValue();

            if ((Input.anyKey || Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || 
                Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            { 
                lastUsedControllerType = ControllerType.KeyboardMouse;
                print("Keyboard using");
            }
            if((Gamepad.current.buttonSouth.isPressed || Gamepad.current.buttonEast.isPressed || Gamepad.current.buttonWest.isPressed || Gamepad.current.buttonNorth.isPressed
                 || Gamepad.current.leftShoulder.isPressed || Gamepad.current.rightShoulder.isPressed || Gamepad.current.startButton.isPressed || Gamepad.current.selectButton.isPressed
                  || rightStick.magnitude > 0.1f || leftStick.magnitude > 0.1f || Gamepad.current.leftTrigger.isPressed || Gamepad.current.rightTrigger.isPressed))
            {
                lastUsedControllerType = ControllerType.Gamepad;
                print("Gamepad using");
            }

            if (lastUsedControllerType == ControllerType.Gamepad)
            {
                controlImage.sprite = gamepadIcon;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else if(lastUsedControllerType == ControllerType.KeyboardMouse)
            {
                controlImage.sprite = keyboardIcon;
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            controlImage.sprite = keyboardIcon;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
