using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class GameController : MonoBehaviour {

    public static GameController Instance;
    
    public Vector3 rightControllerOffset  {  set { _rightControllerOffset = value; isSaved = false; } get { return _rightControllerOffset;  } }
    public Vector3 lowerLeftScreenCorner  {  set { _lowerLeftScreenCorner = value; isSaved = false; } get { return _lowerLeftScreenCorner;  } }
    public Vector3 upperLeftScreenCorner  {  set { _upperLeftScreenCorner = value; isSaved = false; } get { return _upperLeftScreenCorner;  } }
    public Vector3 upperRightScreenCorner { set { _upperRightScreenCorner = value; isSaved = false; } get { return _upperRightScreenCorner; } }

    private bool isSaved = true;
    private Vector3 _rightControllerOffset;
    private Vector3 _lowerLeftScreenCorner;
    private Vector3 _upperLeftScreenCorner;
    private Vector3 _upperRightScreenCorner;
    private static string calibrationPath; 

    void Awake() {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            calibrationPath = "D:/workspace/" + "Callibrations.dat";
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
        if (rightControllerOffset != null)
            data.rightControllerOffset = rightControllerOffset;
        if (lowerLeftScreenCorner != null)
            data.lowerLeftScreenCorner = lowerLeftScreenCorner;
        if (upperLeftScreenCorner != null)
            data.upperLeftScreenCorner = upperLeftScreenCorner;
        if (upperRightScreenCorner != null)
            data.upperRightScreenCorner = upperRightScreenCorner;

        Debug.Log(calibrationPath);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(calibrationPath, FileMode.OpenOrCreate);
        bf.Serialize(file, data);
        file.Close();
        isSaved = true;
    }

    public void LoadCalibration()
    {
        if (File.Exists(calibrationPath))
        {
            Debug.Log("Loading calibration data from " + calibrationPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(calibrationPath, FileMode.Open);
            CalibrationData data = (CalibrationData)bf.Deserialize(file);
            file.Close();

            rightControllerOffset = data.rightControllerOffset;
            lowerLeftScreenCorner = data.lowerLeftScreenCorner;
            upperLeftScreenCorner = data.upperLeftScreenCorner;
            upperRightScreenCorner = data.upperRightScreenCorner;
            Debug.Log("Offset: " + GameController.Instance.rightControllerOffset);
            Debug.Log("LL: " + GameController.Instance.lowerLeftScreenCorner);
            Debug.Log("UL: " + GameController.Instance.upperLeftScreenCorner);
            Debug.Log("UR: " + GameController.Instance.upperRightScreenCorner);
        }
        else
        {
            Debug.Log(calibrationPath);
            Debug.Log("No calibration data to load");
        }
    }
}

[Serializable]
class CalibrationData
{
    public float[] data;
    public Vector3 rightControllerOffset {
        get { return new Vector3(data[0 * 3 + 0], data[0 * 3 + 1], data[0 * 3 + 2]);}
        set { data[0 * 3 + 0] = value.x; data[0 * 3 + 1] = value.y; data[0 * 3 + 2] = value.z;}
    }
    public Vector3 lowerLeftScreenCorner
    {
        get { return new Vector3(data[1 * 3 + 0], data[1 * 3 + 1], data[1 * 3 + 2]); }
        set { data[1 * 3 + 0] = value.x; data[1 * 3 + 1] = value.y; data[1 * 3 + 2] = value.z; }
    }
    public Vector3 upperLeftScreenCorner
    {
        get { return new Vector3(data[2 * 3 + 0], data[2 * 3 + 1], data[2 * 3 + 2]); }
        set { data[2 * 3 + 0] = value.x; data[2 * 3 + 1] = value.y; data[2 * 3 + 2] = value.z; }
    }
    public Vector3 upperRightScreenCorner
    {
        get { return new Vector3(data[3 * 3 + 0], data[3 * 3 + 1], data[3 * 3 + 2]); }
        set { data[3 * 3 + 0] = value.x; data[3 * 3 + 1] = value.y; data[3 * 3 + 2] = value.z; }
    }
    public CalibrationData()
    {
        data = new float[3 * 4];
    }
}


