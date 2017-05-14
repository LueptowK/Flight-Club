using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : BehaviorTreeNode {

    // Use this for initialization
    public override bool Tick(AIInput g)
    {
        Vector2 dir = Player.transform.position - g.transform.position;
        dir.y = 0;
        if (dir.magnitude < 0.5f)
        {
            dir = Vector2.zero;
        }
        else
        {
            
            dir.Normalize();
        }
        
        g.setCtrl(new AIInput.aiMove(dir));
        return false;
    }
}
