using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFinishLine : MonoBehaviour {
    public StoryLevelManager manager;

    void Start()
    {
        manager = GameObject.Find("Main Camera").GetComponent<StoryLevelManager>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        manager.finish();
    }
}
