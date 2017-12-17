using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public Vector3 rightControllerOffset; //{  set { _rightControllerOffset = value; isSaved = false; } get { return _rightControllerOffset;  } }
    public Vector3 lowerLeftScreenCorner ;// {  set { _lowerLeftScreenCorner = value; isSaved = false; } get { return _lowerLeftScreenCorner;  } }
    public Vector3 upperLeftScreenCorner ;// {  set { _upperLeftScreenCorner = value; isSaved = false; } get { return _upperLeftScreenCorner;  } }
    public Vector3 upperRightScreenCorner;// { set { _upperRightScreenCorner = value; isSaved = false; } get { return _upperRightScreenCorner; } }

    private bool isSaved = false;
   // private Vector3 _rightControllerOffset;
   // private Vector3 _lowerLeftScreenCorner;
   // private Vector3 _upperLeftScreenCorner;
   // private Vector3 _upperRightScreenCorner;
    private static string calibrationPath; 

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
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.S) && !isSaved)
            SaveCalibration();    
    }

    public void SaveCalibration()
    {
        Debug.Log("Saving calibration data to "  + calibrationPath);
        CalibrationData data = new CalibrationData();
        data.rightControllerOffset = rightControllerOffset;
        data.lowerLeftScreenCorner = lowerLeftScreenCorner;
        data.upperLeftScreenCorner = upperLeftScreenCorner;
        data.upperRightScreenCorner = upperRightScreenCorner;

        Debug.Log(calibrationPath);
        XmlSerializer xmlf = new XmlSerializer(typeof(CalibrationData));
        FileStream file = File.Open(calibrationPath, FileMode.OpenOrCreate);
        xmlf.Serialize(file, data);
        file.Close();
        isSaved = true;
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
            Debug.Log("Offset: " + rightControllerOffset);
            Debug.Log("LL: " + lowerLeftScreenCorner);
            Debug.Log("UL: " + upperLeftScreenCorner);
            Debug.Log("UR: " + upperRightScreenCorner);
        }
        else
        {
            Debug.Log(calibrationPath);
            Debug.Log("No calibration data to load");
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
}


