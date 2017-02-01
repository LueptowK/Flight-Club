using UnityEngine;
using System.Collections;
using System;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    Collider2D collider;
    Vector2 dashVel = Vector2.zero;
    int dashCounter;
    bool dashAvailable;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        collider = GetComponent<Collider2D>();
        dashVel = Vector2.zero;
        dashAvailable = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float moveSpeed = 13f;
        float airSpeed = 0.8f;
        float maxAirSpeed = 8f;
        float dashMagnitude = 20f;

        Vector2 move = ci.move;
        move.y = 0;
        move.x += 0.15f * Math.Sign(move.x);
        if(move.magnitude> 1f)
        {
            move.Normalize();
        }


        Vector2 desired = Vector2.zero;
        if (grounded)
        {
            desired.x = move.x * moveSpeed;
            if (desired.x < 0)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (desired.x > 0)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (!dashAvailable) { dashAvailable = true; }
        }
        else
        {
            desired.x = rb.velocity.x;
            float newVel = rb.velocity.x + move.x * airSpeed;
            if (Math.Abs(rb.velocity.x) < Math.Abs(newVel))
            {
                if (Math.Abs(newVel) < maxAirSpeed)
                {
                    desired.x = newVel;
                }
            }
            else
            {
                desired.x = newVel;
            }
        }
        
        if (ci.Jump && grounded)
        {
            desired += new Vector2(0, 10f);
        }

        if (ci.Dash && !grounded && dashAvailable && ci.move != Vector2.zero)
        {
            dashVel = ci.move.normalized * dashMagnitude;
            dashCounter = 10;
            dashAvailable = false;
        }
        if (dashCounter > 0)
        {
            if (dashCounter == 2)
            {
                dashVel *= 0.65f;
            }
            if (grounded)
            {
                dashVel.y = 0;
            }
            dashCounter--;
        }

        if (dashCounter == 0 && !grounded && ci.Stall)
        {
            dashCounter = 10;
            dashVel = Vector2.zero;
        }
        
        rb.velocity = desired + new Vector2(0, rb.velocity.y);
        if (dashCounter > 0)
        {
            rb.velocity = dashVel;
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
