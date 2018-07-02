using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickOrbit : MonoBehaviour {
    public float maxLatitude = 90f; // Make this Nonnegative
    public float StartingLatitude = 0;
    public float StartingLongitude = 0;
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
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);


        if (System.Math.Abs(thumbstick.x) > 0.1)
        {
            longitude = (longitude + RevolveSensitivity * thumbstick.y + 360) % 360;
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
        if (changed)
        {
            CameraManager.Instance.SetView(longitude, latitude, zoom);
            changed = false;
        }
    }   
}
