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

    //for game start
    public bool hasStarted;
    public FadeUI[] startFades;

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
    public Vector3 currentPoint;

    public bool ending;
    
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

                //on start
                if (!hasStarted)
                {
                    for( int i = 0; i < startFades.Length; i++)
                    {
                        startFades[i].fadingIn = false;
                        startFades[i].StopAllCoroutines();
                        startFades[i].fadingOut = true;
                    }
                    fpc.canMove = true;
                    hasStarted = true;
                }
            }
            //tune down
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                currentStation += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * changeStationSpeed;
                PlayTune(tuneDowns);

                //on start
                if (!hasStarted)
                {
                    for (int i = 0; i < startFades.Length; i++)
                    {
                        startFades[i].fadingIn = false;
                        startFades[i].StopAllCoroutines();
                        startFades[i].fadingOut = true;
                    }
                    fpc.canMove = true;
                    hasStarted = true;
                }
            }
        }

        if (ending)
        {
            fpc.ending = true;
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

            fpc.transform.position = Vector3.MoveTowards(fpc.transform.position, currentPoint, 3 * Time.deltaTime);

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

    //could just randomly attribute all the VOs to dif channels at start
    //then they could be accessible in any order
    //static until you land on it, then it plays in full and you cant change channel until over
    //can forget about triggers this way
    //could do both??

    //called by Radio Trigger obj when player enters
    public void ActivateRadio(int transmissionNum, Transform pointToGoTo)
    {
        //set nec station and range
        necessaryStation = Random.Range(stationMin + necessaryRange, stationMax - necessaryRange);
        necMin = necessaryStation - necessaryRange;
        necMax = necessaryStation + necessaryRange;

        float dist = Mathf.Abs(currentStation - necessaryStation);
        //if its too close to current station val
        if(dist < 2)
        {
            //current station is less than necessary station, add to it
            if(currentStation < necessaryStation)
            {
                if (necessaryStation < stationMax - 2)
                {
                    necessaryStation += 2;
                }
                else
                {
                    necessaryStation = stationMin + 1f;
                }
            }

            //current station is greater than necessaryStation so subtract from it
            else
            {
                if(necessaryStation > stationMin + 2)
                {
                    necessaryStation -= 2;
                }
                else
                {
                    necessaryStation = stationMax - 2;
                }
            }
           
        }

        //set initial volumes
        staticSource.volume = initialVolStatic;
        radioSource.volume = initialVolTransmisision;

        currentPoint = pointToGoTo.position;
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
