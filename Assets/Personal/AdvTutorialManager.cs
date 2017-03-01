using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AdvTutorialManager : MonoBehaviour {
    public GameObject Player;
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;
    public GameObject spawn4;

    private int endCounter;
    GameObject[] spawns;
    private GameObject lastSpawn;
    bool dead;
    bool finished;
    int deathCounter;
	// Use this for initialization
	void Start () {
        endCounter = 120;
        spawns = new GameObject[4];
        spawns[0] = spawn1;
        spawns[1] = spawn2;
        spawns[2] = spawn3;
        spawns[3] = spawn4;

        deathCounter = 0;
        lastSpawn = spawn1;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (finished)
        {
            endCounter--;
            if (endCounter <= 0)
            {
                SceneManager.LoadScene(7);
            }
        }
        if (dead)
        {
            deathCounter--;
            if (deathCounter == 0)
            {
                Player.GetComponent<PlayerMover>().reset();
                Player.transform.position = lastSpawn.transform.position;
                Player.GetComponent<PlayerHealth>().currentHealth = 5;
                dead = false;
            }
        }
        if (Player.GetComponent<PlayerHealth>().currentHealth <= 0 && !dead)
        {
            Player.GetComponent<PlayerMover>().kill();
            deathCounter = 89;
            dead = true;
        }
        
    }

    public void checkpoint(int checkNum)
    {
        if(checkNum == 4)
        {
            finished = true;
        }
        else
        {
        lastSpawn = spawns[checkNum];
        GameObject.Find("Canvas").transform.Find("Complete").gameObject.SetActive(true);
        }
    }
}
