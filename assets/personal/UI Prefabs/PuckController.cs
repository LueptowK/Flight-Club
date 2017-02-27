using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PuckController : MonoBehaviour {
    Rigidbody2D rb;

    public MenuItemAbst selected;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector2 move = new Vector2();
        rb.velocity = Vector2.zero;
        for(int i =0; i < 4; i++)
        {
            GamePadState g = GamePad.GetState((PlayerIndex)i);
            move += StickFixer.fixStick(new Vector2(g.ThumbSticks.Left.X, g.ThumbSticks.Left.Y), 0.15f);

            if (g.Buttons.A == ButtonState.Pressed && selected)
            {
                selected.click();
            }
        }
        rb.velocity += move*10f;

	}
    void OnTriggerEnter2D(Collider2D col)
    {
        if (selected)
        {
            selected.deselect();
        }
        selected = col.GetComponent<MenuItemAbst>();
        selected.select();
    }
}
