using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TuningTrialController : MonoBehaviour
{

    public GameObject object1;
    public GameObject object2;
    
    public Transform object1Origin;
    public Transform object2Origin;
    public JoystickOrbit JoystickController;
    public float TuningIncrement = 1;
    public int TrialNum;
    private int lastTrialNum;
    public Text TrialCounterText;

    public float minGrating = 200;
    public float maxGrating = 1400;

    private RandomizedList<TuningTrialDescription> trials;

    private List<TuningTrialDescription> trainingTrials;
    private List<string> trainingTrialStrings;
    private void Start()
    {

        List<TuningTrialDescription> trialsPrototype = new List<TuningTrialDescription>();
        trialsPrototype.Add(new TuningTrialDescription(303, 961));
        trialsPrototype.Add(new TuningTrialDescription(311, 1235));
        trialsPrototype.Add(new TuningTrialDescription(1044, 367));
        trialsPrototype.Add(new TuningTrialDescription(302, 845));
        trialsPrototype.Add(new TuningTrialDescription(500, 264));
        trialsPrototype.Add(new TuningTrialDescription(364, 254));
        trialsPrototype.Add(new TuningTrialDescription(917, 200));
        trialsPrototype.Add(new TuningTrialDescription(1203, 637));
        trials = new RandomizedList<TuningTrialDescription>(trialsPrototype, GameController.Instance.ParticipantNumber);
        Debug.Log(trials.GetIndicesAsString());
        GameController.Instance.GetTrialTimeElapsed();
        UpdateTrial(0);
    }

    void Update()
    {
        // Switch params
        float error = -1;
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            error = System.Math.Abs(object1.GetComponent<Renderer>().material.GetFloat("_Distance") - object2.GetComponent<Renderer>().material.GetFloat("_Distance"));
            TrialNum = (lastTrialNum + 1) % trials.Count;
        }
        if (TrialNum != lastTrialNum)
        {
            UpdateTrial(TrialNum);
            GameController.Instance.RecordTrialData(CameraManager.Instance.cameraConfig, error, GameController.Instance.GetTrialTimeElapsed(), trials.IndexList[lastTrialNum]);
        }
        lastTrialNum = TrialNum;

        //
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            if (JoystickController != null)
            {
                Debug.Log("Swapped focus");
                JoystickController.jitter(5);
            }


            if (CameraManager.Instance.Focus == object1Origin)
                CameraManager.Instance.Focus = object2Origin;
            else
                CameraManager.Instance.Focus = object1Origin;
        }

        handleTuning();

    }

    void handleTuning()
    {
        float delta = 0;
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            delta = 1;
        }
        if (OVRInput.Get(OVRInput.Button.One))
        {
            delta = -1;
        }

        var mat = object2.GetComponent<Renderer>().material;
        mat.SetFloat("_Distance", System.Math.Min(System.Math.Max(mat.GetFloat("_Distance") + delta, minGrating), maxGrating));

    }

    public class TuningTrialDescription
    {
        float refSpacing;
        float startSpacing;

        public TuningTrialDescription(float _refSpacing, float _startSpacing)
        {
            refSpacing = _refSpacing;
            startSpacing = _startSpacing;
        }

        public void PopulateMaterials(Material refMat, Material tuneMat)
        {
            refMat.SetFloat("_Distance", refSpacing);
            tuneMat.SetFloat("_Distance", startSpacing);
        }
    }

    void UpdateTrial(int index)
    {
        if (index >= trials.Count)
            index = 0;
        trials[index].PopulateMaterials(object1.GetComponent<Renderer>().material, object2.GetComponent<Renderer>().material);
        TrialCounterText.text = "Trial " + (index + 1) + " of " + trials.Count + ".";
    }

}
