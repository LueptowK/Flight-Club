using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSounds : MonoBehaviour
{
    public AudioClip[] getHitSounds;
    public AudioClip[] wallBounceSounds;
    private AudioSource source;
    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void hitWall()
    {
        source.PlayOneShot(wallBounceSounds[UnityEngine.Random.Range(0, wallBounceSounds.Length)], 0.3f);
    }

    public void getHit(int damage)
    {
        source.PlayOneShot(getHitSounds[UnityEngine.Random.Range(0, getHitSounds.Length)], (.2f + (damage / 60f)));
    }
}
