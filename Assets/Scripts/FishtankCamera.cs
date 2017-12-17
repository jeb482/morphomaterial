using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishtankCamera : MonoBehaviour {
    public GameObject leftEyeTracker;
    public GameObject rightEyeTracker; // If only one tracker, set same as left?
    public float nearPlane;
    public float farPlane;
    public Transform virtualScreenXform;

    private Camera cam;
    private Matrix4x4 VRToScreenSpace;
    private Vector2 screenSpaceL;
    private Vector2 screenSpaceH;
    


    // Use this for initialization
    void Start () {
		cam = GetComponent<Camera>();
        updateScreenSpaceTransform();
        Debug.Log("La   " + cam.projectionMatrix);
    }

    /**
     *  Updates the vRToScreenSpace transform based on vr screen coordinates.
     */
    void updateScreenSpaceTransform()
    {
        if ((GameController.Instance.lowerLeftScreenCorner == null) ||
            (GameController.Instance.upperLeftScreenCorner == null) || 
            (GameController.Instance.upperRightScreenCorner == null))
            return;

        Vector3 U = GameController.Instance.upperRightScreenCorner - GameController.Instance.upperLeftScreenCorner;
        Vector3 V = (GameController.Instance.upperLeftScreenCorner - GameController.Instance.lowerLeftScreenCorner).normalized;
        Vector3 W = Vector3.Cross(U, V).normalized;
        U = Vector3.Cross(V, W).normalized;

        VRToScreenSpace.SetRow(0, new Vector4(U.x, V.x, W.x, -GameController.Instance.lowerLeftScreenCorner.x));
        VRToScreenSpace.SetRow(1, new Vector4(U.y, V.y, W.y, -GameController.Instance.lowerLeftScreenCorner.y));
        VRToScreenSpace.SetRow(2, new Vector4(U.z, V.z, W.z, -GameController.Instance.lowerLeftScreenCorner.z));
        VRToScreenSpace.SetRow(3, new Vector4(0,     0,   0,                   1));

        Vector3 H = VRToScreenSpace.MultiplyPoint(GameController.Instance.upperRightScreenCorner);
        Vector3 L = VRToScreenSpace.MultiplyPoint(GameController.Instance.lowerLeftScreenCorner);
        screenSpaceH = new Vector2(H.x, H.y);
        screenSpaceL = new Vector2(L.x, L.y);
    }



    /**
     * Gives the OpenGL camera matrix of a real-world user looking at a screen.
     * 
     * From "High resolution virtual reality," Deering M., SIGGRAPH '92 
     * https://dl.acm.org/citation.cfm?id=134039
     * 
     * Parameters:
     * E - The eye location with respect to the coordinate system of the screen.
     * H - The upper right-hand corner of the viewport in screen space
     * L - The lower right-hand corner of the viewport in screen space
     * N - Near plane (z-axis coordinate)
     * F - Far plane (z-axis coordinate)
     */
    Matrix4x4 getFishtankMatrix(Vector3 E, Vector2 H, Vector2 L, float N, float F)
    {
        Matrix4x4 P = new Matrix4x4();
        P[0, 0] = 2*E.z / (H.x - L.x);
        P[0, 1] = 0;
        P[0, 2] = (H.x + L.x - 2 * E.x) / (H.x - L.x);
        P[0, 3] = (-E.z * (H.x + L.x)) / (H.x - L.x);

        P[1, 0] = 0;
        P[1, 1] = 2*E.z / (H.y - L.y);
        P[1, 2] = (H.y + L.y - 2 * E.y) / (H.y - L.y);
        P[1, 3] = (-E.z * (H.y + L.y)) / (H.y - L.y);

        P[2, 0] = 0;
        P[2, 1] = 0;
        P[2, 2] = (N + F - 2*E.z) / (N - F);
        P[2, 3] = N - E.z - (N*((N + F - 2*E.z) / (N - F)));

        P[3, 0] = 0;
        P[3, 1] = 0;
        P[3, 2] = -1;
        P[3, 3] = E.z;

     //   Debug.Log(P);
        return P;
    }
         
    Matrix4x4 getProjectionMatrix(float near, float far, float width, float height)
    {
        Matrix4x4 P = new Matrix4x4();

        P[0, 0] = 2*near / width;
        P[0, 1] = 0;
        P[0, 2] = 0;
        P[0, 3] = 0;

        P[1, 0] = 0;
        P[1, 1] = 2*near / height;
        P[1, 2] = 0;
        P[1, 3] = 0;

        P[2, 0] = 0;
        P[2, 1] = 0;
        P[2, 2] = -(far + near) / (far - near);
        P[2, 3] = (-2*far*near) / (far - near);

        P[3, 0] = 0;
        P[3, 1] = 0;
        P[3, 2] = -1;
        P[3, 3] = 0;

        return P;
    }

    Vector3 getTransformedEyePose(Vector3 worldSpacePosition)
    {
        Vector3 origin = GameController.Instance.lowerLeftScreenCorner;
        Vector3 x = (GameController.Instance.upperRightScreenCorner - GameController.Instance.upperLeftScreenCorner).normalized;
        Vector3 y = (GameController.Instance.upperLeftScreenCorner - GameController.Instance.lowerLeftScreenCorner).normalized;
        Vector3 z = Vector3.Cross(x, y);
        x = Vector3.Cross(y, z);

        var v = worldSpacePosition - origin;

        return new Vector3(Vector3.Dot(v,x), Vector3.Dot(v,y), Vector3.Dot(v,z));
    }

    // Update is called once per frame
    void Update () {
        if (leftEyeTracker == null)
            return;
        NaiveUpdate();
    }

    void NaiveUpdate()
    {

        Vector3 screenSpaceCamPos = getTransformedEyePose(leftEyeTracker.transform.position);
        Vector3 virtualCameraPosition = virtualScreenXform.localToWorldMatrix.MultiplyPoint(screenSpaceCamPos);

        cam.worldToCameraMatrix = Matrix4x4.LookAt(virtualCameraPosition, virtualScreenXform.position, GameController.Instance.upperLeftScreenCorner - GameController.Instance.lowerLeftScreenCorner);
    }

    void AccurateUpdate()
    {
        screenSpaceL = new Vector3(0, 0);
        screenSpaceH = new Vector3(1, 1);

        // Debug.Log("Pose " + leftEyeTracker.GetComponent<Transform>().localToWorldMatrix);
        Vector3 leftEyePos = leftEyeTracker.GetComponent<Transform>().localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, 0));

        // Debug.Log(leftEyePos);

        // Debug.Log("Tracker Pose " + leftEyePos);

        cam.projectionMatrix = getFishtankMatrix(leftEyePos, screenSpaceH, screenSpaceL, nearPlane, farPlane);
        //cam.projectionMatrix = getProjectionMatrix(nearPlane, farPlane, 1, 2);

        return;
    }
}
