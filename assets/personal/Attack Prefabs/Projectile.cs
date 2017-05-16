using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Vector2 hitboxVector;
    public int hitstun;
    public int hitlag;
    public int damage;
    public float velocity;
    public int maxLifetime=0;
    Rigidbody2D rb;
    [HideInInspector]
    public List<GameObject> hitPlayers;

    AttackManager atk;

    int lifetime = 0;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = (Vector2)(transform.right * velocity);

    }
	public void setMngr(AttackManager a)
    {
        atk = a;
    }
	// Update is called once per frame
	void FixedUpdate () {
        lifetime++;
        if(maxLifetime!=0&& lifetime > maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D playerCol)
    {
        //print("col");
        if (playerCol.tag == "Player"||playerCol.tag == "Enemy")
        {
            if (!hitPlayers.Contains(playerCol.gameObject))
            {
                if (!(playerCol.GetComponent<IFrames>().invincible()))
                {
                    Vector2 knockback;
                    
                    knockback = new Vector2(hitboxVector.x * transform.right.x + hitboxVector.y * transform.up.x, hitboxVector.x * transform.right.y + hitboxVector.y * transform.up.y);


                    playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage );
                    if (atk)
                    {
                        atk.updateLastAttack(AttackManager.AtkType.None);
                    }
                    //print("destroyed");
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if(playerCol.tag == "Target")
            {
                Destroy(playerCol.gameObject);
            }
            if (lifetime != 0) {  Destroy(gameObject); }
            
        }
    }
}
