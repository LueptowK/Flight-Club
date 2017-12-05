using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VersusVictoryManager : MonoBehaviour {
    public GameObject[] PlayerBox;
    public StatTracker[] stats;
    private bool[] Ready;
    private CreatePlayer creator;
    private int delay;
    private int deadCount;
    private int numPlayers;
	// Use this for initialization
	void Start () {
        delay = 30;
        Ready = new bool[4];
        creator = GameObject.Find("PlayerCreator").GetComponent<CreatePlayer>();
        Canvas noContestCanvas = GameObject.Find("NoContestCanvas").GetComponent<Canvas>();
        stats = creator.getStats();
        for (int i = 0; i < 4; i++)
        {
            Ready[i] = false;
            if (stats[i] != null)
            {
                numPlayers++;
                populateStats(i);
            }
            else
            {
                PlayerBox[i].SetActive(false);
                Ready[i] = true;
            }
        }
        Debug.Log(deadCount);
        Debug.Log(numPlayers);
        if (deadCount < numPlayers - 1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (stats[i] != null)
                {
                    Canvas c = PlayerBox[i].transform.Find("Canvas").GetComponent<Canvas>();
                    c.transform.Find("Winner").gameObject.SetActive(false);
                }
            }
            noContestCanvas.transform.Find("No Contest").gameObject.SetActive(true);
        }
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (delay > 0) { delay--; }

        for (int i = 0; i < 4; i++)
        {
            GamePadState g = GamePad.GetState((PlayerIndex)i);
            if ((g.Buttons.A == ButtonState.Pressed || g.Buttons.Start == ButtonState.Pressed) && delay <= 0)
            {
                if (!Ready[i])
                {
                    Ready[i] = true;
                    //visual indicator of readiness?
                    PlayerBox[i].GetComponent<SpriteRenderer>().color = Color.green;
                }
            }
            if(g.Buttons.B == ButtonState.Pressed)
            {
                if (Ready[i])
                {
                    Ready[i] = false;
                    //visual indicator of unreadiness?
                    PlayerBox[i].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }


            //FOR TESTING PURPOSES ONLY
            if (g.DPad.Up == ButtonState.Pressed && i == 0)
            {
                Ready[1] = true;
                PlayerBox[1].GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        bool advance = true;
        for (int i = 0; i < 4; i++)
        {
            if (!Ready[i]) {
                advance = false;
                break;
            }
        }
        if (advance)
        {
            SceneManager.LoadScene(1);
        }
    }

    void populateStats(int playerNum)
    {
        StatTracker stat = stats[playerNum];
        Canvas c = PlayerBox[playerNum].transform.Find("Canvas").GetComponent<Canvas>();
        if (creator.getName(playerNum) != null)
        {
            c.transform.Find("PlayerName").GetComponent<Text>().text = creator.getName(playerNum);
        }
        c.transform.Find("Attacks").GetComponent<Text>().text = stat.attemptedAttacks.ToString();
        if (stat.attemptedAttacks != 0)
        {
            c.transform.Find("Accuracy").GetComponent<Text>().text = (((float)stat.successfulHits / (float)stat.attemptedAttacks)*100).ToString() + "%";
        }
        else
        {
            c.transform.Find("Accuracy").GetComponent<Text>().text = 0.ToString();
        }

        if (stat.dead)
        {
            c.transform.Find("Winner").gameObject.SetActive(false);
            deadCount++;
        }
        c.transform.Find("DamageDealt").GetComponent<Text>().text = stat.damageDealt.ToString();
        c.transform.Find("DamageTaken").GetComponent<Text>().text = stat.damageTaken.ToString();
        c.transform.Find("FinishersHit").GetComponent<Text>().text = stat.successfulFinishers.ToString();
        c.transform.Find("FinisherMax").GetComponent<Text>().text = stat.maxFinisher.ToString();
        c.transform.Find("ShieldStolen").GetComponent<Text>().text = stat.shieldStolen.ToString();
        c.transform.Find("Dashes").GetComponent<Text>().text = stat.timesDashed.ToString();
        c.transform.Find("Hitstalls").GetComponent<Text>().text = stat.hitStalls.ToString();
        c.transform.Find("Projectiles").GetComponent<Text>().text = stat.projectilesFired.ToString();
    }
}
