using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public int maxHealth = 100;
    public int currentHealth;
    public Image img;
    PlayerMover mover;
    public int maxShield = 0;
    public float currentShield;
    public Image shieldImg;
    public float recharge;
    bool recharging;

    int width = 270;
    int offset = 50;

	// Use this for initialization
	void Awake () {
        currentHealth = maxHealth;
        currentShield = maxShield;
        mover = GetComponent<PlayerMover>();
	}
    void FixedUpdate()
    {
        if (recharging)
        {

            currentShield += recharge/60;
            if(currentShield>= maxShield)
            {
                currentShield = maxShield;
                recharging = false;
                //TRANSITION -- FALSE
                shieldImg.transform.SetAsLastSibling();
                mover.phaseUp();
            }
            setShield();
        }
    }
	
	// Update is called once per frame
	public void takeDamage(int damage)
    {
        if (!recharging)
        {
            currentShield -= damage;
            if (currentShield <= 0)
            {
                recharging = true;
                currentShield = 0;
                //TRANSITION -- TRUE
                shieldImg.transform.SetAsFirstSibling();
                setShield();
                mover.phaseDown();
            }
            else
            {
                setShield();
            }
            
        }
        else
        {
            currentHealth -= damage;
            img.fillAmount = (float)currentHealth / maxHealth;
        }

        
    }
    public void reset()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        img.fillAmount = (float)currentHealth / maxHealth;
        shieldImg.fillAmount = (float)currentShield / maxShield;
    }
    void setShield()
    {
        shieldImg.rectTransform.sizeDelta= new Vector2(currentShield / maxShield *width, 30);
        //shieldImg.GetComponent<RectTransform>().anchoredPosition = new Vector3(-(currentShield / maxShield) * width / 2, 0);
        shieldImg.transform.localPosition = new Vector3((currentShield / maxShield) * width / 2 -85, 0);
    }
}
