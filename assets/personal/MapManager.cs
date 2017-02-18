using System.Collections;
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
    private int gameCounter;
    // Use this for initialization
    void Start () {
        gameCounter = 300;
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
        }
    }
    void FixedUpdate()
    {

        foreach(GameObject player in Players)
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
            Canvas c = ((Canvas)FindObjectOfType(typeof(Canvas)));
            c.transform.Find("GAME").gameObject.SetActive(true);
            gameOver = true;
        }

        if (gameOver)
        {
            gameCounter--;
        }

        if (gameCounter == 0)
        {
            Canvas c = ((Canvas)FindObjectOfType(typeof(Canvas)));
            Destroy(c.gameObject);
            foreach (GameObject player in Players)
            {
                Destroy(player);
            }
            SceneManager.LoadScene(1);
        }
    }
	


}
