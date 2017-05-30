using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunge : MovePhysics {
    AttackActive a;

    float speed = 22f;
	// Use this for initialization
	void Start () {
        a = GetComponent<AttackActive>();
    }

    public override AtkMotion motion(Vector2 input)
    {
        AtkMotion m = new AtkMotion();
        if (a.attacking)
        {
            m.use = true;
            m.motion = transform.right * speed;
        }
        else
        {
            m.use = false;
            m.motion = Vector2.zero;
        }

        return m;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
