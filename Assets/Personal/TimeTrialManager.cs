using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TimeTrialManager : Manager {
    public GameObject Player;
    public GameObject spawn;
    private int endCounter;
    private int time = 0;
    private int gameStartCounter = 60;
    GameObject[] spawns;
    bool dead;
    bool finished;
    private Canvas c;
    private GameObject timer;
    int deathCounter;
    bool paused;
    public GameObject pauseScreen;
    public bool isTarget = false;
	// Use this for initialization
	void Start () {
        endCounter = 120;
        c = GameObject.Find("Canvas").transform.GetComponent<Canvas>();
        timer = c.transform.Find("Timer").gameObject;
        Player.GetComponent<PlayerMover>().SinglePlayerStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!paused)
        {
            updateTimer();
            if (gameStartCounter > -15)
            {
                gameStartCounter--;
                if (gameStartCounter == 1)
                {
                    c.transform.Find("READY").gameObject.SetActive(false);
                    c.transform.Find("GO").gameObject.SetActive(true);
                }
                if (gameStartCounter == -14)
                {
                    c.transform.Find("GO").gameObject.SetActive(false);
                }
            }
            if (!finished && !dead && gameStartCounter <= 0)
            {
                time++;
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
                    Player.GetComponent<PlayerMover>().SinglePlayerStart();
                    Player.transform.position = spawn.transform.position;
                    Player.GetComponent<PlayerHealth>().currentHealth = 5;
                    c.transform.Find("READY").gameObject.SetActive(true);
                    gameStartCounter = 60;
                    dead = false;
                    time = 0;
                }
            }
            if (Player.GetComponent<PlayerHealth>().currentHealth <= 0 && !dead)
            {
                Player.GetComponent<PlayerMover>().kill();
                deathCounter = 89;
                dead = true;
            }
            if (isTarget) { if (GameObject.FindGameObjectsWithTag("Target").Length == 0)
                {
                    finish();
                } }
        }
        
    }

    public void finish()
    {
        finished = true;
        c.transform.Find("Complete").gameObject.SetActive(true);
    }

    void updateTimer()
    {
        float realTime = time * Time.fixedDeltaTime;
        float minutes = realTime / 60;
        float seconds = Mathf.Floor(realTime % 60);
        float fraction = (realTime * 100) % 100;
        timer.GetComponent<Text>().text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);
    }

    public override void Pause()
    {
        if(gameStartCounter <= 0 && !paused && !finished)
        {
            Player.GetComponent<PlayerMover>().pause(true);
            paused = true;
            pauseScreen.SetActive(true);
        }
        else if (gameStartCounter <= 0 && paused)
        {
            paused = false;
            Player.GetComponent<PlayerMover>().pause(false);
            pauseScreen.SetActive(false);
        }
    }

    public override void Quit()
    {
        SceneManager.LoadScene(9);
    }

    public override void checkpoint(int checkNum)
    {
        throw new NotImplementedException();
    }
}
