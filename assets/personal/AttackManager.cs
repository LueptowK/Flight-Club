using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {
    public GameObject DownAir;

    GameObject currentAttack;
    public enum AtkType
    {
        DownAir
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

            
        }
        return currentAttack.GetComponent<Attack>().atkFrames;
    }
    public void stopAttack()
    {
        if (currentAttack) { Destroy(currentAttack); }
        
    }
}
