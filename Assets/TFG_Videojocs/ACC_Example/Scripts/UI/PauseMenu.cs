using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Cursor = UnityEngine.Cursor;

public class PauseMenu : MonoBehaviour
{
    [HideInInspector] [SerializeField] private UnityEvent OnPauseMenu;
    [SerializeField] private GameObject tutorialtext;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject currentResolution;
    [SerializeField] private GameObject currentFullScreen;
    [SerializeField] private GameObject currentQuality;
    [SerializeField] private GameObject currentMasterVolume;
    [SerializeField] private GameObject currentSensitivity;
    [SerializeField] private GameObject applyButton;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject unappliedChangesDetectedWindow;
    [SerializeField] private PlayerLook playerLook;

    private GameObject currentOption;
    private InputManager inputManager;
    
    private Resolution[] resolutions;
    private List<String> resolutionOptions;
    private int resolution = 0;

    private string[] qualities = new[] { "Low", "Medium", "High" };
    private int quality;
        
    private FullScreenMode[] fullScreenModes =
        { FullScreenMode.ExclusiveFullScreen, FullScreenMode.MaximizedWindow, FullScreenMode.Windowed };
    private int fullScreenMode;

    private float brightness;

    private float masterVolume;

    private bool anyChange;

    private float sensitivity;

    private void Awake()
    {
        anyChange = false;
        brightness = 0;
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
        OnPauseMenu.AddListener(inputManager.ChangeStateActionMap);
    }

