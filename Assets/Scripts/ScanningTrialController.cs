using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanningTrialController : MonoBehaviour {
    private int trialNum;
    public int TrialNumber
    {
        get { return trialNum;}
        set { UpdateTrial(value); }
    }
    private List<ScanningTrialDescription> trials = new List<ScanningTrialDescription>();

    private void UpdateTrial(int index)
    {
        if (index >= trials.Count)
            index = 0;
        trials[index].PopulateMaterial(CameraManager.Instance.Focus.GetComponent<Renderer>().material);
    }
    
    private class ScanningTrialDescription
    {
        string brushTexturePath;

        public ScanningTrialDescription(string brushTexPath)
        {
            brushTexturePath = brushTexPath;
        }
        public void PopulateMaterial(Material mat)
        {
            var brushTex = Resources.Load("cylinder_maps\\" + brushTexturePath) as Texture2D;
            mat.SetTexture("_AnisoTex", brushTex);
        }

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
