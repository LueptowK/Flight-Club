using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMover : Mover {

    public int shootCooldown =40;
    int shootCDcurrent=0;
    public GameObject projectile;
    public float turnspeed;
    Interpreter ci;
	// Use this for initialization
	void Start () {
        ci = GetComponent<Interpreter>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (shootCDcurrent > 0)
        {
            shootCDcurrent--;
        }
        else if (ci.Shoot)
        {
            shoot();
            shootCDcurrent = shootCooldown;
        }
        float rot = turnspeed / 60;
        float a;
        if (ci.move != Vector2.zero)
        {
            a = Vector2.Angle(transform.right, ci.move);
            rot = rot / a;
            //print(rot);
            if (rot > 1)
            {
                rot = 1;
            }

            Vector2 v2 = ci.move;
            a = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, a), rot);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, turnspeed / 120));
        }
        
        
        
	}
    void shoot()
    {
        GameObject p = Instantiate(projectile, transform.position + transform.right * 1f, transform.rotation);
        p.GetComponent<Projectile>().hitPlayers.Add(gameObject);
    }
    public override void kill()
    {
        Destroy(gameObject);
    }
}
