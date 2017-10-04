using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PerPlayerPuckController : MonoBehaviour
{
    Rigidbody2D rb;
    CreatePlayer creator;
    public int playerNum;
    public GameObject selected;
    public GameObject nameBox;
    public GameObject keyboard;
    public GameObject keyboardUI;
    public GameObject nameString;
    private string name = "";
    private string lastKey = "";
    int keyPresses = 0;
    int aPressCooldown;
    int yPressCooldown;
    int bPressCooldown;
    bool characterSelected;
    bool keyboardActive = false;
    Text nameTag;
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
        nameTag = nameString.GetComponent<Text>();
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

        if (selected && selected.transform.parent != null)
        {
            if (keyboardActive && selected.transform.parent.gameObject == keyboard && g.Buttons.A == ButtonState.Pressed)
            {
                if (selected.name == "DEL" && nameTag.text.Length > 0 && aPressCooldown < 15)
                {
                    nameTag.text = nameTag.text.Substring(0, (nameTag.text.Length - 1));
                    aPressCooldown = 30;
                }
                else if (selected.name == "space")
                {
                    nameTag.text += " ";
                }
                else if (selected.name == "123")
                {
                    keyboardHelper("1234567890");
                }
                else if (selected.name == "specialChars")
                {
                    keyboardHelper("!?-.()$");
                }
                else if (selected.name == "accept")
                {
                    if(nameTag.text != "")
                    {
                        name = nameTag.text;
                    }
                    else
                    {
                        nameTag.text = "Player " + (playerNum + 1).ToString();
                        name = "";
                    }
                    setKeyboardActive(false);
                }
                else if (selected.name == "cancel")
                {
                    nameTag.text = "Player " + (playerNum+1).ToString();
                    name = "";
                    setKeyboardActive(false);
                }
                else
                {
                    keyboardHelper(selected.name);
                }
                aPressCooldown = 30;
            }
        }

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
                if(selected == nameBox)
                {
                    setKeyboardActive(true);
                    nameTag.text = name;
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

    void keyboardHelper(string keys)
    {
        if (aPressCooldown >= 5 && aPressCooldown <= 25 && lastKey == keys)
        {
            nameTag.text = nameTag.text.Substring(0, (nameTag.text.Length - 1));
            if (keyPresses >= keys.Length)
            {
                nameTag.text += keys[0];
                keyPresses = 0;
            }
            else
            {
                nameTag.text += keys[keyPresses];
            }
            keyPresses++;
        }
        else if (nameTag.text.Length < 6 && aPressCooldown < 5)
        {
            nameTag.text += keys[0];
            keyPresses = 1;
            lastKey = keys;
        }
    }

    public void setKeyboardActive(bool active)
    {
        keyboard.SetActive(active);
        keyboardUI.SetActive(active);
        keyboardActive = active;
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
