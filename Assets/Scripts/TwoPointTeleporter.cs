using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPointTeleporter : MonoBehaviour {
    //public Color pathColor;
    public GameObject point1;

    public GameObject point2;
    public GameObject player;
    public GameObject HMD;

    bool validTarget = false;
    private bool isOnPoint1;
    private bool aboutFaceOnPositionChange = true;
	// Use this for initialization
	void Start () {
        isOnPoint1 = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (CameraManager.Instance.cameraConfig != CameraManager.CameraConfiguration.HMDCam)
            return;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            Vector3 otherPoint, currentPoint, newPos;
            if (isOnPoint1)
            {
                currentPoint = point1.transform.position;
                otherPoint = point2.transform.position;
            } else
            {
                currentPoint = point2.transform.position;
                otherPoint = point1.transform.position;
            }

            if (aboutFaceOnPositionChange)
            {
                newPos = otherPoint - (player.transform.position - currentPoint);
                player.transform.Rotate(0, 180, 0, Space.Self);
            } else
            {
                newPos = otherPoint + (player.transform.position - currentPoint);
            }

            newPos.y = player.transform.position.y;
            player.transform.position = newPos;
            isOnPoint1 = !isOnPoint1;
        }	
	}
}
