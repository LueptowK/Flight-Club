using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    Animator ani;
    PlayerMover pm;

    List<UnityEditor.Animations.AnimatorControllerLayer> layers = new List<UnityEditor.Animations.AnimatorControllerLayer>();
    List<UnityEditor.Animations.AnimatorState> statesBase = new List<UnityEditor.Animations.AnimatorState>();

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        ani = GetComponent<Animator>();
        pm = GetComponent<PlayerMover>();


        UnityEditor.Animations.AnimatorController ac = (UnityEditor.Animations.AnimatorController)ani.runtimeAnimatorController;
        foreach (UnityEditor.Animations.AnimatorControllerLayer acl in ac.layers)
        {
            layers.Add(acl);
        }

        // Base layer states
        foreach (UnityEditor.Animations.ChildAnimatorState s in layers[0].stateMachine.states)
        {
            statesBase.Add(s.state);
        }
        
    }

    // Update is called once per frame
    float animSpd = 0.2f;
    private AnimatorStateInfo currentBaseState;

    void FixedUpdate() {




    }

    void LateUpdate()
    {
        ani.SetBool("Running", running);
        ani.SetBool("Grounded", pm.grounded);
        ani.SetBool("OnWall", pm.onWall);
        ani.SetFloat("WalkSpeed", animSpd * Mathf.Pow(Mathf.Abs(rb.velocity.x), 1.1f));
        ani.SetBool("Ceiling", pm.currentState==PlayerMover.PState.CeilingHold);

    }
    public void jump()
    {
        ani.SetTrigger("Jump");
    }
    public void jumpSquat()
    {
        ani.SetTrigger("JumpSquat");
    }
    public void dash()
    {
        ani.SetTrigger("Dash");

    }
    public void TauntD()
    {
        ani.SetTrigger("TauntD");
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
        AnimatorStateInfo currentState = ani.GetCurrentAnimatorStateInfo(0);

        if (currentState.fullPathHash == Animator.StringToHash(stateName)) {  return true; }
        return false;
    }

}
