using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    public float grabRadius;
    public LayerMask grabMask;

    private GameObject grabbedObject;
    private bool grabbing;
	
    void GrabObject()
    {
        grabbing = true;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);
        if (hits.Length > 0)
        {
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
    }

	// Update is called once per frame
	void Update () {
        if (!grabbing && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 1)
            GrabObject();
        else if (grabbing && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) < 1)
            DropObject();
	}
}
