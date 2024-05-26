using ACC_API;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;

public class PauseMenu : MonoBehaviour
{
    [HideInInspector] [SerializeField] private UnityEvent OnPauseMenu;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject buttons;

    private GameObject currentOption;
    private InputManager inputManager;

    private void Awake()
    {
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
        OnPauseMenu.AddListener(inputManager.ChangeStateActionMap);
        
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            if (i == 0)
            {
                buttons.transform.GetChild(i).GetComponent<Button>().onClick.AddListener( () =>
                {
                    ACC_AccessibilityManager.Instance.AudioAccessibility.EnableSubtitlesMenu();
                    title.SetActive(false);
                    buttons.SetActive(false);
                });
            }
            else if (i == 1)
            {
                buttons.transform.GetChild(i).GetComponent<Button>().onClick.AddListener( () =>
                {
                    ACC_AccessibilityManager.Instance.AudioAccessibility.EnableVisualNotificationMenu();
                    title.SetActive(false);
                    buttons.SetActive(false);
                });
            }
            else if (i == 2)
            {
                buttons.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ACC_AccessibilityManager.Instance.VisualAccessibility.EnableHighContrastMenu();
                    title.SetActive(false);
                    buttons.SetActive(false);
                });
            }
            else if (i == 3)
            {
                buttons.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ACC_AccessibilityManager.Instance.MobilityAccessibility.EnableRemapControlsMenu("Keyboard, Mouse");
                    title.SetActive(false);
                    buttons.SetActive(false);
                });
            }
            else if (i == 4)
            {
                buttons.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ACC_AccessibilityManager.Instance.MultifunctionalAccessibility.EnableAudioManagerMenu();
                    title.SetActive(false);
                    buttons.SetActive(false);
                });
            }
        }
    }

    private void OnEnable()
    {
        OnPauseMenu.Invoke();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        // GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sound");
        // foreach (GameObject go in gameObjects)
        // {
        //     if(go.activeSelf) go.GetComponent<AudioSource>().Pause();
        // }
        
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
        OnPauseMenu.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        // GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sound");
        // foreach (GameObject go in gameObjects)
        // {
        //     if(go.activeSelf) go.GetComponent<AudioSource>().Play();
        // }
        
        VideoPlayer[] videoPlayers = FindObjectsOfType<VideoPlayer>();
        foreach (VideoPlayer videoPlayer in videoPlayers)
        {
            if (videoPlayer.isPaused)
            {
                videoPlayer.Play();
            }
        }
    }

    public void Resume()
    {
        gameObject.SetActive(false);
    }

    public void Option(GameObject option)
    {
        if(currentOption!=null) currentOption.SetActive(false);
        currentOption = option;
        currentOption.SetActive(true);
    }
}
