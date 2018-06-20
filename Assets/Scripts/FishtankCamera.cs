﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishtankCamera : MonoBehaviour {
    public GameObject leftEyeTracker;
    public GameObject rightEyeTracker; // If only one tracker, set same as left?
    public float nearPlane;
    public float farPlane;
    public Transform virtualScreenXform;
    public float viewportHeight;
    public float viewportWidth;
    public Vector3 screenSpaceHeadPos;
    public FrustumPlanes frustumPlanes;

    private Camera cam;
    private Vector2 screenSpaceL;
    private Vector2 screenSpaceH;
   
    // Use this for initialization
    void Start () {
        updateWindowData();
        viewportHeight = (GameController.Instance.upperLeftScreenCorner - GameController.Instance.lowerLeftScreenCorner).magnitude;
        viewportWidth = (GameController.Instance.upperRightScreenCorner - GameController.Instance.upperLeftScreenCorner).magnitude;
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
        if (leftEyeTracker == null)
            return;
        UpdateViewFrustum();
    }

    void UpdateViewFrustum()
    {
        screenSpaceL = new Vector3(0, 0, 0);
        screenSpaceH = new Vector3(.475f, .3f, 0);

        Vector3 fishtankEyeOffset = new Vector3(0,0,0);
        if (GameController.Instance.fishtankEyeOffset != null)
            fishtankEyeOffset = GameController.Instance.fishtankEyeOffset;

        screenSpaceHeadPos = getTransformedEyePose(leftEyeTracker.transform.TransformPoint(fishtankEyeOffset));
        Vector3 virtualCameraPosition = virtualScreenXform.localToWorldMatrix.MultiplyPoint(screenSpaceHeadPos);
        transform.position = virtualCameraPosition;

        frustumPlanes.left = -screenSpaceHeadPos.x;
        frustumPlanes.right = screenSpaceH.x - screenSpaceHeadPos.x;
        frustumPlanes.bottom = -screenSpaceHeadPos.y;
        frustumPlanes.top = screenSpaceH.y - screenSpaceHeadPos.y;
        frustumPlanes.zNear = .001f - screenSpaceHeadPos.z;
        frustumPlanes.zFar = -100;

        cam.projectionMatrix = Matrix4x4.Frustum(frustumPlanes);
        return;
    }

    void updateWindowData()
    {
        Debug.Log("Window size: " + Screen.width + ", " + Screen.height);
        Debug.Log("Screen size: " + Screen.currentResolution.width + ", " + Screen.currentResolution.height);
    }
}
