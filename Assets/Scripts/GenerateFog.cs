using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFog : MonoBehaviour
{ 
    //player variables
    GameObject player;
    FirstPersonController fpc;

    public int gridSizeX, gridSizeY;
    public float fogSizeX, fogSizeZ;
    public List<GameObject> fogGrid = new List<GameObject>();
    public GameObject fogPrefab;
    public Transform fogParent;

    public GameObject terrain;

    public Vector3 offset;

    void Awake()
    {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        fpc = player.GetComponent<FirstPersonController>();

        //grab fog size from prefab particle systems
        ParticleSystem.ShapeModule fogShape = fogPrefab.transform.GetChild(0).GetComponent<ParticleSystem>().shape;
        fogSizeX = fogShape.scale.x;
        fogSizeZ = fogShape.scale.z;

        //call generate
        GenerateFogGrid();

        transform.position += offset;
    }
    

    void GenerateFogGrid()
    {
        for (int i = 0, y = 0; y <= gridSizeY; y++)
        {
            for (int x = 0; x <= gridSizeX; x++, i++)
            {
                GameObject fogClone = Instantiate(fogPrefab, new Vector3(x * fogSizeX, terrain.transform.position.y + 1f, y * fogSizeZ), 
                    Quaternion.identity, fogParent);

                fogGrid.Add(fogClone);
            }
        }
    }
}
