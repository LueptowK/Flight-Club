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
    public GameObject SlashFinisher;
    private ControlInterpret ci;

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
        NeutralGround,
        SlashFinisher
    }
	// Use this for initialization
	void Start () {
        ci = GetComponent<ControlInterpret>();
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
            case AtkType.BackGround:
                currentAttack = Instantiate(BackGround, transform, false);
                break;
            case AtkType.ForwardGround:
                currentAttack = Instantiate(ForwardGround, transform, false);
                break;
            case AtkType.NeutralGround:
                currentAttack = Instantiate(NeutralGround, transform, false);
                break;
            case AtkType.SlashFinisher:
                currentAttack = Instantiate(SlashFinisher, transform, false);
                ComboCounter c = GetComponent<ComboCounter>();
                currentAttack.GetComponent<Attack>().comboStrength = c.currentCombo;
                c.reset();
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

    public void currentAttackHitStart()
    {
        if(currentAttack.tag == "FinisherSlash")
        {
            
            float angleDiff = Vector2.Angle(ci.move, new Vector2(1, 0));
            
            if (Vector3.Cross(new Vector3(ci.move.x, ci.move.y, 0), new Vector3(1, 0, 0)).z > 0)
            {
                angleDiff = -angleDiff;
            }

            currentAttack.transform.rotation = Quaternion.Euler(0f, 0f, angleDiff);
        }
    }
}
