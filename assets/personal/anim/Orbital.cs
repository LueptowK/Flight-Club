using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbital : MonoBehaviour {
    public float offset;
	// Use this for initialization
	void Start () {
        float pos = (Time.time % (Mathf.PI * 2) * 4) + offset * Mathf.PI;
        Vector3 mod = (transform.right * Mathf.Sin(pos) + transform.forward * Mathf.Cos(pos)) * 0.75f;
        transform.position = transform.parent.position + mod;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float pos = (Time.time%(Mathf.PI*2)*4)+offset* Mathf.PI;
        Vector3 mod = (transform.right * Mathf.Sin(pos) + transform.forward * Mathf.Cos(pos)) * 0.75f;
        transform.position = transform.parent.position+ mod;
        //transform.localPosition = mod;
	}
}
