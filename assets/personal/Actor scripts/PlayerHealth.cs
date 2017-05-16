using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health {
    
    public Image img;
    
    public int maxShield = 0;
    public float currentShield;
    public Image shieldImg;
    public float recharge;
    bool recharging;

    int width = 270;
    int offset = 50;

    PlayerMover pm;
	// Use this for initialization
	new void Awake()
    {
        currentShield = maxShield;
        pm = (PlayerMover)mover;
        base.Awake();
    }
    void FixedUpdate()
    {
        if (recharging && maxShield > 0)
        {

            currentShield += recharge/60;
            if(currentShield>= maxShield)
            {
                currentShield = maxShield;
                recharging = false;
                //TRANSITION -- FALSE
                shieldImg.transform.SetAsLastSibling();
                pm.phaseUp();
            }
            setShield();
        }
    }
    public void charge(float percent)
    {

        if (recharging && maxShield > 0)
        {

            currentShield += maxShield*percent;
            if (currentShield >= maxShield)
            {
                currentShield = maxShield;
                recharging = false;
                //TRANSITION -- FALSE
                shieldImg.transform.SetAsLastSibling();
                pm.phaseUp();
            }
            setShield();
        }
    }
	
	// Update is called once per frame
	 public override int takeDamage(int damage)
    {
        if (!recharging && maxShield > 0)
        {
            currentShield -= damage;
            if (currentShield <= 0)
            {
                recharging = true;
                currentHealth += (int)currentShield;
                img.fillAmount = (float)currentHealth / maxHealth;
                currentShield = 0;
                //TRANSITION -- TRUE
                shieldImg.transform.SetAsFirstSibling();
                setShield();
                pm.phaseDown();
                return 1;
            }
            else
            {
                setShield();
                return 0;
            }
            
        }
        else
        {
            currentHealth -= damage;
            img.fillAmount = (float)currentHealth / maxHealth;
            return 0;
        }

        
    }
    new public void reset()
    {
        base.reset();
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
