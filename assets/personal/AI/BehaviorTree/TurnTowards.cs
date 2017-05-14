using UnityEngine;
using System.Collections;

public class TurnTowards : BehaviorTreeNode
{

    // Use this for initialization
    public override bool Tick(AIInput g)
    {
        //tur.TurnToward(BehaviorTreeNode.Player.transform.position-tur.transform.position);
        return false;
    }
}
