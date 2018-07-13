using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public Color pathColor;
    public GameObject player;
    public GameObject HMD;

    bool validTarget = false;
    private bool selecting = false;
    private GameObject intersectionTarget = null;
    private GameObject splat = null;
    private GameObject rendererObject;
    private LineRenderer lineRenderer;
    private Vector3[] rayEnds;
	// Use this for initialization
	void Start () {
        Debug.Log("Begin.");
        splat = Instantiate(Resources.Load("PortTarget"), null) as GameObject;
        rendererObject = new GameObject();

        lineRenderer = splat.AddComponent<LineRenderer>();
        rayEnds = new Vector3[2];
        
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.positionCount = 10;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;
        lineRenderer.loop = false;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.widthMultiplier = 0.005f;
        Debug.Log(lineRenderer.isVisible);


    }
	
	// Update is called once per frame
	void Update () {
        if (CameraManager.Instance.cameraConfig != CameraManager.CameraConfiguration.HMDCam)
            return;
	    if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            Debug.Log("splat");
            splat.SetActive(true);
            selecting = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick))
        {
            if (validTarget)
            {
                Vector3 newPos = splat.transform.position + (player.transform.position - HMD.transform.position);
                newPos.y = player.transform.position.y;
                player.transform.position = newPos;
            }
                
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
                //Debug.Log(hit.point);
                splat.transform.position = hit.point;
                splat.transform.rotation = Quaternion.FromToRotation(hit.normal, new Vector3(0, 1, 0));
                rayEnds[1] = splat.transform.position;
                validTarget = true;
            } else
            {
                splat.SetActive(false);
                rayEnds[1] = transform.position + transform.TransformDirection(new Vector3(0, 0, 1)) * 30;
                validTarget = false;
            }
            rayEnds[0] = transform.position;
            Vector3[] lineSeg = new Vector3[10];
            for (int i = 0; i < 10; i++)
            {
                lineSeg[i] = ((9 - i) / 9f) * rayEnds[0] + (i / 9f) * rayEnds[1];
            }
            lineRenderer.SetPositions(lineSeg);


        }
           
        
        	
	}
}
