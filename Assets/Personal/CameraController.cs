using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private GameObject[] players;
    private int numPlayers;
    public float PanSmoothing = .9f;
    public float Bottom = 5f;
    public float screenShake;
    public float minScreenShake = 2f;
    public float shakeSlowRate = 0.95f;
    Vector3 actualPosition;
    void Start()
    {
        actualPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        numPlayers = players.Length;
        Vector3 desiredCenter = FindCenter();
        actualPosition = Vector3.Lerp(desiredCenter, actualPosition, PanSmoothing);
        transform.position = actualPosition + new Vector3(Random.Range(0,screenShake/10), Random.Range(0,screenShake/10), 0);
        if(screenShake < minScreenShake)
        {
            screenShake = 0;
        }
        else
        {
            screenShake = screenShake * shakeSlowRate;
        }
    }

    Vector3 FindCenter()
    {
        Vector3 middle = Vector3.zero;
        float minX = players[0].transform.position.x;
        float minY = players[0].transform.position.y;
        float maxX = minX;
        float maxY = minY;
        for (int i = 1; i < numPlayers; i++)
        {
            minX = Mathf.Min(players[i].transform.position.x, minX);
            maxX = Mathf.Max(players[i].transform.position.x, maxX);
            minY = Mathf.Min(players[i].transform.position.y, minY);
            maxY = Mathf.Max(players[i].transform.position.y, maxY);
        }
        for (int i = 0; i < numPlayers; i++)
        {
            middle = new Vector3((maxX + minX)/2, (maxY + minY)/2, 0);
        }

        return new Vector3(middle.x, middle.y + Bottom, this.transform.position.z);
    }
}