using UnityEngine;
using System.Collections;

public class TurnTowards : BehaviorTreeNode
{

    // Use this for initialization
    public override bool Tick(AIInput g)
    {
        g.move(Player.transform.position);
        return false;
    }
}
