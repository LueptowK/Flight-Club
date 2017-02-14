using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public bool isBasic;
    public int windup, atkTime, endLag;
    // Use this for initialization

    List<GameObject> alreadyHit;
    int frameNum=0;
    List<GameObject> activeBoxes;
    List<List<GameObject>> frameByFrame;
	void Start () {
        alreadyHit = new List<GameObject>();
        alreadyHit.Add(transform.parent.gameObject);
        if (!isBasic) {
            activeBoxes = new List<GameObject>();
            frameByFrame = new List<List<GameObject>>();
            for(int i =0; i<= atkTime; i++)
            {
                frameByFrame.Add(new List<GameObject>());
            }
            foreach (Transform child in transform)
            {
                string name = child.name;
                int index = Int32.Parse(name.Remove(name.Length - 1)) - 1;
                //print(index);
                frameByFrame[index].Add(child.gameObject);

            }
        }
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
            else
            {
                closeBoxes();
                int frame = frameNum - windup - 1;
                openBoxes(frame);
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
                else
                {
                    closeBoxes();
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
        alreadyHit[0].GetComponent<PlayerMover>().restoreTools();
    }
    public List<GameObject> hit
    {
        get
        {
            return alreadyHit;
        }
    }
    void closeBoxes()
    {
        foreach(GameObject hitbox in activeBoxes)
        {
            hitbox.SetActive(false);

        }
        activeBoxes = new List<GameObject>();
    }
    void openBoxes(int frame)
    {
        foreach(GameObject box in frameByFrame[frame])
        {
            box.SetActive(true);
            activeBoxes.Add(box);
        }
    }
}
