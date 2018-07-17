using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFromController : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
        if (GameController.Instance.fishtankEyeOffset != null)
            transform.localPosition = GameController.Instance.fishtankEyeOffset;
        else
            transform.localPosition = Vector3.zero;

    }
}
