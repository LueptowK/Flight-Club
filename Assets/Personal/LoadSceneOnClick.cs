using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReturnToCharacterSelect()
    {
        Destroy(GameObject.Find("HealthUI").transform.parent.gameObject);
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in Players)
        {
            Destroy(player);
        }
        SceneManager.LoadScene(1);
    }
}
