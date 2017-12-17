using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPositioner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameController.Instance.LoadCalibration();
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Offset: " + GameController.Instance.rightControllerOffset);
        // Debug.Log("LL: " + GameController.Instance.lowerLeftScreenCorner);
        // Debug.Log("UL: " + GameController.Instance.upperLeftScreenCorner);
        // Debug.Log("UR: " + GameController.Instance.upperRightScreenCorner);
        transform.localScale = new Vector3((GameController.Instance.upperRightScreenCorner - GameController.Instance.upperLeftScreenCorner).magnitude / 2,
                                           (GameController.Instance.upperLeftScreenCorner - GameController.Instance.lowerLeftScreenCorner).magnitude / 2,
                                           1);
        //transform.position = (GameController.Instance.upperRightScreenCorner + GameController.Instance.lowerLeftScreenCorner) / 2;

    }
}
