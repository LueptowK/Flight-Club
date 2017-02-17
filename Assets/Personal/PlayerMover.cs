    using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    PlayerAnimator pani;
    Collider2D col;
    AttackManager atk;
    PlayerHealth health;
    ComboCounter combo;




    public enum PState
    {
        Air,
        Ground,
        Dash,
        Stall,
        Hitstun,
        Delay,
        CeilingHold,
        AirAttack,
        GroundAttack,
        Attack, 
    }

    public enum ExecState
    {
        None,
        Jump,
        hitLag,
        AttackAir,
        AttackGround,

    }

    public struct StatePair
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

    StatePair current = new StatePair(PState.Air, 0);
    Queue<StatePair> states = new Queue<StatePair>();

    Vector2 dashVel = Vector2.zero;
    Vector2 hitVector;
    ControlInterpret.StickQuadrant attkQuad;
    float maxDI = 18; //Max DI affect on knockback, in degrees
    bool registerHit = false;
    float hitstunFriction = 0.98f;
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
    int stallTime = 10;
    bool falling = false;
    float maxFallSpeed = 22f;
    float friction = 7f;
    int jumpSquatFrames = 4; // needs to be set before entering ANY DELAY STATE
    int stallCooldown = 40;
    int stallCooldownCurrent = 0;
    float ceilingBooster = 0f;

    [HideInInspector]public bool FacingLeft = false;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        pani = GetComponent<PlayerAnimator>();
        col = GetComponent<Collider2D>();
        atk = GetComponent<AttackManager>();
        health = GetComponent<PlayerHealth>();
        combo = GetComponent<ComboCounter>();
        dashVel = Vector2.zero;
        restoreTools();

    }
    public void restoreTools()
    {
        dashAvailable = true;
        ceilingAvaliable = true;
    }
    // Update is called once per frame
    void FixedUpdate() {
        Vector2 move = ci.move;
        move.y = 0;
        move.x += 0.15f * Math.Sign(move.x);
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        Vector2 desired = Vector2.zero;
        float tempGrav = gravity;
        stallCooldownCurrent--;


        if (registerHit) // hit in the queue
        {
            int safety = states.Count;
            while ((current.state != PState.Delay || current.action != ExecState.hitLag) && safety > 0)
            {
                nextState();
                safety--;
            }
        }

        switch (current.state)
        {
            #region Ground State
            case PState.Ground:
                {
                    desired.x = move.x * moveSpeed;
                    desired.y = rb.velocity.y - tempGrav * 9.8f * Time.fixedDeltaTime;
                    //flip sprite direction to movement direction on ground
                    if (desired.x < 0) { FacingLeft = true; }
                    else if (desired.x > 0) { FacingLeft = false; }
                    falling = false;

                    restoreTools();
                    
                    if (ci.Jump)
                    {
                        states.Enqueue(new StatePair(PState.Delay, jumpSquatFrames, ExecState.Jump));
                    }
                    if (ci.move.y >= 0)
                    {
                        tryDash();
                    }
                    if (states.Count < 1)
                    {
                        tryAttack();
                    }
                    if (ci.TauntDown && desired.x == 0)
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
                    if (!grounded)
                    {
                        states.Enqueue(new StatePair(PState.Air, 0));
                        break;
                    }

                    rb.velocity = desired;
                    break;
                }
            #endregion
            #region Air State
            case PState.Air:
                {
                    if (grounded)
                    {
                        states.Enqueue(new StatePair(PState.Ground, 0));
                    }
                    desired = AirControl(move);
                    if (!tryDash())
                    {
                        tryStall();
                    }
                    tryAttack();


                    if (rb.velocity.y <= 1.5f && ci.fall) //FAST FALL
                    {
                        falling = true;
                    }
                    #region wall
                    if (nearWall) //WALL - NEAR
                    {

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
                            falling = false;
                            rb.velocity = Vector2.zero;

                            pani.jump();



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
                    else if (onCeiling && ci.move.y >= -0.5f && ceilingAvaliable)
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
            #endregion
            #region Dash State
            case PState.Dash:
                rb.velocity = dashVel;
                falling = false;
                if (current.delay == 2) { dashVel *= dashEndMomentum; }
                if (grounded) { dashVel.y = 0; }
                if (states.Count < 1)
                {
                    if (!tryStall())
                    {
                        tryAttack();
                    }
                }

                break;
            #endregion
            #region Stall State
            case PState.Stall:
                rb.velocity = Vector2.zero;
                falling = false;

                if (current.delay == stallTime) { stallCooldownCurrent = stallCooldown; }
                if (current.delay < stallTime && states.Count < 1)
                {
                    if (!tryDash())
                    {
                        tryAttack();
                    }
                }
                break;
            #endregion
            #region Delay State
            case PState.Delay:
                switch (current.action)
                {
                    case ExecState.Jump:
                        rb.velocity *= 0.80f;
                        if (states.Count < 1)
                        {
                            if (!tryDash())
                            {
                                tryAttack();
                            }
                        }
                        break;
                    case ExecState.hitLag:
                        rb.velocity = Vector2.zero;
                        break;
                }

                break;
            #endregion
            #region CeilingHold State
            case PState.CeilingHold:
                rb.velocity = new Vector2(rb.velocity.x + applyFriction(rb.velocity.x), 0f);

                if (ci.Jump)
                {
                    rb.velocity += new Vector2(0, -maxFallSpeed);
                    states.Enqueue(new StatePair(PState.Air, 0));
                }
                if (states.Count < 1)
                {
                    if (!tryDash())
                    {
                        tryStall();
                    }
                }
                if ((states.Count < 1 && ci.move.y < 0) || !onCeiling)
                {
                    states.Enqueue(new StatePair(PState.Air, 0));
                }

                if (states.Count > 0)
                {
                    current.delay = 1;
                }
                break;
            #endregion
            #region Hitstun State
            case PState.Hitstun:
                if (grounded && rb.velocity.y < 0)
                {
                    hitVector = new Vector2(rb.velocity.x, -rb.velocity.y);
                    calculateDI();
                    rb.velocity = hitVector;
                }
                if (nearWall) //WALL - NEAR
                {
                    restoreTools();

                    if (OnRightWall && rb.velocity.x > 0)
                    {
                        hitVector = new Vector2(-rb.velocity.x, rb.velocity.y);
                        calculateDI();
                        rb.velocity = hitVector;

                    }
                    else if (OnLeftWall && rb.velocity.x < 0)
                    {
                        hitVector = new Vector2(-rb.velocity.x, rb.velocity.y);
                        calculateDI();
                        rb.velocity = hitVector;
                    }

                }
                if (onCeiling && rb.velocity.y > 0)
                {
                    dashAvailable = true;
                    hitVector = new Vector2(rb.velocity.x, -rb.velocity.y);
                    calculateDI();
                    rb.velocity = hitVector;
                }
                if (rb.velocity.magnitude > 1.5 * moveSpeed)
                {
                    rb.velocity = rb.velocity * hitstunFriction;
                }
                rb.velocity += new Vector2(0, -gravity * 9.8f * Time.fixedDeltaTime);
                break;
            #endregion
            #region AirAttack State
            case PState.AirAttack:
                if (grounded)
                {
                    states.Enqueue(new StatePair(PState.Ground, 0));
                    current.delay = 0;
                    atk.stopAttack();
                }
                desired = AirControl(move);
                if (rb.velocity.y <= 1.5f && ci.fall) //FAST FALL
                {
                    falling = true;
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
            #endregion
            #region GroundAttack State
            case PState.GroundAttack:
                rb.velocity = new Vector2(rb.velocity.x + applyFriction(rb.velocity.x)*5, rb.velocity.y);
                break;
                #endregion
        }


        #region ceiling booster
        if (current.state == PState.Dash)// Used to find the ceiling after a dash
        {
            ceilingBooster = 0.2f;
        }
        else
        {
            ceilingBooster = 0f;
        }
        #endregion

        current.delay -= 1;
        if (current.delay <= 0)
        {
            #region Exec States
            int frames;
            switch (current.action)
            {
                case ExecState.None:
                    break;
                case ExecState.Jump:
                    rb.velocity = (rb.velocity * (Mathf.Pow(1f / 0.8f, jumpSquatFrames))) + new Vector2(ci.move.x * 0.7f * jumpVel, jumpVel);
                    states.Enqueue(new StatePair(PState.Air, 0));
                    break;
                case ExecState.hitLag:
                    if (ci.move != Vector2.zero)
                    {
                        calculateDI();
                    }
                    rb.velocity = hitVector;
                    break;
                case ExecState.AttackAir:
                    frames = atk.makeAttack(QuadToTypeAir(attkQuad));
                    states.Enqueue(new StatePair(PState.AirAttack, frames));
                    break;
                case ExecState.AttackGround:
                    frames = atk.makeAttack(QuadToTypeGround(attkQuad));
                    states.Enqueue(new StatePair(PState.GroundAttack, frames));
                    states.Enqueue(new StatePair(PState.Ground, 0));
                    break;

                    
            }
            #endregion
            nextState();
        }
        else if (!registerHit)
        {
            pani.StateChange(false);
        }
        else
        {
            registerHit = false;
        }

        

    }
 
    public void getHit(Vector2 knockback, int hitLag, int hitStun, int damage)
    {
        //print(knockback + " ---- " + hitLag+" ---- " + hitStun);
        hitVector = knockback;
        if ((OnLeftWall && hitVector.x < 0) || (OnRightWall && hitVector.x > 0))
        {
            hitVector = new Vector2(-hitVector.x, hitVector.y);
        }
        if ((onCeiling && hitVector.y > 0) || (grounded && hitVector.y < 0))
        {
            hitVector = new Vector2(hitVector.x, -hitVector.y);
        }
        atk.stopAttack();
        states.Enqueue(new StatePair(PState.Delay, hitLag, ExecState.hitLag));
        registerHit = true;
        states.Enqueue(new StatePair(PState.Hitstun, hitStun));
        rb.velocity = Vector2.zero;
        //DAMAGE
        health.takeDamage(damage);
        //combo.incrementCombo(-1);
    }



    #region try moves
    bool tryDash()
    {
        if (ci.Dash && dashAvailable) // DASH
        {
            //calcDashVel();
            dashAvailable = false;
            states.Enqueue(new StatePair(PState.Dash, dashTime));
            return true;
        }
        return false;
    }
    bool calcDashVel()
    {
        Vector2 input = ci.move.normalized;
        float marigin = 0.2f;
        #region forward dash (no input)
        if (input == Vector2.zero)
        {
            if (FacingLeft)
            {
                if (Physics2D.Raycast(transform.position, Vector2.left, col.bounds.extents.x + 0.1f, 1 << 8))
                {
                    input = Vector2.right;
                    changeFace();
                }
                else
                {
                    input = Vector2.left;
                }
            }
            else
            {
                if (Physics2D.Raycast(transform.position, Vector2.right, col.bounds.extents.x + 0.1f, 1 << 8))
                {
                    input = Vector2.left;
                    changeFace();
                }
                else
                {
                    input = Vector2.right;
                }
            }


        }
        #endregion
        if (Physics2D.Raycast(transform.position, Vector2.left, col.bounds.extents.x + 0.5f, 1 << 8))
        {
            if (input.x < marigin)
            {
                if (input.x >= -marigin && input.y != 0)
                {
                    input.x = 0.2f;
                }
                else
                {
                    return false;
                }
            }
            

        }
        else if (Physics2D.Raycast(transform.position, Vector2.right, col.bounds.extents.x + 0.5f, 1 << 8))
        {
            if (input.x > -marigin)
            {
                if (input.x <= marigin && input.y != 0)
                {
                    input.x = -0.2f;
                }
                else
                {
                    return false;
                }
            }

        }
        input = input.normalized;
        dashVel = input * dashMagnitude;
        return true;
    }

    bool tryStall()
    {
        if (ci.Stall && stallCooldownCurrent<= 0)
        {
            states.Enqueue(new StatePair(PState.Stall, stallTime));
            return true;
        }
        return false;
    }
    /*
    bool tryAerialAttack()
    {
        if (ci.Attack && states.Count<1)
        {

            current.action = ExecState.AttackAir;
            attkQuad = ci.AttackQuad;
            return true;
        }
        return false;
    }
    bool tryGroundedAttack()
    {
        if (ci.Attack && states.Count < 1)
        {
            current.action = ExecState.AttackGround;
            attkQuad = ci.AttackQuad;
            return true;
        }
        return false;
    }

    */
    bool tryAttack()
    {
        if (ci.Attack)
        {
            states.Enqueue(new StatePair(PState.Attack, 0));
            attkQuad = ci.AttackQuad;
            return true;
        }
        return false;
    }
    #endregion
    public void changeFace()
    {
        FacingLeft = !FacingLeft;
    }
    public StatePair currentState
    {
        get
        {
            return current;
        }
    }
    public Vector2 dashDirection
    {
        get
        {
            return dashVel;
        }
    }

    #region direction checks
    public bool grounded
    {
        get
        {
            RaycastHit2D ray = Physics2D.BoxCast(transform.position - new Vector3(0, col.bounds.extents.y, 0), new Vector2(col.bounds.extents.x * 1.9f, col.bounds.extents.y * 1f), 0, -Vector2.up, 0.08f, 1 << 10);
            if (rb.velocity.y <= 0.01)
            {
                return ray;
            }
            return false;
        }
    }
    public bool onCeiling
    {
        get
        {
            return Physics2D.Raycast(transform.position, Vector2.up, col.bounds.extents.y + 0.1f+ceilingBooster, 1 << 10);
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
        
        {
            if (states.Count == 0)
            {
                if ((current.state != PState.Ground) && (current.state != PState.Air))
                {
                    current = new StatePair(PState.Air, 0);
                    pani.StateChange(true);
                }
                else
                {
                    pani.StateChange(false);
                }
            }
            else
            {
                current = states.Dequeue();


                switch (current.state)
                {
                    case PState.Attack:
                        int frames;
                        if (grounded)
                        {
                            frames = atk.makeAttack(QuadToTypeGround(attkQuad));
                            current = new StatePair(PState.GroundAttack, frames);
                            states.Enqueue(new StatePair(PState.Ground, 0));
                        }
                        else
                        {
                            frames = atk.makeAttack(QuadToTypeAir(attkQuad));
                            current = new StatePair(PState.AirAttack, frames);
                        }
                        break;
                    case PState.Dash:
                        if (!calcDashVel())
                        {
                            current = new StatePair(PState.Air, 0);
                        }
                        break;
                    case PState.CeilingHold:
                        RaycastHit2D r = Physics2D.Raycast(transform.position, Vector3.up, 2.0f, 1<<10);
                        transform.position = new Vector3(transform.position.x, r.point.y -col.bounds.extents.y, 0);
                        break;
                }
                pani.StateChange(true);
            }
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
        if ((desired.x > 0f && OnRightWall) || (desired.x < 0f && OnLeftWall))
        {
            desired.x = 0;
        }
        return desired;
    }
    void calculateDI()
    {
        if (ci.move == Vector2.zero)
        {
            return;
        }
        Vector2 angle = ci.move;
        float angleDiff = Vector2.Angle(hitVector, angle);

        if (angleDiff > 90)
        {
            angleDiff -= 90;
        }
        if (Vector3.Cross(new Vector3(hitVector.x, hitVector.y, 0), new Vector3(angle.x, angle.y, 0)).z < 0)
        {
            angleDiff = -angleDiff;
        }
        //after calculating the difference in degrees, we set it so a 90 degree difference corresponds to maxDI degrees of vector rotation
        angleDiff = angleDiff * maxDI / 90;
        hitVector = Quaternion.Euler(0, 0, angleDiff) * hitVector;
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
    AttackManager.AtkType QuadToTypeAir(ControlInterpret.StickQuadrant q)
    {
        switch (q)
        {
            case ControlInterpret.StickQuadrant.Neutral:
                return AttackManager.AtkType.NeutralAir;
            case ControlInterpret.StickQuadrant.Up:
                return AttackManager.AtkType.UpAir;
            case ControlInterpret.StickQuadrant.Down:
                return AttackManager.AtkType.DownAir;
            case ControlInterpret.StickQuadrant.Left:
                if (FacingLeft)
                {
                    return AttackManager.AtkType.ForwardAir;
                }
                return AttackManager.AtkType.BackAir;
            case ControlInterpret.StickQuadrant.Right:
                if (FacingLeft)
                {
                    return AttackManager.AtkType.BackAir;
                }
                return AttackManager.AtkType.ForwardAir;
            default:
                return AttackManager.AtkType.NeutralAir;

        }
    }
    AttackManager.AtkType QuadToTypeGround(ControlInterpret.StickQuadrant q)
    {
        switch (q)
        {
            case ControlInterpret.StickQuadrant.Neutral:
                return AttackManager.AtkType.NeutralGround;
            case ControlInterpret.StickQuadrant.Up:
                return AttackManager.AtkType.UpGround;
            case ControlInterpret.StickQuadrant.Down:
                return AttackManager.AtkType.DownGround;
            case ControlInterpret.StickQuadrant.Left:
                if (FacingLeft)
                {
                    return AttackManager.AtkType.ForwardGround;
                }
                return AttackManager.AtkType.BackGround;
            case ControlInterpret.StickQuadrant.Right:
                if (FacingLeft)
                {
                    return AttackManager.AtkType.BackGround;
                }
                return AttackManager.AtkType.ForwardGround;
            default:
                return AttackManager.AtkType.NeutralGround;
        }
    }
}
