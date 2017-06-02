using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour {
    public int lockFrames;
    Rigidbody2D rb;
    Vector3 start;
    int locking = -1;
	// Use this for initialization
	void Start () {
        GameObject child;
        child = transform.GetChild(0).gameObject;
        start = child.transform.position;
        rb = child.GetComponent<Rigidbody2D>();
       
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (locking>=0&& locking <= lockFrames)
        {
            rb.velocity = (transform.position-start)*60f/lockFrames;
            locking++;
            
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        
        
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (locking == -1) {
            locking = 1;
           
        }
    }
}
