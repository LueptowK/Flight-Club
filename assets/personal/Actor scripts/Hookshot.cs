using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookshot : MonoBehaviour {

    public GameObject hook;
    Hook h;
    void Awake()
    {
        h = hook.GetComponent<Hook>();
    }
	public Vector2 NestedUpdate () {
        return h.NestedUpdate();
	}
    public void setPlayer(PlayerMover p)
    {
        h.setPlayer(p);
    }
}
