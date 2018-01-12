using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCornerPositioner : MonoBehaviour {
    public enum ScreenType { Virtual, Real };
    public ScreenType screenType = ScreenType.Real;
    public GameObject bottomLeft;
    public GameObject bottomRight;
    public GameObject topLeft;
    public GameObject topRight;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        switch (screenType)
        {
            case ScreenType.Real:
                bottomLeft.transform.position = GameController.Instance.lowerLeftScreenCorner;
                topRight.transform.position = GameController.Instance.upperRightScreenCorner;
                topLeft.transform.position = GameController.Instance.upperLeftScreenCorner;
                bottomRight.transform.position = GameController.Instance.lowerLeftScreenCorner + (GameController.Instance.upperRightScreenCorner - GameController.Instance.upperLeftScreenCorner);
                break;
            case ScreenType.Virtual:
                break;
        }
    }
}
