using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadBlock : MapBlock
{
    [SerializeField] private GameObject indicatorHorizontal;
    [SerializeField] private GameObject indicatorVertical;
    [SerializeField] private GameObject indicatorTopToRight;
    [SerializeField] private GameObject indicatorTopToLeft;
    [SerializeField] private GameObject indicatorBottomToRight;
    [SerializeField] private GameObject indicatorBottomToLeft;

    private void Start()
    {
        SetRoadMode(RoadMode.Horizontal);
    }

    public void SetRoadMode(RoadMode roadMode)
    {
        switch (roadMode)
        {
            case RoadMode.Horizontal:
                indicatorHorizontal.SetActive(true);
                break;
            case RoadMode.Vertical:
                indicatorVertical.SetActive(true);
                break;
            case RoadMode.TopToRight:
                indicatorTopToRight.SetActive(true);
                break;
            case RoadMode.TopToLeft:
                indicatorTopToLeft.SetActive(true);
                break;
            case RoadMode.BottomToRight:
                indicatorBottomToRight.SetActive(true);
                break;
            case RoadMode.BottomToLeft:
                indicatorBottomToLeft.SetActive(true);
                break;
        }
    }
}

public enum RoadMode
{
    Horizontal = 0,
    Vertical = 1,
    TopToRight = 2,
    TopToLeft = 3,
    BottomToRight = 4,
    BottomToLeft = 5
}