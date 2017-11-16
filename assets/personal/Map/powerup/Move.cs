using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Powerup {

    public Vector2 distance;
    protected override void power(GameObject p)
    {
        Teleport.t(p, distance);
    }
}
