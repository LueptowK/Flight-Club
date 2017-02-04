using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ControlInterpret : MonoBehaviour {

    private PlayerInput control;
    private int JumpDown;
    private int DashDown;

    private List<inputItem> inputHistory;

    public float AxisAdjust = 0.15f;


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

    private float adjustAxis(float axis)
    {
        float perAdj = axis / (1 - AxisAdjust);
        axis += perAdj * AxisAdjust;
        return axis;
    }
    private Vector2 fixStick(Vector2 stick)
    {
        if (stick.magnitude > 1)
        {
            if (stick.x == 1)
            {
                stick.x = Mathf.Sqrt(1 - Mathf.Pow(stick.y, 2));
            }
            else if (stick.y == 1)
            {
                stick.y = Mathf.Sqrt(1 - Mathf.Pow(stick.x, 2));
            }
        }
        stick.x = adjustAxis(stick.x);
        stick.y = adjustAxis(stick.y);
        if (stick.magnitude > 1)
        {
            stick.Normalize();
        }
        return stick;
    }
    // Update is called once per frame
    void FixedUpdate () {

        #region history
        inputItem i = new inputItem();
        i.dir = new Vector2(control.MoveHor, control.MoveVer);
        i.dir = fixStick(i.dir);
        i.time = Time.time;
        if (inputHistory.Count == historyMax)
        {

            inputHistory.RemoveAt(inputHistory.Count - 1);
        }
        //Debug.Log(i.dir);
        inputHistory.Insert(0, i);
        #endregion

        if (control.Jump > 0.55f)
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
    }

    public Vector2 move{
        get
        {
            return inputHistory[0].dir;
        }
       }
    int idleFrames=4;
    public bool idle
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
    public bool Jump {
        get
        {
            if (JumpDown == 1)
            {
                return true;
            }
            return false;
        }
    }
    public bool Dash
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
    public bool Stall
    {
        get
        {
            return control.Stall;
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
