using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlloCamMouseController : MonoBehaviour {
    public float sensitivity;

    bool mousePressed = false;
    bool camMode = false;
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
        if (GameController.Instance.targetObject == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            originalMousePos = Input.mousePosition; 
            worldX = (GetComponent<Camera>().ScreenToWorldPoint(new Vector3(100,0,1)) - GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            worldY = (GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 100, 1)) - GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 1))).normalized;
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                camMode = true;
                originalPosition = this.transform.position;
                originalRotation = this.transform.rotation;
                originalScale = this.transform.localScale;
            }
            else
            {
                camMode = false;
                originalPosition = GameController.Instance.targetObject.transform.position;
                originalRotation = GameController.Instance.targetObject.transform.rotation;
                originalScale = GameController.Instance.targetObject.transform.localScale;
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }

        if (mousePressed)
        {
            Vector3 delta = Input.mousePosition - originalMousePos;
            if (camMode)
            {
                transform.SetPositionAndRotation(originalPosition, originalRotation);
                transform.RotateAround(GameController.Instance.targetObject.transform.position, worldX, sensitivity * -delta.y);
                transform.RotateAround(GameController.Instance.targetObject.transform.position, worldY, sensitivity * delta.x);
            } else
            {
                Debug.Log("la");
                GameController.Instance.targetObject.transform.SetPositionAndRotation(originalPosition, originalRotation);
                GameController.Instance.targetObject.transform.RotateAround(GameController.Instance.targetObject.transform.position, worldX, sensitivity * -delta.y);
                GameController.Instance.targetObject.transform.RotateAround(GameController.Instance.targetObject.transform.position, worldY, sensitivity * delta.x);
            }

   
            //Debug.Log(Input.mousePosition - originalMousePos);

        }
    }
}
