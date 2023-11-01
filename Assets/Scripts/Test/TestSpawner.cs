using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public GameObject sampleCube;
    public Vector2 size;
    public Vector2 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                var obj = Instantiate(sampleCube, new Vector3(x + offset.x, GameManager.Instance.floorYPos, z + offset.y), Quaternion.identity);
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
