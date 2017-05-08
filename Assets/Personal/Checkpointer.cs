using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpointer : MonoBehaviour {

    public int checkNum;

	void OnTriggerEnter2D(Collider2D col)
    {
        GameObject.Find("Main Camera").GetComponent<Manager>().checkpoint(checkNum);
    }
}
