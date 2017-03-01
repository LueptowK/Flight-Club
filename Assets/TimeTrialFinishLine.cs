using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialFinishLine : MonoBehaviour {
    public TimeTrialManager manager;

    void Start()
    {
        manager = GameObject.Find("Main Camera").GetComponent<TimeTrialManager>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        manager.finish();
    }
}
