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
    float moveSpeed = 13f;
    float airSpeed = 0.8f;
    float maxAirSpeed = 8f;
    float dashMagnitude = 20f;
    float gravity = 2f;
    float jumpVel = 10f;
    float dashEndMomentum = 0.65f;
    int dashTime = 10;

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
        
        Vector2 move = ci.move;
        move.y = 0;
        move.x += 0.15f * Math.Sign(move.x);
        if(move.magnitude> 1f)
        {
            move.Normalize();
        }
        Vector2 desired = Vector2.zero;

        //flip sprite direction to movement direction on ground
        if (grounded)
        {
            desired.x = move.x * moveSpeed;
            if (desired.x < 0) { transform.localRotation = Quaternion.Euler(0, 180, 0); }
            else if (desired.x > 0) { transform.localRotation = Quaternion.Euler(0, 0, 0); }
            if (!dashAvailable) { dashAvailable = true; }
            if (ci.Jump) { desired += new Vector2(0, jumpVel); }
        }
        //else
        //{
        //    desired = AirControl(desired,move);
        //}
        else
        {
            if (ci.Dash && dashAvailable && ci.move != Vector2.zero)
            {
                dashVel = ci.move.normalized * dashMagnitude;
                dashCounter = dashTime;
                dashAvailable = false;
            }
            else if (dashCounter == 0 && ci.Stall)
            {
                dashCounter = dashTime;
                dashVel = Vector2.zero;
            }
        }
        
        if (dashCounter > 0)
        {
            if (dashCounter == 2) { dashVel *= dashEndMomentum; }
            if (grounded) { dashVel.y = 0; }
            dashCounter--;
            rb.velocity = dashVel;
        }
        else
        {
            desired = AirControl(move);
            rb.velocity = desired + new Vector2(0, rb.velocity.y - gravity * 9.8f * Time.fixedDeltaTime);
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

    Vector2 AirControl(Vector2 move)
    {
        Vector2 desired = new Vector2 (rb.velocity.x, 0);
        float newVel = rb.velocity.x + move.x * airSpeed;
        //if the current x speed is lower than the desired speed, we use the new speed, UNLESS the new speed is higher than the max speed and the current speed is less than the max speed
        //which is when we use the max speed, or the current speed is higher than the max speed, which is when we allow the speed to go down but not up.
        desired.x = newVel;
        if (Math.Abs(rb.velocity.x) < Math.Abs(newVel))
        {
            if (Math.Abs(newVel) > maxAirSpeed && Math.Abs(rb.velocity.x) < maxAirSpeed)
            {
                desired.x = maxAirSpeed* Mathf.Sign(newVel);
            }
            else if (Math.Abs(newVel) > maxAirSpeed)
            {
                desired.x = rb.velocity.x;
            }
        }
        return desired;
    }
}
