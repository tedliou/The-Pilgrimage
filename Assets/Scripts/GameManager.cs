using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int targetFrameRate = 30;

    [Header("Position Settings")]
    public float playerYPos = 0;
    public float floorYPos = .5f;
    public float propsYPos = 0;
    
    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = targetFrameRate;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
