using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenShapeController : MonoBehaviour {
    public Transform FocusLocation;
    public GameObject Focus;
    public int TrialNum = 0;

    private List<HiddenShapeTrialDesc> Trials = new List<HiddenShapeTrialDesc>();
    private int LastTrialNum = -1;

	// Use this for initialization
	void Start () {
        Trials.Add(new HiddenShapeTrialDesc("cylinder", "square3", new Vector3(0, 0, 0)));
        Trials.Add(new HiddenShapeTrialDesc("cylinder", "star3", new Vector3(0, 0, 0)));
        Trials.Add(new HiddenShapeTrialDesc("cylinder", "triangle1", new Vector3(0, 0, 0)));

        updateTrial(0);
    }
	
	// Update is called once per frame
	void Update () {
        // Switch which model and texture we are using
        if (Input.GetKeyDown(KeyCode.Equals))
            TrialNum = (LastTrialNum + 1) % Trials.Count;
        else if (Input.GetKeyDown(KeyCode.Minus))
            TrialNum = (LastTrialNum - 1 + Trials.Count) % Trials.Count;
        if (TrialNum != LastTrialNum)
            updateTrial(TrialNum);
        LastTrialNum = TrialNum;
		
	}

    void updateTrial(int index)
    {
        if (index >= Trials.Count)
            index = 0;
        var mat = Focus.GetComponent<Renderer>().material;
        Trials[index].PopulateTarget(Focus, mat);
    }


    class HiddenShapeTrialDesc
    {
        public string ModelPath;
        public string TanMapPath;
        public Vector3 OriginOffset;
        public HiddenShapeTrialDesc(string modelPath, string tanMapPath, Vector3 originOffset)
        {
            ModelPath = modelPath;
            TanMapPath = tanMapPath;
            OriginOffset = originOffset;
        }

        public void PopulateTarget(GameObject meshObject, Material mat)
        {
            var anisoTex = Resources.Load("aniso_maps\\" + TanMapPath) as Texture2D;
            mat.SetTexture("_AnisoTex", anisoTex);
            var loadedMesh = Resources.Load("models\\" + ModelPath) as Mesh;
            //meshObject.GetComponent<MeshFilter>().mesh = loadedMesh;
            meshObject.transform.localPosition = OriginOffset;
        }

    }
}
