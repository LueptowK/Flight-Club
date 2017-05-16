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
                transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, input));
            }
            
        }
        return Vector2.zero;
    }

    // Use this for initialization
    
	
	// Update is called once per frame
	void Update () {
		
	}
}
