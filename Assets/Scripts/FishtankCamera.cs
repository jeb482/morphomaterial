using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishtankCamera : MonoBehaviour {
    public GameObject leftEyeTracker;
    public GameObject rightEyeTracker; // If only one tracker, set same as left?
    public Transform focus;
    Vector2 viewportSize = new Vector2(.475f, .3f);
    public Vector3 screenSpaceHeadPos;
    public float viewScale = 1;
    public Vector2 screenDims = new Vector2(.475f,.3f);
    public FrustumPlanes frustumPlanes;
    public float zNear = .1f;
    public float zFar = 10f;

    private Camera cam;
    private Vector2 screenSpaceL;
    private Vector2 screenSpaceH;
    private Matrix4x4 virtualScreenToWorldMat = new Matrix4x4(); 
    // Use this for initialization
    void Start () {
        updateWindowData();
        //viewportHeight = (GameController.Instance.upperLeftScreenCorner - GameController.Instance.lowerLeftScreenCorner).magnitude;
        //viewportWidth = (GameController.Instance.upperRightScreenCorner - GameController.Instance.upperLeftScreenCorner).magnitude;
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
        transform.position = focus.TransformPoint(screenSpaceHeadPos);
        //transform.LookAt(focus.position);

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

    void UpdateViewFrustum()
    {
        screenSpaceL = new Vector3(0, 0, 0);
        screenSpaceH = new Vector3(.475f, .3f, 0);

        // Offset from controller to eye
        Vector3 fishtankEyeOffset = new Vector3(0,0,0);
        if (GameController.Instance.fishtankEyeOffset != null)
            fishtankEyeOffset = GameController.Instance.fishtankEyeOffset;



        // Get 
        screenSpaceHeadPos = getTransformedEyePose(leftEyeTracker.transform.TransformPoint(fishtankEyeOffset));
        //virtualScreenXform.localPosition= new Vector3(-.475f,-.3f,0);
        //Vector3 virtualCameraPosition = virtualScreenXform.localToWorldMatrix.MultiplyPoint(screenSpaceHeadPos);
        //transform.rotation = virtualScreenXform.rotation;
        //transform.position = viewScale*virtualCameraPosition;

        // screenSpaceHeadPos = getTransformedEyePose(leftEyeTracker.transform.TransformPoint(fishtankEyeOffset));
        // virtualScreenXform.localPosition = new Vector3(-screenScale * screenDims.x / 2, -screenScale * screenDims.y / 2, 0);
        // virtualScreenXform.localScale = new Vector3(screenScale * screenDims.x, screenScale * screenDims.y, screenScale);
        // Vector3 virtualCameraPosition = virtualScreenXform.localToWorldMatrix.MultiplyPoint(screenSpaceHeadPos);
        // //virtualScreenToWorldMat.SetTRS(virtualScreenXform.TransformPoint(screenDims.x / 2, screenDims.y / 2, 0),
        // //    virtualScreenXform.rotation,
        // //    new Vector3(screenScale * screenDims.x, screenScale * screenDims.y, screenScale));
        // //Vector3 virtualCameraPosition = virtualScreenToWorldMat.MultiplyPoint(screenSpaceHeadPos);
        // transform.position = virtualCameraPosition;


        frustumPlanes.zNear = .1f; //- ;

        float nearPlaneScale = -(.1f/screenSpaceHeadPos.z);//((screenSpaceHeadPos.z + frustumPlanes.zNear) / screenSpaceHeadPos.z);

        frustumPlanes.left = nearPlaneScale*(-screenSpaceHeadPos.x);
        frustumPlanes.right = nearPlaneScale*(screenSpaceH.x - screenSpaceHeadPos.x);
        frustumPlanes.bottom = nearPlaneScale*(-screenSpaceHeadPos.y);
        frustumPlanes.top = nearPlaneScale*(screenSpaceH.y - screenSpaceHeadPos.y);
        frustumPlanes.zFar = -10f;

        cam.projectionMatrix = Matrix4x4.Frustum(frustumPlanes);
        return;
    }

    void updateWindowData()
    {
        //Debug.Log("Window size: " + Screen.width + ", " + Screen.height);
        //Debug.Log("Screen size: " + Screen.currentResolution.width + ", " + Screen.currentResolution.height);
    }
}
