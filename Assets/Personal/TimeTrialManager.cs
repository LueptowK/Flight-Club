using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TimeTrialManager : MonoBehaviour {
    public GameObject Player;
    public GameObject spawn;
    private int endCounter;
    private int time = 0;
    private int gameStartCounter = 90;
    GameObject[] spawns;
    bool dead;
    bool finished;
    private Canvas c;
    private GameObject timer;
    int deathCounter;
	// Use this for initialization
	void Start () {
        endCounter = 120;
        Canvas c = ((Canvas)FindObjectOfType(typeof(Canvas)));
        timer = c.transform.Find("Timer").gameObject;
        Player.GetComponent<PlayerMover>().mapStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateTimer();
        if (gameStartCounter > -15)
        {
            c = ((Canvas)FindObjectOfType(typeof(Canvas)));
            gameStartCounter--;
            if (gameStartCounter == 61)
            {
                c.transform.Find("3").gameObject.SetActive(false);
                c.transform.Find("2").gameObject.SetActive(true);
            }
            if (gameStartCounter == 31)
            {
                c.transform.Find("2").gameObject.SetActive(false);
                c.transform.Find("1").gameObject.SetActive(true);
            }
            if (gameStartCounter == 1)
            {
                c.transform.Find("1").gameObject.SetActive(false);
                c.transform.Find("GO").gameObject.SetActive(true);
            }
            if (gameStartCounter == -14)
            {
                c.transform.Find("GO").gameObject.SetActive(false);
            }
        }
        if(!finished && gameStartCounter <= 0)
        {
            time++;
        }

        if (finished)
        {
            endCounter--;
            if (endCounter <= 0)
            {
                SceneManager.LoadScene(9);
            }
        }
        if (dead)
        {
            deathCounter--;
            if (deathCounter == 0)
            {
                Player.GetComponent<PlayerMover>().reset();
                Player.transform.position = spawn.transform.position;
                Player.GetComponent<PlayerHealth>().currentHealth = 5;
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
}
