using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Radio : MonoBehaviour
{ 
    //player variables
    GameObject player;
    FirstPersonController fpc;

    //animator ref
    Animator radioAnimator;

    //sound , vo and statics
    AudioSource radioSource;
    public AudioSource staticSource, knobSource;
    public AudioClip[] staticSounds;
    public List<AudioClip> voiceOvers = new List<AudioClip>();
    public int currentVO=0;
    public float initialVolStatic, initialVolTransmisision;
    public float foundSignalVolT;

    //text ref and station vars
    public Text radioText;
    public float currentStation, changeStationSpeed;
    public float stationMin, stationMax;
    public float necessaryStation, necessaryRange = 1, necMin, necMax;
    public bool searchingForBroadcast, listeningToBroadcast;
    public float waitTimer = 0, waitTimerTotal = 1f;

    //visual for changing station
    public GameObject knob;
    public float minKnobRotation, maxKnobRotation;
    public AudioClip[] tuneUps, tuneDowns;

    //2d obj to show radio making noise
    public GameObject radioWaves;
    
    void Start()
    {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();

        //comp refs
        radioAnimator = GetComponent<Animator>();
        radioSource = GetComponent<AudioSource>();

        //set start station to halfway point between range
        float amt = (stationMax - stationMin) / 2;
        currentStation = stationMin + amt;
    }
    
    void Update()
    {
        if (!listeningToBroadcast)
        {
            //tune up
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                currentStation += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * changeStationSpeed;
                PlayTune(tuneUps);
            }
            //tune down
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                currentStation += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * changeStationSpeed;
                PlayTune(tuneDowns);
            }
        }
        

        //lowest station wrap
        if(currentStation < stationMin)
        {
            currentStation = stationMax;
        }
        //highest station wrap
        if (currentStation > stationMax)
        {
            currentStation = stationMin;
        }

        //set radio text
        radioText.text = "- " + currentStation.ToString("F1") + " -";

        //check if current station is approaching necessary range
        if (searchingForBroadcast)
        {
            float dif = Mathf.Abs(currentStation - necessaryStation);

            //play new static when it is no longer playing
            if(staticSource.isPlaying == false)
            {
                PlayStatic();
            }

            //within nec range
            if(currentStation < necMax && currentStation > necMin)
            {
                //alter volumes
                staticSource.volume -= dif * Time.deltaTime;
                radioSource.volume += dif * Time.deltaTime;

                waitTimer += Time.deltaTime;

                if(waitTimer > waitTimerTotal)
                {
                    searchingForBroadcast = false;
                    listeningToBroadcast = true;

                    radioSource.volume = foundSignalVolT;

                    fpc.canMove = true;
                    staticSource.Stop();
                    Debug.Log("found broadcast");
                }
            }
        }

        //need to check when a broadcast has finished
        if (listeningToBroadcast)
        {
            //hard set move speed 
            fpc.currentSpeed = 2.5f;
            if(radioSource.isPlaying == false)
            {
                Debug.Log("finished broadcast");
                listeningToBroadcast = false;
                fpc.currentSpeed = fpc.walkSpeed;
            }
        }

        //turn on and off radio waves
        if (radioSource.isPlaying || staticSource.isPlaying)
        {
            radioWaves.SetActive(true);
        }
        else
        {
            radioWaves.SetActive(false);
        }
    }

    //called by Radio Trigger obj when player enters
    public void ActivateRadio(int transmissionNum)
    {
        //set nec station and range
        necessaryStation = Random.Range(stationMin + necessaryRange, stationMax - necessaryRange);
        necMin = necessaryStation - necessaryRange;
        necMax = necessaryStation + necessaryRange;

        //set initial volumes
        staticSource.volume = initialVolStatic;
        radioSource.volume = initialVolTransmisision;
  
        searchingForBroadcast = true;
        fpc.canMove = false;
        waitTimer = 0;

        //play both audio sources
        PlayStatic();
        PlayTransmission(transmissionNum);
    }

    void PlayTune(AudioClip [] tunes)
    {
        int randomTune = Random.Range(0, tunes.Length);
        knobSource.PlayOneShot(tunes[randomTune]);
    }

    void PlayStatic()
    {
        int randomStatic = Random.Range(0, staticSounds.Length);
        staticSource.PlayOneShot(staticSounds[randomStatic]);
    }

    void PlayTransmission(int voNum)
    {
        radioSource.clip = voiceOvers[voNum];
        radioSource.Play();
    }
}
