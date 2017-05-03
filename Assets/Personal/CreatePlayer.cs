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
    public GameObject Walt;
    public GameObject HealthBar;
    public GameObject portraitSlot1;
    public GameObject portraitSlot2;
    public GameObject portraitSlot3;
    public GameObject portraitSlot4;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;
    private bool[] active;
    playerColor[] keithColors;
    playerColor[] waltColors;
    GameObject[] players;
    GameObject[] CharSelectPortraits;
    
    int activeCount = 0;

    public struct playerColor
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
        keithColors = new playerColor[6];
        keithColors[0] = new playerColor(Color.white);
        keithColors[1] = new playerColor(new Color(1f,.2f,.2f,1f));
        keithColors[2] = new playerColor(new Color(0f, 1f, 1f, 1f));
        keithColors[3] = new playerColor(new Color(.4f,.4f,.4f,1f));
        keithColors[4] = new playerColor(new Color(1f,0.4f,1f));
        keithColors[5] = new playerColor(Color.yellow);

        waltColors = new playerColor[6];
        waltColors[0] = new playerColor(Color.black);
        waltColors[1] = new playerColor(new Color(0.2f,.7f,0.2f));
        waltColors[2] = new playerColor(Color.blue);
        waltColors[3] = new playerColor(Color.white);
        waltColors[4] = new playerColor(new Color32(0x68,0x2f,0x6d, 0xFF));
        waltColors[5] = new playerColor(Color.yellow);


        CharSelectPortraits = new GameObject[4];
        CharSelectPortraits[0] = portraitSlot1;
        CharSelectPortraits[1] = portraitSlot2;
        CharSelectPortraits[2] = portraitSlot3;
        CharSelectPortraits[3] = portraitSlot4;

        active = new bool[4];
        players = new GameObject[4];

        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
    }

    public void changeColor(int player, bool inArray)
    {
        playerColor[] pColors;
        if (players[player].GetComponent<PlayerMover>().cardOne.character == 1)
        {
            pColors = waltColors;
        }
        else
        {
            pColors = keithColors;
        }
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
        playerColor[] pColors;
        if (players[player].GetComponent<PlayerMover>().cardOne.character == 1)
        {
            pColors = waltColors;
        }
        else
        {
            pColors = keithColors;
        }
        pColors[i].owner = player;
        Color finalColor = pColors[i].color;
        if (players[player].GetComponent<PlayerMover>().cardOne.character == 1)
        {
            players[player].GetComponent<SpriteRenderer>().material.shader = shaderGUItext;
            CharSelectPortraits[player].transform.GetChild(0).GetComponent<SpriteRenderer>().material.shader = shaderGUItext;
        }
        players[player].GetComponent<SpriteRenderer>().color = finalColor;
        players[player].GetComponent<PlayerHealth>().img.transform.parent.Find("BarIdentifier").GetComponent<Image>().color = finalColor;
        CharSelectPortraits[player].transform.GetChild(0).GetComponent<SpriteRenderer>().color = finalColor;
    }

    public void activatePlayer(int playerNum, int character)
    {
        if (!active[playerNum])
        {
            GameObject p;
            if (character == 0)
            {
                p = Instantiate(Keith);
            }
            else if (character == 1)
            {
                p = Instantiate(Walt);
            }
            else
            {
                p = null;
                print("fuck");
            }
            GameObject h = Instantiate(HealthBar, Canvas.transform.Find("HealthUI").transform);
            p.GetComponent<PlayerInput>().PlayerNumber = playerNum;
            p.GetComponent<PlayerHealth>().img = h.transform.Find("BarFill").gameObject.GetComponent<Image>();
            p.GetComponent<PlayerHealth>().shieldImg = h.transform.Find("BarShield").gameObject.GetComponent<Image>();
            //h.SetActive(false);
            p.SetActive(false);
            active[playerNum] = true;
            players[playerNum] = p;
            CharSelectPortraits[playerNum].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            if (character == 0)
            {
                changeColor(playerNum, false);
            }
            else
            {
                changeColor(playerNum, false);
            }
            
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
            activatePlayer(1, 0);
        }
    }
}
