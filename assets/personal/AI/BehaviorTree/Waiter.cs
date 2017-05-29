using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : BehaviorTreeNode
{
    public float waitTime;
    public override void Activate(AIInput g)
    {
        g.wait(waitTime);
        //Debug.Log(arrivalTime+ "here");
    }
    public override bool Tick(AIInput g)
    {

        g.setCtrl(new AIInput.aiMove(Vector2.zero));
        g.stall();

        if (g.waiting)
        {
            return true;
        }
        return false;
    }
}
