﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {
    public GameObject DownAir;
    public GameObject UpAir;
    public GameObject BackAir;
    public GameObject ForwardAir;
    public GameObject NeutralAir;

    GameObject currentAttack;
    public enum AtkType
    {
        DownAir,
        UpAir,
        ForwardAir,
        BackAir,
        NeutralAir
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int makeAttack(AtkType a)
    {
        
        switch (a)
        {
            case AtkType.DownAir:
                currentAttack = Instantiate(DownAir, transform,false);
                break;
            case AtkType.NeutralAir:
                currentAttack = Instantiate(NeutralAir, transform, false);
                break;
            case AtkType.UpAir:
                currentAttack = Instantiate(UpAir, transform, false);
                break;
            case AtkType.ForwardAir:
                currentAttack = Instantiate(ForwardAir, transform, false);
                break;
            case AtkType.BackAir:
                currentAttack = Instantiate(BackAir, transform, false);
                break;


        }
        return currentAttack.GetComponent<Attack>().atkFrames;
    }
    public void stopAttack()
    {
        if (currentAttack) { Destroy(currentAttack); }
        
    }
}