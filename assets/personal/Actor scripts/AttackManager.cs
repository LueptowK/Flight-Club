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
    public GameObject Finisher;
    public GameObject RangedAttack;
    public GameObject TouchAttack;
    private ControlInterpret ci;
    PlayerMover pm;
    ComboCounter combo;

    GameObject currentTouchAttack;
    GameObject currentAttack;
    [HideInInspector]
    public AtkType lastAttack;
    [HideInInspector]
    public List<GameObject> alreadyHitByType;
    public enum AtkType
    {
        None,
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
        Finisher
    }
	// Use this for initialization
	void Start () {
        ci = GetComponent<ControlInterpret>();
        combo = GetComponent<ComboCounter>();
        pm = GetComponent<PlayerMover>();
        lastAttack = AtkType.None;
        if (TouchAttack)
        {
            currentTouchAttack = Instantiate(TouchAttack, transform);
        }
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!currentTouchAttack&& TouchAttack)
        {
            if (pm.actionable)
            {
                currentTouchAttack=Instantiate(TouchAttack, transform);
            }
        }
	}
    public void NestedUpdate()
    {
        if (currentAttack)
        {
            currentAttack.GetComponent<AttackActive>().NestedUpdate();
        }
    }
    public void shoot(bool backwards)
    {
        Quaternion rot;
        float a = 0;
        if (pm.FacingLeft)
        {
            a += 180;
        }
        if (backwards)
        {
            a += 180;
        }
        rot = Quaternion.Euler(0, 0, a);
        GameObject m = Instantiate(RangedAttack, GetComponent<PlayerAnimator>().ShootPos(backwards), rot);
        Projectile p = m.GetComponent<Projectile>();
        p.hitPlayers.Add(gameObject);
        p.setMngr(this);
        if (combo)
        {
            combo.incrementCombo(-1);
            combo.resetComboTime();
        }
        
    }
    public void updateLastAttack(AtkType t)
    {
        if(t!= lastAttack)
        {
            lastAttack = t;
            alreadyHitByType = new List<GameObject>();
        }
    }
    public void addHit(GameObject player)
    {
        alreadyHitByType.Add(player);
    }
    public void lag(bool hitLagState)
    {
        pm.hitting(hitLagState);
    }
    public void resetHitList()
    {
        alreadyHitByType = new List<GameObject>();
        lastAttack = AtkType.None;
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
            case AtkType.Finisher:
                currentAttack = Instantiate(Finisher, transform, false);
                ComboCounter c = GetComponent<ComboCounter>();
                currentAttack.GetComponent<AttackActive>().comboStrength = c.currentCombo;
                c.reset();
                break;
            default:
                print("Not implemented");
                currentAttack = Instantiate(NeutralAir, transform, false);
                break;


        }
        currentAttack.GetComponent<AttackActive>().setType(a);
        return currentAttack.GetComponent<AttackActive>().atkFrames;
    }
    public int stopAttack()
    {
        if (currentTouchAttack)
        {
            Destroy(currentTouchAttack);
            
        }
        if (currentAttack) {
            int frames = currentAttack.GetComponent<AttackActive>().ending();
            if (currentAttack.GetComponent<AttackActive>().inHitlag)
            {
                lag(false);
            }
            Destroy(currentAttack);
            return frames;
        }
        return 0;
    }
    public void atkFinished(AtkType t)
    {
        pm.atkFinished(t== AtkType.Finisher);

        Destroy(currentAttack);
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
    public Vector2 getFinisherMotion(Vector2 vel)
    {
        Finisher f = currentAttack.GetComponent<Finisher>();
        if (f)
        {
            return f.motion(vel);
        }
        else
        {
            print("fucked up");
            return Vector2.zero;
        }
    }
}
