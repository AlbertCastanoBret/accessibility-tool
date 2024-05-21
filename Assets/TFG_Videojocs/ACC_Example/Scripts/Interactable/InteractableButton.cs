using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableButton : AbstractInteractable
{

    public bool IsInspecting { get; set; }
    private PlayerLook playerLook;

    private void Start()
    {
        IsInspecting = false;
        playerLook = player.GetComponent<PlayerLook>();
    }

    private void OnEnable()
    {
        InputManager.OnInteraction += ChangeScene;
    }

    private void OnDisable()
    {
        InputManager.OnInteraction -= ChangeScene;
    }

    private void Update()
    {
        if (isOver && !IsInspecting)
        {
            canvas.SetActive(true);
            canvas.transform.LookAt(playerLook.transform, new Vector3(0, 180, 0));
        }
        else if (IsInspecting && canvas.activeSelf || !IsInspecting && !isOver)
        {
            canvas.SetActive(false);
        }
    }

    private void ChangeScene()
    {
        if(isOver) SceneManager.LoadScene(3);
    }
    
    
    
}
