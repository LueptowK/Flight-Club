using UnityEngine;
using System.Collections;
using System;
using XInputDotNetPure;

public class PlayerInput : MonoBehaviour {
    public int PlayerNumber = 0;

    private GamePadState state;
    
    void FixedUpdate()
    {
        state = GamePad.GetState((PlayerIndex)PlayerNumber);
    }
    void Update()
    {
        if (Time.timeScale == 0)
        {
            state = GamePad.GetState((PlayerIndex)PlayerNumber);
        }
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
    public float AtkVer
    {
        get
        {
            float ver = state.ThumbSticks.Right.Y;
            return ver;

        }
    }
    public float AtkHor
    {
        get
        {
            float hor = state.ThumbSticks.Right.X;
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
            bool stall = state.Buttons.RightStick == ButtonState.Pressed;
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
    public bool Pause
    {
        get
        {
            return state.Buttons.Start == ButtonState.Pressed;
        }
    }
    public bool FinisherSlash
    {
        get
        {
            return state.Buttons.X == ButtonState.Pressed;
        }
    }
    public bool Attack
    {
        get
        {
            return state.Buttons.A == ButtonState.Pressed;
        }
    }
    public bool Dodge
    {
        get
        {
            return state.Buttons.B == ButtonState.Pressed;
        }
    }
    public bool Shoot
    {
        get
        {
            return state.Buttons.RightShoulder == ButtonState.Pressed;
        }
    }
    public bool PauseExit
    {
        get
        {
            return (state.Buttons.A == ButtonState.Pressed && state.Buttons.X == ButtonState.Pressed && (state.Triggers.Left > 0.55f) && (state.Triggers.Right > 0.55f));
        }
    }
}
