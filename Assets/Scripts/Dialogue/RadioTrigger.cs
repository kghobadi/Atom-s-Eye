using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioTrigger : MonoBehaviour
{
    Radio radioScript;
    public int transmission;
    public bool hasActivated;

    public Transform pointOfInterest;

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
                    radioScript.ActivateRadio(transmission, pointOfInterest);
                    hasActivated = true;
                }
            }
        }
    }
}
