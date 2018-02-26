using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTextureBlit : MonoBehaviour {
    public Material material;


    private Texture2D original;

	// Use this for initialization
	void Start () {
//        material.ToString
//        original = (Texture2D)material.GetTexture();
        Debug.Log(material.ToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
