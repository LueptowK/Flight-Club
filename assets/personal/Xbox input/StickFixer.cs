using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickFixer : MonoBehaviour {

    private static float adjustAxis(float axis, float adjust)
    {
        float perAdj = axis / (1 - adjust);
        axis += perAdj * adjust;
        return axis;
    }
    public static Vector2 fixStick(Vector2 stick, float adjust)
    {
        if (stick.magnitude > 1)
        {
            if (stick.x == 1)
            {
                stick.x = Mathf.Sqrt(1 - Mathf.Pow(stick.y, 2));
            }
            else if (stick.y == 1)
            {
                stick.y = Mathf.Sqrt(1 - Mathf.Pow(stick.x, 2));
            }
        }
        stick.x = adjustAxis(stick.x, adjust);
        stick.y = adjustAxis(stick.y, adjust);
        if (stick.magnitude > 1)
        {
            stick.Normalize();
        }
        return stick;
    }
}
