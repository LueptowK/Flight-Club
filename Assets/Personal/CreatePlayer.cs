using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

public class CreatePlayer : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject Keith;
    public GameObject HealthBar;
    public GameObject portraitSlot1;
    public GameObject portraitSlot2;
    public GameObject portraitSlot3;
    public GameObject portraitSlot4;
    private bool[] active;
    playerColor[] pColors;
    GameObject[] players;
    GameObject[] CharSelectPortraits;
    
    int activeCount = 0;

    struct playerColor
    {
        public Color color;
        public int owner;
        public playerColor(Color c)
        {
            this.color = c;
            this.owner = -1;
        }

    }
    void Awake()
    {
        pColors = new playerColor[6];
        pColors[0] = new playerColor(Color.white);
        pColors[1] = new playerColor(new Color(1f,.2f,.2f,1f));
        pColors[2] = new playerColor(new Color(0f, 1f, 1f, 1f));
        pColors[3] = new playerColor(new Color(.4f,.4f,.4f,1f));
        pColors[4] = new playerColor(new Color(1f,0.4f,1f));
        pColors[5] = new playerColor(Color.yellow);

        CharSelectPortraits = new GameObject[4];
        CharSelectPortraits[0] = portraitSlot1;
        CharSelectPortraits[1] = portraitSlot2;
        CharSelectPortraits[2] = portraitSlot3;
        CharSelectPortraits[3] = portraitSlot4;

        active = new bool[4];
        players = new GameObject[4];
    }

    public void changeColor(int player, bool inArray)
    {
        if (inArray)
        {
            int j;
            for (j = 0; j < pColors.Length; j++)
            {
                if (pColors[j].owner == player)
                {
                    pColors[j].owner = -1;
                    break;
                }
            }
            for (int a = 1; a < pColors.Length + 1; a++)
            {
                int index = (a + j) % pColors.Length;
                if (pColors[index].owner == -1)
                {

                    setColor(index, player);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < pColors.Length; i++)
            {
                if (pColors[i].owner == -1)
                {
                    setColor(i, player);
                    break;
                }
            }
        }

    }
    public void setColor(int i, int player)
    {
        pColors[i].owner = player;
        players[player].GetComponent<SpriteRenderer>().color = pColors[i].color;
        players[player].GetComponent<PlayerHealth>().img.transform.parent.Find("BarIdentifier").GetComponent<Image>().color = pColors[i].color;
        CharSelectPortraits[player].transform.GetChild(0).GetComponent<SpriteRenderer>().color = pColors[i].color;
    }

    public void activatePlayer(int playerNum)
    {
        if (!active[playerNum])
        {
            GameObject p = Instantiate(Keith);
            GameObject h = Instantiate(HealthBar, Canvas.transform.Find("HealthUI").transform);
            p.GetComponent<PlayerInput>().PlayerNumber = playerNum;
            p.GetComponent<PlayerHealth>().img = h.transform.Find("BarFill").gameObject.GetComponent<Image>();
            //h.SetActive(false);
            p.SetActive(false);
            active[playerNum] = true;
            players[playerNum] = p;
            CharSelectPortraits[playerNum].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            changeColor(playerNum, false);
            activeCount++;

            
        }
    }

    public void deactivatePlayer(int playerNum)
    {
        print("deactivatePlayer not implemented yet");
    }

    public void goToMapSelect()
    {
        if (activeCount > 1)
        {
            DontDestroyOnLoad(Canvas);
            for (int j = 0; j < players.Length; j++)
            {
                if (active[j])
                {
                    players[j].SetActive(true);
                    DontDestroyOnLoad(players[j]);
                    players[j].GetComponent<ComboCounter>().reset();
                    players[j].GetComponent<SpriteRenderer>().enabled = false;
                    //reset(players[i]);
                }
            }
            //Destroy(Canvas.transform.FindChild("TutorialText").gameObject);
            SceneManager.LoadScene(2); //UPDATE TO MAP SELECT SCREEN WHEN THAT EXISTS
        }

    }

    public void createDummy()
    {
        if (!active[1])
        {
            activatePlayer(1);
        }
    }
}
