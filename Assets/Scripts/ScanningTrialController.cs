using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanningTrialController : MonoBehaviour {
    public int TrialNum;
    public bool doTraining;
    public GameObject scanningObject;
    public Text TrialCounterText;
    private int lastTrialNum;

    private RandomizedList<ScanningTrialDescription> trials;
    private List<ScanningTrialDescription> trainingTrials;
    private List<string> trainingTrialStrings;

    private void UpdateTrial(int index)
    {
        if (doTraining)
        {
            if (index >= trainingTrials.Count)
            {
                TrialNum = 0;
                lastTrialNum = 0;
                doTraining = false;
                UpdateTrial(0);
                return;
            }
            trainingTrials[index].PopulateMaterial(scanningObject.GetComponent<Renderer>().material, MyColors.Metals[Random.Range(0, MyColors.Metals.Length)]);
            TrialCounterText.text = trainingTrialStrings[index];
            return;
        }
        
        if (index >= trials.Count)
            index = 0;
        trials[index].PopulateMaterial(scanningObject.GetComponent<Renderer>().material, MyColors.Metals[Random.Range(0,MyColors.Metals.Length)]);
        TrialCounterText.text = "Trial " + (index + 1) + " of " + trials.Count + ".";
    }
    
    private class ScanningTrialDescription
    {
        string brushTexturePath;

        public ScanningTrialDescription(string brushTexPath)
        {
            brushTexturePath = brushTexPath;
        }
        public void PopulateMaterial(Material mat, Color color)
        {
            var brushTex = Resources.Load("cylinder_maps\\" + brushTexturePath) as Texture2D;
            mat.SetTexture("_AnisoTex", brushTex);
            mat.SetColor("_Color", color);
        }

    }

    

	// Use this for initialization
	void Start () {
        if (doTraining)
        {
            trainingTrials = new List<ScanningTrialDescription>();
            for (int i = 1; i < 6; i++)
            {
                trainingTrials.Add(new ScanningTrialDescription("training\\training" + i));
            }
            trainingTrialStrings = new List<string>();
            trainingTrialStrings.Add("Look for a big circle (1/5)");
            trainingTrialStrings.Add("This one is smaller (2/5)");
            trainingTrialStrings.Add("Check on the caps (3/5)");
            trainingTrialStrings.Add("This one is fainter (4/5)");
            trainingTrialStrings.Add("This one is quite small (5/5)");
        }

        List<ScanningTrialDescription> trialsPrototype = new List<ScanningTrialDescription>();
        for (int i = 1; i < 13; i++)
        {
            trialsPrototype.Add(new ScanningTrialDescription("" + i));
        }
        trials = new RandomizedList<ScanningTrialDescription>(trialsPrototype, GameController.Instance.ParticipantNumber);
        Debug.Log("Randomizing scanning trials for Participant " + GameController.Instance.ParticipantNumber);
        Debug.Log(trials.GetIndicesAsString());
        UpdateTrial(0);
        GameController.Instance.GetTrialTimeElapsed();
    }
	
	// Update is called once per frame
	void Update () {
        ScanningShape shape = ScanningShape.None;
       
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Debug.Log(7);
            shape = ScanningShape.Triangle;
            TrialNum = (lastTrialNum + 1) % trials.Count;
        } else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            shape = ScanningShape.Square;
            TrialNum = (lastTrialNum + 1) % trials.Count;
        } else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            shape = ScanningShape.Star;
            TrialNum = (lastTrialNum + 1) % trials.Count;
        }

        if (TrialNum != lastTrialNum)
        {
            UpdateTrial(TrialNum);
            if (!doTraining)
            {
                GameController.Instance.RecordTrialData(CameraManager.Instance.cameraConfig, shape, GameController.Instance.GetTrialTimeElapsed(), lastTrialNum);
            }
            
        }
            
        lastTrialNum = TrialNum;
    }

    public enum ScanningShape
    {
        None, Triangle, Square, Star
    };
}
