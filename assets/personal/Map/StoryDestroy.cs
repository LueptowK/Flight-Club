using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDestroy : MonoBehaviour {
    public StoryLevelManager manager;

    void Start()
    {
        manager = GameObject.Find("Main Camera").GetComponent<StoryLevelManager>();
    }

    public void finish()
    {
        manager.finish();
    }
}
