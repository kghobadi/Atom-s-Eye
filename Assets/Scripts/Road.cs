using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    //player variables
    GameObject player;
    FirstPersonController fpc;

    void Start()
    {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            fpc.spawnFootsteps = false;
            fpc.currentFootsteps = fpc.indoorFootsteps;
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
