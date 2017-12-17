using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateSunRelative : MonoBehaviour {
    public Transform sun;
    public Transform controller;

    private Quaternion inverseControllerRot;
    private Quaternion originalSunRot;
    

    private bool grabbed = false;
	
	// Update is called once per frame
	void Update () {
	    if (!grabbed && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) >= .95)
        {
            grabbed = true;
            Debug.Log("Grabbed");
            originalSunRot = sun.rotation;
            inverseControllerRot = Quaternion.Inverse(controller.rotation);
        } else if (grabbed && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) <.95)
        {
            Debug.Log("Grabbed");
            grabbed = false;
        } else if (grabbed)
        {
            Quaternion relativeRotation = controller.rotation * inverseControllerRot;
            // TODO: Axis align the rotation
            sun.transform.rotation = relativeRotation * originalSunRot;
            // TODO: If upsidown, reverse
            
        }


    }
}
