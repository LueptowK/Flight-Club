﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    GameObject[] Spawns;
    GameObject[] Players;
    List<GameObject> removed;
    public GameObject GAME;
    private bool gameOver;
    private int gameEndCounter;
    private int gameStartCounter;
    private Canvas c;
    // Use this for initialization
    void Start () {
        gameEndCounter = 300;
        gameStartCounter = 90;
        gameOver = false;
        removed = new List<GameObject>();
        Spawns = GameObject.FindGameObjectsWithTag("Respawn");
        Players = GameObject.FindGameObjectsWithTag("Player");
        List<int> spawnL = new List<int>();
        spawnL.AddRange(Enumerable.Range(0,Spawns.Length-1));
        foreach(GameObject player in Players)
        {
            int spnInd = spawnL[(int)Random.Range(0, spawnL.Count)];
            spawnL.Remove(spnInd);
            GameObject spawn = Spawns[spnInd];
            player.transform.position = spawn.transform.position;
            player.GetComponent<PlayerHealth>().reset();
            player.GetComponent<ComboCounter>().reset();
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<PlayerMover>().mapStart();
        }

        Canvas c = ((Canvas)FindObjectOfType(typeof(Canvas)));
        c.transform.Find("3").gameObject.SetActive(true);

    }
    void FixedUpdate()
    {
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
        }

        if (gameEndCounter == 0)
        {
            Destroy(c.gameObject);
            foreach (GameObject player in Players)
            {
                Destroy(player);
            }
            SceneManager.LoadScene(1);
        }
    }
	


}
