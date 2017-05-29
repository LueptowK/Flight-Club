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
    // Use this for initialization
    void Start () {
        ci = GetComponent<Interpreter>();
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
