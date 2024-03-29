﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using System.Xml.Serialization;
using System.IO;
using System;


public class GameController : MonoBehaviour {

    public static GameController Instance;
    public int ParticipantNumber = -1;

    public Vector3 rightControllerOffset;
    public Vector3 lowerLeftScreenCorner;
    public Vector3 upperLeftScreenCorner;
    public Vector3 upperRightScreenCorner;
    public Vector3 fishtankEyeOffset;
    public GameObject targetObject;

    public Matrix4x4 realWorldToScreen;

    /// <summary>
    /// Data CSV Layout Description:
    /// Column 0 = Labels
    /// Row 0  Viewport Scanning Triangle/Square/Star
    /// Row 1  Viewport Scanning Duration
    /// Row 2  FishTank Scanning Triangle/Square/Star
    /// Row 3  FishTank Scanning Duration
    /// Row 4  HMD      Scanning Triangle/Square/Star
    /// Row 5  HMD      Scanning Duration
    /// Row 6  Viewport Compare  Same/Different
    /// Row 7  Viewport Compare  Duration
    /// Row 8  FishTank Compare  Same/Different
    /// Row 9  FishTank Compare  Duration
    /// Row 10 HMD      Compare  Same/Different
    /// Row 11 HMD      Compare  Duration
    /// Row 12 Viewport Tuneing  Distance
    /// Row 13 Viewport Tuneing  Duration
    /// Row 14 FishTank Tuneing  Distance
    /// Row 15 FishTank Tuneing  Duration
    /// Row 16 HMD      Tuneing  Distance
    /// Row 17 HMD      Tuneing  Duration
    /// </summary>
    public EasyCsv.Csv dataCsv;

    public void RecordTrialData(CameraManager.CameraConfiguration config, ScanningTrialController.ScanningShape shape, TimeSpan duration, int trialNum)
    {
        dataCsv[2 * (int)config][trialNum+1] = shape.ToString();
        dataCsv[2 * (int)config + 1][trialNum+1] = duration.TotalMilliseconds.ToString();
    }

    public void RecordTrialData(CameraManager.CameraConfiguration config, WoodTrialController.WoodComparison comp, TimeSpan duration, int trialNum)
    {
        dataCsv[2 * (int)config + 6][trialNum+1] = comp.ToString();
        dataCsv[2 * (int)config + 7][trialNum+1] = duration.TotalMilliseconds.ToString();
    }

    public void RecordTrialData(CameraManager.CameraConfiguration config, float tuningDifference, TimeSpan duration, int trialNum)
    {
        dataCsv[2 * (int)config + 12][trialNum + 1] = tuningDifference.ToString();
        dataCsv[2 * (int)config + 13][trialNum + 1] = duration.TotalMilliseconds.ToString();
    }

    enum Experiment { None, Wood, Bottle };

    private Experiment currentExperiment = Experiment.Bottle;
    private bool loadingScene = false;

    private bool isSaved = false;
    private static string calibrationPath;
    private DateTime lastTimeChecked;

    void Awake() {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            calibrationPath = "D:/workspace/" + "Callibrations.xml";
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCalibration();
        targetObject = Instantiate(Resources.Load("Bottle")) as GameObject;
        DontDestroyOnLoad(targetObject);
        dataCsv = new EasyCsv.Csv();
    }

