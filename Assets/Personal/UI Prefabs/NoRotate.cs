using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotate : MonoBehaviour
{
    Quaternion rotation;

    void Awake()
    {
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.transform.position + new Vector3(0, 1.5f, 0);
        transform.rotation = rotation;
        if (gameObject.transform.parent.transform.localScale.y < 0)
        {
            transform.localScale = new Vector3(System.Math.Abs(transform.localScale.x), -System.Math.Abs(transform.localScale.y), transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(System.Math.Abs(transform.localScale.x), System.Math.Abs(transform.localScale.y), transform.localScale.z);
        }
    }
}