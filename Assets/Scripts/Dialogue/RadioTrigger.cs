using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioTrigger : MonoBehaviour
{
    Radio radioScript;
    public int transmission;
    public bool hasActivated;
    public bool isEnding;
    public Transform pointOfInterest;

    public GameObject endingPortal;
    public GameObject[] clouds;

    void Start()
    {
        radioScript = GameObject.FindGameObjectWithTag("Radio").GetComponent<Radio>();
        pointOfInterest = transform.GetChild(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasActivated)
        {
            if (other.gameObject.tag == "Player")
            {
                if (!radioScript.searchingForBroadcast && !radioScript.listeningToBroadcast)
                {
                    if (isEnding)
                    {
                        radioScript.ending = true;
                        endingPortal.SetActive(true);
                        Debug.Log("This is the end!!!");
                        for(int i = 0; i < clouds.Length; i++)
                        {
                            clouds[i].SetActive(true);
                        }
                    }

                    radioScript.ActivateRadio(transmission, pointOfInterest);
                  
                    hasActivated = true;
                }
            }
        }
    }
}
