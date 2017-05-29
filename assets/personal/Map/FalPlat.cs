using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalPlat : MonoBehaviour {

    // Use this for initialization
    
    
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover pm =other.gameObject.GetComponent<PlayerMover>();
        if (pm&&other.transform.position.y-other.GetComponent<Collider2D>().bounds.extents.y*0.8f<transform.position.y+GetComponent<Collider2D>().bounds.extents.y)
        {
            
            pm.setLayer(true);
        }
        
    }
}
