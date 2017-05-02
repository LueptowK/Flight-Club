using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public int maxHealth = 100;
    public int currentHealth;
    public Image img;

    public int maxShield = 0;
    public int currentShield;

    

	// Use this for initialization
	void Awake () {
        currentHealth = maxHealth;
        currentShield = maxShield;
	}
	
	// Update is called once per frame
	public void takeDamage(int damage)
    {
        currentHealth -= damage;
        img.fillAmount = (float)currentHealth / maxHealth;
    }
    public void reset()
    {
        currentHealth = maxHealth;
        img.fillAmount = (float)currentHealth / maxHealth;
    }
}
