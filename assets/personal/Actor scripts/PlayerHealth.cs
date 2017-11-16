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
    public int segments = 2;
    int currentSegment;

    public float outOfCombat = -1;
    float lastHit =0;

    int width = 270;
    int offset = 50;

    PlayerMover pm;
	// Use this for initialization
	new void Awake()
    {
        base.Awake();
        currentShield = maxShield;
        //pm = (PlayerMover)mover;
        pm = (PlayerMover)mover;
        currentSegment = 1;
        
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
        else if(outOfCombat!=-1&&currentShield< maxShield && Time.time>lastHit+outOfCombat){
            currentShield += recharge / 60;
            if (currentShield >= maxShield)
            {
                currentShield = maxShield;
               
            }
            setShield();
        }
    }
    public void charge(int charge) //previously a percentage
    {

        if (recharging && maxShield > 0)
        {

            currentShield += charge;
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
        lastHit = Time.time;
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
            if ((currentHealth <= (maxHealth / segments)*(segments - currentSegment))&&segments!=currentSegment)
            {
                currentHealth = maxHealth / 2;
                currentShield = maxShield;
                recharging = false;
                //TRANSITION -- FALSE
                shieldImg.transform.SetAsLastSibling();
                pm.segment();
                setShield();
                currentSegment++;
                img.fillAmount = (float)currentHealth / maxHealth;
                return 2;
            }
            img.fillAmount = (float)currentHealth / maxHealth;
            return 0;
        }
        
        
    }

    public bool drainShield(int drain)
    {
        if (currentShield >= drain)
        {
            currentShield -= drain;
            setShield();
            return true;
        }
        else
        {
            return false;
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
