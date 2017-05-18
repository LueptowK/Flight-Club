using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Finisher
{

    AttackActive a;
    void Start()
    {
        a = GetComponent<AttackActive>();
        
    }
    bool dir = false;
    public override Vector2 motion(Vector2 input)
    {
        if (a.attacking)
        {
            if (dir==false)
            {
                dir = true;
                print(input.normalized);
                Vector2 v2 = input;
                print(v2);
                float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                print(angle);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
        }
        return Vector2.zero;
    }

    // Use this for initialization
    
	
	// Update is called once per frame
	void Update () {
		
	}
}
