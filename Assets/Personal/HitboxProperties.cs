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
            if (!(playerCol.GetComponent<IFrames>().invincible()))
            {
                Vector2 knockback = new Vector2(hitboxVector.x * transform.right.x, hitboxVector.y);
                playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage);
            }
        }
        else if (playerCol.gameObject.CompareTag("Target"))
        {
            Destroy(playerCol.gameObject);
            atk.addHit(playerCol.gameObject);
        }
        else if (!atk.hit.Contains(playerCol.gameObject))
        {
            if (!(playerCol.GetComponent<IFrames>().invincible()))
            {
                Vector2 knockback;
                if (transform.parent.parent.localScale.y < 0)
                {
                    knockback = new Vector2(hitboxVector.x * transform.right.x + hitboxVector.y * -transform.up.x, hitboxVector.x * transform.right.y + hitboxVector.y * -transform.up.y);

                }
                else
                {
                    knockback = new Vector2(hitboxVector.x * transform.right.x + hitboxVector.y * transform.up.x, hitboxVector.x * transform.right.y + hitboxVector.y * transform.up.y);
                }
                if (transform.parent.tag == "FinisherSlash")
                {
                    int str = transform.parent.GetComponent<Attack>().comboStrength;
                    float strength = str / 5f * 4f;
                    playerCol.GetComponent<PlayerMover>().getHit(knockback * strength / 2, hitlag, hitstun, (int)(damage * strength));
                }
                else
                {
                    playerCol.GetComponent<PlayerMover>().getHit(knockback, hitlag, hitstun, damage);
                }
                atk.addHit(playerCol.gameObject);
            }

        }
        
    }
}
