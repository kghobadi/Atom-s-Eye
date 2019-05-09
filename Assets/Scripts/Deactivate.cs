using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivate : MonoBehaviour
{
    GameObject _player;
    WorldManager wm;

    public bool usesActDist;
    public float activationDist;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        wm = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<WorldManager>();

        if (!usesActDist)
        {
            activationDist = wm.activationDistance;
        }
    }

    void Update()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) > activationDist)
        {
            wm.allInactiveObjects.Add(gameObject);

            gameObject.SetActive(false);
        }
    }
}
