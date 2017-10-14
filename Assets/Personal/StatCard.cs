using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat Card")]
public class StatCard : ScriptableObject {
    public float scale;
    public int character;
    public float maxDI = 18 ;
    public float hitstunFriction =0.98f;
    public int maxDashes =1 ;
    public float moveSpeed =13;
    public float airSpeed =0.8f;
    public float maxAirSpeed =8 ;
    public float dashMagnitude=20;
    public float gravity = 2;
    public float jumpVel = 10;
    public float wallJumpXVel=15;
    public float wallJumpYVel=8;
    public float dashEndMomentum=0.65f;
    public int dashTime=10;
    public int stallTime =10;
    public float maxFallSpeed =22;
    public float friction=7;
    public int jumpSquatFrames=4;
    public int stallCooldown=40;
    public int shootCooldown=30;
    public int shotCost;
}
