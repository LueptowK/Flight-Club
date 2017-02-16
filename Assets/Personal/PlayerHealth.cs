using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public int maxHealth = 100;
    public int currentHealth;
    public Image img;
    

	// Use this for initialization
	void Awake () {
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	public void takeDamage(int damage)
    {
        print("damage taken");
        currentHealth -= damage;
    }

    void FixedUpdate()
    {
        print(currentHealth / 200f);
        img.fillAmount = currentHealth/200f;
        print(currentHealth);
    }
}
