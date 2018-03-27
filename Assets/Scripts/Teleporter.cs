using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    private bool selecting = false;
    private GameObject intersectionTarget = null;
    private GameObject splat = null;
    private LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {
        Debug.Log("Begin.");
        splat = Instantiate(Resources.Load("PortTarget"), null) as GameObject;
        lineRenderer = new LineRenderer();
    }
	
	// Update is called once per frame
	void Update () {
	    if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            Debug.Log("splat");
            splat.SetActive(true);
            selecting = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick))
        {
            Debug.Log("unsplat");
            splat.SetActive(false);
            selecting = false;
            
        }
        else if (selecting) {
            Ray r = new Ray(transform.position, transform.TransformDirection(new Vector3(0,0,1)));
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 16))
            {
                splat.SetActive(true);
                Debug.Log(hit.point);
                splat.transform.position = hit.point;
                splat.transform.rotation = Quaternion.FromToRotation(hit.normal, new Vector3(0, 1, 0));
            } else
            {
                splat.SetActive(false);
            }
        }
           
        
        	
	}
}
