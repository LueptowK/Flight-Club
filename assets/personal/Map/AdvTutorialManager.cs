using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AdvTutorialManager : Manager {
    public GameObject Player;
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;
    public GameObject spawn4;
    public GameObject pauseScreen;

    private int endCounter;
    GameObject[] spawns;
    private GameObject lastSpawn;
    bool dead;
    bool finished;
    bool paused;
    int deathCounter;
    int startTimer;
	// Use this for initialization
	void Start () {
        endCounter = 120;
        spawns = new GameObject[4];
        spawns[0] = spawn1;
        spawns[1] = spawn2;
        spawns[2] = spawn3;
        spawns[3] = spawn4;

        startTimer = 10;
        deathCounter = 0;
        lastSpawn = spawn1;
        Player.GetComponent<PlayerMover>().TutorialStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!paused)
        {
            if (startTimer > 0)
            {
                startTimer--;
            }
            
            if (finished)
            {
                endCounter--;
                if (endCounter <= 0)
                {
                    Quit();
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
        
    }

    public override void checkpoint(int checkNum, Vector2 position)
    {
        if(checkNum == 4)
        {
            finished = true;
            GameObject.Find("Canvas").transform.Find("Complete").gameObject.SetActive(true);
        }
        else
        {
        lastSpawn = spawns[checkNum];
        }
    }

    public override void Pause()
    {
        if (!paused && !finished && (startTimer <= 0))
        {
            Player.GetComponent<PlayerMover>().pause(true);
            paused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else if (paused)
        {
            paused = false;
            Player.GetComponent<PlayerMover>().pause(false);
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public override void Quit()
    {
        SceneManager.LoadScene(7);
    }
}
