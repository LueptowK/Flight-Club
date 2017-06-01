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

    public float minCamZoom = 6;
    public float maxCamSoftZoom = 8f;
    public float SoftZoomMult = 0.35f;
    public float camDivider = 3.8f;

    public bool lookDown = false;
    public GameObject camBaseObj;

    Vector3 actualPosition;
    Camera cam;
    float aspect;
    Transform camBase;
    Transform camTop;
    void Start()
    {
        actualPosition = transform.position;
        cam = GetComponent<Camera>();
        aspect = cam.aspect;

        if (camBaseObj)
        {

            camBase = camBaseObj.transform;
        }
    }
    public void setCamBase(GameObject cbo)
    {
        camBase = cbo?cbo.transform:null;
    }
    public void setCamTop(GameObject cbo)
    {
        camTop = cbo?cbo.transform:null;
    }
    struct box
    {
        public float minX, maxX, minY, maxY;
        

        

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

        box b = getCamMod(players[0]);
        float minX = players[0].transform.position.x+b.minX;
        float minY = players[0].transform.position.y+b.minY;
        
        float maxX = players[0].transform.position.x + b.maxX;
        float maxY = players[0].transform.position.y + b.maxY;
        for (int i = 1; i < numPlayers; i++)
        {
            b = getCamMod(players[i]);


            minX = Mathf.Min(players[i].transform.position.x+b.minX, minX);
            maxX = Mathf.Max(players[i].transform.position.x+b.maxX, maxX);
            minY = Mathf.Min(players[i].transform.position.y+b.minY, minY);
            maxY = Mathf.Max(players[i].transform.position.y+b.minY, maxY);
        }

        if (camBase)
        {
            //minY = Mathf.Min(minY, camBase.transform.position.y);
            minY = camBase.transform.position.y;
        }
        if (camTop)
        {
            maxY = Mathf.Max(maxY, camTop.transform.position.y);
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
                size = maxCamSoftZoom + (size - maxCamSoftZoom) * SoftZoomMult;
            }
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, zoomSmoothing);
        }
        
    }

    box getCamMod(GameObject player)
    {

        Vector2 vel = player.GetComponent<Rigidbody2D>().velocity;
        Vector2 inp = player.GetComponent<Interpreter>().move;
        box mod = new box();
        float min = 0.0f;

        vel *= 0.10f;
        inp *= 10;

        //mod.maxY = Mathf.Max(vel.y, min);
        //mod.minX = Mathf.Min(vel.x, -min);
        //mod.maxX = Mathf.Max(vel.x, min);

        //mod.minY = Mathf.Min(vel.y, -min);
        //mod.maxY = 0;
        //mod.minX = 0;
        //mod.maxX = 0;

        //mod.minY = Mathf.Min(inp.y, -min, vel.y);
        //mod.maxY = Mathf.Max(inp.y, min);
        //mod.minX = Mathf.Min(inp.x, -min);
        //mod.maxX = Mathf.Max(inp.x, min);

        
        if (lookDown)
        {
            mod.minY = Mathf.Min(inp.y, -min, vel.y);
        }
        else
        {
            mod.minY = Mathf.Min(vel.y, -min);
        }
        
        mod.maxY = 0;
        mod.minX = 0;
        mod.maxX = 0;




        return mod;
    }
}