using System;
using UnityEngine;

public enum DeciderType
{
    /// <summary>
    /// Always willing to run
    /// </summary>
    Always,

    LineOfSight,

    FacingTarget,
    Half,

    Threat0

}
public static class DeciderImplementation
{


    /// <summary>
    /// Run the specified decider and returns its value
    /// </summary>
    /// <param name="d">Decider to run</param>
    /// <param name="tank">Tank being controlled</param>
    /// <returns>True if decider wants to run</returns>
    public static bool Decide(this DeciderType d, AIInput g)
    {
        Vector3 direction = BehaviorTreeNode.Player.position - g.transform.position;
        switch (d)
        {
            case DeciderType.Always:
                return true;

            case DeciderType.LineOfSight:
                //Debug.DrawRay(tur.transform.position + new Vector3(0, 1, 0), BehaviorTreeNode.Player.position - tur.transform.position);
                Vector3 adjust = new Vector3(0, 0.2f, 0);
                Vector3 dif = direction - adjust;
                
                return !Physics2D.Raycast(g.transform.position+ adjust, direction, direction.magnitude,  (1<<8) | (1 << 10));                //Debug.Log(a);
                

            case DeciderType.FacingTarget:
                
                
                return Vector3.Angle(g.transform.right, direction) < 10;

            case DeciderType.Threat0:
                Vector3 vec= g.threat[0];
                bool r = Vec3.lessThan(Vec3.Abs(direction), vec);
                return r;
            case DeciderType.Half:
                Health h = g.GetComponent<Health>();
                return (float)(h.currentHealth) / h.maxHealth > 0.5f;

            default:
                throw new ArgumentException("Unknown Decider: " + d);
        }
    }

}