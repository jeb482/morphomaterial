﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScanningTrialController : MonoBehaviour {
    public int TrialNum;
    public GameObject scanningObject;
    private int lastTrialNum;

    private RandomizedList<ScanningTrialDescription> trials;

    private void UpdateTrial(int index)
    {
        if (index >= trials.Count)
            index = 0;
        trials[index].PopulateMaterial(scanningObject.GetComponent<Renderer>().material, MyColors.Metals[Random.Range(0,MyColors.Metals.Length)]);
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
        List<ScanningTrialDescription> trialsPrototype = new List<ScanningTrialDescription>();
        for (int i = 1; i < 13; i++)
        {
            trialsPrototype.Add(new ScanningTrialDescription("" + i));
        }
        trials = new RandomizedList<ScanningTrialDescription>(trialsPrototype, GameController.Instance.ParticipantNumber);
        Debug.Log("Randomizing scanning trials for Participant " + GameController.Instance.ParticipantNumber);
        Debug.Log(trials.GetIndicesAsString());
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
            GameController.Instance.RecordTrialData(CameraManager.Instance.cameraConfig, shape, GameController.Instance.GetTrialTimeElapsed(), lastTrialNum);
        }
            
        lastTrialNum = TrialNum;
    }

    public enum ScanningShape
    {
        None, Triangle, Square, Star
    };
}
