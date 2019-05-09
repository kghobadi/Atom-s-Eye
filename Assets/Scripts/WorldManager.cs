using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{ 
    //inactive object list for storing stuff we turn off
    public List<GameObject> allInactiveObjects = new List<GameObject>();
    public float activationDistance = 75f;

    //player variables
    GameObject player;
    FirstPersonController fpc;

    void Start()
    {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();
    }
    
    void Update()
    {
        //if player moves, we call the deactivation functions and the pacman functions
        if (fpc.moving)
        {
            StoreDeactiveObjects();
            
        }
    }

    void StoreDeactiveObjects()
    {
        //loop through all objects and check distances from player
        for (int i = 0; i < allInactiveObjects.Count; i++)
        {
            if (allInactiveObjects[i] != null)
            {
                float distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, player.transform.position);

                if (distanceFromPlayer < (activationDistance + 10))
                {
                    //set object active
                    allInactiveObjects[i].SetActive(true);
                    //remove from list
                    allInactiveObjects.Remove(allInactiveObjects[i]);
                    //move i back once to account for change in list index
                    i--;
                }
            }
            //obj is destroyed
            else
            {
                //remove from list
                allInactiveObjects.Remove(allInactiveObjects[i]);
                //move i back once to account for change in list index
                i--;
            }

        }
    }
}
