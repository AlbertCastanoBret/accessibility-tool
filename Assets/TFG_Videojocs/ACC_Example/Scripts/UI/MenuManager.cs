using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        DataPersistenceManager.CreateDataPersistenceManager();
    }

    public void Play()
    {
        DataPersistenceManager.instance.NewGame();
    }

    public void Load()
    {
        DataPersistenceManager.instance.LoadGame();
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
