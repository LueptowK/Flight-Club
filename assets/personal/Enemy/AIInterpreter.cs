using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInterpreter : Interpreter
{
    public override bool Attack
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override ControlInterpret.StickQuadrant AttackQuad
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override bool Dash
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override bool Dodge
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override bool fall
    {
        get
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    public override Vector2 move
    {
        get
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    public override bool TauntDown
    {
        get
        {
            return false;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
