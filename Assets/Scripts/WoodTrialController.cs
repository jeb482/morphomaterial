using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using CsvHelper

public class WoodTrialController : MonoBehaviour {


    public GameObject block1;
    public GameObject block2;
    public Transform block1Origin;
    public Transform block2Origin;
    public int TrialNum;
    private int lastTrialNum;
    private RandomizedList<WoodTrialDescription> trials;

    private void Start()
    {
        List<WoodTrialDescription> trialsProtoype = new List<WoodTrialDescription>();
        trialsProtoype.Add(new WoodTrialDescription("cmaple-sq", "cmaple-sq", "cmaple-sq", "cmaple-sq", 0.05f, "cmaple-sq", "cmaple-sq", "cmaple-sq", "cmaple-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("walnut1-sq", "walnut1-sq", "walnut1-sq", "walnut1-sq", 0.05f, "walnut1-sq", "walnut1-sq", "walnut1-sq", "walnut1-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("walnut2-sq", "walnut2-sq", "walnut2-sq", "walnut2-sq", 0.05f, "walnut2-sq", "walnut2-sq", "walnut2-sq", "walnut2-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("padauk-sq", "padauk-sq", "padauk-sq", "padauk-sq", 0.05f, "padauk-sq", "padauk-sq", "padauk-sq", "padauk-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("walnut1-sq", "cmaple-sq", "cmaple-sq", "cmaple-sq", 0.05f, "cmaple-sq", "cmaple-sq", "cmaple-sq", "cmaple-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("walnut1-sq", "walnut2-sq", "walnut1-sq", "walnut1-sq", 0.05f, "walnut1-sq", "walnut1-sq", "walnut1-sq", "walnut1-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("walnut2-sq", "walnut2-sq", "padauk-sq", "walnut2-sq", 0.05f, "walnut2-sq", "walnut2-sq", "walnut2-sq", "walnut2-sq", 0.05f));
        trialsProtoype.Add(new WoodTrialDescription("padauk-sq", "padauk-sq", "padauk-sq", "cmaple-sq", 0.05f, "padauk-sq", "padauk-sq", "padauk-sq", "padauk-sq", 0.05f));

        trials = new RandomizedList<WoodTrialDescription>(trialsProtoype, GameController.Instance.ParticipantNumber);
        UpdateTrial(0);
        Debug.Log("Randomizing scanning trials for Participant " + GameController.Instance.ParticipantNumber);
        Debug.Log(trials.GetIndicesAsString());
        GameController.Instance.GetTrialTimeElapsed();
    }

    void Update()
    {
        // Switch wood models on blocks
        WoodComparison comp = WoodComparison.None;
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            comp = WoodComparison.Same;
            TrialNum = (lastTrialNum + 1) % trials.Count;
        } else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            comp = WoodComparison.Different;
            TrialNum = (lastTrialNum + 1) % trials.Count;
        }
        if (TrialNum != lastTrialNum)
        {
            UpdateTrial(TrialNum);
            GameController.Instance.RecordTrialData(CameraManager.Instance.cameraConfig, comp, GameController.Instance.GetTrialTimeElapsed(), lastTrialNum);
        }
        lastTrialNum = TrialNum;

        // Switch block of focus
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            Debug.Log("Swapped focus");
            if (CameraManager.Instance.Focus == block1Origin)
                CameraManager.Instance.Focus = block2Origin;
            else
                CameraManager.Instance.Focus = block1Origin;
        }


    }


    // Use this for initialization
    // Need from resources and assign to tetures like "_DiffuseTex"
    void UpdateTrial(int index)
    {
        if (index >= trials.Count)
            index = 0;

        trials[index].PopulateMaterials(block1.GetComponent<Renderer>().material, block2.GetComponent<Renderer>().material);
    }


    private class WoodTrialDescription
    {
        string fiberAxisPath1;
        string highLightWidthPath1;
        string diffusePath1;
        string fiberColorPath1;
        float specReflection1;
        string fiberAxisPath2;
        string highLightWidthPath2;
        string diffusePath2;
        string fiberColorPath2;
        float specReflection2;

        public WoodTrialDescription(string axis1, string hilite1, string diffuse1, string fiber1, float spec1,
            string axis2, string hilite2, string diffuse2, string fiber2, float spec2)
        {
            fiberAxisPath1 = axis1;
            highLightWidthPath1 = hilite1;
            diffusePath1 = diffuse1;
            fiberColorPath1 = fiber1;
            specReflection1 = spec1;
            fiberAxisPath2 = axis2;
            highLightWidthPath2 = hilite2;
            diffusePath2 = diffuse2;
            fiberColorPath2 = fiber2;
            specReflection2 = spec2;
        }

        public void PopulateMaterials(Material mat1, Material mat2)
        {
            var axTex1 = Resources.Load("wood\\" + fiberAxisPath1 + "\\axis") as Texture2D;
            mat1.SetTexture("_FiberAxisTex", axTex1);
            var hiliteTex1 = Resources.Load("wood\\" + highLightWidthPath1 + "\\hilight") as Texture2D;
            mat1.SetTexture("_HighlightWidthTex", hiliteTex1);
            var diffTex1 = Resources.Load("wood\\" + diffusePath1 + "\\diffuse") as Texture2D;
            mat1.SetTexture("_DiffuseTex", diffTex1);
            var fiberTex1 = Resources.Load("wood\\" + fiberColorPath1 + "\\fiber") as Texture2D;
            mat1.SetTexture("_FiberColorTex", fiberTex1);
            // Deal with specular reflection here
            var axTex2 = Resources.Load("wood\\" + fiberAxisPath2 + "\\axis") as Texture2D;
            mat2.SetTexture("_FiberAxisTex", axTex2);
            var hiliteTex2 = Resources.Load("wood\\" + highLightWidthPath2 + "\\hilight") as Texture2D;
            mat2.SetTexture("_HighlightWidthTex", hiliteTex2);
            var diffTex2 = Resources.Load("wood\\" + diffusePath2 + "\\diffuse") as Texture2D;
            mat2.SetTexture("_DiffuseTex", diffTex2);
            var fiberTex2 = Resources.Load("wood\\" + fiberColorPath2 + "\\fiber") as Texture2D;
            mat2.SetTexture("_FiberColorTex", fiberTex2);
            // Deal with specular reflection here
        }
    }

    public enum WoodComparison
    {
        None,Same,Different
    };
}
