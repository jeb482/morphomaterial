using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackFromScreen : MonoBehaviour {

    public Transform virtualScreenXform;
    public Transform controllerXform;

    // Use this for initialization
    void Start () {
	  	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = virtualScreenXform.localToWorldMatrix.MultiplyPoint(
                             GameController.Instance.realWorldToScreen.MultiplyPoint(
                             controllerXform.position));
	}
}
