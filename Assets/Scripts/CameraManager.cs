using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public GameObject FishTank;
    public GameObject Viewport;
    public GameObject Player;
    public Transform Focus;
    public float RotationSensitivity = 1f;
    public float FishTankDistance = 3f;
    public float ZoomSensitivity = 8f;

    public enum CameraConfiguration { HMDCam, FishTankCam, ViewportCam};
    public CameraConfiguration cameraConfig = CameraConfiguration.HMDCam;

    private GameObject CameraRig;
    private GameObject VirtualScreen;
    private Camera FishtankCam;

    private bool isOribiting = false;
    private Vector3 originalMousePos;
    private Vector3 worldX;
    private Vector3 worldY;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    // Use this for initialization
    void Start () {
        Debug.Log("Laaa");
        CameraRig = Player.transform.Find("OVRCameraRig").gameObject;
        VirtualScreen = FishTank.transform.Find("VirtualScreenLocation").gameObject;
        FishtankCam = FishTank.transform.Find("FishtankCamera").gameObject.GetComponent<Camera>();
        //Debug.Log();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetConfig(CameraConfiguration.HMDCam);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetConfig(CameraConfiguration.FishTankCam);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetConfig(CameraConfiguration.ViewportCam);

        if (cameraConfig == CameraConfiguration.FishTankCam || cameraConfig == CameraConfiguration.ViewportCam)
            HandleZoom();
            HandleOrbit();
    }

    void HandleZoom()
    {
        float deltaZ = Input.GetAxis("Mouse ScrollWheel");
        FishTankDistance += ZoomSensitivity * deltaZ;
    }

    void HandleOrbit()
    {
        Camera currentCam;
        Transform currentCamXform;
        if (cameraConfig == CameraConfiguration.ViewportCam)
        {
            currentCam = Viewport.GetComponent<Camera>();
            currentCamXform = Viewport.transform;
        }
        else
        {
            currentCam = FishtankCam;
            currentCamXform = VirtualScreen.transform;
        }
            

        if (Input.GetMouseButtonDown(0))
        {
            originalMousePos = Input.mousePosition;
            
            worldX = (currentCam.ScreenToWorldPoint(new Vector3(100, 0, 1)) - currentCam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            worldY = (currentCam.ScreenToWorldPoint(new Vector3(0, 100, 1)) - currentCam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            //if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            //{
                isOribiting = true;
                originalPosition = currentCamXform.position;
                originalRotation = currentCamXform.rotation;
                //originalScale = currentCam.transform.lossyScale;
            //}


        }



        if (isOribiting && Focus != null)
        {
            Vector3 delta = Input.mousePosition - originalMousePos;
            currentCamXform.SetPositionAndRotation(originalPosition, originalRotation);
            currentCamXform.RotateAround(Focus.transform.position, worldX, RotationSensitivity * -delta.y);
            currentCamXform.RotateAround(Focus.transform.position, worldY, RotationSensitivity * delta.x);
            //currentCamXform.localScale = new Vector3(1/currentCamXform.parent.localScale.x, 1/currentCamXform.parent.localScale.y, 1/currentCamXform.parent.localScale.z);
            if (cameraConfig == CameraConfiguration.ViewportCam)
                currentCamXform.LookAt(Focus);
            else
            {
                currentCamXform.LookAt(Focus);
                FishtankCam.transform.LookAt(Focus);
                currentCamXform.localPosition *= (FishTankDistance / FishtankCam.transform.localPosition.magnitude); 
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isOribiting = false;
        }

    }

    void SetConfig(CameraConfiguration config)
    {
        Debug.Log(config);
        cameraConfig = config;
        switch (config)
        {
            case CameraConfiguration.HMDCam:
                Viewport.SetActive(false);
                CameraRig.SetActive(true);
                Player.SetActive(true);
                FishTank.SetActive(false);
                break;
            case CameraConfiguration.FishTankCam:
                ConfigureFishtank();
                break;
            case CameraConfiguration.ViewportCam:
                ConfigureViewport();
                break;
        }
    }

    void ConfigureViewport()
    {
        Viewport.SetActive(true);
        CameraRig.SetActive(false);
        Player.SetActive(false);
        FishTank.SetActive(false);
        if (Focus != null) {
            var Cam = Viewport.GetComponent<Camera>();
            Cam.transform.SetParent(Focus);
            Cam.transform.localPosition = new Vector3(0,0,-3);
            Cam.transform.LookAt(Focus);
        }
    }

    void ConfigureFishtank()
    {
        Viewport.SetActive(false);
        Player.SetActive(true);
        Player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        CameraRig.SetActive(false);
        FishTank.SetActive(true);
        if (Focus != null)
        {
            FishtankCam.transform.SetParent(Focus);
            VirtualScreen.transform.SetParent(Focus);
            VirtualScreen.transform.localPosition = new Vector3(0, 0, -FishTankDistance);
            VirtualScreen.transform.LookAt(Vector3.zero);
        }
    }

}
