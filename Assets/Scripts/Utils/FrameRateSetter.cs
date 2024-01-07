using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateSetter : Singleton<FrameRateSetter>
{
    public int targetFrameRate = 60;
    
    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
