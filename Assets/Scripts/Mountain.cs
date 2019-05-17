using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountain : MonoBehaviour
{
    //player variables
    GameObject player;
    FirstPersonController fpc;
    WorldManager wm;

    void Start()
    {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();
        wm = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<WorldManager>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            fpc.spawnFootsteps = false;
            fpc.currentFootsteps = fpc.pathFootsteps;
            Camera.main.farClipPlane = 250;
            wm.activationDistance = 150f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            fpc.spawnFootsteps = true;
            fpc.currentFootsteps = fpc.forestFootsteps;
        }
    }
}
