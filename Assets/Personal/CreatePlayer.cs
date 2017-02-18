using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

public class CreatePlayer : MonoBehaviour {
    public GameObject Canvas;
    public GameObject Keith;
    public GameObject HealthBar;
    private GamePadState[] inputs;
    private bool[] active;
    playerColor[] pColors;
    int[] inputCD;
    GameObject[] players;
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
        pColors[1] = new playerColor(Color.red);
        pColors[2] = new playerColor(Color.blue);
        pColors[3] = new playerColor(Color.green);
        pColors[4] = new playerColor(Color.magenta);
        pColors[5] = new playerColor(Color.yellow);


        inputs = new GamePadState[4];
        active = new bool[4];
        players = new GameObject[4];
        inputCD = new int[4];
        for(int i =0; i< inputCD.Length; i++)
        {
            inputCD[i] = 0;
        }
    }

    void FixedUpdate()
    {
        for(int z = 0; z < inputCD.Length; z++)
        {
            if (inputCD[z] > 0)
            {
                inputCD[z]--;
            }
        }
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = GamePad.GetState((PlayerIndex)i);
            if (!active[i])
            {
                if(inputs[i].Buttons.A == ButtonState.Pressed)
                {
                    GameObject p = Instantiate(Keith);
                    GameObject h = Instantiate(HealthBar, Canvas.transform.Find("HealthUI").transform);
                    p.GetComponent<PlayerInput>().PlayerNumber = i;
                    p.GetComponent<PlayerHealth>().img = h.transform.Find("BarFill").gameObject.GetComponent<Image>();
                    //h.SetActive(false);
                    active[i] = true;
                    players[i] = p;
                    changeColor(i, false);


                }

            }
            else if(inputs[i].Buttons.Y== ButtonState.Pressed&& inputCD[i]==0)
            {
                changeColor(i, true);
                inputCD[i] = 30;
            }
            

        }
        int activeCount = 0;
        for (int i = 0; i < active.Length; i++)
        {
            if (active[i])
            {
                activeCount++;
            }
        }
        if (inputs[0].Buttons.Start == ButtonState.Pressed && activeCount > 1)
        {
            DontDestroyOnLoad(Canvas);
            for(int i = 0; i < players.Length; i++)
            {
                if (active[i])
                {
                    DontDestroyOnLoad(players[i]);
                    //reset(players[i]);
                }
            }
            SceneManager.LoadScene(2); //UPDATE TO MAP SELECT SCREEN WHEN THAT EXISTS
        }
    }
    void changeColor(int player, bool inArray)
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
            for(int a =1; a<pColors.Length+1; a++)
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
    void setColor(int i, int player) {
        pColors[i].owner = player;
        players[player].GetComponent<SpriteRenderer>().color = pColors[i].color;
        players[player].GetComponent<PlayerHealth>().img.transform.parent.Find("BarIdentifier").GetComponent<Image>().color = pColors[i].color;
    }
}
