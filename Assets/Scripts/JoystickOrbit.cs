using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickOrbit : MonoBehaviour {
    public float maxLatitude = 90f; // Make this Nonnegative
    public float StartingLatitude = 0;
    public float StartingLongitude = 0;
    public float StartingZoom = 1;
    public float RevolveSensitivity = 0.25f;
    public float zoomSensitivity = 0.05f;
    public float maxZoom = 5;
    public float minZoom = 0.01f;

    private float latitude;
    private float longitude;
    private float zoom;

    private bool changed = true;

	// Use this for initialization
	void Start () {
        latitude = StartingLatitude;
        longitude = StartingLongitude;
        zoom = StartingZoom;
        changed = true;
	}

    /// <summary>
    /// Sets longitude equal its opposite.
    /// </summary>
    public void flipLongitude()
    {
        longitude = (longitude + 180) % 360;
    }

    /// <summary>
    /// Randomly selects a viewing direction within deg degrees of 
    /// the current one.
    /// </summary>
    /// <param name="degrees"></param>
    public void jitter(float deg)
    {
        longitude = (longitude + Random.Range(-Mathf.Abs(deg), Mathf.Abs(deg)) + 360) % 360;
    }

	// Update is called once per frame
	void Update () {
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);


        if (System.Math.Abs(thumbstick.x) > 0.1)
        {
            longitude = (longitude + RevolveSensitivity * thumbstick.x + 360) % 360;
            changed = true;
        }
        if (System.Math.Abs(thumbstick.y) > 0.1)
        {
            latitude = System.Math.Max(System.Math.Min(maxLatitude, latitude + RevolveSensitivity * thumbstick.y), -maxLatitude);
            changed = true;
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            zoom = System.Math.Max(minZoom, zoom - zoomSensitivity);
            changed = true;
        }
        if (OVRInput.Get(OVRInput.Button.One))
        {
            zoom = System.Math.Min(maxZoom, zoom + zoomSensitivity);
            changed = true;
        }

        {
            CameraManager.Instance.SetView(longitude * (float)System.Math.PI/180, latitude*(float)System.Math.PI / 180, zoom);
            changed = false;
        }
    }   
}
