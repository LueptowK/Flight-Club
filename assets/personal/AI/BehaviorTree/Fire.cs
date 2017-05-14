using UnityEngine;

public class Fire : BehaviorTreeNode
{
    public override bool Tick(AIInput g)
    {
        Vector3 adjust = new Vector3(0, 1, 0);
        Vector3 direction = BehaviorTreeNode.Player.position - g.transform.position - adjust;
        //tur.TurnToward(direction);
        //tur.Fire(direction);
        return false;
    }
}
