using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticGhost : MonoBehaviour
{
    //player variables
    GameObject player;
    Camera playerCam;
    FirstPersonController fpc;

    Animator ghostAnimator;
    FadeSprite fadeScript;

    public bool following;
    public Transform pointToFollow;
    public float followSpeed;
    public float followDist, fadedDist;

    AudioSource ghostSource;
    public AudioClip[] spookSounds;
    float dist;

    void Start()
    {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();
        playerCam = Camera.main;
        fadeScript = GetComponent<FadeSprite>();
        ghostAnimator = GetComponent<Animator>();
        ghostSource = GetComponent<AudioSource>();
    }

    //when player reaches the ending, shift animator to end state
    //this will flash sprites between black and white and have them orbit player
    //as the transmission ends, portal opens at the top of the radio tower
    // you lerp into it and game is over
    
    void Update()
    {
        dist = Vector3.Distance(transform.position, player.transform.position);

        //assign following if player approaches
        if (dist < followDist && !following)
        {
            //can follow player
            if(fpc.followSpots.Count > 0)
                AssignFollowSpot();

            //walk towards the north
            else
            {
                fadeScript.fadingOut = true;
            }
        }

        if (following)
        {
            //look at player
            transform.LookAt(playerCam.transform.position, Vector3.up);
            //always half the speed of the player
            followSpeed = fpc.currentSpeed / 2;

            Vector3 point = new Vector3(pointToFollow.position.x, fpc.transform.position.y + 2f, pointToFollow.position.z);
            //move after player
            transform.position = Vector3.MoveTowards(transform.position, point, followSpeed * Time.deltaTime);
        }
        
    }

    //get assigned random follow spot from player
    void AssignFollowSpot()
    {
        int randomFollowSpot = Random.Range(0, fpc.followSpots.Count);

        pointToFollow = fpc.followSpots[randomFollowSpot];

        fpc.followSpots.Remove(fpc.followSpots[randomFollowSpot]);

        following = true;
    }

    //called when the Renderer comp becomes visible to any camera
    void OnBecameInvisible()
    {
        fadeScript.fadingIn = true;
        fadeScript.fadingOut = false;

        //stop animator
        if (ghostAnimator.enabled)
            ghostAnimator.enabled = false;
    }


    IEnumerator StartAnimator()
    {
        float randomWait = Random.Range(0.1f, 0.5f);
        yield return new WaitForSeconds(randomWait);
        ghostAnimator.enabled = true;
    }


    //turn on animator
    void OnBecameVisible()
    {
        fadeScript.fadingIn = false;
        fadeScript.fadingOut = true;
        
        if(dist < followDist)
        {
            PlaySpookSound();
        }

        if (!ghostAnimator.enabled)
        {
            StartCoroutine(StartAnimator());
        }
    }

    void PlaySpookSound()
    {
        int randomSpook = Random.Range(0, spookSounds.Length);
        if(ghostSource.isPlaying == false)
        {
            ghostSource.PlayOneShot(spookSounds[randomSpook]);
        }
    }
}
