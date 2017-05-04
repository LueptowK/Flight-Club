using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private GameObject[] players;
    private int numPlayers;
    public float PanSmoothing = .9f;
    public float zoomSmoothing = 0.04f;
    public float Bottom = 6f;
    public float screenShake;
    public float minScreenShake = 2f;
    public float shakeSlowRate = 0.95f;
    public float damageToShakeRatio = 0.045f;

    public float minCamZoom = 5;
    public float maxCamSoftZoom = 8f;
    public float camDivider = 3.8f;

    Vector3 actualPosition;
    Camera cam;
    float aspect;
    void Start()
    {
        actualPosition = transform.position;
        cam = GetComponent<Camera>();
        aspect = cam.aspect;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        numPlayers = players.Length;
        MoveCamera();
        
        transform.position = actualPosition + new Vector3(Random.Range(-screenShake*damageToShakeRatio, screenShake*damageToShakeRatio), Random.Range(-screenShake*damageToShakeRatio, screenShake*damageToShakeRatio), 0);
        if(screenShake < minScreenShake)
        {
            screenShake = 0;
        }
        else
        {
            screenShake = screenShake * shakeSlowRate;
        }
    }

    void MoveCamera()
    {
        Vector3 middle = Vector3.zero;
        float minX = players[0].transform.position.x;
        float fallSpd = players[0].GetComponent<Rigidbody2D>().velocity.y;
        fallSpd *= 0.15f;
        fallSpd = Mathf.Min(fallSpd, -1);
        float minY = players[0].transform.position.y+fallSpd;
        
        float maxX = minX;
        float maxY = minY;
        for (int i = 1; i < numPlayers; i++)
        {
            fallSpd = players[0].GetComponent<Rigidbody2D>().velocity.y;
            fallSpd *= 0.1f;
            fallSpd = Mathf.Min(fallSpd, -1);

            minX = Mathf.Min(players[i].transform.position.x, minX);
            maxX = Mathf.Max(players[i].transform.position.x, maxX);
            minY = Mathf.Min(players[i].transform.position.y+fallSpd, minY);
            maxY = Mathf.Max(players[i].transform.position.y, maxY);
        }
        
        middle = new Vector3((maxX + minX)/2, (maxY + minY)/2, 0);

        float wid = maxX - minX;
        float high = maxY - minY;
        float sizeV = high * aspect* 1.0f;
        float sizeH = wid;
        
        float size = Mathf.Max(sizeH, sizeV);

        // make it a little bigger than needed
        size /= camDivider; //ratio for zoom
        if(size < minCamZoom)
        {
            size = minCamZoom;
        }

        
        Vector3 desiredCenter= new Vector3(middle.x, middle.y + Bottom, this.transform.position.z);
        actualPosition = Vector3.Lerp(desiredCenter, actualPosition, PanSmoothing);
        if (size > cam.orthographicSize)
        {
            cam.orthographicSize = size;
        }
        else
        {
            size *= 1.2f;
            if(size> maxCamSoftZoom)
            {
                size = maxCamSoftZoom + (size - maxCamSoftZoom) * 0.35f;
            }
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, zoomSmoothing);
        }
        
    }
}