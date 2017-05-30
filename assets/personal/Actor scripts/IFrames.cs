using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFrames : MonoBehaviour {
    int iFrames;
	// Use this for initialization
	void Start () {
        iFrames = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (iFrames > 0 && !gameObject.GetComponent<PlayerMover>().paused)
        if (iFrames > 0 )
        {
            iFrames--;
            if (iFrames == 0)
            {
                Collider2D col = GetComponent<Collider2D>();
                ContactFilter2D cf = new ContactFilter2D();
                Collider2D[] ret = new Collider2D[5];
                cf.layerMask = (1 << 11);
                cf.useLayerMask = true;
                cf.useTriggers = true;
                col.OverlapCollider(cf, ret);
                
                for(int i=0; i<ret.Length; i++)
                {
                    //print(i);
                    if (ret[i] == null)
                    {
                        break;
                    }

                    HitboxProperties h = ret[i].GetComponent<HitboxProperties>();
                    Attack atk = h.GetComponentInParent<Attack>();
                    if (!atk||!atk.hit.Contains(gameObject))
                    {
                        h.collidePlayer(col);
                    }
                    
                        

                }

            }
        }
	}

    public void SetFrames(int i)
    {
        iFrames = i;
    }
    public bool invincible()
    {
        return iFrames > 0;
    }
}
