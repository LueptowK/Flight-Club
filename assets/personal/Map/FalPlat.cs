using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalPlat : MonoBehaviour {

    // Use this for initialization
    
    
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover pm =other.gameObject.GetComponent<PlayerMover>();
        if (pm)
        {
           
            pm.setLayer(true);
        }
        
    }
}
