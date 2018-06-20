using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the user to record 3 points that define an AR fishtank screen
/// by pressing the A button on the Oculus controller.
/// </summary>
public class recordScreenPosition : MonoBehaviour {

    public Transform leftController;
    private bool aButtonDown = false;
    private int numPointsRecorded = 0;

    // Update is called once per frame.
    void Update()
    {
        if (!aButtonDown && OVRInput.GetDown(OVRInput.Button.One))
        {
            switch (numPointsRecorded)
            {
                case 0:
                    GameController.Instance.lowerLeftScreenCorner = transform.position;
                    Debug.Log("Lower left recorded");
                    break;
                case 1:
                    GameController.Instance.upperLeftScreenCorner = transform.position;
                    Debug.Log("Upper left recorded");
                    break;
                case 2:
                    GameController.Instance.upperRightScreenCorner = transform.position;
                    Debug.Log("Upper right recorded");
                    break;
                case 3:
                    GameController.Instance.fishtankEyeOffset = leftController.InverseTransformPoint(transform.position);
                    Debug.Log("FishTank Offset Recorded" + leftController.InverseTransformPoint(transform.position));
                    break;
            }
            numPointsRecorded = (numPointsRecorded + 1) % 4;
            aButtonDown = true;
        }
        else if (aButtonDown && !OVRInput.GetDown(OVRInput.Button.One))
            aButtonDown = false;
    }
}
