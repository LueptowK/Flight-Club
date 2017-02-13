using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public bool isBasic;
    public int windup, atkTime, endLag;
    // Use this for initialization

    List<GameObject> alreadyHit;
    int frameNum=0;
	void Start () {
        alreadyHit = new List<GameObject>();
        alreadyHit.Add(transform.parent.gameObject);

    }
    public int atkFrames
    {
        get
        {
            return windup + atkTime + endLag;
        }
    }

	// Update is called once per frame
	void Update () {
        if (frameNum > windup && frameNum <= windup + atkTime)
        {
            if (isBasic)
            {
                if (frameNum == windup + 1)
                {
                    foreach(Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
        else if (frameNum > windup + atkTime)
        {
            
            if (frameNum == windup + atkTime + 1)
            {
                if (isBasic)
                {
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            
        }
        if (frameNum > atkFrames)
        {
            Destroy(gameObject);
        }
        
        frameNum++;
	}
    public void addHit(GameObject h)
    {
        alreadyHit.Add(h);
    }
    public List<GameObject> hit
    {
        get
        {
            return alreadyHit;
        }
    }
}
