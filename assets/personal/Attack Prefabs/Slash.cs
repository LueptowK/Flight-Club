using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MovePhysics
{

    AttackActive a;
    void Start()
    {
        a = GetComponent<AttackActive>();
        
    }
    bool dir = false;
    public override AtkMotion motion(Vector2 input)
    {
        if (a.attacking)
        {
            if (dir==false)
            {
                dir = true;

                Vector2 v2 = input;

                float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
        }
        AtkMotion m = new AtkMotion();
        m.use = true;
        m.motion = Vector2.zero;
        return m;
    }

    // Use this for initialization
    
	
	// Update is called once per frame
	void Update () {
		
	}
}
