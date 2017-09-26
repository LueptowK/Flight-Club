using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ControlInterpret : Interpreter {

    private PlayerInput control;
    private int JumpDown;
    private int DashDown;
    private int AttackDown;
    private int SlashDown;
    private int DodgeDown;
    private int PauseDown;
    int ShootDown;

    private List<inputItem> inputHistory;
    List<StickQuadrant> Quads;
    public float AxisAdjust = 0.15f;

    public enum StickQuadrant
    {
        Neutral,
        Down,
        Up,
        Left, 
        Right
    }

    private struct inputItem
    {
        public Vector2 dir;
        public float time;
    }
    private int historyMax = 60;


	// Use this for initialization
	void Start () {
        control = GetComponent<PlayerInput>();
        inputHistory = new List<inputItem>(historyMax);
        inputItem i = new inputItem();
        i.time = Time.time;
        i.dir = new Vector2();
        for(int j=0; j<10;j++)
        {
            inputHistory.Insert(0, i);
        }

        Quads = new List<StickQuadrant>();
        Quads.Add(StickQuadrant.Neutral);
        Quads.Add(StickQuadrant.Neutral);
    }

    #region Joystick motions


    private bool outer = false;
    public bool tap()
    {
        float buffer = 0.02f;
        float threshholdHigh = 0.75f;
        //float thresholdLow = 0.55f;
        //float angleDev = 10f;

        Vector2 current = inputHistory[0].dir;
        //Debug.Log(current.magnitude);
        if (current.magnitude > 0.98f)
        {

            bool overcome = false;
            if (!outer)
            {
                
                for(int i =1; i <=timeIndex(buffer); i++)
                {
                    if (!overcome)
                    {
                        if (inputHistory[i].dir.magnitude < threshholdHigh)
                        {
                            //Debug.Log(i + " --- " + inputHistory[i].dir.magnitude);
                            overcome = true;
                        }
                    }
                    /*
                    else
                    {
                        
                        if (inputHistory[i].dir.magnitude < thresholdLow)
                        {
                            return false;
                        }
                        
                        else if (inputHistory[i].dir.magnitude > 0.98f)
                        {
                            //DOUBLE
                            //return true;
                        }
                        
                    }
                    */
                }
                outer = true;
                return overcome;

                //DOUBLE
                //return false;
            }




            return false;
        }
        else
        {
            outer = false;
            return false;
        }

        
    }
    #endregion

    
    // Update is called once per frame
    void FixedUpdate () {

        #region history
        inputItem i = new inputItem();
        i.dir = new Vector2(control.MoveHor, control.MoveVer);
        i.dir = StickFixer.fixStick(i.dir, AxisAdjust);
        i.time = Time.time;
        if (inputHistory.Count == historyMax)
        {

            inputHistory.RemoveAt(inputHistory.Count - 1);
        }
        //Debug.Log(i.dir);
        inputHistory.Insert(0, i);
        #endregion

        if (control.Jump > 0.55f || control.MoveVer > 0.55f)
        {
            JumpDown += 1;
        }
        else
        {
            JumpDown = 0;
        }
        //print(JumpDown);
        if (control.Dash > 0.55f)
        {
            DashDown += 1;
        }
        else
        {
            DashDown = 0;
        }
        if (control.Attack)
        {
            AttackDown += 1;
        }
        else
        {
            AttackDown = 0;
        }
        if (control.FinisherSlash)
        {
            SlashDown += 1;
        }
        else
        {
            SlashDown = 0;
        }
        if (control.Dodge)
        {
            DodgeDown += 1;
        }
        else
        {
            DodgeDown = 0;
        }
        
        if (control.Shoot)
        {
            ShootDown += 1;
        }
        else
        {
            ShootDown = 0;
        }
        Quads.RemoveAt(1);
        Quads.Insert(0, AQuad);

    }
    void Update()
    {
        if (control.Pause)
        {
            PauseDown += 1;
        }
        else
        {
            PauseDown = 0;
        }
    }

    public override Vector2 move{
        get
        {
            return inputHistory[0].dir;
        }
    }
    public StickQuadrant moveQuad
    {
        get
        {
            return getQuad(move);
        }
    }
    int idleFrames=4;
    public override bool idle
    {
        get
        {
            for(int i = 0; i < idleFrames; i++)
            {
                if (inputHistory[i].dir.x != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
    int fallFrames = 1;
    public override bool fall
    {
        get
        {
            for (int i = 0; i < fallFrames; i++)
            {
                Vector2 dir = inputHistory[i].dir;
                if (!(Math.Abs(dir.x)<0.1f&&dir.y<-0.9))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public override bool Jump {
        get
        {
            if (JumpDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public override bool Dash
    {
        get
        {
            if (DashDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public override bool Stall
    {
        get
        {
            return control.Stall;
        }
    }
    public override bool Slash
    {
        get
        {
            if (SlashDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public override bool Pause
    {
        get
        {

            if(PauseDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public override bool Dodge
    {
        get
        {
            if (DodgeDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public override bool Shoot
    {
        get
        {
            if (ShootDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public override bool TauntDown
    {
        get
        {
            return control.TauntD;
        }
    }
    public override bool PauseExit
    {
        get
        {
            return control.PauseExit;
        }
    }

    #region attack
    public Vector2 AttackStick
    {
        get
        {
            return StickFixer.fixStick(new Vector2(control.AtkHor, control.AtkVer), AxisAdjust);
        }
    }
    StickQuadrant AQuad
    {
        get
        {
            return getQuad(AttackStick);
        }
    }
    public override StickQuadrant AttackQuad
    {
        get
        {
            //print("get");
            return Quads[0];
            

        }
    }
    public override bool Attack{
        get
        {
            if (AttackDown == 1)
            {
                return true;
            }
            else if (Quads[0] != Quads[1]&& Quads[0]!= StickQuadrant.Neutral)
            {
                return true;
            }
            return false;
        }
    }
    #endregion

    StickQuadrant getQuad(Vector2 stick)
    {
        if (stick.magnitude <= 0.5f)
        {
            return StickQuadrant.Neutral;
        }
        else
        {

            float angleDiff = Vector2.Angle(stick, new Vector2(1, 1));


            if (Vector3.Cross(new Vector3(stick.x, stick.y, 0), new Vector3(1, 1, 0)).z < 0)
            {
                angleDiff = -angleDiff;
            }

            //print(angleDiff);

            if (angleDiff < -90)
            {
                return StickQuadrant.Left;
            }
            else if (angleDiff < 0)
            {
                return StickQuadrant.Up;
            }
            else if (angleDiff < 90)
            {
                return StickQuadrant.Right;
            }
            else
            {
                return StickQuadrant.Down;
            }
        }
    }
    // for lookup
    private int timeIndex(float time)
    {
        for(int i =0; i<inputHistory.Count; i++)
        {
            if (Time.time - inputHistory[i].time > time)
            {
                return i;
            }
        }
        return -1;
    }

}
