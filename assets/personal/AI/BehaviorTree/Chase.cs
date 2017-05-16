using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : BehaviorTreeNode {

    public float jumpRange = 0;
    // Use this for initialization
    public override bool Tick(AIInput g)
    {
        g.moveHor(Player.transform.position);
        if (jumpRange != 0)
        {
            Vector3 dif = Player.transform.position - g.transform.position;
            if (Mathf.Abs(dif.x) < jumpRange&& dif.y>1)
            {
                g.jump();
            }
        }
          
        return false;
    }
}
