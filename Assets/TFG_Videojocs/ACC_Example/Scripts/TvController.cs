using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Video;

public class TvController : AbstractPuzzleController, I_InteractablePuzzleController
{
    [SerializeField] private List<InventoryItemData> vhsTapes;
    [SerializeField] private GameObject screen;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Animation reproductor;
    [SerializeField] private GameObject sound;
    [SerializeField] private List<Material> materials;
    [SerializeField] private GameObject light;

    [SerializeField] private UnityEvent<TimelineAsset> onStartTape;

    private GameObject currentGameObject;
    private InventoryItemData currentVhsTape;
    private bool isPlaying, firstTape;

    public bool IsOnTransition { get; set; }

    private void Start()
    {
        isPlaying = false;
        firstTape = false;
        currentVhsTape = null;
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += ThrowVhsTape;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= ThrowVhsTape;
    }

    public void PutVhsTape(InventoryItemData inventoryItemData, int index)
    {
        int i = vhsTapes.FindIndex(tape => tape.id == inventoryItemData.id);
        Debug.Log(i);
        Debug.Log(isPlaying);
        
        if (isPlaying==false && i != -1 && currentVhsTape==null)
        {
            if (i == 0) firstTape = true;
            isPlaying = true;
            inventorySystem.Remove(vhsTapes[i]);
            inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
            currentVhsTape = inventoryItemData;
            StartCoroutine(EnterAnimation());
        }
    }

    private IEnumerator EnterAnimation()
    {
        reproductor.Play();
        yield return new WaitForSeconds(reproductor.clip.length);
        currentGameObject = Instantiate(currentVhsTape.prefab, gameObject.transform);
        currentGameObject.GetComponent<Collider>().enabled = false;
        currentGameObject.transform.localScale = new Vector3(0.9747536f, 0.9747536f, 0.9747536f);
        Animation inventoryItemDataAnimation = currentGameObject.GetComponent<Animation>();
        inventoryItemDataAnimation.Play();
        yield return new WaitForSeconds(inventoryItemDataAnimation.clip.length);
        
        reproductor.Play("ClosePlayer");
        yield return new WaitForSeconds(reproductor.GetClip("ClosePlayer").length+0.5f);

        videoPlayer.clip = currentVhsTape.videoClip;
        videoPlayer.SetTargetAudioSource(0, screen.GetComponent<AudioSource>());
        videoPlayer.Play();
        onStartTape.Invoke(currentVhsTape.timelineAsset);
        
        light.SetActive(true);
        screen.GetComponent<MeshRenderer>().material = materials[1];
    }
    
    public void RemoveObject(GameObject gameObject)
    {
        /*if (isPlaying==false && currentVhsTape != null)
        {
            inventorySystem.Add(currentVhsTape);
            currentVhsTape = null;
            currentGameObject = null;
        }*/
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        
    }

    private void ThrowVhsTape(VideoPlayer source)
    {
        StartCoroutine(ExitAnimation());
    }

    private IEnumerator ExitAnimation()
    {
        videoPlayer.clip = null;
        screen.GetComponent<MeshRenderer>().material = materials[0];
        light.SetActive(false);
        yield return new WaitForSeconds(1);
        
        reproductor.Play();
        yield return new WaitForSeconds(reproductor.clip.length);
        
        Animation inventoryItemDataAnimation = currentGameObject.GetComponent<Animation>();
        inventoryItemDataAnimation.Play("Tape2");
        currentGameObject.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(inventoryItemDataAnimation.GetClip("Tape2").length);
        
        reproductor.Play("ClosePlayer");
        yield return new WaitForSeconds(reproductor.GetClip("ClosePlayer").length+0.5f);
        
        isPlaying = false;
        inventorySystem.Add(currentVhsTape);
        currentVhsTape = null;
        currentGameObject = null;

        yield return new WaitForSeconds(2f);
        if (sound != null && !sound.IsDestroyed()) sound.SetActive(true);
        }
    
    public bool CanOpenInventory(GameObject gameObject)
    {
        return !isPlaying&&currentVhsTape==null;
    }
}
