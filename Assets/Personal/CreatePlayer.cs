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
    public bool[] active;
    playerColor[] keithColors;
    playerColor[] waltColors;
    playerInfo[] players;
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


        CharSelectPortraits = new GameObject[4];
        CharSelectPortraits[0] = portraitSlot1;
        CharSelectPortraits[1] = portraitSlot2;
        CharSelectPortraits[2] = portraitSlot3;
        CharSelectPortraits[3] = portraitSlot4;

        active = new bool[4];
        players = new playerInfo[4];

        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
    }

    public void changeColor(int player, bool inArray)
    {
        playerColor[] pColors;
        if (players[player].character == 1)
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
        if (players[player].character == 1)
        {
            pColors = waltColors;
        }
        else
        {
            pColors = keithColors;
        }
        pColors[i].owner = player;
        Color finalColor = pColors[i].color;
        if (players[player].character == 1)
        {
            //players[player].GetComponent<SpriteRenderer>().material.shader = shaderGUItext;
            CharSelectPortraits[player].transform.GetChild(0).GetComponent<SpriteRenderer>().material.shader = shaderGUItext;
        }
        else
        {
            //players[player].GetComponent<SpriteRenderer>().material.shader = shaderSpritesDefault;
            CharSelectPortraits[player].transform.GetChild(0).GetComponent<SpriteRenderer>().material.shader = shaderSpritesDefault;
        }
        players[player].colorNum = i;
        //players[player].GetComponent<PlayerHealth>().img.transform.parent.Find("BarIdentifier").GetComponent<Image>().color = finalColor;
        CharSelectPortraits[player].transform.GetChild(0).GetComponent<SpriteRenderer>().color = finalColor;
    }

    public void activatePlayer(int playerNum, int character)
    {
        if(active[playerNum] && character != players[playerNum].character)
        {
            deactivatePlayer(playerNum);
        }
        if (!active[playerNum])
        {
            //GameObject p;
            //if (character == 0)
            //{
            //    p = Instantiate(Keith);
            //}
            //else if (character == 1)
            //{
            //    p = Instantiate(Walt);
            //}
            //else if (character == -1)
            //{
            //    print("RANDOM CHARACTER");
            //    return;
            //}
            //else
            //{
            //    p = null;
            //    print("fuck");
            //}
            //GameObject h = Instantiate(HealthBar, Canvas.transform.Find("HealthUI").transform);
            //p.GetComponent<PlayerInput>().PlayerNumber = playerNum;
            //p.GetComponent<PlayerHealth>().img = h.transform.Find("BarFill").gameObject.GetComponent<Image>();
            //p.GetComponent<PlayerHealth>().shieldImg = h.transform.Find("BarShield").gameObject.GetComponent<Image>();
            //h.SetActive(false);
            //p.SetActive(false);
            //p.transform.position = new Vector3(999, -999, 0);
            //players[playerNum] = p;
            players[playerNum].character = character;
            active[playerNum] = true;
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
        //GameObject p = players[playerNum];
        //Destroy(p.GetComponent<PlayerHealth>().img.transform.parent.gameObject);
        active[playerNum] = false;
        CharSelectPortraits[playerNum].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        //Destroy(p);

        

        playerColor[] pColors;
        if (players[playerNum].character == 1)
        {
            pColors = waltColors;
        }
        else
        {
            pColors = keithColors;
        }
        for (int i = 0; i<6; i++)
        {
            if (pColors[i].owner == playerNum)
            {
                pColors[i].owner = -1;
            }
        }
        activeCount--;
    }

    public void goToMapSelect()
    {
        if (activeCount > 1)
        {
            DontDestroyOnLoad(Canvas);
            spawnPlayers();
            //Destroy(Canvas.transform.FindChild("TutorialText").gameObject);
            SceneManager.LoadScene(2); //UPDATE TO MAP SELECT SCREEN WHEN THAT EXISTS
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
                if (players[j].character == 0)
                {
                    p = Instantiate(Keith);
                    pColors = keithColors;
                }
                else if (players[j].character == 1)
                {
                    p = Instantiate(Walt);
                    pColors = waltColors;
                }
                else if (players[j].character == -1)
                {
                    print("RANDOM CHARACTER");
                    return;
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

                //h.SetActive(false);
                p.transform.position = new Vector3(999, -999, 0);
                DontDestroyOnLoad(p);
                p.GetComponent<ComboCounter>().reset();
                p.GetComponent<SpriteRenderer>().enabled = false;
                //reset(p);
                DontDestroyOnLoad(this);
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
                CharSelectPortraits[i].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                setColor(players[i].colorNum, i);
            }
        }

        
    }
}
