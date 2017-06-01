using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBaseChanger : MonoBehaviour {
    GameObject main;
    public GameObject baseCam;
    public GameObject topCam;
	// Use this for initialization
	void Start () {
        main =GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D()
    {
        
        main.GetComponent<CameraController>().setCamBase( baseCam?baseCam:null);
        main.GetComponent<CameraController>().setCamTop(topCam?topCam:null);

    }
}
