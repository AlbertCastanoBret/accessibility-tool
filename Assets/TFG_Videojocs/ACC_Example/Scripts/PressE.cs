using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PressE : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public void ShowText()
    {
        text.text = "You need the warehouse key. Press E to interact.";
    }
    
    public void HideText()
    {
        text.text = "";
    }
}
