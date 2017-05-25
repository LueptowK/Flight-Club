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
        if (other.gameObject.tag != null&&other.transform.parent==null)
        {
            //print("parented");
            Vector3 s= interchangeScale(other.transform, true);
            other.transform.parent = transform;
            other.transform.localScale = s;

        }

    }
    void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.tag != null && other.transform.parent == transform) 
        {
            //print("DE parented");
            Vector3 s = interchangeScale(other.transform, false);
            other.transform.parent = null;
            other.transform.localScale = s;
        }
    }
    Vector3 interchangeScale(Transform t, bool giving)
    {
        if(giving){
            return Vec3.Div(t.localScale, transform.localScale);
        }
        else
        {
            return Vec3.Mult(t.localScale, transform.localScale);
        }
        
    }

   
}
