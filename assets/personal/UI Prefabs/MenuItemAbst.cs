using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class MenuItemAbst : MonoBehaviour {
    // Use this for initialization
    public abstract void click();
    SpriteRenderer sp;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    public void select()
    {
        sp.color = new Color(0.3f, 0.3f, 0.3f);
    }
    public void deselect()
    {
        sp.color = new Color(1f, 1f, 1f);
    }
}
