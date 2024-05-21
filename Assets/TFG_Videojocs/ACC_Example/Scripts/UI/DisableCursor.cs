using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCursor : MonoBehaviour
{
    [SerializeField]
    private bool cursorDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        if (cursorDisabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else{
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
