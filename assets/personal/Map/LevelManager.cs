using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : Manager
{
    public GameObject Player;
    public GameObject pauseScreen;

    private int endCounter;
    public GameObject startSpawn;
    private Vector2 currentSpawn;
    public Checkpointer finalPoint;
    bool dead;
    bool finished;
    bool paused;
    int deathCounter;
    int startTimer;
    // Use this for initialization
    void Start()
    {
        endCounter = 120;

        currentSpawn = startSpawn.transform.position;
        startTimer = 10;
        deathCounter = 0;
        //currentSpawn = spawn1;
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
                    Player.transform.position = currentSpawn;
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
        if (checkNum == finalPoint.checkNum)
        {
            finished = true;
            GameObject.Find("Canvas").transform.Find("Complete").gameObject.SetActive(true);
        }
        else
        {
            currentSpawn = position;
        }
    }

    public override void Pause()
    {
        if (!paused && !finished && (startTimer <= 0))
        {
            Player.GetComponent<PlayerMover>().pause(true);
            paused = true;
            pauseScreen.SetActive(true);
        }
        else if (paused)
        {
            paused = false;
            Player.GetComponent<PlayerMover>().pause(false);
            pauseScreen.SetActive(false);
        }
    }

    public override void Quit()
    {
        SceneManager.LoadScene(7);
    }
}
