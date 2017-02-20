using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFrames : MonoBehaviour {
    int iFrames;
	// Use this for initialization
	void Start () {
        iFrames = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (iFrames > 0)
        {
            iFrames--;
        }
	}

    public void SetFrames(int i)
    {
        iFrames = i;
    }
    public bool invincible()
    {
        return iFrames > 0;
    }
}
