using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {
    Rigidbody2D rb;
    public float speed;
    public GameObject chain;
    Vector2 origin;
    SpriteRenderer ren;
    bool extending;
    PlayerMover pm;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        origin = transform.parent.position;
        ren = chain.GetComponent<SpriteRenderer>();
        extending = true;
        
    }
    public void setPlayer(PlayerMover p)
    {
        pm = p;
    }
	
	// Update is called once per frame
	public Vector2 NestedUpdate () {
        origin = pm.transform.position;
        //if (extending)
        //{

        //}
        Vector2 dif = (Vector2)transform.position - origin;
        ren.size = new Vector2(dif.magnitude * 0.20f, 0.08f);
        chain.transform.position = origin + dif / 2f - 0.1f*(Vector2)transform.right;
        chain.transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg);
        return dif;
	}
    void OnCollisionEnter2D(Collision2D col)
    {
        if (extending)
        {
            extending = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = false;
            pm.hookshotHit();
        }
    }

}
