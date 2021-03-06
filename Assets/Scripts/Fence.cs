﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    //player variables
    GameObject player;

    AudioSource fenceSource;
    public AudioClip[] fenceSounds;

    void Start()
    {
        fenceSource = GetComponent<AudioSource>();
        
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (fenceSource.isPlaying == false)
            {
                int randomSound = Random.Range(0, fenceSounds.Length);
                float randomPitch = Random.Range(0.85f, 1.15f);
                fenceSource.pitch = randomPitch;
                fenceSource.PlayOneShot(fenceSounds[randomSound]);
            }
        }
    }
}
