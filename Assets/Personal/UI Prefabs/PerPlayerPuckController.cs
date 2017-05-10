using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PerPlayerPuckController : MonoBehaviour
{
    Rigidbody2D rb;
    CreatePlayer creator;
    public int playerNum;
    public GameObject selected;
    int aPressCooldown;
    int yPressCooldown;
    int bPressCooldown;
    bool characterSelected;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        selected = null;
        aPressCooldown = 0;
        yPressCooldown = 0;
        bPressCooldown = 0;
        creator = GameObject.Find("PlayerCreator").GetComponent<CreatePlayer>();
        characterSelected = creator.active[playerNum];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (aPressCooldown > 0)
        {
            aPressCooldown--;
        }
        if (yPressCooldown > 0)
        {
            yPressCooldown--;
        }
        if(bPressCooldown > 0)
        {
            bPressCooldown--;
        }
        Vector2 move = new Vector2();
        rb.velocity = Vector2.zero;
        GamePadState g = GamePad.GetState((PlayerIndex)playerNum);
        move += StickFixer.fixStick(new Vector2(g.ThumbSticks.Left.X, g.ThumbSticks.Left.Y), 0.15f);

        if (g.Buttons.A == ButtonState.Pressed && selected && aPressCooldown == 0)
        {
            MapItem MenuButton = selected.GetComponent<MapItem>();
            if (MenuButton != null)
            {
                MenuButton.click();
            }
            else
            {
                aPressCooldown = 30;
                if(selected.name == "CharSelectSquareKeith")
                {
                    creator.activatePlayer(playerNum, 0);
                    characterSelected = true;
                }
                if(selected.name == "CharSelectSquareWalt")
                {
                    creator.activatePlayer(playerNum, 1);
                    characterSelected = true;
                }
                if(selected.name == "CharSelectSquareRandom")
                {
                    creator.activatePlayer(playerNum, -1);
                    characterSelected = true;
                }
            }

        }
        if (g.Buttons.Y == ButtonState.Pressed && characterSelected && yPressCooldown == 0)
        {
            creator.changeColor(playerNum, true);
            yPressCooldown = 30;
        }
        if (g.Buttons.Start == ButtonState.Pressed)
        {
            creator.goToMapSelect();
        }
        rb.velocity += move * 10f;

        if(playerNum == 0 && g.DPad.Up == ButtonState.Pressed)
        {
            creator.createDummy();
        }

        if(g.Buttons.B == ButtonState.Pressed && characterSelected && bPressCooldown == 0)
        {
            creator.deactivatePlayer(playerNum);
            characterSelected = false;
            bPressCooldown = 30;
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (selected != null)
        {
            MapItem MenuButton = selected.GetComponent<MapItem>();
            if (MenuButton != null)
            {
                MenuButton.deselect();
            }
        }
        selected = col.gameObject;
        if (selected.GetComponent<MapItem>() != null)
        {
            selected.GetComponent<MapItem>().select();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == selected)
        {
            selected = null;
            if (col.gameObject.GetComponent<MapItem>() != null)
            {
                col.gameObject.GetComponent<MapItem>().deselect();
            }

        }

    }
}
