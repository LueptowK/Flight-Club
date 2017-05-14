/// <summary>
/// Behavior that does nothing.
/// </summary>
using UnityEngine;
using System.Collections;/// 

public class Idle : BehaviorTreeNode
{
    public override bool Tick (AIInput g) {

        g.setCtrl(new AIInput.aiMove(Vector2.zero));
        return false;
    }
}
