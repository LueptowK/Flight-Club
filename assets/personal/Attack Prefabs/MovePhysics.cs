using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovePhysics : MonoBehaviour {

    public struct AtkMotion
    {
        public bool use;
        public Vector2 motion;
    }
    // Use this for initialization
    void Start () {
		
	}
    public abstract AtkMotion motion(Vector2 input);
	// Update is called once per frame
	void Update () {
		
	}
}
