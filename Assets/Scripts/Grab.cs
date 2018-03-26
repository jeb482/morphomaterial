using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    public float grabRadius;
    public LayerMask grabMask;

    private GameObject grabbedObject;
    private bool grabbing = false;
	
    void GrabObject()
    {
        grabbing = true;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);        
        if (hits.Length > 0)
        {
            Debug.Log("Grabbed");
            
            int closestHit = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < hits[closestHit].distance)
                    closestHit = i;
            }
            grabbedObject = hits[closestHit].transform.gameObject;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            grabbedObject.transform.position = transform.position;
            grabbedObject.transform.parent = transform;
            // Grab the object
        }
    }

    void DropObject()
    {
        grabbing = false;
        if (grabbedObject != null)
        {
            Debug.Log("Dropped");
            grabbedObject.transform.parent = null;
            grabbedObject = null;
        }

    }

	// Update is called once per frame
	void Update () {
        //;
        if (!grabbing && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.9)
        {
            Debug.Log("Trying to grab: " + OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger));
            GrabObject();
        }
        else if (grabbing && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) < 0.9)
            DropObject();
	}
}
