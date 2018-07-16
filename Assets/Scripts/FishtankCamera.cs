using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishtankCamera : MonoBehaviour {
    public GameObject leftEyeTracker;
    public GameObject rightEyeTracker; // If only one tracker, set same as left?
    public Transform Focus;
    Vector2 viewportSize = new Vector2(.475f, .3f);
    public Vector3 screenSpaceHeadPos;
    public float viewScale = 1;
    public Vector2 validScaleRange = new Vector2(0.1f,3);
    public float scaleSensitivity = 1;
    public float RotationSensitivity = 1;

    public Vector2 screenDims = new Vector2(.475f,.3f);
    public FrustumPlanes frustumPlanes;
    public float zNear = .1f;
    public float zFar = 10f;

    private Quaternion orbitRotation = Quaternion.identity;
    private Camera cam;
    private Vector2 screenSpaceL;
    private Vector2 screenSpaceH;
    private Matrix4x4 virtualScreenToWorldMat = new Matrix4x4();
    private bool isOrbiting = false;
    private Vector3 lastMousePos;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    Transform parentTransform;
    


    // Use this for initialization
    void Start () {
        updateWindowData();
        cam = GetComponent<Camera>();
    }



    /// <summary>
    /// Returns the position of the eye in the coordinate space of the physical screen.
    /// </summary>
    /// <param name="worldSpacePosition"></param>
    /// <returns></returns>
    Vector3 getTransformedEyePose(Vector3 worldSpacePosition)
    {
        return GameController.Instance.realWorldToScreen.MultiplyPoint3x4(worldSpacePosition);
    }


    public void orbitAndZoom()
    {
        return;
        // Establish whether or not we are orbiting
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            isOrbiting = true; lastMousePos = Input.mousePosition;
            lastMousePos = Input.mousePosition;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isOrbiting = false;
            return;
        }

        // Handle input for revolution.
        if (isOrbiting && Focus != null)
        {
            Vector3 screenX = (cam.ScreenToWorldPoint(new Vector3(100, 0, 1)) - cam.ScreenToWorldPoint(new Vector3(0, 0, 1)));
            screenX.y = 0;
            screenX.Normalize();
            Vector3 screenY = (cam.ScreenToWorldPoint(new Vector3(0, 100, 1)) - cam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;

            Vector3 delta = Input.mousePosition - lastMousePos;
            Debug.Log(delta);
            Vector3 oldViewDir = transform.InverseTransformVector(new Vector3(0, 0, 1)).normalized;
            transform.RotateAround(Focus.transform.position, screenX, RotationSensitivity * -delta.y);
            Vector3 newViewDir = transform.InverseTransformVector(new Vector3(0, 0, 1)).normalized;

            if (System.Math.Sign(newViewDir.x) != System.Math.Sign(oldViewDir.x) ||
                System.Math.Sign(newViewDir.z) != System.Math.Sign(oldViewDir.z) &&
                (oldViewDir.x != 0 || oldViewDir.z != 0))
            {
                transform.RotateAround(Focus.transform.position, screenX, RotationSensitivity * delta.y);
            }
            transform.RotateAround(Focus.transform.position, new Vector3(0, 1, 0), RotationSensitivity * delta.x);
            lastMousePos = Input.mousePosition;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        modifyScale();

    }

    public void modifyScale()
    {
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        //.3viewScale = System.Math.Min(validScaleRange.y, System.Math.Max(validScaleRange.x, viewScale + scaleSensitivity* zoomInput));
    }


    // Update is called once per frame
    void Update () {
        updateWindowData();
        if (leftEyeTracker == null)
            return;
        UpdateViewFrustumFocus();
    }

    void UpdateViewFrustumFocus()
    {
        Vector2 screemDims = new Vector2(.475f, .3f);

        // Deal with offset from controller to eye
        Vector3 fishtankEyeOffset = new Vector3(0, 0, 0);
        if (GameController.Instance.fishtankEyeOffset != null)
            fishtankEyeOffset = GameController.Instance.fishtankEyeOffset;

        // Get head pos in Screen Frame
        screenSpaceHeadPos = getTransformedEyePose(leftEyeTracker.transform.TransformPoint(fishtankEyeOffset));

        // Align with virtual screen;
        
        transform.position = Focus.position + orbitRotation * screenSpaceHeadPos;//*viewScale;//Focus.transform.TransformPoint(viewScale*screenSpaceHeadPos);//orbitRotation * screenSpaceHeadPos;//Focus.position+ orbitRotation*screenSpaceHeadPos;
        transform.rotation = orbitRotation;

        // Construct a viewing frustum intersecting the screen.
        frustumPlanes.zNear = zNear;
        float screenToNearScale = -frustumPlanes.zNear / screenSpaceHeadPos.z;
        float halfS = .5f * viewScale;
        frustumPlanes.top    = screenToNearScale*( halfS*screenDims.y - screenSpaceHeadPos.y);
        frustumPlanes.right  = screenToNearScale*( halfS*screenDims.x - screenSpaceHeadPos.x);
        frustumPlanes.bottom = screenToNearScale*(-halfS*screenDims.y - screenSpaceHeadPos.y);
        frustumPlanes.left   = screenToNearScale*(-halfS*screenDims.x - screenSpaceHeadPos.x);
        frustumPlanes.zFar = zFar;
        cam.projectionMatrix = Matrix4x4.Frustum(frustumPlanes);
    }

    public void SetView(float longitude, float latitude, float zoom)
    {
        orbitRotation = Quaternion.AngleAxis(longitude* 180/(float)System.Math.PI, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(latitude * 180 / (float)System.Math.PI, new Vector3(1,0,0));
        viewScale = zoom;
    }

    void updateWindowData()
    {
        //Debug.Log("Window size: " + Screen.width + ", " + Screen.height);
        //Debug.Log("Screen size: " + Screen.currentResolution.width + ", " + Screen.currentResolution.height);
    }
}
