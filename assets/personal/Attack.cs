using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public bool isBasic;
    public int windup, atkTime, endLag;
    public int attackReduction, landLag=6;
    public int[] autoCanFrames;
    public int autoCanLag;
    // Use this for initialization

    AttackManager mngr;
    List<GameObject> alreadyHit;
    int frameNum=0;
    List<GameObject> activeBoxes;
    List<List<GameObject>> frameByFrame;

    [HideInInspector]
    public int comboStrength = 0;

    public GameObject windUpBox; //only used in basic
    GameObject animBox;
    Transform target;
    AttackManager.AtkType type;
	void Start () {
        mngr= GetComponentInParent<AttackManager>();
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
    public void setType(AttackManager.AtkType t)
    {
        type = t;
    }
    public int atkFrames
    {
        get
        {
            return windup + atkTime + endLag;
        }
    }

	// Update is called once per frame
	public void NestedUpdate () {
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
                    animBox.SetActive(false);
                }
            }
            else
            {
                closeBoxes();
                int frame = frameNum - windup - 1;
                openBoxes(frame);
            }
        }
        else if (frameNum > windup + atkTime && frameNum <=atkFrames)
        {
            
            if (frameNum == windup + atkTime + 1)
            {
                if (isBasic)
                {
                    
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    animBox.SetActive(true);
                }
                else
                {
                    closeBoxes();
                }
            }

            if (isBasic)
            {
                animBox.transform.position = Vector3.Lerp(target.position, transform.position, (float)(frameNum - (windup + atkTime)) / (float)(endLag + 1));
                animBox.transform.localScale = Vector3.Lerp( target.localScale, new Vector3(0.2f, 0.2f, 1), (float)(frameNum - (windup + atkTime)) / (float)(endLag+1));
            }
            
        }
        else if (frameNum > atkFrames)
        {
            mngr.atkFinished();
            Destroy(gameObject);
        }
        else
        {
            if (isBasic)
            {
                animBox.transform.position = Vector3.Lerp(transform.position, target.position, (float)frameNum / (float)windup);
                animBox.transform.localScale = Vector3.Lerp(new Vector3(0.2f, 0.2f, 1), target.localScale, (float)frameNum / (float)windup);
                
            }
            else
            {
                if(frameNum == windup)
                {
                    transform.parent.GetComponent<AttackManager>().currentAttackHitStart();
                }
            }
        }
        
        frameNum++;
	}
    public void addHit(GameObject h)
    {
        
        alreadyHit.Add(h);
        mngr.updateLastAttack(type);
        alreadyHit[0].GetComponent<PlayerMover>().restoreTools();
        if (type != AttackManager.AtkType.SlashFinisher &&  !mngr.alreadyHitByType.Contains(h))
        {
            alreadyHit[0].GetComponent<ComboCounter>().incrementCombo(1);
        }
        else
        {
            GetComponentInParent<ComboCounter>().resetComboTime();
        }
        mngr.addHit(h);
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
    public int ending()
    {
        int guess = landLag;
        if (alreadyHit.Count > 1)
        {
            guess -= attackReduction;
        }
        if (Array.IndexOf(autoCanFrames, frameNum) != -1)
        {
            guess = autoCanLag;
        }
        int remain = atkFrames - frameNum;
        if (remain < guess)
        {
            guess = remain;
        }
        return guess;
    }
}
