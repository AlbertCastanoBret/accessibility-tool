using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class PaintingsController : AbstractPuzzleController, I_InteractablePuzzleController
{
    [SerializeField] private List<InventoryItemData> paintings;
    [SerializeField] private List<GameObject> altarDoorKnobs;
    [SerializeField] private Animation animation;
    [SerializeField] private AudioClip librarySound;
    [SerializeField] private AudioSource audioSource;
    private List<bool> picturesPlacedList;

    public bool IsOnTransition { get; set; }

    private void Start()
    {
        picturesPlacedList = new List<bool>();
        foreach (Transform child in transform)
        {
            if (child.childCount != 0 && child.GetChild(0).TryGetComponent(out InteractableItemObject interactableItemObject))
            {
                if (paintings[child.GetSiblingIndex()].id == interactableItemObject.GetReferenceItem().id)
                {
                    picturesPlacedList.Add(true);
                }
                else picturesPlacedList.Add(false);
            }
            else picturesPlacedList.Add(false);
        }
        altarDoorKnobs.ForEach(go => go.GetComponent<Collider>().enabled = false);
    }

    public void CheckPaintingsOnStart()
    {
        picturesPlacedList = new List<bool>();
        foreach (Transform child in transform)
        {
            if (child.childCount != 0 && child.GetChild(0).TryGetComponent(out InteractableItemObject interactableItemObject))
            {
                if (paintings[child.GetSiblingIndex()].id == interactableItemObject.GetReferenceItem().id)
                {
                    picturesPlacedList.Add(true);
                }
                else picturesPlacedList.Add(false);
            }
            else picturesPlacedList.Add(false);
        }
        altarDoorKnobs.ForEach(go => go.GetComponent<Collider>().enabled=false);
    }

    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Collider>().enabled = true;
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Collider>().enabled = false;
        }
    }

    public void PutPainting(InventoryItemData inventoryItemData, int index)
    {
        if (inventoryItemData.name == "Painting")
        {
            inventorySystem.Remove(inventoryItemData);
            inventorySystem.SetItemPosition(inventorySystem.GetItemPosition()-1);
            GameObject newPainting = Instantiate(inventoryItemData.prefab, transform.GetChild(index), true);
            newPainting.transform.localPosition = new Vector3(0, 0, 5f);
            transform.GetChild(index).localPosition = new Vector3(transform.GetChild(index).localPosition.x - 1, transform.GetChild(index).localPosition.y, transform.GetChild(index).localPosition.z);
            newPainting.transform.localEulerAngles = new Vector3(0, 0, 180 + newPainting.transform.localEulerAngles.z);
            newPainting.GetComponent<InteractableItemObject>().AddOnPuzzle(transform.GetChild(index).gameObject, this);

            if (paintings[index].id == inventoryItemData.id)
            {
                picturesPlacedList[index] = true;

                if (picturesPlacedList.All(placed => placed))
                {
                    foreach (Transform child in transform)
                    {
                        child.GetComponent<Collider>().enabled = false;
                        child.GetComponent<InteractablePuzzle>().enabled = false;
                        child.GetChild(0).GetComponent<Collider>().enabled = false;
                        child.GetChild(0).GetComponent<InteractableItemObject>().enabled = false;
                    }
                    audioSource.PlayOneShot(librarySound);
                    StartCoroutine(MoveLibrary());
                    enabled = false;
                }
            }
        }
    }

    private IEnumerator MoveLibrary()
    {
        animation.Play();
        yield return new WaitForSeconds(animation.clip.length - 0.5f);
        altarDoorKnobs.ForEach(go => go.GetComponent<Collider>().enabled = true);
    }

    public void RemoveObject(GameObject gameObject)
    {
        /*inventorySystem.Add(gameObject.transform.GetChild(0).GetComponent<InteractableItemObject>().GetReferenceItem());
        picturesPlacedList[gameObject.transform.GetSiblingIndex()] = false;
        Destroy(gameObject.transform.GetChild(0).gameObject);*/
    }

    public void RemoveObject(GameObject parent, GameObject gameObject)
    {
        parent.transform.localPosition = new Vector3(parent.transform.localPosition.x + 1, parent.transform.localPosition.y, parent.transform.localPosition.z);
        print(parent.transform.localPosition);
        picturesPlacedList[parent.transform.GetSiblingIndex()] = false;
        Destroy(gameObject);
    }

    public bool CanOpenInventory(GameObject gameObject)
    {
        return gameObject.transform.childCount == 0;
    }
}
