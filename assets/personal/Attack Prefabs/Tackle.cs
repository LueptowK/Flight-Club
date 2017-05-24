using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tackle : MovePhysics {
    Vector2 tackleDir = Vector2.zero;
    AttackActive a;
    AttackManager mngr;

    int lifetime = 0;

    float speed = 45;

    public override AtkMotion motion(Vector2 input)
    {
        AtkMotion m = new AtkMotion();
        m.use = true;
        if (a.attacking)
        {
            if(tackleDir == Vector2.zero)
            {
                tackleDir = input.normalized;
                if(input == Vector2.zero)
                {
                    tackleDir = transform.right;
                }
            }
            m.motion = tackleDir * speed;
            return m;
        }
        m.motion = Vector2.zero;
        return m;
    }

    // Use this for initialization
    void Start () {
        a = GetComponent<AttackActive>();
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
