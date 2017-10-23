    using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerMover : Mover {

    Rigidbody2D rb;
    Interpreter ci;
    PlayerAnimator pani;
    Collider2D col;
    AttackManager atk;
    Health health;
    ComboCounter combo;
    CameraController cam;
    IFrames iframes;
    Manager man;
    ActorSounds sounds;
    StatTracker tracker;


    public PhysicsMaterial2D neutral;//unused
    public PhysicsMaterial2D bounce; //unused
    public GameObject PhaseUpPre;
    public GameObject PhaseTintPre;
    

    //private AudioSource source;

    GameObject PhaseTint;

    public StatCard cardOne;
    public StatCard cardTwo;
    bool phase2;

    const int PLAYER_LAYER = 9;
    const int FALLING_LAYER = 17;

    public enum PState
    {
        Free,
        Dash,
        Stall,
        Hitstun,
        Delay,
        CeilingHold,
        AirAttack,
        GroundAttack,
        Attack, 
        Finisher,
        Burnout,
        Flip,
        Shoot,
        ShootWall,
        Parry
    }

    public enum ExecState
    {
        None,
        Jump,
        hitLag,
        Death,
        Destroy,
        mapStart,
        Shoot,
        LandLag,
        Grabbed

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

    StatePair current = new StatePair(PState.Free, 0);
    Queue<StatePair> states = new Queue<StatePair>();

    Vector2 dashVel = Vector2.zero;
    Vector2 hitVector;
    ControlInterpret.StickQuadrant attkQuad;
    public int character;
    float maxDI; //Max DI affect on knockback, in degrees
    bool registerHit = false;
    float hitstunFriction;
    int dashCounter;
    int maxDashes;
    int dashesAvailable;
    bool ceilingAvaliable;
    bool dead;
    float moveSpeed;
    float airSpeed;
    float maxAirSpeed;
    float dashMagnitude;
    float gravity;
    float jumpVel;
    float wallJumpXVel;
    float wallJumpYVel;
    float dashEndMomentum;
    int dashTime;
    int stallTime;
    bool falling = false;
    float maxFallSpeed;
    float friction;
    int jumpSquatFrames; // needs to be set before entering ANY DELAY STATE
    int stallCooldown;
    int stallCooldownCurrent = 0;
    int shootCooldown;
    int shootCooldownCurrent = 0;
    float ceilingBooster = 0f;
    bool hittingLag = false;
    Vector2 hittingLagVel;
    Vector3 actualPosition;
    float hitlagShake = 0.2f;


    Vector2 resumeVelocity;

    public bool paused;


    int grabDamage;
    int grabHitstun;
    int grabHitlag;
    Attack grabAtk;
    Vector3 grabDif;

    [HideInInspector]public bool FacingLeft = false;

    [HideInInspector]
    public bool FlipBack = false;

    // Use this for initialization
    void Start() {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<Interpreter>();
        pani = GetComponent<PlayerAnimator>();
        col = GetComponent<Collider2D>();
        atk = GetComponent<AttackManager>();
        health = GetComponent<Health>();
        combo = GetComponent<ComboCounter>();
        iframes = GetComponent<IFrames>();
        sounds = GetComponent<ActorSounds>();
        tracker = GetComponent<StatTracker>();
        dashVel = Vector2.zero;
        restoreTools();
        dead = false;
        paused = false;
        rb.sharedMaterial = neutral;
        phase2 = false;
        loadCard(cardOne);
    }
    void loadCard(StatCard card)
    {
        character = card.character;
        maxDI = card.maxDI;
        hitstunFriction = card.hitstunFriction;
        maxDashes = card.maxDashes;
        //if (maxDashes < dashesAvailable)
        //{
        dashesAvailable = maxDashes;
        //}
        moveSpeed = card.moveSpeed;
        airSpeed = card.airSpeed;
        maxAirSpeed = card.maxAirSpeed;
        dashMagnitude = card.dashMagnitude;
        gravity = card.gravity;
        jumpVel = card.jumpVel;
        wallJumpXVel = card.wallJumpXVel;
        wallJumpYVel = card.wallJumpYVel;
        dashEndMomentum = card.dashEndMomentum;
        dashTime = card.dashTime;
        stallTime = card.stallTime;
        maxFallSpeed = card.maxFallSpeed;
        friction = card.friction;
        jumpSquatFrames = card.jumpSquatFrames;
        stallCooldown = card.stallCooldown;
        shootCooldown = card.shootCooldown;
    }
    public void restoreTools()
    {
        dashesAvailable = maxDashes;
        ceilingAvaliable = true;
    }
    // Update is called once per frame
    void FixedUpdate() {
        
        if (!paused)
        {
            resetLayer();
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
            shootCooldownCurrent--;

            if (registerHit) // hit in the queue
            {
                atk.stopAttack();
                int safety = states.Count;
                falling = false;
                while (!(current.state == PState.Delay && (current.action == ExecState.hitLag||current.action== ExecState.Grabbed)) && safety > 0)
                {
                    nextState();
                    safety--;
                }
            }
            Vector2 vel;
            switch (current.state)
            {
                #region Free State
                case PState.Free:
                    {
                        if (grounded)
                        {
                            #region ground
                            desired.x = move.x * moveSpeed;
                            desired.y = rb.velocity.y - tempGrav * 9.8f * Time.fixedDeltaTime;
                            //flip sprite direction to movement direction on ground
                            if (desired.x < 0) { FacingLeft = true; }
                            else if (desired.x > 0) { FacingLeft = false; }
                            falling = false;

                            restoreTools();
                            changeLayer(ci.move);
                            if (ci.Jump || ci.TapJump)
                            {
                                states.Enqueue(new StatePair(PState.Delay, jumpSquatFrames, ExecState.Jump));
                            }
                            if (ci.move.y >= 0)
                            {
                                tryDash();
                            }
                            if (states.Count < 1)
                            {
                                if (!tryAttack())
                                {
                                    if (!tryFinisher())
                                    {
                                        tryShoot();
                                    }
                                }
                            }
                            if (ci.TauntDown && desired.x == 0)
                            {
                                pani.TauntD();

                                states.Enqueue(new StatePair(PState.Delay, 45));
                            }
                            tryDodge();
                            if (nearWall)
                            {
                                if ((desired.x > 0f && OnRightWall) || (desired.x < 0f && OnLeftWall))
                                {
                                    desired.x = 0;
                                }
                            }


                            rb.velocity = desired;
                            break;
                            #endregion
                        }
                        else
                        {
                            #region air
                            desired = AirControl(move);
                            if (!tryDash())
                            {
                                tryStall();
                            }
                            if (states.Count < 1)
                            {
                                if (!tryAttack())
                                {
                                    if (!tryFinisher())
                                    {
                                        tryShoot();
                                    }
                                }
                            }


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
                                        RaycastHit2D r = wallCast(false);
                                        transform.position = new Vector3(r.point.x - col.bounds.extents.x, transform.position.y, 0);
                                        rb.velocity = new Vector2(0, rb.velocity.y);
                                    }
                                    else
                                    {
                                        RaycastHit2D r = wallCast(true);
                                        transform.position = new Vector3(r.point.x + col.bounds.extents.x, transform.position.y, 0);
                                        rb.velocity = new Vector2(0, rb.velocity.y);
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
                            else if (onCeiling && ci.move.y > 0 && ceilingAvaliable)
                            {
                                states.Enqueue(new StatePair(PState.CeilingHold, 30));
                                ceilingAvaliable = false;
                                dashesAvailable = maxDashes;
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
                        #endregion
                    }
                    break;
                #endregion
                #region Dash State
                case PState.Dash:
                    rb.velocity = dashVel;
                    falling = false;
                    setLayer(true);
                    if (current.delay == 2) { dashVel *= dashEndMomentum; }
                    if (grounded) { dashVel.y = 0; }
                    if (states.Count < 1)
                    {
                        if (!tryStall())
                        {
                            if (!tryDash())
                            {
                                if (!tryAttack())
                                {
                                    tryFinisher();
                                }
                            }
                        }
                    }

                    break;
                #endregion
                #region Stall State
                case PState.Stall:
                    rb.velocity = Vector2.zero;
                    falling = false;
                    if (ci.move.x < 0) { FacingLeft = true; }
                    else if (ci.move.x > 0) { FacingLeft = false; }
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
                            //rb.velocity *= 0.80f;
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
                            transform.position = actualPosition + new Vector3(UnityEngine.Random.Range(-hitlagShake, hitlagShake), UnityEngine.Random.Range(-hitlagShake, hitlagShake), 0);
                            break;
                        case ExecState.mapStart:
                            rb.velocity = new Vector2(0, rb.velocity.y - 9.8f * Time.fixedDeltaTime);
                            break;
                        case ExecState.None:
                            rb.velocity = Vector2.zero;
                            break;
                        case ExecState.LandLag:
                            float tempX = rb.velocity.x;
                            
                            tempX += applyFriction(tempX, 3.0f);
                            
                            if(!grounded)
                            {
                                current.delay = 0;
                            }
                            rb.velocity = new Vector2(tempX, rb.velocity.y - 9.8f * tempGrav * Time.fixedDeltaTime);
                            break;
                        case ExecState.Grabbed:
                            rb.velocity = Vector2.zero;

                           
                            if (!grabAtk)
                            {
                                states.Enqueue(new StatePair(PState.Free, 0));
                                break;
                            }
                            else
                            {
                                transform.position = grabAtk.gameObject.transform.position + grabDif;
                            }
                        
                            break;
                        case ExecState.Death:
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
                        states.Enqueue(new StatePair(PState.Free, 0));
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
                        states.Enqueue(new StatePair(PState.Free, 0));
                    }

                    if (states.Count > 0)
                    {
                        current.delay = 1;
                    }
                    break;
                #endregion
                #region Hitstun State
                case PState.Hitstun:
                    //rb.sharedMaterial = bounce;

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
                            sounds.hitWall();
                        }
                        else if (OnLeftWall && rb.velocity.x < 0)
                        {
                            hitVector = new Vector2(-rb.velocity.x, rb.velocity.y);
                            calculateDI();
                            rb.velocity = hitVector;
                            sounds.hitWall();
                        }

                    }
                   
                    if (onCeiling && rb.velocity.y > 0)
                    {
                        dashesAvailable = maxDashes;
                        hitVector = new Vector2(rb.velocity.x, -rb.velocity.y);
                        calculateDI();
                        rb.velocity = hitVector;
                        sounds.hitWall();
                    }
                    if (rb.velocity.magnitude > 1.5 * moveSpeed && current.delay < 50)
                    {
                        rb.velocity = rb.velocity * hitstunFriction;
                    }
                    
                    rb.velocity += new Vector2(0, -gravity * 9.8f * Time.fixedDeltaTime);
                    break;
                #endregion
                #region AirAttack State
                case PState.AirAttack:
                    atk.NestedUpdate();
                    if (grounded)
                    {
                        int lag =atk.stopAttack();
                        if (lag < 0)
                        {
                            lag = 0; //safety 
                        }
                        current.delay = 0;
                        states.Enqueue(new StatePair(PState.Delay, lag, ExecState.LandLag));
                        // vv was used for old attack states
                        // current.delay = 0;
                        
                    }
                    if (!hittingLag)
                    {
                        MovePhysics.AtkMotion m = atk.getMotion(ci.move);
                        if (m.use)
                        {
                            rb.velocity = m.motion;
                            
                        }
                        else
                        {
                            
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
                        }
                        
                    }
                    break;
                #endregion
                #region GroundAttack State
                case PState.GroundAttack:
                    atk.NestedUpdate();
                    if (!hittingLag)
                    {
                        MovePhysics.AtkMotion m = atk.getMotion(ci.move);
                        if (m.use)
                        {
                            rb.velocity = m.motion;
                        }
                        else
                        {
                            
                            rb.velocity = new Vector2(rb.velocity.x + applyFriction(rb.velocity.x, 5f), rb.velocity.y);
                        }
                        
                    }   
                    if (!grounded)
                    {
                        int lag = atk.stopAttack();
                        current.delay = 0;
                        states.Enqueue(new StatePair(PState.Delay, lag, ExecState.LandLag));
                    }
                    break;
                #endregion
                #region Finisher State
                case PState.Finisher:
                    atk.NestedUpdate();
                    
                    rb.velocity = atk.getMotion(ci.move).motion;
                    break;
                #endregion
                #region Burnout State
                case PState.Burnout:
                    rb.velocity += new Vector2(0, -gravity * 9.8f * Time.fixedDeltaTime);
                    break;
                #endregion              
                #region Shoot State
                case PState.Shoot:
                    vel = rb.velocity;
                    if (grounded)
                    {
                        alignGround();
                        vel.x += applyFriction(vel.x, 2);
                    }
                    vel.y += -gravity * 9.8f * Time.fixedDeltaTime;
                    rb.velocity = vel;

                    if (current.action == ExecState.Shoot&& current.delay!=1)
                    {
                        if (ci.move.x < 0) { FacingLeft = true; }
                        else if (ci.move.x > 0) { FacingLeft = false; }
                    }
                    break;
                #endregion
                #region Shoot Wall State
                case PState.ShootWall:
                    vel = rb.velocity;
                    vel.y += applyFriction(rb.velocity.y);

                    vel.y += -gravity * 9.8f * Time.fixedDeltaTime;
                    if (vel.y < -3)
                    {
                        vel = new Vector2(0, -3);
                    }
                    rb.velocity = vel;
                    break;
                #endregion
                #region Parry State
                case PState.Parry:
                    vel = rb.velocity;
                    vel.x += applyFriction(rb.velocity.y,2);
                    rb.velocity = vel;
                    break;
                    #endregion

                #region Flip State (OLD)
                    //case PState.Flip:
                    //    tempGrav *= 2f;
                    //    float velx = rb.velocity.x;
                    //    if (grounded)
                    //    {
                    //        velx = 0;
                    //        alignGround();
                    //        if (states.Count < 1)
                    //        {
                    //            tryAttack();
                    //        }
                    //    }
                    //    rb.velocity = new Vector2(velx, rb.velocity.y - tempGrav * 9.8f * Time.fixedDeltaTime);
                    //    break;
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

            if (current.delay > 0)
            {
                current.delay -= 1;
            }
            
            if (current.delay == 0)
            {
                
                nextState();

            }
            else if (!registerHit)
            {
                pani.StateChange(false);
            }
            else
            {
                pani.StateChange(true);
                registerHit = false;
            }

            
        }
        
    }
    #region State Changes
    void leaveState()
    {
        switch (current.action)
        {
            case ExecState.None:
                break;
            case ExecState.Jump:
                Vector2 ogVel = (rb.velocity *(1f / 0.6f));
                rb.velocity = new Vector2(ci.move.x * 0.5f * ogVel.x * Mathf.Sign(ogVel.x) + ogVel.x * 0.5f, jumpVel);
                if (this.GetComponentInParent<PlatformMov>() != null)
                {
                    rb.velocity += this.GetComponentInParent<PlatformMov>().speed;
                }
                //states.Enqueue(new StatePair(PState.Air, 0));
                break;
            case ExecState.Death:
                pani.Die();
                break;
            case ExecState.Destroy:
                gameObject.SetActive(false);
                //Debug.Break();
                break;
            #region OLD DODGE
            /* 
        case ExecState.Dodge:
            iframes.SetFrames(20);
            break;
        case ExecState.Flip:
            vel = new Vector2();
            vel += Vector2.up * 8f;
            float velx = 20f;
            if (FlipBack)
            {
                if (FacingLeft)
                {
                    vel += Vector2.right * velx;
                }
                else
                {
                    vel += Vector2.left * velx;
                }

            }
            else
            {
                if (FacingLeft)
                {
                    vel += Vector2.left * velx;
                }
                else
                {
                    vel += Vector2.right * velx;
                }
            }
            rb.velocity = vel;
            iframes.SetFrames(24);
            break; */
            #endregion
            case ExecState.Shoot:
                if (current.state == PState.Shoot)
                {
                    atk.shoot(false);
                }
                else
                {
                    atk.shoot(true);
                }

                break;

        }
    }
    void enterState()
    {
        switch (current.state)
        {

            case PState.Finisher:
                atk.makeAttack(AttackManager.AtkType.Finisher);
                //current.delay = frames;

                //states.Enqueue(new StatePair(PState.Ground,0));
                break;
            case PState.Attack:
                if (grounded)
                {
                    atk.makeAttack(QuadToTypeGround(attkQuad));
                    current = new StatePair(PState.GroundAttack, -1);
                    //states.Enqueue(new StatePair(PState.Ground, 0));
                }
                else
                {
                    atk.makeAttack(QuadToTypeAir(attkQuad));
                    current = new StatePair(PState.AirAttack, -1);
                }
                break;
            case PState.Dash:

                if (!calcDashVel())
                {
                    current = new StatePair(PState.Free, 0);
                }
                break;
            case PState.CeilingHold:
                RaycastHit2D r = Physics2D.Raycast(transform.position, Vector3.up, 2.0f, 1 << 10);
                transform.position = new Vector3(transform.position.x, r.point.y - col.bounds.extents.y, 0);
                break;
            case PState.Burnout:
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(8f, rb.velocity.y));
                break;
            case PState.Hitstun:
                if ((OnLeftWall && hitVector.x < 0) || (OnRightWall && hitVector.x > 0))
                {
                    hitVector = new Vector2(-hitVector.x, hitVector.y);
                }
                if ((onCeiling && hitVector.y > 0) || (grounded && hitVector.y < 0))
                {
                    hitVector = new Vector2(hitVector.x, -hitVector.y);
                }

                if (ci.move != Vector2.zero)
                {
                    calculateDI();
                }

                transform.position = actualPosition;
                rb.velocity = hitVector;
                break;
            case PState.Delay:
                if(current.action == ExecState.Jump)
                {
                    rb.velocity *= 0.6f;
                }
                else if (current.action == ExecState.Death)
                {
                    atk.stopAttack();
                }
                break;
        }
    }
    void nextState()
    {

        if (states.Count == 0)
        {
            if ((current.state != PState.Free))
            {
                leaveState();
                current = new StatePair(PState.Free, 0);
                if (grounded)
                {
                    alignGround();
                    rb.velocity = new Vector2(rb.velocity.x, 0);

                }

                pani.StateChange(true);
            }
            else
            {
                pani.StateChange(false);
            }
        }
        else
        {
            leaveState();
            current = states.Dequeue();
            //print(current.action);

            enterState();
            pani.StateChange(true);
        }



    }
    #endregion
    void Update()
    {
        if (man != null)
        {

            if (ci.Pause)
            {
                man.Pause();
            }
            if (paused)
            {
                if (ci.PauseExit)
                {
                    Time.timeScale = 1;
                    man.Quit();
                }
            }
        }
    }
    #region Getting Hit On
    public void grabFin()
    {
        
        takeDamage(grabDamage, grabHitlag, grabHitstun);
    }
    public void getHit(Vector2 knockback, int hitLag, int hitStun, int damage)
    {
        getHit( knockback, hitLag, hitStun,damage, null);
    }
    public void getHit(Vector2 knockback, int hitLag, int hitStun, int damage, Attack a)
    {
        sounds.getHit(damage);
        if (!dead)
        {
            //print(knockback + " ---- " + hitLag+" ---- " + hitStun);
            if(current.state== PState.Parry&& (!a || a.tag!= "Finisher"))
            {
                iframes.SetFrames(60);
                //((PlayerHealth)health).charge(0.15f);
            }
            else
            {
                hitVector = knockback;
                registerHit = true;
                rb.velocity = Vector2.zero;
                states = new Queue<StatePair>();

                

                if (a!=null && a.isActive &&((AttackActive)a).isGrab)
                {
                    grabDamage = damage;
                    grabAtk = a;
                    grabHitstun = hitStun;
                    grabHitlag = hitLag;
                    //grabDif = transform.position - a.transform.position;
                    //grabDif = a.transform.up * 1f;
                    grabDif = Vector2.zero;
                    states.Enqueue(new StatePair(PState.Delay, -1, ExecState.Grabbed));
                }
                else
                {
                    takeDamage(damage, hitLag, hitStun);
                    if (a != null && !a.isActive&&!(health.currentHealth<=0))
                    {
                        iframes.SetFrames(75);
                    }
                }

                
                

                
            }
            
            //DAMAGE
            
            
        }
    }
    void takeDamage(int damage, int hitLag, int hitStun)
    {
        tracker.takeDamage(damage);
        int result = health.takeDamage(damage);
        if (result == 0)
        {
            if (hitLag > 0)
            {
                states.Enqueue(new StatePair(PState.Delay, hitLag, ExecState.hitLag));
            }
           
            states.Enqueue(new StatePair(PState.Hitstun, hitStun));
        }
        else //result is 1
        {
            phaseDownState(hitStun);
        }
        actualPosition = transform.position;
        cam.screenShake = (float)damage;
    }

    void phaseDownState(int hitStun)
    {
        states.Enqueue(new StatePair(PState.Delay, 30, ExecState.hitLag));
        states.Enqueue(new StatePair(PState.Burnout, Mathf.Max(40, hitStun)));
        iframes.SetFrames(Mathf.Max(90, hitStun) + 25);
        combo.reset();
    }
    #endregion
    #region try moves
    bool tryDash()
    {
        if (ci.Dash && (dashesAvailable > 0)) // DASH
        {
            //calcDashVel();
            dashesAvailable--;
            states.Enqueue(new StatePair(PState.Dash, dashTime));
            sounds.dash();
            tracker.dash();
            return true;
        }
        return false;
    }
    bool calcDashVel()
    {
        Vector2 input = ci.move.normalized;
        float marigin = 0.70f;
        float minimum = 0.2f;
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
            if (input.x < minimum)
            {
                if (input.x >= -marigin && input.y != 0)
                {
                    input.x = minimum;
                }
                else
                {
                    return false;
                }
            }
            

        }
        else if (Physics2D.Raycast(transform.position, Vector2.right, col.bounds.extents.x + 0.5f, 1 << 8))
        {
            if (input.x > -minimum)
            {
                if (input.x <= marigin && input.y != 0)
                {
                    input.x = -minimum;
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
            tracker.Stall();
            return true;
        }
        return false;
    }


    bool tryFinisher()
    {
        if (ci.Slash && combo.currentCombo > 0&& !phase2)
        {
            states.Enqueue(new StatePair(PState.Finisher, -1));
            tracker.finisherAttempt();
            return true;
        }
        return false;
    }
    bool tryAttack()
    {
        if (ci.Attack)
        {
            attkQuad = ci.AttackQuad;
            //if (phase2)
            //{
            //    if(attkQuad!= ControlInterpret.StickQuadrant.Neutral || grounded)
            //    {
            //        return false;
            //    }
            //}
            states.Enqueue(new StatePair(PState.Attack, 0));
            tracker.hitAttempt();
            return true;
        }
        return false;
    }
    bool tryDodge()
    {

        #region OLD DODGE CODE
        /*
        if (ci.Dodge&& states.Count<1)
        {
            if(ci.moveQuad == ControlInterpret.StickQuadrant.Down)
            {
                states.Enqueue(new StatePair(PState.Delay, 4, ExecState.Dodge));
                states.Enqueue(new StatePair(PState.Delay, 30));
                //states.Enqueue(new StatePair(PState.Ground, 0));
                return true;
            }
            else if(ci.moveQuad == ControlInterpret.StickQuadrant.Right||ci.moveQuad == ControlInterpret.StickQuadrant.Left)
            {
                if((ci.moveQuad == ControlInterpret.StickQuadrant.Left&& !FacingLeft)||(ci.moveQuad == ControlInterpret.StickQuadrant.Right) && FacingLeft)
                {
                    FlipBack = true;
                }
                else
                {
                    FlipBack = false;
                }
                states.Enqueue(new StatePair(PState.Delay, 4, ExecState.Flip));
                states.Enqueue(new StatePair(PState.Flip, 30));
                states.Enqueue(new StatePair(PState.Delay, 10, ExecState.Normal));
                //states.Enqueue(new StatePair(PState.Ground, 0));
                return true;
            }
            else
            {
                return false;
            }

        }

        return false;
        */
        #endregion
        //if (ci.Dodge && states.Count < 1&& phase2)
        //{
        //    states.Enqueue(new StatePair(PState.Delay, 1));
        //    states.Enqueue(new StatePair(PState.Parry, 10 ));
        //    states.Enqueue(new StatePair(PState.Delay, 30));
        //    return true;
        //}
        return false;

    }

    bool trySpecialDamage(int damage)
    {
        return ((PlayerHealth)health).drainShield(damage);
    }

    bool tryShoot()
    {
        if (ci.Shoot&&!phase2&&shootCooldownCurrent<=0)
        {
            if (trySpecialDamage(5))
            {
                if (onWall && !grounded)
                {
                    states.Enqueue(new StatePair(PState.ShootWall, 2, ExecState.Shoot));
                    states.Enqueue(new StatePair(PState.ShootWall, 5));
                }
                else
                {
                    states.Enqueue(new StatePair(PState.Shoot, 2, ExecState.Shoot));
                    states.Enqueue(new StatePair(PState.Shoot, 5));
                }

                shootCooldownCurrent = shootCooldown;
            }



            tracker.projectile();
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
            int layerMask = 1 << 10;
            if(gameObject.layer == PLAYER_LAYER)
            {
                layerMask = layerMask | 1 << 16;
            }
            RaycastHit2D ray = Physics2D.BoxCast(transform.position - new Vector3(0, col.bounds.extents.y, 0), new Vector2(col.bounds.extents.x * 1.9f, col.bounds.extents.y * 1f), 0, -Vector2.up, 0.08f, layerMask);
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
            return (OnRightWall && ci.move.x > 0f && rb.velocity.x>=0 ) || (OnLeftWall && ci.move.x < 0f && rb.velocity.x < 0.001f);
        }
    }

    public bool OnRightWall
    {
        get
        {
            return wallCast(false);
            //return Physics2D.Raycast(transform.position, Vector2.right, col.bounds.extents.x + 0.1f, 1 << 8);
        }
    }
    public bool OnLeftWall
    {
        get
        {
            return wallCast(true);

            //return Physics2D.Raycast(transform.position, -Vector2.right, col.bounds.extents.x + 0.1f, 1 << 8);

        }
    }
    RaycastHit2D wallCast(bool left)
    {
        Vector3 dist = new Vector3(col.bounds.extents.x, 0);
        Vector2 box = new Vector2(col.bounds.extents.x / 2, col.bounds.extents.y / 2);
        Vector2 dir = Vector2.right;
        if (left)
        {
            dir = -dir;
            dist = -dist;
        }
        return Physics2D.BoxCast(transform.position + dist / 2, box, 0, dir, 0.2f, 1 << 8);

    }
    #endregion
    
    public void atkFinished(bool burnout)
    {
        if (burnout)
        {
            states.Enqueue(new StatePair(PState.Burnout, 110));
        }
        current.delay = 0;
        
    }
    void alignGround()
    {
        RaycastHit2D r = Physics2D.BoxCast(transform.position - new Vector3(0, col.bounds.extents.y, 0), new Vector2(col.bounds.extents.x * 1.9f, col.bounds.extents.y * 1f), 0, -Vector2.up, 0.08f, 1 << 10|1<<16);
        transform.position = new Vector3(transform.position.x, r.point.y + col.bounds.extents.y, 0);
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
    float applyFriction(float vel, float mult = 1)
    {
        float newVel = -Math.Sign(vel) * friction * mult * Time.fixedDeltaTime;
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

    public override void kill()
    {

        //pani.StateChange(true);
        
        registerHit = false;
        dead = true;
        if (combo) //enemies dont have combo
        {
            combo.reset();
        }
        if (PhaseTint)
        {
            Destroy(PhaseTint);
        }
        iframes.SetFrames(0);
        current.delay = 0;
        states = new Queue<StatePair>();
        states.Enqueue(new StatePair(PState.Delay, 30, ExecState.Death));
        states.Enqueue( new StatePair(PState.Delay, 60, ExecState.Destroy));
    }

    public void mapStart()
    {
        states = new Queue<StatePair>();
        states.Enqueue(new StatePair(PState.Delay, 90, ExecState.mapStart));
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        man = GameObject.Find("Main Camera").GetComponent<Manager>();
    }

    public void SinglePlayerStart()
    {
        states = new Queue<StatePair>();
        states.Enqueue(new StatePair(PState.Delay, 60, ExecState.mapStart));
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        man = GameObject.Find("Main Camera").GetComponent<Manager>();
    }

    public void TutorialStart()
    {
        states = new Queue<StatePair>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        man = GameObject.Find("Main Camera").GetComponent<Manager>();
    }

    public void reset()
    {
        states = new Queue<StatePair>();
        health.currentHealth = health.maxHealth;
        dead = false;
        current = new StatePair(PState.Free, 0);
    }

    public void pause(bool pausing)
    {
        if (paused && !pausing)
        {
            //rb.velocity = resumeVelocity;
            //pani.pause(false);
            paused = false;
            /*
            if (combo)
            {
                combo.paused = false;
            }
            */
        }
        if (!paused && pausing)
        {
            //resumeVelocity = rb.velocity;
            //rb.velocity = Vector2.zero;
            //pani.pause(true);
            paused = true;
            /*
            if (combo)
            {
                combo.paused = true;
            }
            */
        }
    }

    public void hitting(bool isHitting, bool useVel = true)
    {
        if (isHitting)
        {
            if (ci.Stall && !phase2 && trySpecialDamage(2))
            {
                hittingLagVel = Vector2.zero;
                tracker.hitStall();
            }
            else
            {
                hittingLagVel = rb.velocity;
            }
            rb.velocity = Vector2.zero;
            hittingLag = true;
        }
        else
        {
            if (useVel)
            {
                rb.velocity = hittingLagVel;
            }
            
            hittingLag = false;
        }
        
    }
    

    public void phaseUp()
    {
        phase2 = false;
        //loadCard(cardOne);
        Instantiate(PhaseUpPre, transform);
        loadCard(cardOne);
        Destroy(PhaseTint);
    }
    public void phaseDown()
    {
        phase2 = true;
        //loadCard(cardTwo);
        PhaseTint = Instantiate(PhaseTintPre, transform);
        //maxDashes = 0;
    }
    public bool actionable
    {
        get
        {
            PState s = current.state;
            if(s== PState.Hitstun)
            {
                return false;
            }
            else if(s == PState.Delay)
            {
                ExecState a = current.action;
                if (a == ExecState.hitLag || a == ExecState.Grabbed||a == ExecState.Death ||a == ExecState.Destroy)
                {
                    
                    return false;
                }
                return true;
            }
            return true;
        }
    }

    void changeLayer(Vector2 inp)
    {
        if (inp.y < -0.7)
        {
            setLayer(true);
            
        }
    }
    void resetLayer()
    {
        if(gameObject.layer == FALLING_LAYER&& current.state!= PState.Dash)
        {
            ContactFilter2D cf = new ContactFilter2D();
            Collider2D[] ret = new Collider2D[1];
            cf.layerMask = (1 << 16);
            cf.useLayerMask = true;
            cf.useTriggers = true;
            col.OverlapCollider(cf, ret);
            if (ret[0] == null)
            {
                setLayer(false);
            }
        }
    }
    public void setLayer(bool falling)
    {

        if (falling)
        {
            gameObject.layer = FALLING_LAYER;
    
        }
        else
        {
            gameObject.layer = PLAYER_LAYER;
        }
    }

    public bool isPhase2()
    {
        return phase2;
    }
    
}
