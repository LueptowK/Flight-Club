using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class CreatePlayer : MonoBehaviour {
    public GameObject Keith;
    private PlayerInput[] inputs;
    private bool[] active;

    void Awake()
    {
        inputs = GetComponents<PlayerInput>();
        active = new bool[3];
    }

    void FixedUpdate()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i].AttackD && !active[i])
            {
                Instantiate(Keith);
                Keith.GetComponent<PlayerInput>().PlayerNumber = i;
                active[i] = true;
            }
        }
    }
}
