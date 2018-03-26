using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchRotation : MonoBehaviour {
    public GameObject target;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
            transform.rotation = target.transform.rotation;
    }
}