    private void OnEnable()
    {
        tutorialtext.SetActive(false);
        OnPauseMenu.Invoke();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sound");
        foreach (GameObject go in gameObjects)
        {
            if(go.activeSelf) go.GetComponent<AudioSource>().Pause();
        }
        
        VideoPlayer[] videoPlayers = FindObjectsOfType<VideoPlayer>();
        foreach (VideoPlayer videoPlayer in videoPlayers)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }

    }

    private void OnDisable()
    {
        tutorialtext.SetActive(true);
        OnPauseMenu.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sound");
        foreach (GameObject go in gameObjects)
        {
            if(go.activeSelf) go.GetComponent<AudioSource>().Play();
        }
        
        VideoPlayer[] videoPlayers = FindObjectsOfType<VideoPlayer>();
        foreach (VideoPlayer videoPlayer in videoPlayers)
        {
            if (videoPlayer.isPaused)
            {
                videoPlayer.Play();
            }
        }
    }

    private void CheckResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionOptions = new List<string>();
        resolution = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (Screen.fullScreen && resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                resolution = i;
            }
        }
        currentResolution.GetComponent<TextMeshProUGUI>().text = resolutionOptions[resolution];
    }

    private void CheckQuality()
    {
        for (int i = 0; i < qualities.Length; i++)
        {
            if (QualitySettings.names[QualitySettings.GetQualityLevel()] == qualities[i])
            {
                quality = i;
            }
        }
        currentQuality.GetComponent<TextMeshProUGUI>().text = qualities[quality];
    }

    private void CheckFullScreen()
    {
        for (int i = 0; i < fullScreenModes.Length; i++)
        {
            if (Screen.fullScreenMode == fullScreenModes[i])
            {
                fullScreenMode = i;
            }
        }
        currentFullScreen.GetComponent<TextMeshProUGUI>().text = fullScreenModes[fullScreenMode].ToString();
    }

    public void CheckBrightness(float newBrightness)
    {
        brightness = newBrightness;
    }

    public void CheckMasterVolume(float newVolume)
    {
        anyChange = true;
        audioMixer.SetFloat("Master Volume", Mathf.Log10(newVolume) * 20);
    }

    public void CheckMouseSensitivity(float newMouseSensitivity)
    {
        anyChange = true;
    }

    public void LeftResolution()
    {
        if (resolution - 1 < 0) resolution = resolutionOptions.Count - 1;
        else resolution--;
        currentResolution.GetComponent<TextMeshProUGUI>().text = resolutionOptions[resolution];
    }
    
    public void RightResolution()
    {
        if (resolution + 1 > resolutionOptions.Count-1) resolution = 0;
        else resolution++;
        currentResolution.GetComponent<TextMeshProUGUI>().text = resolutionOptions[resolution];
    }

    public void LeftQuality()
    {
        if (quality - 1 < 0) quality = qualities.Length - 1;
        else quality--;
        currentQuality.GetComponent<TextMeshProUGUI>().text = qualities[quality];
    }

    public void RightQuality()
    {
        if (quality + 1 > qualities.Length - 1) quality = 0;
        else quality++;
        currentQuality.GetComponent<TextMeshProUGUI>().text = qualities[quality];
    }

    public void LeftFullScreen()
    {
        if (fullScreenMode - 1 < 0) fullScreenMode = fullScreenModes.Length - 1;
        else fullScreenMode--;
        currentFullScreen.GetComponent<TextMeshProUGUI>().text = fullScreenModes[fullScreenMode].ToString();
    }

    public void RightFullScreen()
    {
        if (fullScreenMode + 1 > fullScreenModes.Length - 1) fullScreenMode = 0;
        else fullScreenMode++;
        currentFullScreen.GetComponent<TextMeshProUGUI>().text = fullScreenModes[fullScreenMode].ToString();
    }

    public void ApplyButton()
    {
        Screen.SetResolution(resolutions[resolution].width, resolutions[resolution].height, Screen.fullScreen);
        QualitySettings.SetQualityLevel(quality);
        Screen.fullScreenMode = fullScreenModes[fullScreenMode];
        Screen.brightness = brightness;

        float sliderValue = currentMasterVolume.GetComponent<ControlSlider>().GetSlider().value;
        audioMixer.SetFloat("Master Volume", Mathf.Log10(sliderValue) * 20);

        float sliderSensitivity = currentSensitivity.GetComponent<ControlSlider>().GetSlider().value;
        playerLook.SetSensibilitat(sliderSensitivity);
    }

    private bool CheckAnyChanges()
    {
        if (Screen.fullScreen && resolutions[resolution].width != Screen.currentResolution.width ||
            resolutions[resolution].height != Screen.currentResolution.height) return true;

        if (qualities[quality] != QualitySettings.names[QualitySettings.GetQualityLevel()]) return true;

        if (fullScreenModes[fullScreenMode] != Screen.fullScreenMode) return true;
        
        float currentVolume, TOLERANCE = 0.01f;
        audioMixer.GetFloat("Master Volume", out currentVolume);

        if (Math.Abs(masterVolume - currentVolume) > TOLERANCE) return true;

        float currentSensitivity = playerLook.GetSensibilitat();
        if (Math.Abs(sensitivity - currentSensitivity) > TOLERANCE) return true;
        return false;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        //DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
    
    public void EnterOptionsMenu()
    {
        optionsMenu.SetActive(true);
        buttons.SetActive(false);
        CheckResolutions();
        CheckQuality();
        CheckFullScreen();
        audioMixer.GetFloat("Master Volume", out masterVolume);
        currentMasterVolume.GetComponent<ControlSlider>().SetSlider(Mathf.Pow(10, (masterVolume / 20)));
        currentSensitivity.GetComponent<ControlSlider>().SetSlider(playerLook.GetSensibilitat());
    }

    public void ExitOptionsMenu()
    {
        if (CheckAnyChanges())
        {
            if(currentOption!=null) currentOption.SetActive(false);
            unappliedChangesDetectedWindow.SetActive(true);
        }
        else
        {
            optionsMenu.SetActive(false);
            buttons.SetActive(true);
            currentOption.SetActive(false);
            currentMasterVolume.GetComponent<ControlSlider>().SetSlider(Mathf.Pow(10, (masterVolume / 20)));
            currentSensitivity.GetComponent<ControlSlider>().SetSlider(playerLook.GetSensibilitat());
        }
    }
    
    public void YesButton()
    {
        unappliedChangesDetectedWindow.SetActive(false);
        ApplyButton();
        optionsMenu.SetActive(false);
        buttons.SetActive(true);
    }

    public void NoButton()
    {
        unappliedChangesDetectedWindow.SetActive(false);
        optionsMenu.SetActive(false);
        buttons.SetActive(true);
        currentMasterVolume.GetComponent<ControlSlider>().SetSlider(Mathf.Pow(10, (masterVolume / 20)));
        currentSensitivity.GetComponent<ControlSlider>().SetSlider(playerLook.GetSensibilitat());
    }

    public void Option(GameObject option)
    {
        if(currentOption!=null) currentOption.SetActive(false);
        currentOption = option;
        currentOption.SetActive(true);
    }

    public void ShowApplyButton(bool canApply)
    {
        applyButton.SetActive(canApply);
    }
    
}
