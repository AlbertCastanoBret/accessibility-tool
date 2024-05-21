using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager instance;

    private List<IDataPersistence> dataPersistenceObjects;
    private GameData gameData;
    private FileDataHandler fileDataHandler;
    private string fileName;
    
    public static void CreateDataPersistenceManager()
    {
        if (instance == null)
        {
            GameObject gameObject = new GameObject("DataPersistenceManager");
            DontDestroyOnLoad(gameObject);
            instance = gameObject.AddComponent<DataPersistenceManager>();
        }
    }

    private void Start()
    {
        fileName = "data.game";
        print("Path: " + Application.persistentDataPath);
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void NewGame()
    {
        gameData = new GameData();
        StartCoroutine(LoadSceneAsync(false));
    }

    public void LoadGame()
    {
        gameData = fileDataHandler.Load();
        if (gameData != null)
        {
            StartCoroutine(LoadSceneAsync(true));
        }
    }
    
    private IEnumerator LoadSceneAsync(bool loadGame)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (loadGame)
        {
            foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
            {
                dataPersistenceObject.LoadData(gameData);
            }
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }
        fileDataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> persistenceObjects =
            FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(persistenceObjects);
    }
        
}
