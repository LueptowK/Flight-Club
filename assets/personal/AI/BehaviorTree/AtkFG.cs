using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkFG : BehaviorTreeNode
{

    public override bool Tick(AIInput g)
    {
        g.moveHor(Player.transform.position);
        g.FG(Player.transform.position.x - g.transform.position.x < 0);



        return false;
    }

        // Update is called once per frame
    void Update () {
		
	}
}
