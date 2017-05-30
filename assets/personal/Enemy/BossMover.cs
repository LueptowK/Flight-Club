using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMover : Mover {
    public override void kill()
    {
        Destroy(gameObject);
    }
    public float speed;
    //public float maxSpeed;
    Interpreter ci;
    Rigidbody2D rb;
    Vector2 knockback;
    bool inHitstun;
    int hitstunCounter;
    // Use this for initialization
    void Start () {
        ci = GetComponent<Interpreter>();
        rb = GetComponent<Rigidbody2D>();
        inHitstun = false;
        hitstunCounter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (inHitstun)
        {
            rb.velocity = knockback;
            knockback = knockback * 0.95f;
            hitstunCounter--;
            if (hitstunCounter == 0)
            {
                inHitstun = false;
            }
        }
        else
        {
            if (ci.move.magnitude > 0)
            {
                rb.velocity = ci.move * speed;

            }

            /*
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity.Normalize();
                rb.velocity *= maxSpeed;
            }
            */
            if (ci.Stall)
            {
                rb.velocity *= 0.60f;
            }
        }
	}

    public void getHit(Vector2 kb, int hitLag, int hitStun, int damage)
    {
        knockback = kb*.8f;
        hitstunCounter = hitStun;
        inHitstun = true;
    }
}
