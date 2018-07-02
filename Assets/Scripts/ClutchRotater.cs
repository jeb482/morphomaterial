using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutchRotater : MonoBehaviour {
    public Transform controller;

    bool clutchActive = false;
    Quaternion startingControllerRotation;
    Quaternion startingRotation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float gripAxis = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        //Debug.Log(gripAxis);
        if (!clutchActive &&  gripAxis > 0.8)
        {
            clutchActive = true;
            startingControllerRotation = controller.rotation;
            startingRotation = CameraManager.Instance.Focus.rotation;
        } else if (clutchActive && gripAxis < 0.1)
        {
            clutchActive = false;
        }
        else if (clutchActive)
        {
            Quaternion absoluteRotation = controller.rotation * Quaternion.Inverse(startingControllerRotation);
            Quaternion world2Screen = GameController.Instance.realWorldToScreen.rotation;
            Quaternion world2View = CameraManager.Instance.GetCameraOrientation(); 
            switch (CameraManager.Instance.cameraConfig)
            {
                case CameraManager.CameraConfiguration.HMDCam:
                    CameraManager.Instance.Focus.rotation = absoluteRotation * startingRotation;
                    break;
                case CameraManager.CameraConfiguration.ViewportCam:
                    CameraManager.Instance.Focus.rotation =   world2Screen * world2View * (absoluteRotation) *  Quaternion.Inverse(world2View) * Quaternion.Inverse(world2Screen) * startingRotation;
                    break;
                case CameraManager.CameraConfiguration.FishTankCam:
                    CameraManager.Instance.Focus.rotation = (absoluteRotation) * world2View * Quaternion.Inverse(world2Screen) * startingRotation;
                    break;

            }
        }
            
    }
}
