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
    public GameObject[] nametags;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;
    private playerColor[][] Colors = new playerColor[2][];
    public bool[] active;
    playerColor[] keithColors;
    playerColor[] waltColors;
    playerInfo[] players;
    GameObject[] CharSelectPortraits;

    int numCharacters;
    
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

    public struct playerInfo
    {
        public int character;
        public int colorNum;
    }

    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            foreach (CreatePlayer i in FindObjectsOfType(GetType()))
            {
                if (this != i)
                {
                    i.reset();
                }
            }
            Destroy(gameObject);
        }

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

        Colors[0] = keithColors;
        Colors[1] = waltColors;
        //Colors[][] is addressed as Colors[character number][color number]

        CharSelectPortraits = new GameObject[4];
        CharSelectPortraits[0] = portraitSlot1;
        CharSelectPortraits[1] = portraitSlot2;
        CharSelectPortraits[2] = portraitSlot3;
        CharSelectPortraits[3] = portraitSlot4;

        active = new bool[4];
        players = new playerInfo[4];
    }

    public void changeColor(int player, bool inArray)
    {
        playerColor[] pColors;
        if (players[player].character == -1) { return; }

        pColors = Colors[players[player].character];
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
        Colors[players[player].character] = pColors;
    }
    public void setColor(int i, int player)
    {
        Colors[players[player].character][i].owner = player;
        Color finalColor = Colors[players[player].character][i].color;
        players[player].colorNum = i;
        CharSelectPortraits[player].transform.GetChild(players[player].character).GetComponent<SpriteRenderer>().color = finalColor;
    }

    public void activatePlayer(int playerNum, int character)
    {
        if(active[playerNum] && character != players[playerNum].character)
        {
            deactivatePlayer(playerNum);
        }
        if (!active[playerNum])
        {
            players[playerNum].character = character;
            active[playerNum] = true;

            //THIS IF SHOULD CHANGE/go away once we have random char images
            if (character != -1)
            {
                CharSelectPortraits[playerNum].transform.GetChild(character).GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                CharSelectPortraits[playerNum].transform.FindChild("PlayerSlotRandom").GetComponent<SpriteRenderer>().enabled = true;
            }
            changeColor(playerNum, false);
            
            activeCount++;
        }
    }

    public void deactivatePlayer(int playerNum)
    {
        active[playerNum] = false;
        if (players[playerNum].character != -1)
        {
            CharSelectPortraits[playerNum].transform.GetChild(players[playerNum].character).GetComponent<SpriteRenderer>().enabled = false;
            for(int i = 0; i < 6; i++)
        {
                if (Colors[players[playerNum].character][i].owner == playerNum)
                {
                    Colors[players[playerNum].character][i].owner = -1;
                }
            }
        }
        else
        {
            CharSelectPortraits[playerNum].transform.FindChild("PlayerSlotRandom").GetComponent<SpriteRenderer>().enabled = false;
        }    

        activeCount--;
    }

    public void goToMapSelect()
    {
        if (activeCount > 1)
        {
            DontDestroyOnLoad(Canvas);
            DontDestroyOnLoad(this);
            SceneManager.LoadScene(2);
        }

    }

    public void spawnPlayers()
    {
        for (int j = 0; j < players.Length; j++)
        {
            if (active[j])
            {
                GameObject p;
                playerColor[] pColors;
                int character = players[j].character;
                if (character == -1)
                {
                    character = Random.Range(0, 2);
                    players[j].colorNum = Random.Range(0, 6);
                    while (Colors[character][players[j].colorNum].owner != -1)
                    {
                        players[j].colorNum = Random.Range(0, 6);
                    }
                    Colors[character][players[j].colorNum].owner = j;
                }


                if (character == 0)
                {
                    p = Instantiate(Keith);
                    pColors = Colors[character];
                }
                else if (character == 1)
                {
                    p = Instantiate(Walt);
                    pColors = Colors[character];
                }
                else
                {
                    p = null;
                    print("fuck");
                    return;
                }

                GameObject h = Instantiate(HealthBar, Canvas.transform.Find("HealthUI").transform);
                p.GetComponent<PlayerInput>().PlayerNumber = j;
                p.GetComponent<PlayerHealth>().img = h.transform.Find("BarFill").gameObject.GetComponent<Image>();
                p.GetComponent<PlayerHealth>().shieldImg = h.transform.Find("BarShield").gameObject.GetComponent<Image>();

                Color finalColor = pColors[players[j].colorNum].color;
                p.GetComponent<SpriteRenderer>().color = finalColor;
                p.GetComponent<PlayerHealth>().img.transform.parent.Find("BarIdentifier").GetComponent<Image>().color = finalColor;
                p.GetComponent<PlayerHealth>().img.transform.parent.Find("BarIdentifier").GetComponent<Image>().material = p.GetComponent<SpriteRenderer>().material;

                p.transform.position = new Vector3(999, -999, 0);
                DontDestroyOnLoad(p);
                p.GetComponent<ComboCounter>().reset();
                p.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void createDummy()
    {
        if (!active[1])
        {
            activatePlayer(1, 0);
        }
    }

    public void reset()
    {
        Canvas = GameObject.Find("Canvas");
        portraitSlot1 = GameObject.Find("CharSelectPlayer");
        portraitSlot2 = GameObject.Find("CharSelectPlayer (1)");
        portraitSlot3 = GameObject.Find("CharSelectPlayer (2)");
        portraitSlot4 = GameObject.Find("CharSelectPlayer (3)");
        CharSelectPortraits[0] = portraitSlot1;
        CharSelectPortraits[1] = portraitSlot2;
        CharSelectPortraits[2] = portraitSlot3;
        CharSelectPortraits[3] = portraitSlot4;

        for (int i=0; i<4; i++)
        {
            if (active[i])
            {
                //THIS IF SHOULD CHANGE once there are random character images
                if (players[i].character != -1)
                {
                    CharSelectPortraits[i].transform.GetChild(players[i].character).GetComponent<SpriteRenderer>().enabled = true;
                    setColor(players[i].colorNum, i);
                }
                else
                {
                    CharSelectPortraits[i].transform.FindChild("PlayerSlotRandom").GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        
    }
}
