using UnityEngine;
using System.Collections;
using System;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;

    Vector2 dashVel = Vector2.zero;
    int dashCounter;
    bool dashAvailable;
    float moveSpeed = 13f;
    float airSpeed = 0.8f;
    float maxAirSpeed = 8f;
    float dashMagnitude = 20f;
    float gravity = 2f;
    float jumpVel = 10f;
    float wallJumpXVel = 15f;
    float wallJumpYVel = 5f;
    float dashEndMomentum = 0.65f;
    int dashTime = 10;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();

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

        if (onWall)
        {
            if (ci.Jump) { rb.velocity += new Vector2(wallJumpXVel*-Math.Sign(move.x), wallJumpYVel); }
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
            if (!grounded) { desired = AirControl(move); }
            rb.velocity = desired + new Vector2(0, rb.velocity.y - gravity * 9.8f * Time.fixedDeltaTime);
        }
    }

    public bool grounded
    {
        get
        {
            RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0, -transform.up, 0.08f, 1 << 8);
            return ray;
        }
    }

    public bool onWall
    {
        get
        {
            RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(1f, 0.5f), 0, transform.right, 0.08f, 1 << 8);
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
