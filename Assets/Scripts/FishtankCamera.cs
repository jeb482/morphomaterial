using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishtankCamera : MonoBehaviour {

    public Vector3 screenBottomLeft;
    public Vector3 screenTopLeft;
    public Vector3 screenTopRight;
    public GameObject leftEyeTracker;
    public GameObject rightEyeTracker; // If only one tracker, set same as left?
    public float nearPlane;
    public float farPlane;

    private Camera cam;
    private Matrix4x4 VRToScreenSpace;
    private Vector2 screenSpaceL;
    private Vector2 screenSpaceH;
    


    // Use this for initialization
    void Start () {
		cam = GetComponent<Camera>();
        updateScreenSpaceTransform();
    }

    /**
     *  Updates the vRToScreenSpace transform based on vr screen coordinates.
     */
    void updateScreenSpaceTransform()
    {
        if (screenBottomLeft == null || screenTopLeft == null || screenTopRight == null)
            return;

        Vector3 U = screenTopRight - screenTopLeft;
        Vector3 V = (screenTopLeft  - screenBottomLeft).normalized;
        Vector3 W = Vector3.Cross(U, V).normalized;
        U = Vector3.Cross(V, W).normalized;

        VRToScreenSpace.SetRow(0, new Vector4(U.x, V.x, W.x, -screenBottomLeft.x));
        VRToScreenSpace.SetRow(1, new Vector4(U.y, V.y, W.y, -screenBottomLeft.y));
        VRToScreenSpace.SetRow(2, new Vector4(U.z, V.z, W.z, -screenBottomLeft.z));
        VRToScreenSpace.SetRow(3, new Vector4(0,     0,   0,                   1));

        Vector3 H = VRToScreenSpace.MultiplyPoint(screenTopRight);
        Vector3 L = VRToScreenSpace.MultiplyPoint(screenBottomLeft);
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
        P[2, 3] = N - E.z - N*((N + F - 2*E.z) / (N - F));

        P[3, 0] = 0;
        P[3, 1] = 0;
        P[3, 2] = -1;
        P[3, 3] = E.z;
        return P;
    }
         

    // Update is called once per frame
    void Update () {
        if (leftEyeTracker == null || rightEyeTracker == null)
            return;

        Vector3 leftEyePos = leftEyeTracker.GetComponent<Transform>().position;
        Vector3 rightEyePos = rightEyeTracker.GetComponent<Transform>().position;

        cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, getFishtankMatrix(VRToScreenSpace.MultiplyPoint(leftEyePos),screenSpaceH, screenSpaceL, nearPlane, farPlane));
        cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, getFishtankMatrix(VRToScreenSpace.MultiplyPoint(rightEyePos), screenSpaceH, screenSpaceL, nearPlane, farPlane));
    }
}
