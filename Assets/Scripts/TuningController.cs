using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TuningTrialController : MonoBehaviour
{

    public GameObject object1;
    public GameObject object2;
    
    public Transform object1Origin;
    public Transform object2Origin;
    public JoystickOrbit JoystickController;
    public float TuningIncrement = 1;
    public int trialNum;
    private int lastTrialNum;
    public List<float> diffractionSpacings = new List<float>();
    public float minGrating = 200;
    public float maxGrating = 1400;


    private void Start()
    {
        diffractionSpacings.Add(400);
        diffractionSpacings.Add(800);
        diffractionSpacings.Add(600);
    }

    void Update()
    {


        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            if (JoystickController != null)
            {
                Debug.Log("Swapped focus");
                JoystickController.flipLongitude();
                JoystickController.jitter(5);
            }


            if (CameraManager.Instance.Focus == object1Origin)
                CameraManager.Instance.Focus = object2Origin;
            else
                CameraManager.Instance.Focus = object1Origin;
        }

        handleTuning();

    }

    void handleTuning()
    {
        float delta = 0;
        if (CameraManager.Instance.cameraConfig == CameraManager.CameraConfiguration.HMDCam)
        {
            delta = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
            // HMD CONTROLS
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                delta = 1;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                delta = -1;
            }
        }

        var mat = object2.GetComponent<Renderer>().material;
        mat.SetFloat("_Distance", System.Math.Min(System.Math.Max(mat.GetFloat("_Distance") + delta, minGrating), maxGrating));

    }

    public class TuningTrialDescription
    {

    }
}
