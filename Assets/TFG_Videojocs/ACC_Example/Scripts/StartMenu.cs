using System;
using System.Collections;
using System.Collections.Generic;
using ACC_API;
using UnityEngine;
using UnityEngine.Events;

public class StartMenu : MonoBehaviour
{
    [HideInInspector] [SerializeField] private UnityEvent OnPauseMenu;
    private InputManager inputManager;
    public bool isEnded{ get; private set;}
    
    private void Awake()
    {
        isEnded = false;
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
        OnPauseMenu.AddListener(inputManager.ChangeStateActionMap);
    }
    void Start()
    {
        ACC_AccessibilityManager.Instance.LoadAllUserPreferences();
        ACC_AccessibilityManager.Instance.AudioAccessibility.PlaySubtitle("TapeSubtitles");
        OnPauseMenu.Invoke();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            isEnded = true;
            OnPauseMenu.Invoke();
            gameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }
}
