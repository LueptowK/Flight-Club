using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointerTutorial : MonoBehaviour {

    public int checkNum;

	void OnTriggerEnter2D(Collider2D col)
    {
        GameObject.Find("Main Camera").GetComponent<TutorialManager>().checkpoint(checkNum, this.transform.position);
    }
}
