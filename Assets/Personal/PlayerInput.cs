using UnityEngine;
using System.Collections;
using System;
using XInputDotNetPure;

public class PlayerInput : Controller {
    public float AxisAdjust = 0.15f;
    public int PlayerNumber = 1;

    private GamePadState state;
    
    void FixedUpdate()
    {
        state = GamePad.GetState((PlayerIndex)PlayerNumber);
    }

    public override float MoveVer { get {
            //float ver = Input.GetAxis("Vertical" + PlayerNumber);
            float ver = state.ThumbSticks.Left.Y;
            return ver;
            
        }
    }
    public override float MoveHor { get {
            float hor = state.ThumbSticks.Left.X;
            return hor;
        } }
    public override float LookVer
    {
        get
        {
            float ver = Input.GetAxis("LookV");
            float perAdj = ver / (1 - AxisAdjust);
            if (ver != 0) { ver += perAdj * AxisAdjust; }
            return ver;

        }
    }
    public override float LookHor
    {
        get
        {
            float hor = Input.GetAxis("LookH");
            float perAdj = hor / (1 - AxisAdjust);
            if (hor != 0) { hor += perAdj * AxisAdjust; }
            return hor;
        }
    }
    public override float Jump
    {
        get
        {
            float jump = state.Triggers.Right;
            //print(jump + "  --  " + PlayerNumber);
            return jump;
        }
    }
    public override float Dash
    {
        get
        {
            float dash = state.Triggers.Left;
            return dash;
        }
    }
    public override bool Stall
    {
        get
        {
            bool stall = state.Buttons.LeftStick == ButtonState.Pressed;
            return stall;
        }
    }
}
