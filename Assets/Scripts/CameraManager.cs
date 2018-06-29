using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager Instance;

    public GameObject FishTank;
    public GameObject Viewport;
    public GameObject Player;
    public Transform Focus;
    public Transform EyeLocation;
    
    public float RotationSensitivity = 1f;
    public float CameraDistance = 3f;
    public float ZoomSensitivity = 8f;
    public float MinimumCameraDistance = 1f;
    public float MaximumCameraDistance = 8f;

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
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        switch (cameraConfig)
        {
            case CameraConfiguration.FishTankCam:
                fishTankCam.modifyScale(zoomInput);
                break;
            case CameraConfiguration.ViewportCam:
                GetOrbitInput(Viewport.GetComponent<Camera>(), Viewport.transform);
                Draw(Viewport.GetComponent<Camera>(), Viewport.transform, zoomInput);
                break;
            default:
                return;
        }
        
        

        LastFocus = Focus;
    }

    void GetOrbitInput(Camera currentCam, Transform currentCamXform)
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            originalMousePos = Input.mousePosition;
            
            worldX = (currentCam.ScreenToWorldPoint(new Vector3(100, 0, 1)) - currentCam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            worldY = (currentCam.ScreenToWorldPoint(new Vector3(0, 100, 1)) - currentCam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            
            isOribiting = true;
            originalPosition = currentCamXform.position;
            originalRotation = currentCamXform.rotation;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isOribiting = false;
        }
    }
     
    /// <summary>
    /// Sets the camera transformation given it's orinetation and distance with respect to the Focus
    /// </summary>
    void Draw(Camera currentCam, Transform currentCamXform, float zoomInput)
    {

        CameraDistance = System.Math.Min(MaximumCameraDistance, System.Math.Max(MinimumCameraDistance, CameraDistance + zoomInput));
        // Update transformation based on orbit
        if (isOribiting && Focus != null)
        {
            Vector3 delta = Input.mousePosition - originalMousePos;
            currentCamXform.SetPositionAndRotation(originalPosition, originalRotation);

            // Don't let user rotate over y axis.{
            var oldViewDir = currentCamXform.InverseTransformVector(new Vector3(0, 0, 1)).normalized;
            currentCamXform.RotateAround(Focus.transform.position, worldX, RotationSensitivity * -delta.y);
            var newViewDir = currentCamXform.InverseTransformVector(new Vector3(0, 0, 1)).normalized;
            if ((lockDeltaY && System.Math.Abs(delta.y) > System.Math.Abs(lastDeltaY)) || (System.Math.Sign(newViewDir.x) != System.Math.Sign(oldViewDir.x) || System.Math.Sign(newViewDir.z) != System.Math.Sign(oldViewDir.z) && (oldViewDir.x != 0 || oldViewDir.z != 0)))
            {
                currentCamXform.RotateAround(Focus.transform.position, worldX, RotationSensitivity * delta.y - lastDeltaY);
                lockDeltaY = true;
            }
            else
            {
                lockDeltaY = false;
                lastDeltaY = delta.y;
                currentCamXform.RotateAround(Focus.transform.position, worldY, RotationSensitivity * delta.x);
            }

            // Rotate about Y
            currentCamXform.LookAt(Focus);
            if (Focus != null)
                currentCamXform.localPosition *= (CameraDistance / currentCamXform.localPosition.magnitude);
           
        }

//        // Update distance
//        if (Focus != null)
//        {
//            currentCamXform.localPosition *= (CameraDistance / currentCamXform.localPosition.magnitude);
//        }
    }

    void SetConfig(CameraConfiguration config)
    {
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
            fishTankCam.focus = Focus;
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

    public void RecenterFishTank()
    {
        Debug.Log("Re-centering FishTank");

        // The vector from the screen center to the user's eye .
        Vector3 oldScreenCenter = (GameController.Instance.lowerLeftScreenCorner + GameController.Instance.upperRightScreenCorner) / 2;
        Vector3 centerDiff = EyeLocation.TransformPoint(GameController.Instance.fishtankEyeOffset) - oldScreenCenter;

        // The vector of the user's nose points in.
        Vector3 headForward = EyeLocation.TransformPoint(GameController.Instance.fishtankEyeOffset) - FishtankCamObj.transform.position;
        headForward.y = 0;
        headForward.Normalize();

        // Subtract out component of difference parallel to user's gaze.
        centerDiff -= Vector3.Dot(centerDiff, headForward) * headForward;


        GameController.Instance.lowerLeftScreenCorner += centerDiff;
        GameController.Instance.upperLeftScreenCorner += centerDiff;
        GameController.Instance.upperRightScreenCorner += centerDiff;

        GameController.Instance.updateRealWorldToScreen();

        //ConfigureFishtank();
    }

}
