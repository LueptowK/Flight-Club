using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

    protected AttackManager mngr;
    protected List<GameObject> alreadyHit;

    [HideInInspector]
    public bool isActive;
    // Use this for initialization
    protected void Start () {
        mngr = GetComponentInParent<AttackManager>();
        resetHitList();
    }

    public void resetHitList()
    {
        alreadyHit = new List<GameObject>();
        alreadyHit.Add(transform.parent.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public List<GameObject> hit
    {
        get
        {
            return alreadyHit;
        }
    }
    public abstract void addHit(GameObject h, int hitLag);

}
