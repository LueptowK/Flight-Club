using UnityEngine;
using System.Collections;
using System;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    Collider2D collider;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        collider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float moveSpeed = 13f;
        float airSpeed = 0.8f;
        float maxAirSpeed = 8f;
        Vector2 move = ci.move;
        move.y = 0;
        move.x += 0.15f * Math.Sign(move.x);
        if(move.magnitude> 1f)
        {
            move.Normalize();
        }


        Vector2 desired;
        if (grounded)
        {
            desired = move * moveSpeed; 
            
            rb.velocity = desired;
        }
        else
        {
            desired = move*airSpeed;
            float newVel = rb.velocity.x + desired.x;
            if (Math.Abs(rb.velocity.x) < Math.Abs(newVel))
            {
                if ( !(Math.Abs(newVel) > maxAirSpeed))
                {
                    rb.velocity = new Vector2(newVel, rb.velocity.y);
                }
                    
            }
            else
            {
                rb.velocity = new Vector2(newVel, rb.velocity.y);
            }
        }

        print(rb.velocity.magnitude);
        if (ci.Jump&& grounded)
        {
            rb.velocity += new Vector2(0, 10f);
        }
    }
    bool grounded
    {
        get
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, -transform.up, collider.bounds.extents.y + 0.1f, 1<<8);
            return ray;


        }
    }
}
