using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour {
    public int attemptedAttacks;
    public int successfulHits;
    public int damageDealt;
    public int damageTaken;
    public int finishers;
    public int successfulFinishers;
    public int finisherDamage;
    public int maxFinisher;
    public int shieldStolen;
    public int timesDashed;
    public int hitStalls;
    public int stalls;
    public int projectilesFired;
    public int[] timesSegmented;
    public int[] segmentsTaken;
    public bool dead;
	// Use this for initialization

    public void copyStats(StatTracker tracker)
    {
        attemptedAttacks = tracker.attemptedAttacks;
        successfulHits = tracker.successfulHits;
        damageDealt = tracker.damageDealt;
        damageTaken = tracker.damageTaken;
        finishers = tracker.finishers;
        successfulFinishers = tracker.successfulFinishers;
        finisherDamage = tracker.finisherDamage;
        maxFinisher = tracker.maxFinisher;
        shieldStolen = tracker.shieldStolen;
        timesDashed = tracker.timesDashed;
        hitStalls = tracker.hitStalls;
        stalls = tracker.stalls;
        projectilesFired = tracker.projectilesFired;
        timesSegmented = tracker.timesSegmented;
        segmentsTaken = tracker.segmentsTaken;
        dead = tracker.dead;

}

	void Start () {
        timesSegmented = new int[4];
        segmentsTaken = new int[4];
	}

    public void hitAttempt()
    {
        attemptedAttacks++;
    }

    public void die()
    {
        dead = true;
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
