using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {
    public GameObject DownAir;
    public GameObject UpAir;
    public GameObject BackAir;
    public GameObject ForwardAir;
    public GameObject NeutralAir;
    public GameObject DownGround;
    public GameObject UpGround;
    public GameObject BackGround;
    public GameObject ForwardGround;
    public GameObject NeutralGround;

    GameObject currentAttack;
    public enum AtkType
    {
        DownAir,
        UpAir,
        ForwardAir,
        BackAir,
        NeutralAir,
        DownGround,
        UpGround,
        ForwardGround,
        BackGround,
        NeutralGround
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
            case AtkType.DownGround:
                currentAttack = Instantiate(DownGround, transform, false);
                break;
            case AtkType.UpGround:
                currentAttack = Instantiate(UpGround, transform, false);
                break;
            default:
                print("Not implemented");
                currentAttack = Instantiate(NeutralAir, transform, false);
                break;


        }
        return currentAttack.GetComponent<Attack>().atkFrames;
    }
    public void stopAttack()
    {
        if (currentAttack) { Destroy(currentAttack); }
        
    }
}
