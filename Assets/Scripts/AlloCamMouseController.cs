using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlloCamMouseController : MonoBehaviour {
    public GameObject target;
    public float sensitivity;

    bool mousePressed = false;
    bool altMode = false;
    Vector3 originalMousePos;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalScale;

    Vector3 worldX;
    Vector3 worldY;
	// Use this for initialization
	void Start () {
		
	}



    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            originalPosition = this.transform.position;
            originalRotation = this.transform.rotation;
            originalScale = this.transform.localScale;
            originalMousePos = Input.mousePosition;
            
            worldX = (GetComponent<Camera>().ScreenToWorldPoint(new Vector3(100,0,1)) - GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            worldY = (GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 100, 1)) - GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            Debug.Log(worldX + " "  + worldY);
            if (Input.GetKey(KeyCode.LeftAlt) || (Input.GetKey(KeyCode.LeftAlt)))
            {
                altMode = true;
                Debug.Log("Alt is down.");
            }
            else
            {
                altMode = false;
                Debug.Log("No Alt, right?");
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }

        if (mousePressed)
        {
            Vector3 delta = Input.mousePosition - originalMousePos;
            transform.SetPositionAndRotation(originalPosition, originalRotation);

            transform.RotateAround(target.transform.position, worldX, sensitivity * -delta.y);
            transform.RotateAround(target.transform.position, worldY, sensitivity*delta.x);
   
            //Debug.Log(Input.mousePosition - originalMousePos);

        }
    }
}
