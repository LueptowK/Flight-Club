using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreHP : Powerup {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    protected override void power(GameObject p)
    {
        p.GetComponent<PlayerHealth>().reset();
    }
}
