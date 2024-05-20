using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject spawnObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnObject()
    {
        Instantiate(spawnObject, spawnLocation.position, spawnLocation.rotation);
    }
}
