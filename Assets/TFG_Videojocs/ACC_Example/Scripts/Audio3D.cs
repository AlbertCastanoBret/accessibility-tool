using System;
using System.Collections;
using System.Collections.Generic;
using ACC_API;
using UnityEngine;

public class Audio3D : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ACC_AccessibilityManager.Instance.AudioAccessibility.PlayVisualNotification("Rain");
        }
    }
}
