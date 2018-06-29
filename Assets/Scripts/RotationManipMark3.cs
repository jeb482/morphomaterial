using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManipMark3 : MonoBehaviour {
    private Vector3 lastMousePos;
    public float Sensitivity = 1000;
    private bool mouseDown;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
        {
            mouseDown = true;
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
            mouseDown = false;

        if (mouseDown)
        {
            Plane p = new Plane(CameraManager.Instance.getViewDirection(), CameraManager.Instance.Focus.position);


            Ray lastRay = CameraManager.Instance.ScreenPointToRay(lastMousePos);
            Ray thisRay = CameraManager.Instance.ScreenPointToRay(Input.mousePosition);
            

            Vector3 camPos = CameraManager.Instance.GetCamPos();
            float t1;
            float t2;

            p.Raycast(lastRay, out t1);
            p.Raycast(thisRay, out t2);

            Vector3 mouseClick1 = lastRay.origin + t1 * lastRay.direction;
            Vector3 mouseClick2 = thisRay.origin + t2 * thisRay.direction;

            Vector3 planeDelta = mouseClick2 - mouseClick1;
            
            Vector3 axis = Vector3.Cross(planeDelta.normalized, CameraManager.Instance.getViewDirection()).normalized;
            Debug.Log(axis);
            CameraManager.Instance.Focus.transform.Rotate(axis,Sensitivity*(Input.mousePosition-lastMousePos).magnitude,Space.World);

            lastMousePos = Input.mousePosition;
        }


	}
}
