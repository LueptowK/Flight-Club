using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    Animator ani;
    PlayerMover pm;
    bool backDash =false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        ani = GetComponent<Animator>();
        pm = GetComponent<PlayerMover>();


        
    }

    // Update is called once per frame
    float animSpd = 0.2f;
    private AnimatorStateInfo currentBaseState;

    void FixedUpdate() {
        PlayerMover.StatePair c = pm.currentState;
        float a, b;

        switch (c.state)
        {
            case PlayerMover.PState.Dash:

                if (pm.FacingLeft)
                {
                    Vector2 v2 = pm.dashDirection- Vector2.left;
                    a = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg *-1;
                    a += 180;

                }
                else
                {
                    Vector2 v2 = pm.dashDirection - Vector2.right;
                    a = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

                }
                if (backDash)
                {
                    a += 200;
                }
                break;
            default:
                a = 0;
                break;

        }
        


        if (pm.FacingLeft){ b = 180;}
        else{b = 0;}

        transform.rotation = Quaternion.Euler(0, b, a);
        

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
        GroundAttack
        
    }
    void LateUpdate()
    {


        ani.SetBool("Running", running);
        ani.SetBool("Grounded", pm.grounded);
        ani.SetBool("OnWall", pm.onWall);
        ani.SetFloat("WalkSpeed", animSpd * Mathf.Pow(Mathf.Abs(rb.velocity.x), 1.1f));


        
    }
    public void jump()
    {
        ani.SetTrigger("Jump");
    }
    public void TauntD()
    {
        ani.SetTrigger("TauntD");
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
