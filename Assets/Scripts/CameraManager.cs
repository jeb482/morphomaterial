using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The CameraManager is a singleton class which switches between the Viewport,
/// Fishtank, and HMD configurations, enabling and disabling the appropriate
/// components in each configuration, and calling appropriate methods from
/// the Viewport, Fishtank, and HMD apparatuses from frame to frame.
/// </summary>
public class CameraManager : MonoBehaviour {
    public static CameraManager Instance;

    public GameObject FishTank;
    public GameObject Viewport;
    public GameObject Player;
    public Transform Focus;
    public Transform EyeLocation;
    
    public enum CameraConfiguration { HMDCam, FishTankCam, ViewportCam};
    public CameraConfiguration cameraConfig = CameraConfiguration.HMDCam;

    private GameObject CameraRig;
    private GameObject FishtankCamObj;
    //private Camera FishtankCam;

    private bool isOribiting = false;
    private Vector3 originalMousePos;
    private Vector3 worldX;
    private Vector3 worldY;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float lastDeltaY = 0;
    private bool lockDeltaY = false;
    private FishtankCamera fishTankCam;
    private ViewportCamera viewportCam;
    private GameObject HMDAnchor;
    private Transform LastFocus;
    


    // Create as a singleton
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        CameraRig = Player.transform.Find("OVRCameraRig").gameObject;
        FishtankCamObj = FishTank.transform.Find("FishtankCamera").gameObject;
        fishTankCam = FishtankCamObj.GetComponent<FishtankCamera>();
        viewportCam = Viewport.GetComponent<ViewportCamera>();
        HMDAnchor = CameraRig.transform.Find("TrackingSpace").Find("CenterEyeAnchor").gameObject;
        //FishtankCam = FishtankCamObj.GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        // Set the configuration
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetConfig(CameraConfiguration.HMDCam);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetConfig(CameraConfiguration.FishTankCam);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetConfig(CameraConfiguration.ViewportCam);
        else if (LastFocus != Focus)
            SetConfig(cameraConfig);

        // Update camera
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                //fishTankCam.orbitAndZoom();
                fishTankCam.modifyScale();
                break;
            case CameraConfiguration.ViewportCam:
                viewportCam.orbitAndZoom();
                break;
            default:
                return;
        }
        
        // Guard against focus being changed externally
        LastFocus = Focus;
    }

    void SetConfig(CameraConfiguration config)
    {
        cameraConfig = config;
        switch (config)
        {
            case CameraConfiguration.HMDCam:
                Viewport.SetActive(false);
                CameraRig.SetActive(true);
                HMDAnchor.SetActive(true);
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
        Player.SetActive(true);
        CameraRig.SetActive(true);
        HMDAnchor.SetActive(false);
        FishTank.SetActive(false);
        if (Focus != null) {
            var Cam = Viewport.GetComponent<Camera>();
            viewportCam.Focus = Focus;
            
            Cam.transform.position = Focus.transform.TransformPoint(new Vector3(0,0,-3));
            Cam.transform.LookAt(Focus);
        }
    }

    void ConfigureFishtank()
    {
        Viewport.SetActive(false);
        Player.SetActive(true);
        HMDAnchor.SetActive(false);
        Player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        CameraRig.SetActive(false);
        FishTank.SetActive(true);
        if (Focus != null)
        {
            fishTankCam.Focus = Focus;
        }
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                return FishtankCamObj.GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
            case CameraConfiguration.ViewportCam:
                return Viewport.GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
            default:
                return new Vector3(0, 0, 0);
        }
    }


    /// <summary>
    /// Returns a unit vector in the world space direction of the active camera's gaze.
    /// </summary>
    /// <returns></returns>
    public Vector3 getViewDirection()
    {
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                return FishtankCamObj.GetComponent<Camera>().transform.TransformVector(new Vector3(0,0,1)).normalized;
            case CameraConfiguration.ViewportCam:
                return Viewport.GetComponent<Camera>().transform.TransformVector(new Vector3(0, 0, 1)).normalized;
            default:
                return new Vector3(0, 0, 0);
        }
    }

    public Vector3 GetCamPos()
    {
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                return FishtankCamObj.GetComponent<Camera>().transform.position;
            case CameraConfiguration.ViewportCam:
                return Viewport.GetComponent<Camera>().transform.position;
            default:
                return new Vector3(0, 0, 0);
        }
    }

    public Ray ScreenPointToRay(Vector3 screenPoint)
    {
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                return FishtankCamObj.GetComponent<Camera>().ScreenPointToRay(screenPoint);
            case CameraConfiguration.ViewportCam:
                return Viewport.GetComponent<Camera>().ScreenPointToRay(screenPoint);
            default:
                return new Ray();
        }
    }

    public Quaternion GetCameraOrientation()
    {
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                return FishtankCamObj.GetComponent<Camera>().transform.rotation;
            case CameraConfiguration.ViewportCam:
                return Viewport.GetComponent<Camera>().transform.rotation;
            default:
                return HMDAnchor.transform.rotation;
        }
    }

    public void SetView(float longitude, float latitude, float zoom)
    {
        Debug.Log(longitude + " " + latitude + " " + zoom);
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                return;
            case CameraConfiguration.ViewportCam:
                viewportCam.SetView(longitude, latitude, zoom);
                return;
            default:
                return;
        }
    }
}
