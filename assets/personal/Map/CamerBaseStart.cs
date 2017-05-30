using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerBaseStart : MonoBehaviour {
    GameObject main;
    public GameObject baseCam;
	// Use this for initialization
	void Start () {
        main =GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D()
    {
        main.GetComponent<CameraController>().setCamBase( baseCam);
    }
}
