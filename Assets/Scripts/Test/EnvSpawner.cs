using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSpawner : MonoBehaviour
{
    public GameObject sampleCube;
    public Vector2 size;
    public Vector2 offset;

    public Vector2[] stations = new Vector2[]{};
    public Vector2 stationSize;
    
    
    void Start()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                if (Array.Exists(stations, e => 
                        (x >= e.x) && (z >= e.y) && 
                        (x <= e.x + stationSize.x - 1) && (z <= e.y + stationSize.y - 1)))
                {
                    continue;
                }
                
                var obj = Instantiate(sampleCube, new Vector3(x + offset.x, GameManager.Instance.floorYPos, z + offset.y), Quaternion.identity);
                
            }
        }
    }

    
    void Update()
    {
        
    }
}
