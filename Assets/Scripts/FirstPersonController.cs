using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    //timers and values for speed
    public float currentSpeed, walkSpeed, sprintSpeed;
    public float scrollSpeed = 2.0f;
    float sprintTimer = 0;
    public float sprintTimerMax = 1;

    float footStepTimer = 0;
    public float footStepTimerTotal = 0.5f;

    CharacterController player;
    GroundCamera mouseLook;
    Vector3 movement;

    //for footstep sounds
    public AudioClip[] currentFootsteps, indoorFootsteps, forestFootsteps, pathFootsteps;
    AudioSource playerAudSource;

    //for screamer
    public AudioSource[] voices;
    public int currentVoice;
    public AudioClip[] myGibberishSounds;

    public bool moving;

    Vector3 lastPosition;

    //public Animator guyAnimator;

    Vector3 origScale;
    Vector3 origCamPos;

    //for bounding
    public float xMin, xMax, zMin, zMax;

    //for spawning footprints
    public bool spawnFootsteps, leftOrRightFoot;
    public GameObject leftFoot, rightFoot;
    public Transform footstepParent;

    void Start()
    {
        player = GetComponent<CharacterController>();
        playerAudSource = GetComponent<AudioSource>();
        mouseLook = GetComponentInChildren<GroundCamera>();

        origScale = transform.localScale;
        origCamPos = Camera.main.transform.localPosition;
        Cursor.visible = false;
        spawnFootsteps = true;
    }

    void Update()
    {
        //WASD controls
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            moving = true;

            float moveForwardBackward = Input.GetAxis("Vertical") * currentSpeed;
            float moveLeftRight = Input.GetAxis("Horizontal") * currentSpeed;

            movement = new Vector3(moveLeftRight, 0, moveForwardBackward);

            SprintSpeed();
        }
        //when not moving
        else
        {
            moving = false;
            movement = Vector3.zero;
            currentSpeed = walkSpeed;
        }

        movement = transform.rotation * movement;
        player.Move(movement * Time.deltaTime);

        player.Move(new Vector3(0, -0.5f, 0));

        //PlayerBounding();

        //for animator
        if (moving)
        {
            //guyAnimator.SetBool("moving", true);
            mouseLook.HeadBob();
        }
        else
        {
            //guyAnimator.SetBool("moving", false);
        }

        //hold escape to shrink screm and quit
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    
    //increases move speed while player is moving over time
    public void SprintSpeed()
    {
        footStepTimer -= Time.deltaTime;
        if (footStepTimer < 0)
        {
            PlayFootStepAudio();
            footStepTimer = footStepTimerTotal;
        }

        sprintTimer += Time.deltaTime;
        //while speed is less than sprint, autoAdd
        if (sprintTimer > sprintTimerMax && currentSpeed < sprintSpeed)
        {
            currentSpeed += Time.deltaTime;
        }
    }

    private void PlayFootStepAudio()
    {
        int n = Random.Range(0, currentFootsteps.Length);
        float randomPitch = Random.Range(0.6f, 1.2f);
        playerAudSource.pitch = randomPitch;
        playerAudSource.clip = currentFootsteps[n];
        playerAudSource.PlayOneShot(playerAudSource.clip, 1f);
        // move picked sound to index 0 so it's not picked next time
        currentFootsteps[n] = currentFootsteps[0];
        currentFootsteps[0] = playerAudSource.clip;

        //spawn footprint
        if (spawnFootsteps)
        {
            SpawnFootprint();
        }
    }

    //leave footprints behind as you walk around
    void SpawnFootprint()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - player.height / 2 + 0.01f, transform.position.z);
        GameObject footprint = null;
        if (leftOrRightFoot)
        {
            spawnPos += new Vector3(-0.3f, 0, 0);
            footprint = Instantiate(leftFoot, spawnPos, Quaternion.identity, transform);
        }
        else
        {
            spawnPos += new Vector3(0.3f, 0, 0);
            footprint = Instantiate(rightFoot, spawnPos, Quaternion.identity, transform);
        }

        footprint.transform.localEulerAngles = new Vector3(90, 0, 0);

        StartCoroutine(WaitToStampFootprint(footprint));

        leftOrRightFoot = !leftOrRightFoot;
    }

    IEnumerator WaitToStampFootprint(GameObject footPrint)
    {
        yield return new WaitForSeconds(0.01f);
        footPrint.transform.SetParent(footstepParent);
    }

    //bound the player within the square of the map
    void PlayerBounding()
    {
        //above xmax
        if (transform.position.x > xMax)
        {
            transform.position = new Vector3(xMin + 15, transform.position.y, transform.position.z);
        }
        //below xmin
        if (transform.position.x < xMin)
        {
            transform.position = new Vector3(xMax - 15, transform.position.y, transform.position.z);
        }
        //above zmax
        if (transform.position.z > zMax)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zMin + 15);
        }
        //below zmin    
        if (transform.position.z < zMin)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zMax - 15);
        }
    }

}
