using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject interactionMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject itemCounter;
    [SerializeField] private GameObject itemCounterPuzzle;
    [SerializeField] private GameObject readableInventory;
    [SerializeField] private GameObject readTextInventory;
    [SerializeField] private GameObject readableInspector;
    [SerializeField] private GameObject readTextInspector;
    [SerializeField] private GameObject itemDescriptionInventory;
    [SerializeField] private GameObject itemDescriptionInspector;
    [SerializeField] private GameObject itemDescriptionPuzzle;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject scrollbar;
    [SerializeField] private List<Image> imagesScrollbar;

    [SerializeField] private GameObject leaveActionInv;
    

    [SerializeField] private GameObject useAction;
    [SerializeField] private GameObject useInventory;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private GameObject videoTapeSubtitles;
    [SerializeField] private GameObject credits;
    [SerializeField] private FPSController fpsController;
    
    private DepthOfFieldController depthOfFieldController;
    [SerializeField] private AudioClip openMenuSound;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("SFXAudioSource").GetComponent<AudioSource>();
        depthOfFieldController = GameObject.FindGameObjectWithTag("CameraRoot").GetComponent<DepthOfFieldController>();
        videoTapeSubtitles.GetComponent<PlayableDirector>().stopped += OnStopPlayingTimeline;
    }

    private void OnEnable()
    {
        InputManager.OnPauseMenu += ChangeStatePauseMenu;
    }

    private void OnDisable()
    {
        InputManager.OnPauseMenu -= ChangeStatePauseMenu;
    }

    public void ChangeStateInteractionMenu(bool state)
    {
        interactionMenu.SetActive(state);
    }

    public void ChangeStateUse(bool state)
    {
        if (state)
        {
            leaveActionInv.GetComponent<RectTransform>().anchoredPosition = new Vector3(-181, leaveActionInv.GetComponent<RectTransform>().anchoredPosition.y);
        }
        else
        {
            leaveActionInv.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, leaveActionInv.GetComponent<RectTransform>().anchoredPosition.y);
        }
        useAction.SetActive(state);
    }

    public void ChangeStateIsCanReadInv(bool state)
    {
        if (state)
        {
            leaveActionInv.GetComponent<RectTransform>().anchoredPosition = new Vector3(-181, leaveActionInv.GetComponent<RectTransform>().anchoredPosition.y);
        }
        else
        {
            leaveActionInv.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, leaveActionInv.GetComponent<RectTransform>().anchoredPosition.y);
        }
        readableInventory.SetActive(state);
    }

    public void ChangeReadTextInv(string state)
    {
        readTextInventory.GetComponent<TextMeshProUGUI>().text = state;
    }

    public void ShowReadTextInv()
    {
        if (readTextInventory.activeSelf)
        {
            readTextInventory.SetActive(false);
        }
        else
        {
            readTextInventory.SetActive(true);
        }
    }

    public void ChangeStateIsCanReadIns(bool state)
    {
        readableInspector.SetActive(state);
    }

    public void ChangeReadTextIns(string state)
    {
        readTextInspector.GetComponent<TextMeshProUGUI>().text = state;
    }

    public void ShowReadTextIns()
    {
        if (readTextInspector.activeSelf)
        {
            readTextInspector.SetActive(false);
            imagesScrollbar.ForEach(image => image.enabled = false);
        }
        else
        {
            readTextInspector.SetActive(true);
            imagesScrollbar.ForEach(image => image.enabled = true);
        }
    }

    public void ChangeStateInventoryMenu(bool state)
    {
        inventoryMenu.SetActive(state);
    }

    public void UpdateItemCounter(int current, int total)
    {
        itemCounter.GetComponent<TextMeshProUGUI>().text = "" + current + "/" + total + "";
    }

    public void UpdateItemCounterPuzzle(int current, int total)
    {
        itemCounterPuzzle.GetComponent<TextMeshProUGUI>().text = "" + current + "/" + total + "";
    }

    public void ChangeStatePauseMenu()
    {
        if (!fpsController.GetIsInspecting())
        {
            if (!optionsMenu.activeSelf)
            {
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                if (pauseMenu.activeSelf)
                {
                    depthOfFieldController.enableBlur();
                    audioSource.PlayOneShot(openMenuSound);
                }
                else
                {
                    depthOfFieldController.disableBlur();
                    audioSource.PlayOneShot(openMenuSound);
                }

            }
            else
            {
                pauseMenu.GetComponent<PauseMenu>().ExitOptionsMenu();
            }
        }
    }

    public void ChangeStateUseInventory(bool state)
    {
        useInventory.SetActive(state);
    }

    public void AddDescriptionInventory(bool state, string name, string description)
    {
        itemDescriptionInventory.SetActive(state);
        itemDescriptionInventory.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        itemDescriptionInventory.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
    }

    public void AddDescriptionInspector(bool state, string name, string description)
    {
        itemDescriptionInspector.SetActive(state);
        itemDescriptionInspector.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        itemDescriptionInspector.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
    }

    public void AddDescriptionPuzzle(bool state, string name, string description)
    {
        itemDescriptionPuzzle.SetActive(state);
        itemDescriptionPuzzle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        itemDescriptionPuzzle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
    }

    public void EnableBlackScreen()
    {
        blackScreen.SetActive(true);
        StartCoroutine(TransitionBlackScreen());
    }

    private IEnumerator TransitionBlackScreen()
    {
        Image image = blackScreen.GetComponent<Image>();
        Color initialColor = image.color;
        Color finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
        float passedTime = 0, factor = 0;

        while (passedTime < 2)
        {
            factor = passedTime / 2;
            image.color = Color.Lerp(initialColor, finalColor, factor);

            passedTime += Time.deltaTime;
            yield return null;
        }
        image.color = finalColor;

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(3);

        /*yield return new WaitForSeconds(5f);

        credits.SetActive(true);
        credits.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(credits.GetComponent<Animation>().clip.length + 2f);

        TextMeshProUGUI tribute = credits.transform.Find("Tribute").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI finalPhrase = tribute.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        initialColor = finalPhrase.color;
        finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
        passedTime = 0;

        while (passedTime < 3)
        {
            factor = passedTime / 3;
            finalPhrase.color = Color.Lerp(initialColor, finalColor, factor);

            passedTime += Time.deltaTime;
            yield return null;
        }
        finalPhrase.color = finalColor;

        yield return new WaitForSeconds(3.5f);

        initialColor = tribute.color;
        finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        passedTime = 0;

        while (passedTime < 1.5f)
        {
            factor = passedTime / 1.5f;
            tribute.color = Color.Lerp(initialColor, finalColor, factor);

            passedTime += Time.deltaTime;
            yield return null;
        }
        tribute.color = finalColor;

        yield return new WaitForSeconds(8f);

        initialColor = finalPhrase.color;
        finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        passedTime = 0;

        while (passedTime < 3)
        {
            factor = passedTime / 3;
            finalPhrase.color = Color.Lerp(initialColor, finalColor, factor);

            passedTime += Time.deltaTime;
            yield return null;
        }
        finalPhrase.color = finalColor;

        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(1);*/
    }

    public void PlayTimeline(TimelineAsset timelineAsset)
    {
        videoTapeSubtitles.SetActive(true);
        PlayableDirector playableDirector = videoTapeSubtitles.GetComponent<PlayableDirector>();
        playableDirector.playableAsset = timelineAsset;
        playableDirector.Play();
    }

    private void OnStopPlayingTimeline(PlayableDirector playableDirector)
    {
        videoTapeSubtitles.SetActive(false);
    }


}
