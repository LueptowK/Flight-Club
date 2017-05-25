using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMov : MonoBehaviour {

    public Vector2 speed;
	// Use this for initialization
	void Start () {
        //GetComponent<Rigidbody2D>().velocity = speed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        transform.position += (Vector3)speed * Time.fixedDeltaTime;
	}
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != null)
        {
            //print("parented");
            other.transform.parent = transform;
            //other.transform.localScale = interchangeScale(other.transform);
        }

    }
    void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.tag != null)
        {
            //print("DE parented");
            other.transform.parent = null;
        }
    }


   
}
