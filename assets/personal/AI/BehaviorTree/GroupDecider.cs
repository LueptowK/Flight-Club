using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Behavior tree node that choses between child nodes
/// and recursive runs the selected child.
/// </summary>
[Serializable]
public class GroupDecider : BehaviorTreeNode
{
    /// <summary>
    /// Children of this node.  When we run, we recursively run one of them.
    /// </summary>
    public List<BehaviorTreeNode> Children = new List<BehaviorTreeNode>();
    
    /// <summary>
    /// Child currently selected to run.  We continue to run it until its Tick() method returns false.
    /// </summary>
    private BehaviorTreeNode selected;

    /// <summary>
    /// Policy to use in selecting child to run
    /// </summary>
    public SelectionPolicy Policy = SelectionPolicy.Prioritized;

    public enum SelectionPolicy {
        /// <summary>
        /// Take the first child whose Decide() method returns true.
        /// </summary>
        Prioritized,
        /// <summary>
        /// Randomly choose a child whose Decide method returns true.
        /// </summary>
        Random,
        /// <summary>
        /// Run children in order
        /// </summary>
        Sequential,
        /// <summary>
        /// Run children in order, looping forever.
        /// </summary>
        Loop
    }
    int sequentialChild = 0;
    /// <summary>
    /// We're not running anymore; recursively deactivate our selected child.
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    public override void Deactivate(AIInput g)
    {
        if (selected)
        {
            selected.Deactivate(g);
            selected = null;
        }
    }

#if DEBUG
    public override void Activate(AIInput g)
    {
        // Check to make sure the subset property is satisfied
        if (!Children.Any(c => c.Decide(g)))
            Debug.Log(name + " activated without runnable child");
    }
#endif

    /// <summary>
    /// Run our selected child.  If no child is selected, select one.  If can't select one, return false.
    /// </summary>
    /// <param name="tank">Tank being controlled.</param>
    /// <returns>Whether we want to continue running.</returns>
    public override bool Tick (AIInput g)
    {
        BehaviorTreeNode next = SelectChild(g);

        //Debug.Log(next);
        if (next == null)
        {
            return false;
        }
        
        if(selected != next)
        {
            Deactivate(g);
            next.Activate(g);
            selected = next;
        }
        bool response= next.Tick(g);
        if (response==false)
        {
            Deactivate(g);
            sequentialChild++;
            sequentialChild %= Children.Count;
        }
        return response;
    }

    /// <summary>
    /// Select a child to run based on the policy.
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    /// <returns>Child to run, or null if no runnable children.</returns>
    private BehaviorTreeNode SelectChild(AIInput g)
    {
        switch (Policy)
        {
            case SelectionPolicy.Prioritized:
                
                for (int i = 0; i < Children.Count; i++)
                {
                    BehaviorTreeNode child = Children[i];
                    //Debug.Log(child +" - "+i);
                    if (selected == child)
                    {
                        return child;
                    }
                    if (child.Decide(g))
                    {
                        return child;
                    }
                }
                //Debug.Log("none");
                return null;
            case SelectionPolicy.Sequential:

                for (int i = 0; i < Children.Count; i++)
                {
                    int j = (i + sequentialChild) % Children.Count;
                    BehaviorTreeNode child = Children[j];
                    //Debug.Log(child +" - "+i);
                    if (selected == child)
                    {
                        return child;
                    }
                    if (child.Decide(g))
                    {
                        sequentialChild = j;
                        return child;
                    }
                }
                //Debug.Log("none");
                return null;
            default:
                throw new NotImplementedException("Unimplemented policy: " + Policy);
        }
    }

    #region Debugging support
    public override void GetCurrentPath(StringBuilder b)
    {
        base.GetCurrentPath(b);
        if (selected != null)
        {
            b.Append('/');
            selected.GetCurrentPath(b);
        }
    }

 

    #endregion
}

