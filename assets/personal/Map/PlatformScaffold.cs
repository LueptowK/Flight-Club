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

	// Use this for initialization
	void Start () {
        Platforms = new List<GameObject>();
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
        setNextPlat();
        spawnPlat();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 scaff = scaffSize;
        scaff.z = 1;
        if (Vec3.lessThan(scaff, Platforms[0].transform.localPosition))
        {
            GameObject g = Platforms[0];
            Platforms.Remove(g);
            Destroy(g);
        }
        GameObject last = Platforms[Platforms.Count - 1];
        Vector3 v = -(Vector3)speed * spawnSpeed;
        if (Vec3.lessThan(last.transform.localPosition+v, scaff)){
            spawnPlat();

        }
	}
    void spawnPlat()
    {
        GameObject plat = Instantiate(platform, transform.position + (Vector3)nextPlt, transform.rotation, transform);
        plat.GetComponent<PlatformMov>().speed = speed;
        Platforms.Add(plat);
        setNextPlat();
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
            x = Random.value - 0.5f * scaffSize.x;
            y = scaffSize.y;
            if (!pos)
            {
                y = -y;
            }
        }
        nextPlt = new Vector2(x, y);
    }
}
