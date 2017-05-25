using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour {
    public GameObject partPre;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player")
        {
            power(other.gameObject);
            if (partPre)
            {
                Instantiate(partPre, other.transform);
            }
            Destroy(gameObject);
        }
    }
    protected abstract void power(GameObject Player);
}
