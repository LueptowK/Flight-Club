using UnityEngine;
using System.Collections;
using System;
using XInputDotNetPure;

public class PlayerInput : MonoBehaviour {
    public int PlayerNumber = 1;

    private GamePadState state;
    
    void FixedUpdate()
    {
        state = GamePad.GetState((PlayerIndex)PlayerNumber);
    }

    public float MoveVer { get {
            //float ver = Input.GetAxis("Vertical" + PlayerNumber);
            float ver = state.ThumbSticks.Left.Y;
            return ver;
            
        }
    }
    public float MoveHor { get {
            float hor = state.ThumbSticks.Left.X;
            return hor;
        } }
    public float LookVer
    {
        get
        {
            float ver = state.ThumbSticks.Right.Y;
            return ver;

        }
    }
    public float LookHor
    {
        get
        {
            float hor = state.ThumbSticks.Left.X;
            return hor;
        }
    }
    public float Jump
    {
        get
        {
            float jump = state.Triggers.Right;
            //print(jump + "  --  " + PlayerNumber);
            return jump;
        }
    }
    public float Dash
    {
        get
        {
            float dash = state.Triggers.Left;
            return dash;
        }
    }
    public bool Stall
    {
        get
        {
            bool stall = state.Buttons.LeftStick == ButtonState.Pressed;
            return stall;
        }
    }
    public bool TauntD
    {
        get
        {
            return state.DPad.Down==ButtonState.Pressed;
        }
    }
}
