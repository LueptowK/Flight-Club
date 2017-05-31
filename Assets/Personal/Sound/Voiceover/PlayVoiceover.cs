using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVoiceover : MonoBehaviour {

    public AudioClip soundToPlay;
    private AudioSource audio;

    // Use this for initialization
    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = soundToPlay;
    }

    void onTriggerEnter2D(Collider2D other)
    {
        audio.Play();
    }
}
