using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StoryLevelManager : Manager
{
    public GameObject Player;
    public GameObject pauseScreen;

    private int endCounter;
    public GameObject startSpawn;
    private Vector2 currentSpawn;
    public Checkpointer finalPoint;
    public int nextScene;
    bool dead;
    bool finished;
    bool paused;
    int deathCounter;
    int startTimer;

    Canvas c;
    // Use this for initialization
    void Start()
    {
        endCounter = 120;
        currentSpawn = startSpawn.transform.position;
        startTimer = 10;
        deathCounter = 0;
        //currentSpawn = spawn1;
        Player.GetComponent<PlayerMover>().TutorialStart();
        c = ((Canvas)FindObjectOfType(typeof(Canvas)));
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
                    win();
                }
            }
            if (dead)
            {
                deathCounter--;
                if (deathCounter == 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    //Quit();
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
    public void finish()
    {
        c.transform.Find("Complete").gameObject.SetActive(true);
        finished = true;
    }

    public void win()
    {
        SceneManager.LoadScene(nextScene);
    }
}

