using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

public class VersusVictoryManager : MonoBehaviour {
    public GameObject[] PlayerBox;
    public StatTracker[] stats;
    private bool[] Ready;
	// Use this for initialization
	void Start () {
        Ready = new bool[4];
        stats = GameObject.Find("PlayerCreator").GetComponent<CreatePlayer>().getStats();
        Debug.Log(GameObject.Find("PlayerCreator").GetComponent<CreatePlayer>().getStats());
        Debug.Log(stats[0]);
        for (int i = 0; i < 4; i++)
        {
            Ready[i] = false;
            if (stats[i] != null)
            {
                populateStats();
            }
            else
            {
                PlayerBox[i].SetActive(false);
                Ready[i] = true;
            }
        }
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            GamePadState g = GamePad.GetState((PlayerIndex)i);
            if (g.Buttons.A == ButtonState.Pressed || g.Buttons.Start == ButtonState.Pressed)
            {
                if (!Ready[i])
                {
                    Ready[i] = true;
                    //visual indicator of readiness?
                }
            }
            if(g.Buttons.B == ButtonState.Pressed)
            {
                if (Ready[i])
                {
                    Ready[i] = false;
                    //visual indicator of unreadiness?
                }
            }


            //FOR TESTING PURPOSES ONLY
            if (g.DPad.Up == ButtonState.Pressed && i == 0)
            {
                Ready[1] = true;
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

    void populateStats()
    {

    }
}
