using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerScreen : MonoBehaviour
{ 
    //player variables
    GameObject player;
    FirstPersonController fpc;
    Camera playerCam;

    Animator screenAnimator;

    Vector3 origRot;

    void Start()
    { 
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();
        playerCam = Camera.main;

        screenAnimator = GetComponent<Animator>();
        origRot = transform.localEulerAngles;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        //look at player anim and rot
        if (dist < 10)
        {
            //look at player
            transform.LookAt(playerCam.transform.position, Vector3.up);
            screenAnimator.SetBool("open", true);

        }
        //close eye
        else
        {
            transform.localEulerAngles = origRot;
            screenAnimator.SetBool("open", false);
        }
    }
}
