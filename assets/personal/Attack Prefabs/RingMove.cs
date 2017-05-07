using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMove : MonoBehaviour {

    public int totalLifetime = 30;

    int lifetime;
    int hold = 5;
    int end = 10;
	// Use this for initialization
	void Start () {
        transform.localPosition = new Vector3(0, -0.6f);
        lifetime = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (lifetime > totalLifetime)
        {
            transform.localPosition += new Vector3(0, 0.1f);
            transform.localScale *= 0.75f;
        }
        else if (lifetime > hold)
        {
            transform.localPosition = Vector3.Lerp(new Vector3(0, -0.6f), new Vector3(0, 0.6f),((float)(lifetime-hold))/(totalLifetime-hold));
        }



        lifetime++;
        if (lifetime>totalLifetime+end)
        {
            Destroy(gameObject);
        }
	}
}
