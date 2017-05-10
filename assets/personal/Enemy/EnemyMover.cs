using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour {
    Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.velocity += new Vector2(0, 1f * -9.8f * Time.fixedDeltaTime);
	}
    public void getHit(Vector2 knockback, int hitLag, int hitStun, int damage)
    {
        getHit(knockback, hitLag, hitStun, damage, null);

    }
    public void getHit(Vector2 knockback, int hitLag, int hitStun, int damage, Attack a)
    {
        rb.velocity = knockback;
    }
}
