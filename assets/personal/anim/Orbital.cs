using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbital : MonoBehaviour {
    public float offset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float pos = (Time.time%(Mathf.PI*2)*4)+offset* Mathf.PI;
        transform.position = transform.parent.position+ (transform.right*Mathf.Sin(pos)+ transform.forward* Mathf.Cos(pos))*0.75f;
	}
}
