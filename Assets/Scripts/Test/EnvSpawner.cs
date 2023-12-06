using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSpawner : MonoBehaviour
{
    public GameObject sampleCube;
    public GameObject temple;
    public Vector2 size;
    public Vector2 offset;

    public Vector2[] stations = new Vector2[]{};
    public Vector2 stationSize;
    
    
    void Start()
    {
        for (var x = 0; x < size.x; x++)
        {
            for (var z = 0; z < size.y; z++)
            {
                var buildGround = true;
                var buildTemple = false;
                
                foreach (var e in stations)
                {
                    if ((x >= e.x) && (z >= e.y) && 
                              (x <= e.x + stationSize.x - 1) && (z <= e.y + stationSize.y - 1))
                    {
                        buildGround = false;
                        
                        if (x == e.x && z == e.y)
                        {
                            buildTemple = true;
                        }
                    }
                    
                    if (buildTemple)
                        break;
                }
                
                

                if (buildGround)
                {
                    var obj = Instantiate(sampleCube, new Vector3(x + offset.x, GameManager.Instance.floorYPos, z + offset.y), Quaternion.identity, transform);

                }
                else if (buildTemple)
                {
                    var obj = Instantiate(temple, new Vector3(x + offset.x, GameManager.Instance.floorYPos, z + offset.y), Quaternion.identity, transform);

                }
            }
        }
    }

    
    void Update()
    {
        
    }
}
