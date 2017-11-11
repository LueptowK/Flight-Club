using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : Manager {
    GameObject[] Spawns;
    GameObject[] Players;
    List<GameObject> removed;
    public GameObject pauseScreen;
    private bool gameOver;
    private int gameEndCounter;
    private int gameStartCounter;
    private Canvas c;
    private bool paused;
    public AudioClip song;
    private AudioSource source;

    // Use this for initialization
    void Start () {
        gameEndCounter = 300;
        gameStartCounter = 90;
        gameOver = false;
        removed = new List<GameObject>();
        Spawns = GameObject.FindGameObjectsWithTag("Respawn");
        Players = GameObject.FindGameObjectsWithTag("Player");
        source = GetComponent<AudioSource>();
        source.volume = 0.4f;
        source.loop = true;
        List<int> spawnL = new List<int>();
        spawnL.AddRange(Enumerable.Range(0,Spawns.Length-1));
        foreach(GameObject player in Players)
        {
            player.GetComponent<SpriteRenderer>().enabled = true;
            int spnInd = spawnL[(int)UnityEngine.Random.Range(0, spawnL.Count)];
            spawnL.Remove(spnInd);
            GameObject spawn = Spawns[spnInd];
            player.transform.position = spawn.transform.position;
            player.GetComponent<PlayerHealth>().reset();
            player.GetComponent<ComboCounter>().reset();
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<PlayerMover>().mapStart();
        }

        c = GameObject.Find("UI").GetComponent<Canvas>();
        c.transform.Find("3").gameObject.SetActive(true);
        c.transform.Find("HealthUI").gameObject.SetActive(true);

    }
    void FixedUpdate()
    {
        if (gameStartCounter > -15)
        {
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
                source.clip = song;
                source.Play();
            }
            if (gameStartCounter == -14)
            {
                c.transform.Find("GO").gameObject.SetActive(false);
            }


        }
        foreach (GameObject player in Players)
        {
            if (!removed.Contains(player))
            {
                if (player.GetComponent<PlayerHealth>().currentHealth <= 0)
                {
                    player.GetComponent<PlayerMover>().kill();
                    removed.Add(player);
                }
            }
           
        }

        if (removed.Count >= Players.Length - 1 && !gameOver)
        {
            c.transform.Find("GAME").gameObject.SetActive(true);
            gameOver = true;
        }

        if (gameOver)
        {
            gameEndCounter--;
            source.volume -= 0.003f;
        }

        if (gameEndCounter == 0)
        {
            Quit();
        }
    }

    public override void Pause()
    {
        if (!paused && !gameOver && (gameStartCounter <= 0))
        {
            foreach (GameObject player in Players)
            {
                player.GetComponent<PlayerMover>().pause(true);
            }
            paused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
            source.volume = 0.2f;
        }
        else if (paused)
        {
            foreach (GameObject player in Players)
            {
                player.GetComponent<PlayerMover>().pause(false);
            }
            paused = false;
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
            source.volume = 0.4f;
        }
    }

    public override void Quit()
    {

        CreatePlayer creator = GameObject.Find("PlayerCreator").GetComponent<CreatePlayer>();
        Debug.Log(creator);
        foreach (GameObject player in Players)
        {
            creator.holdStats(player.GetComponent<PlayerMover>().playerNum, player.GetComponent<StatTracker>());
            Destroy(player);
        }
        Destroy(c.gameObject);
        SceneManager.LoadScene(18);
    }

    public override void checkpoint(int checkNum, Vector2 position)
    {
        throw new NotImplementedException();
    }
}