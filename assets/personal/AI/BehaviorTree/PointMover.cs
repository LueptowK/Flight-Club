using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMover : BehaviorTreeNode {
    

    Vector3 selected;
    float range = 1f;
    

    public override void Activate(AIInput g)
    {
        selected = g.pickPoint().transform.position;
        //Debug.Log(arrivalTime+ "here");
    }

    public override bool Tick(AIInput g)
    {

        
            g.move(selected);
            if ((g.transform.position - selected).magnitude <= range)
            {
                return false;
            }
            return true;
       
            
            
       

        
    }
}
