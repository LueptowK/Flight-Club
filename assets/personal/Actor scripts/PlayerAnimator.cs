using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    Rigidbody2D rb;
    Interpreter ci;
    Animator ani;
    PlayerMover pm;
    AttackManager am;
    bool backDash =false;

    public float playerScale;
    Vector3 realScale;

    float currentSpeed = 1f;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<Interpreter>();
        ani = GetComponent<Animator>();
        pm = GetComponent<PlayerMover>();
        am = GetComponent<AttackManager>();
        playerScale = pm.cardOne.scale;
        realScale = new Vector3(playerScale,playerScale,playerScale);


    }
    public Vector3 ShootPos(bool backwards)
    {
        Vector3 xComp = transform.right * playerScale * 0.5f;
        if (backwards)
        {
            xComp *= -1;
        }
        if (pm.FacingLeft)
        {
            return transform.position + xComp + -transform.up * playerScale * 0.5f;
        }
        else
        {
            return transform.position + xComp + transform.up * playerScale * 0.5f;
        }
            
    }
    // Update is called once per frame
    float animSpd = 0.2f;
    private AnimatorStateInfo currentBaseState;

    void FixedUpdate() {
        PlayerMover.StatePair c = pm.currentState;
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        float a, b;
        realScale = transform.localScale;
        

        switch (c.state)
        {
            case PlayerMover.PState.Dash:
                a = rotFromVec(pm.dashDirection,0);

                
                if (backDash)
                {
                    a += 200;
                }
                break;
            case PlayerMover.PState.Burnout:

                if (pm.grounded)
                {
                    a = 0;
                }
                else
                {
                    int direction = 1;
                    if (pm.FacingLeft)
                    {
                        direction = -1;
                    }
                    float secPerRot = 0.6f;
                    float t = Time.time % secPerRot;
                    a = t * 360 *direction /secPerRot ;
                }

                break;
            case PlayerMover.PState.Finisher:
                a = rotFromVec(rb.velocity,-90);
                break;
            default:
                a = 0;
                break;

        }
        


        if (pm.FacingLeft){ b = -Mathf.Abs(realScale.y); a += 180; }
        else{b = Mathf.Abs(realScale.y); }

        //transform.rotation= Quaternion.Euler(0, 0, a);
        transform.localRotation = Quaternion.Euler(0, 0, a);
        transform.localScale = new Vector3(realScale.x, b, realScale.z);

        Color tmp = spr.color;
        if (GetComponent<IFrames>().invincible())
        {
            float secPerCycle = 0.2f;
            float t = Time.time % secPerCycle;
            float per = Mathf.Abs(t - (secPerCycle / 2))/(secPerCycle/2);
            //print(per);
            //spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 255 - (130 * per));
            tmp.a = 0.5f - 0.3f * per;
        }
        else if(c.state== PlayerMover.PState.Parry)
        {
            float secPerCycle = 0.05f;
            float t = Time.time % secPerCycle;
            float per = Mathf.Abs(t - (secPerCycle / 2)) / (secPerCycle / 2);
            //print(per);
            //spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 255 - (130 * per));
            tmp.a = 1f - 0.8f * per;
        }
        else
        {
            tmp.a = 1f;
        }

        spr.color = tmp;

    }
    float rotFromVec(Vector2 v, float extra)
    {
        float a;
        if (v == Vector2.zero)
        {
            a = 0;
        }
        else {
            if (pm.FacingLeft)
            {
                Vector2 v2 = v - Vector2.left;
                a = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                a += 180;
                a -= extra;
            }
            else
            {
                Vector2 v2 = v - Vector2.right;
                a = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                a += extra;
            }
        }
        return a;
    }
    enum AnimationState
    {
        Unknown, //0
        Ground,
        JumpSquat,
        Air,
        Ceiling,
        Dash, //5
        Stall,
        Hitlag,
        Hitstun, //8
        AirAttack, 
        GroundAttack,
        Finisher,
        Burnout,  //12
        Flip,
        Shoot,
        LandLag,
        Parry //16
        
    }
    void LateUpdate()
    {


        ani.SetBool("Running", running);
        ani.SetBool("Grounded", pm.grounded);
        ani.SetBool("OnWall", pm.onWall);
        ani.SetFloat("WalkSpeed", animSpd * Mathf.Pow(Mathf.Abs(rb.velocity.x), 1.1f));

        ani.speed = currentSpeed;
        
    }
    public void updateFacing()
    {
        float b= realScale.y;
        if (pm.FacingLeft) { b = -b;}
        transform.localScale = new Vector3(realScale.x, b, realScale.z);

    }
    public void jump()
    {
        ani.SetTrigger("Jump");
    }
    public void TauntD()
    {
        ani.SetTrigger("TauntD");
    }
    public void Die()
    {
        ani.SetTrigger("Die");
    }
    public void pause(bool paused)
    {
        if (paused)
        {
            currentSpeed = 0;
        }
        else
        {
            currentSpeed = 1;
        }
    }
    public void StateChange(bool s)
    {

        ani.SetBool("StateChange", s);
        if (s)
        {
            ReadState();
        }
     


    }

    public void ReadState()
    {
        PlayerMover.StatePair c = pm.currentState;
        if (c.state == PlayerMover.PState.Ground)
        {
            ani.SetInteger("State", (int)AnimationState.Ground);
        }
        else if (c.state == PlayerMover.PState.Delay)
        {
            if (c.action == PlayerMover.ExecState.Jump)
            {
                ani.SetInteger("State", (int)AnimationState.JumpSquat);
            }
            else if (c.action == PlayerMover.ExecState.hitLag)
            {
                ani.SetInteger("State", (int)AnimationState.Hitlag);
            }
            else if (c.action == PlayerMover.ExecState.Death)
            {
                ani.SetInteger("State", (int)AnimationState.Hitlag);
            }
            else if (c.action == PlayerMover.ExecState.mapStart)
            {
                ani.SetInteger("State", (int)AnimationState.Ground);
            }
            else if (c.action == PlayerMover.ExecState.LandLag)
            {
                ani.SetInteger("State", (int)AnimationState.LandLag);
            }
            else if (c.action == PlayerMover.ExecState.Grabbed)
            {
                ani.SetInteger("State", (int)AnimationState.Hitstun);
            }
            else
            {
                ani.SetInteger("State", (int)AnimationState.Unknown);
            }
        }
        else if (c.state == PlayerMover.PState.Air)
        {
            ani.SetInteger("State", (int)AnimationState.Air);
        }
        else if (c.state == PlayerMover.PState.CeilingHold)
        {
            ani.SetInteger("State", (int)AnimationState.Ceiling);
        }
        else if (c.state == PlayerMover.PState.Dash)
        {
            ani.SetInteger("State", (int)AnimationState.Dash);
            backDash = false;
            if (pm.FacingLeft && pm.dashDirection.x > 0)
            {
                backDash = true;
            }
            else if (!pm.FacingLeft && pm.dashDirection.x < 0)
            {
                backDash = true;
            }

            ani.SetBool("DashBack", backDash);
        }
        else if (c.state == PlayerMover.PState.Stall)
        {
            ani.SetInteger("State", (int)AnimationState.Stall);
        }
        else if (c.state == PlayerMover.PState.Hitstun)
        {
            ani.SetInteger("State", (int)AnimationState.Hitstun);
        }
        else if (c.state == PlayerMover.PState.AirAttack)
        {
            ani.SetInteger("State", (int)AnimationState.AirAttack);
        }
        else if (c.state == PlayerMover.PState.GroundAttack)
        {
            ani.SetInteger("State", (int)AnimationState.GroundAttack);
        }
        else if (c.state == PlayerMover.PState.Finisher)
        {
            ani.SetInteger("State", (int)AnimationState.Finisher);
        }
        else if (c.state == PlayerMover.PState.Burnout)
        {
            ani.SetInteger("State", (int)AnimationState.Burnout);
        }
        else if (c.state == PlayerMover.PState.Flip)
        {
            ani.SetInteger("State", (int)AnimationState.Flip);
        }
        else if (c.state == PlayerMover.PState.Shoot)
        {
            ani.SetInteger("State", (int)AnimationState.Shoot);
        }
        else if (c.state == PlayerMover.PState.ShootWall)
        {
            ani.SetInteger("State", (int)AnimationState.Shoot);
        }
        else if (c.state == PlayerMover.PState.Parry)
        {
            ani.SetInteger("State", (int)AnimationState.Parry);
        }
        else
        {
            ani.SetInteger("State", (int)AnimationState.Unknown);
        }
    }



    bool running
    {
        get
        {
            float hor = ci.move.x;
            float vel = rb.velocity.x;
            if (!ci.idle && Mathf.Sign(hor) * Mathf.Sign(vel)!= -1&&vel!=0)
            {
                return true;
            }
            return false;
        }
    }

   


    protected bool CompareBaseState(string stateName)
    {

        /*
        if (currentState.fullPathHash == Animator.StringToHash(stateName)) {  return true; }
        */
        return false;
        
    }

}
