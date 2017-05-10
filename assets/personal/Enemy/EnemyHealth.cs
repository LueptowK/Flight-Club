using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health {

	// Use this for initialization
	void Start () {
		
	}
	 public override int takeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            mover.kill();
        }
        return 0;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
