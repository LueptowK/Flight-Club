using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class DetectPlayer : MonoBehaviour
{

    bool[] activePlayers;
    GameObject[] pucks;
    public GameObject puck1;
    public GameObject puck2;
    public GameObject puck3;
    public GameObject puck4;

    void Start()
    {
        activePlayers = new bool[4];
        pucks = new GameObject[4];
        pucks[0] = puck1;
        pucks[1] = puck2;
        pucks[2] = puck3;
        pucks[3] = puck4;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 move = new Vector2();

        for (int i = 0; i < 4; i++)
        {
            GamePadState g = GamePad.GetState((PlayerIndex)i);
            move = StickFixer.fixStick(new Vector2(g.ThumbSticks.Left.X, g.ThumbSticks.Left.Y), 0.15f);
            if ((g.Buttons.A == ButtonState.Pressed || move != Vector2.zero) && !activePlayers[i])
            {
                pucks[i].GetComponent<SpriteRenderer>().enabled = true;
                activePlayers[i] = true;
            }
        }
    }
}