using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vec3
{
    public static bool lessThan(Vector3 v1, Vector3 v2)
    {
        return v1.x <= v2.x && v1.y <= v2.y && v1.z <= v2.z;
    }

    public static Vector3 Abs(Vector3 v1)
    {
        return new Vector3(Mathf.Abs(v1.x), Mathf.Abs(v1.y), Mathf.Abs(v1.z));
    }
    public static Vector3 Mult(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x*v2.x, v1.y*v2.y, v1.z*v2.z);
    }
    public static Vector3 Div(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }
}

