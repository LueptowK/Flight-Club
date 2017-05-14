using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A waypoint to be used for path planning
/// </summary>
public class Waypoint : MonoBehaviour
{
    /// <summary>
    /// Other Waypoints you can get to from this one with a straight-line path
    /// </summary>
    [NonSerialized]
    public List<Waypoint> Neighbors = new List<Waypoint>();

    /// <summary>
    /// Used in path planning; next closest node to the start node
    /// </summary>
    private Waypoint predecessor;

    /// <summary>
    /// Cached list of all waypoints.
    /// </summary>
    static Waypoint[] AllWaypoints;

    /// <summary>
    /// Compute the Neighbors list
    /// </summary>
    internal void Start()
    {
        var position = transform.position;
        if (AllWaypoints == null)
        {
            AllWaypoints = FindObjectsOfType<Waypoint>();
        }

        foreach (var wp in AllWaypoints) 
            if (wp != this && !BehaviorTreeNode.WallBetween(position, wp.transform.position))
                Neighbors.Add(wp);
    }

    /// <summary>
    /// Visualize the waypoint graph
    /// </summary>
    internal void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, 0.5f);
        foreach (var wp in Neighbors)
            Gizmos.DrawLine(position, wp.transform.position);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(NearestWaypointTo(BehaviorTreeNode.Player.transform.position).transform.position, 1f);
    }

    /// <summary>
    /// Nearest waypoint to specified location that is reachable by a straight-line path.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Waypoint NearestWaypointTo(Vector2 position)
    {
        Waypoint nearest = null;
        var minDist = float.PositiveInfinity;
        for (int i = 0; i < AllWaypoints.Length; i++)
        {
            var wp = AllWaypoints[i];
            var p = wp.transform.position;
            var d = Vector2.Distance(position, p);
            if (d < minDist && !BehaviorTreeNode.WallBetween(p, position))
            {
                nearest = wp;
                minDist = d;
            }
        }
        return nearest;
    }

    /// <summary>
    /// Returns a series of waypoints to take to get to the specified position
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="end">Desired endpoint</param>
    /// <returns></returns>
    public static List<Waypoint> FindPath(Vector2 start, Vector2 end)
    {
        return FindPath(NearestWaypointTo(start), NearestWaypointTo(end));
    }

    /// <summary>
    /// Finds a sequence of waypoints between a specified pair of waypoints.
    /// IMPORTANT: this is a deliberately bad path planner; it's just BFS and doesn't
    /// pay attention to edge lengths.  Your job is to make it find the actually shortest path.
    /// </summary>
    /// <param name="start">Starting waypoint</param>
    /// <param name="end">Goal waypoint</param>
    /// <returns></returns>
    static List<Waypoint> FindPath(Waypoint start, Waypoint end)
    {
        
        
        // A* Search
        var pq = new PriorityQueue<List<Waypoint>>();
        List<Waypoint> starter = new List<Waypoint>();
        starter.Insert(0, start);
        pq.Enqueue(starter, Vector2.Distance(start.transform.position, end.transform.position));
        starter = Astar(pq, end);

        // Reconstruct the path
        starter.Reverse();
        return starter;
    }
    private static List<Waypoint> Astar(PriorityQueue<List<Waypoint>> pq, Waypoint end)
    {
        
        List<Waypoint> path = pq.Dequeue();
        Waypoint front = path[0];
        if (path.Count > 100)
        {
            throw new OverflowException("didnt prune correctly");
        }
        float currentDist = pathDist(path);
        PriorityQueue<Waypoint> neighbors= new PriorityQueue<Waypoint>();
        foreach (Waypoint w in front.Neighbors)
        {
            float g = currentDist + Vector2.Distance(front.transform.position, w.transform.position);
            float h = Vector2.Distance(w.transform.position, end.transform.position);
            List<Waypoint> newPath = new List<Waypoint>(path);
            newPath.Insert(0, w);
            if (h == 0)
            {
                return newPath;
            }
            pq.Enqueue(newPath, g + h);
        }
        
        return Astar(pq, end);

        
    }
    private static float pathDist(List<Waypoint> path)
    {
        float dist=0;
        for(int i=0; i < path.Count-2; i++)
        {
            dist += Vector2.Distance(path[i].transform.position, path[i + 1].transform.position);
        }
        return dist;
    }
}

class Pair<T>
{
    public T First { get; private set; }
    public T Second { get; private set; }

    public Pair(T first, T second)
    {
        First = first;
        Second = second;
    }

    public override int GetHashCode()
    {
        return First.GetHashCode() ^ Second.GetHashCode();
    }

    public override bool Equals(object other)
    {
        Pair<T> pair = other as Pair<T>;
        if (pair == null)
        {
            return false;
        }
        return (this.First.Equals(pair.First) && this.Second.Equals(pair.Second));
    }
}

class PairComparer<T> : IComparer<Pair<T>> where T : IComparable
{
    public int Compare(Pair<T> x, Pair<T> y)
    {
        if (x.First.CompareTo(y.First) < 0)
        {
            return -1;
        }
        else if (x.First.CompareTo(y.First) > 0)
        {
            return 1;
        }
        else {
            return x.Second.CompareTo(y.Second);
        }
    }
}
class PriorityQueue<T>
{
    SortedList<Pair<float>, T> _list;
    int count;

    public PriorityQueue()
    {
        _list = new SortedList<Pair<float>, T>(new PairComparer<float>());
    }

    public void Enqueue(T item, float priority)
    {
        _list.Add(new Pair<float>(priority, count), item);
        count++;
    }

    public T Dequeue()
    {
        T item = _list[_list.Keys[0]];
        _list.RemoveAt(0);
        return item;
    }
    public T Peek()
    {
        return _list[_list.Keys[0]];
    }
    public float PeekPrior()
    {
        return _list.Keys[0].First;
    }
}