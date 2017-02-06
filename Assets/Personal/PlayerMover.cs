using UnityEngine;
using System.Collections;
using System;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    PlayerAnimator pani;
    Collider2D col;

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
    float wallJumpYVel = 8f;
    float dashEndMomentum = 0.65f;
    int dashTime = 10;

    bool FacingLeft = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        pani = GetComponent<PlayerAnimator>();
        col = GetComponent<Collider2D>();
        dashVel = Vector2.zero;
        dashAvailable = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector2 move = ci.move;
        //print(move);
        move.y = 0;
        move.x += 0.15f * Math.Sign(move.x);
        if(move.magnitude> 1f)
        {
            move.Normalize();
        }
        Vector2 desired = Vector2.zero;
        float tempGrav = gravity;

        //flip sprite direction to movement direction on ground
        if (grounded)
        {
            desired.x = move.x * moveSpeed;
            if (desired.x < 0) { FacingLeft = true; }
            else if (desired.x > 0) { FacingLeft = false; }

            
            dashAvailable = true; 
            if (ci.Jump) { desired += new Vector2(0, jumpVel); pani.jump(); }
        }
        else
        {
            desired = AirControl(move);
            if (ci.Dash && dashAvailable && ci.move != Vector2.zero) // DASH
            {
                dashVel = ci.move.normalized * dashMagnitude;
                dashCounter = dashTime;
                dashAvailable = false;
            }
            else if (dashCounter == 0 && ci.Stall) //STALL
            {
                dashCounter = 1;
                dashVel = Vector2.zero;
            }
            if (nearWall) //WALL - NEAR
            {
                if((desired.x>0f && OnRightWall) || (desired.x < 0f && OnLeftWall))
                {
                    desired.x = 0;
                }

                if (onWall) //WALL - HANG
                {
                    if (rb.velocity.y > 0)
                    {
                        rb.velocity = Vector2.zero;
                    }
                    if (OnRightWall)
                    {
                        FacingLeft = false;
 
                    }
                    else
                    {
                        FacingLeft = true;
                    }
                    dashAvailable = true;
                    tempGrav = 0.3f;
                }
                if (ci.Jump) // WALLJUMP
                {
                    dashAvailable = true;
                    if (OnRightWall)
                    {
                        desired = new Vector2(-wallJumpXVel, wallJumpYVel);
                        FacingLeft = true;
                    }
                    else
                    {
                        desired = new Vector2(wallJumpXVel, wallJumpYVel);
                        FacingLeft = false;
                    }
                }

            }
        }



        if (dashCounter > 0) //DASH OR STALL MOVEMENT
        {
            if (dashCounter == 2) { dashVel *= dashEndMomentum; }
            if (grounded) { dashVel.y = 0; }
            dashCounter--;
            rb.velocity = dashVel;
        }
        else
        {

            rb.velocity = desired + new Vector2(0, rb.velocity.y - tempGrav * 9.8f * Time.fixedDeltaTime);
        }

        if (FacingLeft)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public bool grounded
    {
        get
        {
            RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(col.bounds.extents.x*2f, col.bounds.extents.y*2f), 0, -transform.up, 0.08f, 1 << 10);
            return ray;
        }
    }

    public bool nearWall
    {
        get
        {
            return OnRightWall || OnLeftWall;
        }
    }

    public bool onWall
    {
        get
        {
            return (OnRightWall && ci.move.x > 0f) || (OnLeftWall && ci.move.x < 0f);
        }
    }

    public bool OnRightWall
    {
        get
        {
            return Physics2D.Raycast(transform.position, Vector2.right, col.bounds.extents.x + 0.1f, 1 << 8);
        }
    }
    public bool OnLeftWall
    {
        get
        {
            
            return Physics2D.Raycast(transform.position, -Vector2.right, col.bounds.extents.x + 0.1f, 1 << 8);

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
