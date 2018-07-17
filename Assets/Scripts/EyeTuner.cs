using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTuner : MonoBehaviour {
    public float sensitivity = 0.001f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftArrow))
            GameController.Instance.fishtankEyeOffset.x -= sensitivity;

        if (Input.GetKey(KeyCode.RightArrow))
            GameController.Instance.fishtankEyeOffset.x += sensitivity;

        if (Input.GetKey(KeyCode.UpArrow))
            GameController.Instance.fishtankEyeOffset.y += sensitivity;

        if (Input.GetKey(KeyCode.DownArrow))
            GameController.Instance.fishtankEyeOffset.y -= sensitivity;

        if (Input.GetKey(KeyCode.A))
            GameController.Instance.fishtankEyeOffset.z += sensitivity;

        if (Input.GetKey(KeyCode.Z))
            GameController.Instance.fishtankEyeOffset.z -= sensitivity;
        Debug.Log(GameController.Instance.fishtankEyeOffset);
    }
}
