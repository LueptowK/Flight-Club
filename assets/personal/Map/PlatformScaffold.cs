using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScaffold : MonoBehaviour {

    public GameObject platform;
    public Vector2 speed;
    public float spawnSpeed;
    public Vector2 scaffSize;
    //public float distMin;


    List<GameObject> Platforms;
    Vector2 nextPlt;
    bool wall, pos;
    int spawnCoolDown=0;

    bool paused = false;
	// Use this for initialization
	void Start () {
        Platforms = new List<GameObject>();
        calcSide();
        setNextPlat();
        spawnPlat();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 scaff = scaffSize;
        scaff.z = 0;
        if (Platforms.Count>0&&!Vec3.lessThan( Vec3.Abs(Platforms[0].transform.localPosition),scaff))
        {
            GameObject g = Platforms[0];
            Platforms.Remove(g);
            Destroy(g);
        }

        if (!paused)
        {
            spawnCoolDown--;
        }
        
        if (spawnCoolDown<=0){
            spawnPlat();

        }
	}
    void spawnPlat()
    {
        GameObject plat = Instantiate(platform, transform.position + (Vector3)nextPlt, transform.rotation, transform);
        plat.GetComponent<PlatformMov>().speed = speed;
        Platforms.Add(plat);
        setNextPlat();
        spawnCoolDown = (int)(spawnSpeed * 60);
    }

    void setNextPlat()
    {
        float x, y;
        if (wall)
        {
            y = (Random.value - 0.5f) * scaffSize.y;
            x = scaffSize.x;
            if (!pos)
            {
                x = -x;
            }
            
        }
        else
        {
            x = (Random.value - 0.5f) * scaffSize.x;
            y = scaffSize.y;
            if (!pos)
            {
                y = -y;
            }
        }
        nextPlt = new Vector2(x, y);
    }
    public void pause()
    {
        foreach(GameObject p in Platforms)
        {
            p.GetComponent<PlatformMov>().speed = Vector2.zero;
        }
        paused = true;
    }
    public void unpause()
    {
        foreach (GameObject p in Platforms)
        {
            p.GetComponent<PlatformMov>().speed = speed;
        }
        paused = false;
    }
    public void changeSpeed(Vector2 newSpeed)
    {
        speed = newSpeed;
        calcSide();
        setNextPlat();
    }
    public void reverse()
    {
        Platforms.Reverse();
    }
    void calcSide()
    {
        if (Mathf.Abs(speed.x) > Mathf.Abs(speed.y))
        {
            wall = true;
        }
        else
        {
            wall = false;
        }
        if (wall)
        {
            if (speed.x < 0)
            {
                pos = true;
            }
            else
            {
                pos = false;
            }
        }
        else
        {
            if (speed.y < 0)
            {
                pos = true;
            }
            else
            {
                pos = false;
            }
        }
    }
}
