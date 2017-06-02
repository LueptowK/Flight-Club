using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMover : Mover {
    public override void kill()
    {
        dying = true;
        ani.SetTrigger("Die");
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Destroy(barrel);
    }

    void destroy()
    {
        scaffs.end();
        Ender.finish();
        Destroy(gameObject);
    }
    public float speed;
    //public float maxSpeed;
    Interpreter ci;
    Rigidbody2D rb;
    Vector2 knockback;
    ActorSounds sounds;
    SpriteRenderer sprite;
    EnemyHealth hp;
    Animator ani;
    bool inHitstun;
    bool inHitlag;
    int hitstunCounter;
    int hitlagCounter;
    GameObject barrel;
    public GameObject BossShot;

    Scaffoldcont scaffs;
    StoryDestroy Ender;

    bool enraged;
    bool engaged;
    float length = 0.6f;
    int shootCD= 50;
    int currentCD = 0;

    int deathCount = 120;
    bool dying = false;
    // Use this for initialization




    void Start () {
        ci = GetComponent<Interpreter>();
        rb = GetComponent<Rigidbody2D>();
        sounds = GetComponent<ActorSounds>();
        sprite = GetComponent<SpriteRenderer>();
        hp = GetComponent<EnemyHealth>();
        ani = GetComponent<Animator>();
        scaffs = GetComponent<Scaffoldcont>();
        Ender = GetComponent<StoryDestroy>();
        barrel = transform.GetChild(0).gameObject;
        inHitstun = false;
        hitstunCounter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        sprite.color = Color.white;
        if (!dying)
        {
            if (inHitlag)
            {
                rb.velocity = Vector2.zero;
                hitlagCounter--;
                if (hitlagCounter == 0)
                {
                    inHitlag = false;
                    inHitstun = true;
                }
                if (hitlagCounter % 2 == 0)
                {
                    sprite.color = Color.black;
                }
            }
            else if (inHitstun)
            {
                rb.velocity = knockback;
                knockback = knockback * 0.85f;
                hitstunCounter--;
                if (hitstunCounter == 0)
                {
                    inHitstun = false;
                }
            }

            else
            {
                if (ci.move.magnitude > 0)
                {
                    rb.velocity = ci.move * speed;

                }

                /*
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity.Normalize();
                    rb.velocity *= maxSpeed;
                }
                */

                if (enraged)
                {
                    if (!engaged)
                    {
                        barrel.transform.localPosition += new Vector3(length / 300, 0);
                        if (barrel.transform.localPosition.x >= length)
                        {
                            engaged = true;
                        }
                    }
                    else if (currentCD == 0)
                    {
                        shoot();
                        currentCD = shootCD;
                    }
                    else
                    {
                        currentCD--;
                    }
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 187f / 120));
                }
                else
                {

                    if ((float)(hp.currentHealth) / hp.maxHealth < 0.75f)
                    {
                        enraged = true;
                    }
                }
                if (ci.Stall)
                {
                    rb.velocity *= 0.75f;
                }
            }
        }
        else
        {
            deathCount--;
            if (deathCount == 0)
            {
                destroy();
            }
        }
	}
    void shoot()
    {
        GameObject p = Instantiate(BossShot, transform.position + transform.right * 1.2f, transform.rotation);
        p.GetComponent<Projectile>().hitPlayers.Add(gameObject);
    }

    public void getHit(Vector2 kb, int hitLag, int hitStun, int damage)
    {
        knockback = kb*.8f;
        hitstunCounter = hitStun;
        hitlagCounter = hitLag;
        inHitlag = true;
        sounds.getHit(damage);
        hp.takeDamage(damage);
    }
}
