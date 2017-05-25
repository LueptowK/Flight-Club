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
            if(other.transform.parent == null)
            {
                parentObj(other.gameObject, true);
            }
            else if (other.transform.parent.position.y>transform.position.y)
            {
                other.transform.parent.GetComponent<PlatformMov>().parentObj(other.gameObject, false);
                parentObj(other.gameObject, true);
            }
            

        }

    }
    void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.tag != null && other.transform.parent == transform) 
        {
            //print("DE parented");
            parentObj(other.gameObject, false);
        }
    }
    public void parentObj(GameObject g, bool p)
    {
        if (p)
        {
            Vector3 s = interchangeScale(g.transform, true);
            g.transform.parent = transform;
            g.transform.localScale = s;
        }
        else
        {
            Vector3 s = interchangeScale(g.transform, false);
            g.transform.parent = null;
            g.transform.localScale = s;
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
