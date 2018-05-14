using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public GameObject FishTank;
    public GameObject Viewport;
    public GameObject Player;

    public enum CameraConfiguration { HMDCam, FishTankCam, ViewportCam};
    public CameraConfiguration cameraConfig = CameraConfiguration.HMDCam;

    private GameObject CameraRig;
    // Use this for initialization
	void Start () {
        Debug.Log("Laaa");
        CameraRig = Player.transform.Find("OVRCameraRig").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        //if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        //    return;
        //Debug.Log("controlDown");
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetConfig(CameraConfiguration.HMDCam);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetConfig(CameraConfiguration.FishTankCam);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetConfig(CameraConfiguration.ViewportCam);
    }

    void SetConfig(CameraConfiguration config)
    {
        Debug.Log(config);
        cameraConfig = config;
        switch (config)
        {
            case CameraConfiguration.HMDCam:
                Viewport.SetActive(false);
                CameraRig.SetActive(true);
               // CameraRig.GetComponent<Camera>().enabled = true;
                Player.SetActive(true);
                FishTank.SetActive(false);
                break;
            case CameraConfiguration.FishTankCam:
                Viewport.SetActive(false);
                Player.SetActive(true);
                Player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                CameraRig.SetActive(false);
                //CameraRig.GetComponent<Camera>().enabled = false;
                FishTank.SetActive(true);
                break;
            case CameraConfiguration.ViewportCam:
                Viewport.SetActive(true);
                CameraRig.SetActive(false);
                Player.SetActive(false);
                FishTank.SetActive(false);
                break;
        }
    }

}
