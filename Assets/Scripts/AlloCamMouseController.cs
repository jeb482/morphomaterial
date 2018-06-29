using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlloCamMouseController : MonoBehaviour {
    public float sensitivity = 1;
    public bool allowObjectRotation = true;
    public Transform Focus; 
    public CameraManager camManager;

    bool mousePressed = false;
    bool camMode = false;
    Vector3 originalMousePos;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalScale;

    Vector3 worldX;
    Vector3 worldY;


	// Use this for initialization
	void Start () {
		
	}



    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.targetObject == null || camManager.cameraConfig == CameraManager.CameraConfiguration.HMDCam)
            return;

        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
        {
            mousePressed = true;
            originalMousePos = Input.mousePosition;
            worldX = (camManager.ScreenToWorldPoint(new Vector3(100, 0, 1)) - camManager.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            worldY = (camManager.ScreenToWorldPoint(new Vector3(0, 100, 1)) - camManager.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            originalPosition = Focus.position;
            originalRotation = Focus.rotation;
            originalScale = Focus.localScale;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }

        if (mousePressed)
        {
            Vector3 delta = Input.mousePosition - originalMousePos;
            Debug.Log("Hey" + delta.x);
            Focus.SetPositionAndRotation(originalPosition, originalRotation);
            Focus.Rotate(worldX, sensitivity * delta.y, Space.World);
            Focus.Rotate(worldY, sensitivity * -delta.x, Space.World);
        }
    }
}
