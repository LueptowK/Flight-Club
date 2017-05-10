using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapItem : MenuItemAbst {
    public int nextScene;
	// Use this for initialization
	public override void click()
    {
        if (nextScene == -1)
        {
            Application.Quit();
        }

        if(nextScene == -2)
        {
            MapItem[] maps = FindObjectsOfType<MapItem>();
            int i = Random.Range(0, maps.Length);
            while (maps[i].nextScene == -2 || maps[i].nextScene == 1)
            {
                i = Random.Range(0, maps.Length);
            }
            SceneManager.LoadScene(maps[i].nextScene);
        }

        SceneManager.LoadScene(nextScene);
    }
}
