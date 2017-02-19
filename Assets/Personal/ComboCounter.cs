using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCounter : MonoBehaviour {
    public int maxCombo = 6;
    public int currentCombo;
    public int comboCountdown;
    public int comboDegradeTime = 60;
    public int comboDegradeDelay = 120;
    public GameObject OrbitalPrefab;
    GameObject[] Orbitals;
	// Use this for initialization
	void Awake () {
        currentCombo = 0;
        comboCountdown = -1;
        Orbitals = new GameObject[maxCombo];
        for (int i = 0; i < maxCombo; i++)
        {
            Orbitals[i] = Instantiate(OrbitalPrefab);
            Orbitals[i].SetActive(false);
            Orbitals[i].transform.SetParent(gameObject.transform);
        }
        InitializeOrbitals();
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        
        if (comboCountdown > 0)
        {
            comboCountdown--;
        }
        else
        {
            incrementCombo(-1);
        }
    }

	public void incrementCombo(int increment)
    {
        if ((currentCombo == 0 && increment < 0) || (currentCombo == maxCombo && increment > 0))
        {
            return;
        }
        currentCombo += increment;
        setOrbitals();
        if (increment > 0)
        {
            comboCountdown = comboDegradeDelay + comboDegradeTime;
        }
        else
        {
            if (currentCombo > 0)
            {
                comboCountdown = comboDegradeTime;
            }
            else
            {
                comboCountdown = -1;
            }
        }
        if (currentCombo == maxCombo)
        {
            for (int i = 0; i < maxCombo; i++)
            {
                Orbitals[i].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 255);
            }
        }
        if (currentCombo < maxCombo)
        {
            for (int i = 0; i < maxCombo; i++)
            {
                Orbitals[i].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            }
        }
    }


    //this kinda sucks b/c it needs to be hardcoded, but I couldn't think of a good way around it
    void InitializeOrbitals()
    {
        Orbitals[1].GetComponent<Orbital>().offset = 1f;
        Orbitals[2].GetComponent<Orbital>().offset = 0.75f;
        Orbitals[2].transform.rotation = Quaternion.Euler(0, 0, 45);
        Orbitals[3].GetComponent<Orbital>().offset = 1.5f;
        Orbitals[3].transform.rotation = Quaternion.Euler(0, 0, -45);
        Orbitals[4].GetComponent<Orbital>().offset = 1.25f;
        Orbitals[4].transform.rotation = Quaternion.Euler(0, 0, 90);
        Orbitals[5].GetComponent<Orbital>().offset = 0.25f;
        Orbitals[5].transform.rotation = Quaternion.Euler(0, 0, 45);
    }

    void setOrbitals()
    {
        for (int i = 0; i < currentCombo; i++)
        {
            Orbitals[i].SetActive(true);
        }
        for (int i = currentCombo; i < maxCombo; i++)
        {
            Orbitals[i].SetActive(false);
        }
    }
    public void reset()
    {
        currentCombo = 0;
        setOrbitals();
    }
}
