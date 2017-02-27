using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapItem : MenuItemAbst {
    public int nextScene;
	// Use this for initialization
	public override void click()
    {
        SceneManager.LoadScene(nextScene);
    }
}
