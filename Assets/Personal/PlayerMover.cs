using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour {

    Rigidbody2D rb;
    ControlInterpret ci;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        ci = GetComponent<ControlInterpret>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 force = ci.move*15f;
        print(force);
        force.y = 0;
        rb.AddForce(force);

	}
}
