using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour {
    private PlayerMover mover;
    private int attemptedAttacks;
    private int successfulHits;
    private int damageDealt;
    private int damageTaken;
    private int finishers;
    private int successfulFinishers;
    private int finisherDamage;
    private int maxFinisher;
    private int shieldStolen;
    private int timesDashed;
    private int hitStalls;
    private int stalls;
    private int projectilesFired;
    private int[] timesSegmented;
    private int[] segmentsTaken;
	// Use this for initialization
	void Start () {
        mover = GetComponentInParent<PlayerMover>();
        timesSegmented = new int[4];
        segmentsTaken = new int[4];
	}

    public void hitAttempt()
    {
        attemptedAttacks++;
    }

    public void hitStall()
    {
        hitStalls++;
    }

    public void Stall()
    {
        stalls++;
    }

    public void projectile()
    {
        projectilesFired++;
    }

    public void dash()
    {
        timesDashed++;
    }

    public void successfulHit()
    {
        successfulHits++;
    }

    public void successfulFinisher()
    {
        successfulFinishers++;
    }

    public void dealDamage(int damage)
    {
        damageDealt += damage;
    }

    public void takeDamage(int damage)
    {
        damageTaken += damage;
    }

    public void finisherAttempt()
    {
        finishers++;
    }

    public void dealFinisher(int damage)
    {
        finisherDamage += damage;
        damageDealt += damage;
        if (damage > maxFinisher)
        {
            maxFinisher = damage;
        }
    }

    public void stealShield(int damage)
    {
        shieldStolen += damage;
    }
}
