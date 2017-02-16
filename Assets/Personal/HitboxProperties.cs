using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxProperties : MonoBehaviour
{
    public Vector2 hitboxVector;
    public int hitstun;
    public int hitlag;
    public int damage;
    [HideInInspector]
    public List<GameObject> hitPlayers;

    Attack atk;
    // Use this for initialization
    void Start()
    {
        atk = GetComponentInParent<Attack>();
    }

    void OnTriggerEnter2D(Collider2D playerCol)
    {
        //print("collided");
        if (gameObject.CompareTag("Hazard"))
        {
            Vector2 knockback = new Vector2(hitboxVector.x * transform.right.x, hitboxVector.y);
            playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage);
        }
        else if (!atk.hit.Contains(playerCol.gameObject))
        {
            Vector2 knockback = new Vector2(hitboxVector.x * transform.right.x, hitboxVector.y);
            playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage);
            atk.addHit(playerCol.gameObject);
        }
        

    }
}
