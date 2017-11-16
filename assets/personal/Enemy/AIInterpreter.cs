using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInterpreter : Interpreter
{
    AIInput ai;
    // Use this for initialization
    void Start()
    {
        ai = GetComponent<AIInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool Attack
    {
        get
        {
            return ai.ctrl.attack;
        }
    }

    public override ControlInterpret.StickQuadrant AttackQuad
    {
        get
        {
            return ai.ctrl.quad;
        }
    }

    public override bool Dash
    {
        get
        {
            return ai.ctrl.dash;
        }
    }

    public override bool Dodge
    {
        get
        {
            return ai.ctrl.dodge;
        }
    }

    public override bool fall
    {
        get
        {
            return ai.ctrl.fall;
        }
    }

    public override bool idle
    {
        get
        {
            return false;
        }
    }

    public override bool Jump
    {
        get
        {
            return ai.ctrl.jump;
        }
    }

    public override bool TapJump
    {
        get
        {
            return false;
        }
    }

    public override Vector2 move
    {
        get
        {
            return ai.ctrl.move;
        }
    }

    public override bool Pause
    {
        get
        {
            return false;
        }
    }

    public override bool PauseExit
    {
        get
        {
            return false;
        }
    }

    public override bool Shoot
    {
        get
        {
            return ai.ctrl.shoot;
        }
    }

    public override bool Slash
    {
        get
        {
            return false;
        }
    }

    public override bool Stall
    {
        get
        {
            return ai.ctrl.stall;
        }
    }

    public override bool TauntDown
    {
        get
        {
            return false;
        }
    }
    public override bool DashHold
    {
        get
        {
            return false;
        }
    }
    


}
