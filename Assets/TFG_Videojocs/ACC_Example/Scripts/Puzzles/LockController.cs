using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockController : AbstractPuzzleController, I_InspectPuzzleObjectController
{
    [SerializeField] [Range(0, 9)] private int[] currentCode;
    [SerializeField] [Range(0, 9)] private int[] solutionCode = {1, 2, 3};
    [SerializeField] private GameObject mainKnob;
    [SerializeField] private GameObject otherKnob;
    [SerializeField] private List<GameObject> objectsToDelete;
    
    private InteractablePuzzle interactablePuzzle;
    private bool isOnPuzzle=false;
    private int selectedDial=0;

    private void Start()
    {
        for (int i=0; i<3; i++)
        {
            transform.GetChild(i).localEulerAngles = new Vector3(transform.GetChild(i).localEulerAngles.x,
                transform.GetChild(i).localEulerAngles.y, 180 + (36*(currentCode[i])));
        }
    }

    private void OnEnable()
    {
        InputManager.OnArrowRight += MoveRight;
        InputManager.OnArrowLeft += MoveLeft;
        InputManager.OnArrowUp += MoveUp;
        InputManager.OnArrowDown += MoveDown;
    }

    private void OnDisable()
    {
        InputManager.OnArrowRight -= MoveRight;
        InputManager.OnArrowLeft -= MoveLeft;
        InputManager.OnArrowUp -= MoveUp;
        InputManager.OnArrowDown -= MoveDown;
    }

    private void MoveRight()
    {
        if (isOnPuzzle)
        {
            Vector3 localEulerAngles = transform.GetChild(selectedDial).localEulerAngles;
            transform.GetChild(selectedDial).localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, localEulerAngles.z + 36);
            if(currentCode[selectedDial]<9) currentCode[selectedDial] += 1;
            else currentCode[selectedDial] = 0;
        }
    }

    private void MoveLeft()
    {
        if (isOnPuzzle)
        {
            Vector3 localEulerAngles = transform.GetChild(selectedDial).localEulerAngles;
            transform.GetChild(selectedDial).localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, localEulerAngles.z - 36);
            if(currentCode[selectedDial]>0) currentCode[selectedDial] -= 1;
            else currentCode[selectedDial] = 9;
        }
    }

    private void MoveUp()
    {
        if (isOnPuzzle && selectedDial-1 >= 0) {
            transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled = false;
            selectedDial -= 1;
            transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled = true;
        }
    }

    private void MoveDown()
    {
        if (isOnPuzzle && selectedDial+1 <= 2) {
            transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled = false;
            selectedDial += 1;
            transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled = true;
        }
    }

    public void CheckPuzzle()
    {
        if (isOnPuzzle && currentCode.SequenceEqual(solutionCode))
        {
            completed = true;
            isOnPuzzle = false;
            
            mainKnob.GetComponent<Collider>().enabled = true;

            if (otherKnob != null)
            {
                otherKnob.GetComponent<Collider>().enabled = true;
            }
            foreach (GameObject element in objectsToDelete) {
                element.SetActive(false);
            }
        }
        
        
    }

    public void Inspect(bool isOnPuzzle)
    {
        this.isOnPuzzle = isOnPuzzle;
        if (transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled)
        {
            transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled = false;
        }
        else
        {
            transform.GetChild(selectedDial).GetComponent<OutlineScript>().enabled = true;
        }
    }
}
