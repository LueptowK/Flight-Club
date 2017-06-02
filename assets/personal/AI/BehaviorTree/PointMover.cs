using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMover : BehaviorTreeNode {
    

    
    float range = 1f;
    

    public override void Activate(AIInput g)
    {
        g.pickPoint();
        //Debug.Log(arrivalTime+ "here");
    }

    public override bool Tick(AIInput g)
    {
        Vector3 selected;
        if (!g.point)
        {
            return false;
        }
        
        selected = g.point.transform.position;
        //Debug.Log(selected);
        g.move(selected);
            if ((g.transform.position - selected).magnitude <= range)
            {
                return false;
            }
            return true;
       
            
            
       

        
    }
}
