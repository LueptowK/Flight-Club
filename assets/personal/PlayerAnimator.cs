using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
    Animator ani;

    List<UnityEditor.Animations.AnimatorControllerLayer> layers = new List<UnityEditor.Animations.AnimatorControllerLayer>();
    List<UnityEditor.Animations.AnimatorState> statesBase = new List<UnityEditor.Animations.AnimatorState>();

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
        ani = GetComponent<Animator>();


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
    float animSpd = 0.3f;
    void FixedUpdate() {
        foreach (UnityEditor.Animations.AnimatorState state in statesBase)
        {
            if (state.name == "walkstart")
            {
                state.speed = rb.velocity.magnitude* animSpd*3f;
            }
            else if(state.name == "walkloop")
            {
                state.speed = rb.velocity.magnitude * animSpd;
            }
        }
        ani.SetBool("Running", running);
    }

    bool running
    {
        get
        {
            float hor = ci.move.x;
            float vel = rb.velocity.x;
            if (!ci.idle && Mathf.Sign(hor) * Mathf.Sign(vel)!= -1)
            {
                return true;
            }
            return false;
        }
    }

    #region unused
    protected bool CompareBaseState(string stateName)
    {
        AnimatorStateInfo currentState = ani.GetCurrentAnimatorStateInfo(0);

        if (currentState.fullPathHash == Animator.StringToHash(stateName)) { return true; }
        return false;
    }
    #endregion
}
