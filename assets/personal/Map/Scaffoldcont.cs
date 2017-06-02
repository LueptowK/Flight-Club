using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaffoldcont : MonoBehaviour {
    public GameObject[] Scaffs;

    public int swapCD;
    
    int currentCD;
	// Use this for initialization
	void Start () {
		
	}
    public void end()
    {
        swap(true);
    }

    // Update is called once per frame
    void FixedUpdate () {
        currentCD--;
        if (currentCD == 0)
        {
            swap(true);
        }
        else if (currentCD == -1)
        {
            swap(false);
            currentCD = swapCD;
        }
	}
    void swap(bool pause)
    {
        if (pause)
        {
            foreach (GameObject s in Scaffs)
            {
                PlatformScaffold sc = s.GetComponent<PlatformScaffold>();
                sc.pause();
                sc.changeSpeed(-sc.speed);
                sc.reverse();

            }
        }
        else
        {
            foreach (GameObject s in Scaffs)
            {
                PlatformScaffold sc = s.GetComponent<PlatformScaffold>();
                sc.unpause();


            }
        }
    }
}
