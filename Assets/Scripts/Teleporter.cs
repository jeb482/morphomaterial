using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    private bool selecting = false;
    private GameObject intersectionTarget = null;
    private GameObject splat = null; 
	// Use this for initialization
	void Start () {
        Debug.Log("Up.");
	}
	
	// Update is called once per frame
	void Update () {
	    if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown))
        {
            Debug.Log("splat");
            splat = (GameObject) Resources.Load("PortTarget");
            selecting = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickDown))
        {
            Debug.Log("unsplat");
            Destroy(splat);
            splat = null;
        }
        else if (selecting) {
            Ray r = new Ray(transform.position, transform.TransformPoint(new Vector3(0,0,1)));
            RaycastHit hit;
            if (Physics.Raycast(r, out hit))
            {
                splat.transform.position = hit.point;
                splat.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), hit.normal);
            }
        }
           
        
        	
	}
}
