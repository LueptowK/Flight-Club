using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport {

	public static void t(GameObject j, Vector2 v)
    {
        Vector2 end = (Vector2)j.transform.position + v;
        Vector3 ex = j.GetComponent<Collider2D>().bounds.extents;
        Collider2D col =Physics2D.OverlapBox(end, ex, 0, 1 << 8 | 1 << 10);
        RaycastHit2D[] terrain;
        terrain = Physics2D.BoxCastAll(j.transform.position, ex, 0, v.normalized, v.magnitude, 1 << 8 | 1 << 10);
        
        foreach(RaycastHit2D r in terrain)
        {
            if (r.transform.CompareTag("Boundary"))
            {
                end = r.centroid;
                break;
            }
            if (col)
            {
                if (r.collider == col)
                {
                    end= r.centroid;
                    break;
                }
            }
        }
        
        j.transform.position = end;
    }
}
