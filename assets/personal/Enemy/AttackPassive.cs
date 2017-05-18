using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPassive : Attack {

	// Use this for initialization
	void Awake () {
        base.Start();
        isActive = false;
        foreach (Transform child in transform)
        {
            child.GetComponent<HitboxProperties>().setAtk();
            
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public override void addHit(GameObject h, int hitLag)
    {
        //Nothing! Horray!
    }
}
