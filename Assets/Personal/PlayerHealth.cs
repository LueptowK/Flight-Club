using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public int maxHealth = 200;
    public int currentHealth;
    public Slider slider;
    

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
        slider.value = currentHealth;
        print(currentHealth);
    }
}
