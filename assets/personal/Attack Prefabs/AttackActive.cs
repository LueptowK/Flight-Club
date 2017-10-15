using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackActive : Attack {
    public bool isBasic;
    public int windup, atkTime, endLag;
    public int attackReduction, landLag=6;
    public int[] autoCanFrames;
    public int autoCanLag;

    public bool isGrab;
    // Use this for initialization

    
    
    int frameNum=1;
    List<GameObject> activeBoxes;
    List<List<GameObject>> frameByFrame;

    int currentHitlag=0;
    int grabHitlagPending = 0;
    [HideInInspector]
    public int comboStrength = 0;

    public GameObject windUpBox; //only used in basic
    GameObject animBox;
    Transform target;
    AttackManager.AtkType type;
	new void Start () {
        base.Start();
        isActive = true;
        if (!isBasic) {
            activeBoxes = new List<GameObject>();
            frameByFrame = new List<List<GameObject>>();
            for(int i =0; i<= atkTime; i++)
            {
                frameByFrame.Add(new List<GameObject>());
            }
            foreach (Transform child in transform)
            {
                child.GetComponent<HitboxProperties>().setAtk();
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
                child.GetComponent<HitboxProperties>().setAtk();
                target = child.transform;
            }
            animBox = Instantiate(windUpBox, transform);
            
        }
    }
    public bool released
    {
        get{
            return frameNum > windup + atkTime;
        }
    }
    public bool attacking
    {
        get
        {
            return frameNum > windup && frameNum <= windup + atkTime;
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
        if (currentHitlag > 0)
        {
            
            currentHitlag--;
            if (currentHitlag == 0)
            {
                mngr.lag(false);
            }
        }
        else {
            if (frameNum > windup && frameNum <= windup + atkTime)
            {

                if (isBasic)
                {
                    if (frameNum == windup + 1)
                    {

                        foreach (Transform child in transform)
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
            else if (frameNum > windup + atkTime && frameNum <= atkFrames)
            {

                if (frameNum == windup + atkTime + 1)
                {
                    if (isGrab)
                    {
                        
                        grabDamage();
                    }
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
                    if (alreadyHit.Count > 1)
                    {
                        endLag -= attackReduction;
                    }
                }

                if (isBasic)
                {
                    animBox.transform.position = Vector3.Lerp(target.position, transform.position, (float)(frameNum - (windup + atkTime)) / (float)(endLag + 1));
                    animBox.transform.localScale = Vector3.Lerp(target.localScale, new Vector3(0.2f, 0.2f, 1), (float)(frameNum - (windup + atkTime)) / (float)(endLag + 1));
                }

            }
            else if (frameNum > atkFrames)
            {
                mngr.atkFinished(type);
           
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
                    
                }
            }

            frameNum++;
        }
	}
    public void grabDamage()
    {
        for (int i = 0; i < alreadyHit.Count; i++)
        {
            if (i > 0)
            {
                GameObject g = alreadyHit[i];
                if (g)
                {
                    g.GetComponent<PlayerMover>().grabFin();
                }
                
            }
        }
        if (alreadyHit.Count > 1&& grabHitlagPending!=0)
        {
            currentHitlag = grabHitlagPending;
            mngr.lag(true);
        }
        
            
        
    }
    public void windDown()
    {
        frameNum = windup + atkTime;
    }
    public override void addHit(GameObject h, int hitLag)
    {

        alreadyHit.Add(h);
        mngr.addHit(h);
        mngr.updateLastAttack(type);
        alreadyHit[0].GetComponent<PlayerMover>().restoreTools();
        ComboCounter c = GetComponentInParent<ComboCounter>();
        if (c)
        {
            if (type != AttackManager.AtkType.Finisher && !mngr.alreadyHitByType.Contains(h) && !GetComponentInParent<PlayerMover>().isPhase2())
            {
                c.incrementCombo(1);
            }
            else
            {
                c.resetComboTime();
            }
        }
        

        if(type!= AttackManager.AtkType.Finisher && hitLag>0 /*<- safety*/)
        {
            if (isGrab)
            {

                grabHitlagPending = hitLag;
            }
            else {
                currentHitlag = hitLag;
                mngr.lag(true);
            }
            
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
    public bool inHitlag
    {
        get
        {
            return currentHitlag > 0;
        }
    }
}
