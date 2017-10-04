using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interpreter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public abstract Vector2 move
    {
        get;
    }
    public abstract bool idle
    {
        get;
    }
    public abstract bool fall
    {
        get;
    }
    public abstract bool Jump
    {
        get;
        }
    public abstract bool TapJump
    {
        get;
        }
    public abstract bool Dash
    {
        get;
        }
    public abstract bool Stall
    {
        get;
        }
    public abstract bool Slash
    {
        get;
            }
    public abstract bool Pause
    {
        get;
            }
    public abstract bool Dodge
    {
        get;
            }
    public abstract bool Shoot
    {
        get;
            }
    public abstract bool TauntDown
    {
        get;
            }
    public abstract bool PauseExit
    {
        get;
            }
    public abstract ControlInterpret.StickQuadrant AttackQuad
    {
        get;
            }
    public abstract bool Attack
    {
        get;
            }


        }

