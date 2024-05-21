using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldController : MonoBehaviour
{
    //Private Serialized Variables
    [SerializeField] private Volume volume;
    [SerializeField] private float maxDistance; //bokeh 5, gaussian 6
    [SerializeField] private float focusSpeed; //bokeh 8, gaussian 4
    [SerializeField] private int defaultFocalLength;  //60
    [SerializeField] private int inspectingFocalLength;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool Gaussian;
    private bool menuOpen;

    //Private Variables
    private GameObject player;
    private FPSController fpsController;

    //Public Variables
    Ray raycast;
    RaycastHit hit;
    bool isHit;
    float hitDistance;
    DepthOfField depthOfField;

    void Start()
    {
        menuOpen = false;
        //Get the reference to the player and the fpsController
        player = GameObject.FindGameObjectWithTag("Player");
        fpsController = player.GetComponent<FPSController>();

        volume.profile.TryGet(out depthOfField);
        if (Gaussian)
        {
            depthOfField.mode.value = DepthOfFieldMode.Gaussian;
        }
        else
        {
            depthOfField.mode.value = DepthOfFieldMode.Bokeh;
        }
    }

    void Update()
    {
        if (!menuOpen)
        {
            //Create a raycast to detect the distance between the player and the object
            int layerMask = LayerMask.GetMask("Default", "Wall", "Door", "Glass");
            raycast = new Ray(transform.position, transform.forward * maxDistance);
            isHit = false;

            //If the raycast hits something, set the hit distance to the distance between the player and the object
            if (Physics.SphereCast(raycast, 0.1f, out hit, maxDistance, layerMask))
            {
                isHit = true;
                hitDistance = Vector3.Distance(transform.position, hit.point);
            }
            else
            {
                if (hitDistance < maxDistance)
                {
                    hitDistance++;
                }
            }

            SetFocus();
            if (!(depthOfField.focusDistance.value - hitDistance < 2f) && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (!(hitDistance - depthOfField.focusDistance.value < 2f) && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

    }

    void SetFocus()
    {
        //close start 1 end 3 & far start 6 end 10
        //depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, Time.deltaTime * focusSpeed);
        if (Gaussian)
        {
            depthOfField.gaussianEnd.value = Mathf.Lerp(depthOfField.gaussianEnd.value, hitDistance + 2, Time.deltaTime * focusSpeed);
            depthOfField.gaussianStart.value = Mathf.Lerp(depthOfField.gaussianStart.value, hitDistance, Time.deltaTime * focusSpeed);
        }
        else
        {
            depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, Time.deltaTime * focusSpeed);
            if (fpsController.GetIsInspecting())
            {
                depthOfField.focalLength.value = Mathf.Lerp(depthOfField.focalLength.value, inspectingFocalLength, Time.deltaTime * 0.25f);
            }
            else
            {
                depthOfField.focalLength.value = Mathf.Lerp(depthOfField.focalLength.value, defaultFocalLength, Time.deltaTime * 0.25f);
            }
        }

    }

    private void OnDrawGizmos()
    {
        //Draw a sphere at the hit point and a red line to the hit point
        if (isHit)
        {
            Gizmos.DrawSphere(hit.point, 0.1f);
            Debug.DrawRay(transform.position, transform.forward * hitDistance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
        }
    }

    public void enableBlur()
    {
        menuOpen = true;
        volume.profile.TryGet(out depthOfField);
        depthOfField.focusDistance.value = 0.1f;
    }

    public void disableBlur()
    {
        menuOpen = false;
    }

}
