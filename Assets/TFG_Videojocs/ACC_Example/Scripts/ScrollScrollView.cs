using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScrollView : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 1.0f;

    private bool isScrolling = false;
    private Vector3 previousMousePosition;

    private void OnEnable()
    {
        InputManager.OnMouseScroll += Scroll;
    }

    private void OnDisable()
    {
        InputManager.OnMouseScroll -= Scroll;
    }

    /*void FixedUpdate()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (scrollDelta != 0)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }*/

    private void Scroll(Vector2 scrollDelta)
    {
        float scrollValue = scrollDelta.y;
        if (scrollValue != 0)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
