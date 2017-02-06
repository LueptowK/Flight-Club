using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    PlayerAnimator pani;
    Collider2D col;





    public enum PState
    {
        Free,
        Dash,
        Stall,
        Hitstun,
        Delay,
        CeilingHold
    }

    public enum ExecState
    {
        None,
        Jump
    }

    struct StatePair
    {
        public PState state;
        public int delay;
        public ExecState action;
        public StatePair(PState state, int delay)
        {
            this.state = state;
            this.delay = delay;
            this.action = ExecState.None;
        }
        public StatePair(PState state, int delay, ExecState e)
        {
            this.state = state;
            this.delay = delay;
            this.action = e;
        }
    }

    StatePair current = new StatePair(PState.Free, 0);
    Queue<StatePair> states = new Queue<StatePair>();


    Vector2 dashVel = Vector2.zero;
    int dashCounter;
    bool dashAvailable;
    bool ceilingAvaliable;
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
    int stallTime = 6;
    bool falling = false;
    float maxFallSpeed = 22f;
    float friction = 7f;
    int jumpSquatFrames = 4; // needs to be set before entering ANY DELAY STATE

    int stallCooldown = 40;
    int stallCooldownCurrent = 0;

    bool FacingLeft = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        pani = GetComponent<PlayerAnimator>();
        col = GetComponent<Collider2D>();
        dashVel = Vector2.zero;
        restoreTools();

    }
	void restoreTools()
    {
        dashAvailable = true;
        ceilingAvaliable = true;
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
        stallCooldownCurrent--;


        switch (current.state)
        {      
            case PState.Free:
                if (grounded)
                {
                    desired.x = move.x * moveSpeed;
                    //flip sprite direction to movement direction on ground
                    if (desired.x < 0) { FacingLeft = true; }
                    else if (desired.x > 0) { FacingLeft = false; }
                    falling = false;

                    restoreTools();
                    if (ci.TauntDown&&desired.x==0)
                    {
                        pani.TauntD();

                        states.Enqueue(new StatePair(PState.Delay, 45));
                    }
                    if (nearWall)
                    {
                        if ((desired.x > 0f && OnRightWall) || (desired.x < 0f && OnLeftWall))
                        {
                            desired.x = 0;
                        }
                    }

                        if (ci.Jump)
                    {

                        
                        states.Enqueue(new StatePair(PState.Delay, jumpSquatFrames, ExecState.Jump));
                        pani.jumpSquat();
                    }
                }
                else
                {
                    desired = AirControl(move);
                    tryDash();//DASH
                    tryStall();//STALL
                    if (rb.velocity.y <= 1.5f && ci.fall) //FAST FALL
                    {
                        falling = true;
                    }
                    #region wall
                    if (nearWall) //WALL - NEAR
                    {
                        if ((desired.x > 0f && OnRightWall) || (desired.x < 0f && OnLeftWall))
                        {
                            desired.x = 0;
                        }
                        restoreTools();

                        if (onWall) //WALL - HANG
                        {
                            falling = false;
                            desired.y += applyFriction(rb.velocity.y);


                            if (rb.velocity.y < -3)
                            {
                                rb.velocity = new Vector2(0, -3);
                            }

                            

                            if (OnRightWall)
                            {
                                FacingLeft = false;

                            }
                            else
                            {
                                FacingLeft = true;
                            }

                            
                            //tempGrav = 0.3f;
                        }
                        if (ci.Jump) // WALLJUMP
                        {
                            rb.velocity = Vector2.zero;
                            if (onWall)
                            {
                                pani.jump();
                            }


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
                    #endregion
                    else if (onCeiling && ci.move.y > 0&& ceilingAvaliable)
                    {
                        states.Enqueue(new StatePair(PState.CeilingHold, 30));
                        ceilingAvaliable = false;
                        dashAvailable = true;
                    }
                }
                if (falling)
                {
                    tempGrav *= 5;
                }
                rb.velocity = desired + new Vector2(0, rb.velocity.y - tempGrav * 9.8f * Time.fixedDeltaTime);
                if (rb.velocity.y < -maxFallSpeed)
                {
                    rb.velocity = desired + new Vector2(0, -maxFallSpeed);
                }
                break;
            case PState.Dash:
                rb.velocity = dashVel;
                falling = false;
                
                if(current.delay == dashTime) { pani.dash(); }
                if (current.delay == 2) { dashVel *= dashEndMomentum; }
                if (grounded) { dashVel.y = 0; }
                if (current.delay < dashTime / 2 && states.Count < 1)
                {
                    tryStall();
                }

                break;
            case PState.Stall:
                rb.velocity = Vector2.zero;
                falling = false;

                if (current.delay == stallTime) { stallCooldownCurrent = stallCooldown; }
                if (current.delay < stallTime && states.Count < 1)
                {
                    tryDash();
                }
                break;
            case PState.Delay:
                switch (current.action)
                {
                    case ExecState.Jump:
                        rb.velocity *= 0.80f;
                        break;
                }
                
                break;
            case PState.CeilingHold:
                rb.velocity = new Vector2(rb.velocity.x + applyFriction(rb.velocity.x), 0);

                if (ci.Jump)
                {
                    rb.velocity += new Vector2(0, -maxFallSpeed);
                    states.Enqueue(new StatePair(PState.Free, 0));
                }
                if(states.Count<1){
                    tryDash();
                }
                if (states.Count < 1)
                {
                    tryStall();
                }
                if ((states.Count < 1 &&ci.move.y < 0) || !onCeiling)
                {
                    states.Enqueue(new StatePair(PState.Free, 0));
                }




                if (states.Count > 0)
                {
                    current.delay = 1;
                }
                break;

        }
        current.delay -= 1;
        if (current.delay <= 0)
        {
            switch (current.action)
            {
                case ExecState.None:
                    break;
                case ExecState.Jump:
                    rb.velocity =(rb.velocity*(Mathf.Pow(1f/0.8f, jumpSquatFrames))) + new Vector2(ci.move.x*0.1f*jumpVel, jumpVel);
                    
                    pani.jump();
                    break;
            }
            nextState();
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
    #region try moves
    void tryDash()
    {
        if (ci.Dash && dashAvailable) // DASH
        {
            Vector2 input = ci.move.normalized;
            if(input == Vector2.zero)
            {
                if(Physics2D.Raycast(transform.position, transform.right, col.bounds.extents.x + 0.1f, 1 << 8))
                {
                    input = -transform.right;
                    changeFace();
                }
                else
                {
                    input = transform.right;
                }
                
            }
            dashVel = input * dashMagnitude;

            dashAvailable = false;
            states.Enqueue(new StatePair(PState.Dash, dashTime));
        }
    }

    void tryStall()
    {
        if (ci.Stall && stallCooldownCurrent<= 0)
        {
            states.Enqueue(new StatePair(PState.Stall, stallTime));
            
        }
    }
    #endregion
    void changeFace()
    {
        FacingLeft = !FacingLeft;
    }
    public PState currentState
    {
        get
        {
            return current.state;
        }
    }
    #region direction checks
    public bool grounded
    {
        get
        {
            RaycastHit2D ray = Physics2D.BoxCast(transform.position-new Vector3(0, col.bounds.extents.y,0), new Vector2(col.bounds.extents.x*1.9f, col.bounds.extents.y*1f), 0, -transform.up, 0.08f, 1 << 10);
            return ray;
        }
    }
    public bool onCeiling
    {
        get
        {
            return Physics2D.Raycast(transform.position, transform.up, col.bounds.extents.y + 0.1f, 1 << 10);
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
    #endregion
    void nextState()
    {
        if (states.Count==0)
        {
            current = new StatePair(PState.Free, 0);
        }
        else
        {
            current = states.Dequeue();
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
    float applyFriction(float vel)
    {
        float newVel = -Math.Sign(vel) * friction * Time.fixedDeltaTime;
        if (Math.Sign(vel + newVel) != Math.Sign(vel))
        {
            newVel = -vel;
        }
        return newVel;
    }
}
