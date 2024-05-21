using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerLook : MonoBehaviour, IDataPersistence
{
    private GameObject camera;
    private GameObject flashlight;
    private FPSController fpsController;

    private bool isOnWall;
    
    public float Sensitivity
    {
        get { return sensibilitat; }
        set { sensibilitat = value; }
    }
    [Header("Configuració de la càmera enregistradora de vídeo")]
    [Range(0.01f, 0.2f)][SerializeField] float sensibilitat = 0.1f;
    [Tooltip("Limita la rotació vertical de la càmera")]
    [Range(0f, 90f)][SerializeField] float limitRotacioVertical;
    Vector2 rotation;
    [HideInInspector] public List<Quaternion> positionList;
    [Header("Configuració de la torxa")]
    [Tooltip("Distància/Delay de la llanterna")]
    [SerializeField][Range(0, 20)] private int distancia;
    public float suavitzatDelMoviment;
    private Transform lastTransform;
    private float counter;
    void Start()
    {
        //Get the references from the scene
        fpsController = GetComponent<FPSController>();
        flashlight = GameObject.FindGameObjectWithTag("Flashlight").gameObject;
        camera = GameObject.FindGameObjectWithTag("CameraRoot").gameObject;
        rotation = new Vector2();

        //Initialize the variables
        //sensibilitat = 0.1f;
        limitRotacioVertical = 70f;
        //rotation = Vector2.zero;
        distancia = 10;
        suavitzatDelMoviment = 11f;
        counter = 0;
    }

    private void Update()
    {
        int fps = ((int)(1.0f / Time.deltaTime));
        int distanceDelay = (fps * distancia) / 144;
        positionList.Add(new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w));
        if (positionList.Count > distanceDelay)
        {
            Quaternion smoothFollow = Quaternion.Lerp(flashlight.transform.rotation, positionList[0], suavitzatDelMoviment * Time.deltaTime);
            positionList.RemoveAt(0);
            flashlight.transform.rotation = smoothFollow;
        }
    }

    public void Look(Vector2 input)
    {
        if (!fpsController.GetIsInspecting() && !fpsController.GetIsOpeningDoor())
        {
            rotation.x += input.x * sensibilitat;
            rotation.y += input.y * sensibilitat;
            rotation.y = Mathf.Clamp(rotation.y, -limitRotacioVertical, limitRotacioVertical);
            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
            transform.localRotation = xQuat;
            counter++;
            camera.transform.localRotation = yQuat;
        }
        else if (isOnWall)
        {
            rotation.x += input.x * sensibilitat;
            rotation.x = Mathf.Clamp(rotation.x, -50f, 50f);
            rotation.y += input.y * sensibilitat;
            rotation.y = Mathf.Clamp(rotation.y, -limitRotacioVertical, limitRotacioVertical);
            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            //camera.transform.localRotation = xQuat;
            counter++;
            camera.transform.localRotation = xQuat*yQuat;
        }
    }

    public void SetCameraTransform(Vector2 cameraTransform)
    {
        rotation = new Vector2(cameraTransform.x, cameraTransform.y);
    }

    public void LoadData(GameData gameData)
    {
        rotation.x = gameData.rotation[0];
        rotation.y = gameData.rotation[1];
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.rotation[0] = rotation.x;
        gameData.rotation[1] =  rotation.y;
    }

    public void SetRotationX(float x)
    {
        rotation.x = x;
    }

    public void SetRotationY(float y)
    {
        rotation.y = y;
    }

    public void ResetRotation(int x, int y)
    {
        rotation.x = x;
        rotation.y = y;
    }

    public float GetSensibilitat()
    {
        return sensibilitat;
    }

    public void SetSensibilitat(float newSensibilitat)
    {
        sensibilitat = newSensibilitat;
    }
}