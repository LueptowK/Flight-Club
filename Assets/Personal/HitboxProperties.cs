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
    public void setAtk()
    {
        atk = GetComponentInParent<Attack>();
        //print(atk);
    }
    void OnTriggerEnter2D(Collider2D playerCol)
    {
        //print("collided");
        if (gameObject.CompareTag("Hazard"))
        {
            if (!(playerCol.GetComponent<IFrames>().invincible()))
            {
                Vector2 knockback = new Vector2(hitboxVector.x * transform.right.x, hitboxVector.y);
                playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage);
            }
        }
        else if (playerCol.gameObject.CompareTag("Target"))
        {
            atk.addHit(playerCol.gameObject, hitlag);
            Destroy(playerCol.gameObject);

            
        }
        else if (!atk.hit.Contains(playerCol.gameObject))
        {
            if (!(playerCol.GetComponent<IFrames>().invincible()))
            {
                collidePlayer(playerCol);
            }

        }
        
    }
    public void collidePlayer(Collider2D playerCol)
    {
        Vector2 knockback;
        if (gameObject.CompareTag("Hazard"))
        {
            knockback = new Vector2(hitboxVector.x * transform.right.x, hitboxVector.y);
            playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage);
        }
        else
        {
            if (transform.parent.parent.localScale.y < 0)
            {
                knockback = new Vector2(hitboxVector.x * transform.right.x + hitboxVector.y * -transform.up.x, hitboxVector.x * transform.right.y + hitboxVector.y * -transform.up.y);

            }
            else
            {
                knockback = new Vector2(hitboxVector.x * transform.right.x + hitboxVector.y * transform.up.x, hitboxVector.x * transform.right.y + hitboxVector.y * transform.up.y);
            }
            if (transform.parent.tag == "Finisher")
            {
                int str = transform.parent.GetComponent<Attack>().comboStrength;
                float strength = str / 5f * 4f;
                playerCol.GetComponent<PlayerMover>().getHit(knockback * strength / 2, hitlag, hitstun, (int)(damage * strength), atk);
            }
            else
            {
                playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage, atk);
            }
            atk.addHit(playerCol.gameObject, hitlag);
        }
            
        
    }
}
