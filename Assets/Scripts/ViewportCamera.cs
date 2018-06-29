using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportCamera : MonoBehaviour {
    public Transform Focus;
    public float RotationSensitivity = 1;
    public float distance = 1;
    public float ZoomSensitivity = 1;
    Vector2 validDistanceRange = new Vector2(.5f, 5);

    private Camera cam;
    private bool isOrbiting = false;
    private Vector3 lastMousePos;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    

    void Start () {
        cam = this.GetComponent<Camera>();
	}

    public void orbitAndZoom()
    {
        // Establish whether or not we are orbiting
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            isOrbiting = true; lastMousePos = Input.mousePosition;
            lastMousePos = Input.mousePosition;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isOrbiting = false;
            return;
        }
        
        // Handle input for revolution.
        if (isOrbiting && Focus != null)
        {
            Vector3 screenX = (cam.ScreenToWorldPoint(new Vector3(100, 0, 1)) - cam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            Vector3 screenY = (cam.ScreenToWorldPoint(new Vector3(0, 100, 1)) - cam.ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;

            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 oldViewDir = transform.InverseTransformVector(new Vector3(0, 0, 1)).normalized;
            transform.RotateAround(Focus.transform.position, screenX, RotationSensitivity * -delta.y);
            Vector3 newViewDir = transform.InverseTransformVector(new Vector3(0, 0, 1)).normalized;

            if (System.Math.Sign(newViewDir.x) != System.Math.Sign(oldViewDir.x) ||
                System.Math.Sign(newViewDir.z) != System.Math.Sign(oldViewDir.z) &&
                (oldViewDir.x != 0 || oldViewDir.z != 0))
            {
                transform.RotateAround(Focus.transform.position, screenX, RotationSensitivity * delta.y);
            }
            transform.RotateAround(Focus.transform.position, new Vector3(0,1,0), RotationSensitivity * delta.x);
            lastMousePos = Input.mousePosition;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        if (zoomInput == 0f)
            return;
        distance = System.Math.Min(validDistanceRange.y, System.Math.Max(validDistanceRange.x, distance + ZoomSensitivity * -zoomInput));
        Vector3 displacement = transform.position - Focus.position;
        transform.position = Focus.position + (displacement.normalized) * distance;

    }
    // Update is called once per frame
    void Update () {

    }
    
}
