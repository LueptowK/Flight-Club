using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tackle : Finisher {
    Vector2 tackleDir = Vector2.zero;
    Attack a;
    AttackManager mngr;

    int lifetime = 0;

    float speed = 45;

    public override Vector2 motion(Vector2 input)
    {
        if (a.attacking)
        {
            if(tackleDir == Vector2.zero)
            {
                tackleDir = input;
            }
            return tackleDir * speed;
        }
        return Vector2.zero;
    }

    // Use this for initialization
    void Start () {
        a = GetComponent<Attack>();
        mngr = transform.parent.GetComponent<AttackManager>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        lifetime++;
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (lifetime > 1 &&other.tag!="Player"&&other.tag!="Target")
        {
            //a.grabDamage();
            a.windDown();
        }
        
        //mngr.stopAttack();
    }
}
