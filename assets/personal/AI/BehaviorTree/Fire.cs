using UnityEngine;

public class Fire : BehaviorTreeNode
{
    public override bool Tick(AIInput g)
    {
        g.move(Player.transform.position);
        g.shoot();
        return false;
    }
}
