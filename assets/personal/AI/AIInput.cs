using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour {
    public struct aiMove
    {

        public Vector2 move;
        public bool attack;
        public ControlInterpret.StickQuadrant quad;
        public bool dash;
        public bool dodge;
        public bool fall;
        public bool jump;
        public bool shoot;
        public bool stall;
        public aiMove(Vector2 movement)
        {
            move = movement.normalized;
            attack = false;
            quad = ControlInterpret.StickQuadrant.Neutral;
            dash = false;
            dodge = false;
            fall = false;
            jump = false;
            shoot = false;
            stall = false;
        }
    }
    [HideInInspector]
    public aiMove ctrl;

    public BehaviorTreeNode tree;
    public List<GameObject> points;
    GameObject lastPoint;
    public Vector2[] threat;

   
    float lastWait;
    public bool waiting
    {
        get
        {
            return Time.time<lastWait;
        }
    }
    public void wait(float w)
    {
        lastWait = Time.time+ w;
    }

    PlayerMover pm;
	// Use this for initialization
	void Start () {
        ctrl = new aiMove(Vector2.zero);
        pm = GetComponent<PlayerMover>();
        tree.ReEnable();
	}
	
	// Update is called once per frame
	void Update () {
        tree.Tick(this);
	}

    public void setCtrl(aiMove newCtrl)
    {
        ctrl = newCtrl;
    }

    public void moveHor(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        dir.y = 0;
        if (dir.magnitude < 0.5f)
        {
            dir = Vector2.zero;
        }
        else
        {

            dir.Normalize();
        }


        setCtrl(new AIInput.aiMove(dir));
    }
    public void move(Vector3 target)
    {
        setCtrl(new AIInput.aiMove(target - transform.position));
    }
    public GameObject pickPoint()
    {
        points.Remove(lastPoint);
        GameObject p = points[Mathf.FloorToInt(Random.value * points.Count)];
        if (lastPoint)
        {
            points.Add(lastPoint);
        }

        lastPoint = p;
        return p;
    }
    public void jump()
    {
        ctrl.jump = true;
    }
    public void shoot()
    {
        ctrl.shoot = true;
    }
    public void stall()
    {
        ctrl.stall = true;
    }
    public void FG(bool left)
    {
        if (pm.grounded)
        {
            ctrl.attack = true;
            ctrl.quad = left ? ControlInterpret.StickQuadrant.Left : ControlInterpret.StickQuadrant.Right;
        }
        
    }
}
