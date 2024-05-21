using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string name;
    public GameObject prefab;
    public string description;
    public string text;
    public float kg;
    public VideoClip videoClip;
    public TimelineAsset timelineAsset;
}