    /// <summary>
    /// Checks a singleton timer and resets it to start at the current time.
    /// 
    /// This is used by TrialControllers for each task, and should not
    /// be accessed by other classes, since it will mess up the singleton 
    /// timer.
    /// </summary>
    /// <returns></returns>
    public TimeSpan GetTrialTimeElapsed()
    {
        var now = DateTime.Now;
        TimeSpan elapsed =  now - lastTimeChecked;
        lastTimeChecked = now;
        return elapsed;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.S) && !isSaved)
            SaveCalibration();

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("Timer reset.");
            GetTrialTimeElapsed();
        }
            
    }

    IEnumerator LoadNextScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        string nextScene = "";
        switch (sceneName)
        {
            case "HMD":
                nextScene = "Viewport"; break;
            case "Viewport":
                nextScene = "Fishtank"; break;
            case "Fishtank":
                nextScene = "HMD"; break;    
        }
        if (nextScene != "")
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }

    private void swapExperiment()
    {
        Destroy(targetObject);
        switch(currentExperiment)
        {
            case Experiment.Bottle:
                currentExperiment = Experiment.Wood;
                targetObject = Instantiate(Resources.Load("Bottle")) as GameObject;
                break;
            case Experiment.Wood:
                currentExperiment = Experiment.Bottle;
                targetObject = Instantiate(Resources.Load("Cube")) as GameObject;
                break;
        }
    }

    public void SaveCalibration()
    {
        Debug.Log("Saving calibration data to "  + calibrationPath);
        CalibrationData data = new CalibrationData();
        data.rightControllerOffset = rightControllerOffset;
        data.lowerLeftScreenCorner = lowerLeftScreenCorner;
        data.upperLeftScreenCorner = upperLeftScreenCorner;
        data.upperRightScreenCorner = upperRightScreenCorner;
        data.fishtankEyeOffset = fishtankEyeOffset;

        Debug.Log(calibrationPath);
        XmlSerializer xmlf = new XmlSerializer(typeof(CalibrationData));
        FileStream file = File.Open(calibrationPath, FileMode.OpenOrCreate);
        xmlf.Serialize(file, data);
        file.Close();
        isSaved = true;
        updateRealWorldToScreen();
    }



    public void LoadCalibration()
    {
        if (File.Exists(calibrationPath))
        {
            Debug.Log("Loading calibration data from " + calibrationPath);
            XmlSerializer xmlf = new XmlSerializer(typeof(CalibrationData));
            FileStream file = File.Open(calibrationPath, FileMode.Open);
            CalibrationData data = (CalibrationData)xmlf.Deserialize(file);
            file.Close();

            rightControllerOffset = data.rightControllerOffset;
            lowerLeftScreenCorner = data.lowerLeftScreenCorner;
            upperLeftScreenCorner = data.upperLeftScreenCorner;
            upperRightScreenCorner = data.upperRightScreenCorner;
            fishtankEyeOffset = data.fishtankEyeOffset;
            Debug.Log("Right Controller Offset: " + rightControllerOffset);
            Debug.Log("LL: " + lowerLeftScreenCorner);
            Debug.Log("UL: " + upperLeftScreenCorner);
            Debug.Log("UR: " + upperRightScreenCorner);
            Debug.Log("Fishtank Eye Offset: " + fishtankEyeOffset);
            updateRealWorldToScreen();
        }
        else
        {
            Debug.Log(calibrationPath);
            Debug.Log("No calibration data to load");
        }
    }

    public void updateRealWorldToScreen()
    {
        Vector3 origin = (lowerLeftScreenCorner + upperRightScreenCorner)/2;
        Vector3 x = (upperRightScreenCorner - upperLeftScreenCorner).normalized;
        Vector3 y = (upperLeftScreenCorner  - lowerLeftScreenCorner).normalized;
        Vector3 z = Vector3.Cross(x, y).normalized;
        x = Vector3.Cross(y, z).normalized;

        realWorldToScreen.SetRow(0, new Vector4(x.x, x.y, x.z, -Vector3.Dot(x, origin)));
        realWorldToScreen.SetRow(1, new Vector4(y.x, y.y, y.z, -Vector3.Dot(y, origin)));
        realWorldToScreen.SetRow(2, new Vector4(z.x, z.y, z.z, -Vector3.Dot(z, origin)));
        realWorldToScreen.SetRow(3, new Vector4(  0,   0,   0,                       1));
    }

    public void OnDestroy()
    {
        if (!dataCsv.IsEmpty) {
            dataCsv.WriteToFile("D:\\workspace\\" + DateTime.Now.ToString("MMddHHmmssffff") + ".csv");
        }
    }
}

[Serializable]
public class CalibrationData
{
    [SerializeField] public Vector3 rightControllerOffset;
    [SerializeField] public Vector3 lowerLeftScreenCorner;
    [SerializeField] public Vector3 upperLeftScreenCorner;
    [SerializeField] public Vector3 upperRightScreenCorner;
    [SerializeField] public Vector3 fishtankEyeOffset;
}




