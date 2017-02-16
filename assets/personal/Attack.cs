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

    public GameObject windUpBox; //only used in basic
    GameObject animBox;
    Transform target;
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
        else
        {
            foreach (Transform child in transform)
            {
                target = child.transform;
            }
            animBox = Instantiate(windUpBox, transform);
            
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
                    animBox.SetActive(false);
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
                    // THIS FUCKER
                    //animBox.SetActive(true);
                }
                else
                {
                    closeBoxes();
                }
            }

            if (isBasic)
            {
                //print((float)(frameNum - (windup + atkTime)) / (float)(endLag + 1));
                //print(target.position + "  - --- " + transform.position);
                // THIS SECTION DOESNT WORK - - TURN THAT ^^^^^^ BACK ON
                animBox.transform.position = Vector3.Lerp( target.position, transform.position, (float)(frameNum-(windup+atkTime))/ (float)(endLag+1));
                animBox.transform.localScale = Vector3.Lerp( target.localScale, new Vector3(0.2f, 0.2f, 1), (float)(frameNum - (windup + atkTime)) / (float)(endLag+1));
            }
            
        }
        if (frameNum > atkFrames)
        {
            Destroy(gameObject);
        }
        else
        {
            if (isBasic)
            {
                animBox.transform.position = Vector3.Lerp(transform.position, target.position, (float)frameNum / (float)windup);
                animBox.transform.localScale = Vector3.Lerp(new Vector3(0.2f, 0.2f, 1), target.localScale, (float)frameNum / (float)windup);
                
            }
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
