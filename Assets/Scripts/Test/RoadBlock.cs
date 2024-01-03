using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public enum RoadDirection
{
    Top = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    Unknown = -1
}

public class RoadBlock : BlockBase
{
    public GameObject indicatorHorizontal;
    public GameObject indicatorVertical;
    public GameObject indicatorTopToRight;
    public GameObject indicatorTopToLeft;
    public GameObject indicatorBottomToRight;
    public GameObject indicatorBottomToLeft;

    public bool isPassed = false;
    public RoadDirection previewRoadDirection;

    public Dictionary<RoadDirection, RoadBlock> aroundRoadDict = new Dictionary<RoadDirection, RoadBlock>();
    public int selfScore = 0;
    public bool isBranchRoad = false;
    public int[] roadScores = new int[4];

    private void Awake()
    {
        previewRoadDirection = RoadDirection.Unknown;
    }

    protected override void Start()
    {
        base.Start();
        SetRoadMode(RoadMode.Unknown);
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
            default:
                break;
        }
    }

    [Button("尋路")]
    public void NavigateAll()
    {
        // 執行尋路
        var result = new List<RoadBlock>();
        Navigate(ref selfScore);
        
        // 建立完整路徑
        if (aroundRoadDict.Count == 0)
        {
            return;
        }

        foreach (var ar in aroundRoadDict.Values)
        {
            
        }
    }

    public void Navigate(ref int score)
    {
        FindAround();

        score += 1;

        if (aroundRoadDict.Count == 0)
        {
            isBranchRoad = false;
            selfScore = score;
        }
        else if (aroundRoadDict.Count == 1)
        {
            isBranchRoad = false;
            aroundRoadDict.First().Value.Navigate(ref score);
            selfScore = score;
        }
        else
        {
            selfScore = score;
            isBranchRoad = true;
            for (var i = 0; i < 4; i++)
            {
                roadScores[i] = selfScore;
                var key = (RoadDirection)i;
                if (!aroundRoadDict.ContainsKey(key))
                {
                    continue;
                }
                aroundRoadDict[(RoadDirection)i].Navigate(ref roadScores[i]);
            }

            var selectedIndex = 0;
            for (var i = 1; i < 4; i++)
            {
                if (roadScores[selectedIndex] > roadScores[1])
                {
                    continue;
                }

                selectedIndex = i;
            }

            var finalDir = (RoadDirection)selectedIndex;
            Debug.Log(finalDir);
        }
    }
    

    /// <summary>
    /// 搜尋周圍的道路
    /// </summary>
    [Button("搜尋周圍道路")]
    public void FindAround()
    {
        aroundRoadDict.Clear();
        
        var aroundBlocks = GridSystem.FindAround(this);
        for (var i = 0; i < aroundBlocks.Count; i++)
        {
            // 過濾無方塊
            if (aroundBlocks[i] is null)
            {
                continue;
            }
            
            // 過濾非道路
            if (aroundBlocks[i].blockType != BlockType.Road)
            {
                continue;
            }

            // 神轎已經過的道路不可再使用
            if (isPassed)
            {
                continue;
            }

            // 過濾上一動
            if ((RoadDirection)i == previewRoadDirection)
            {
                continue;
            }

            // 取得道路
            var roadBlock = aroundBlocks[i].GetComponent<RoadBlock>();
            
            // 過濾已被設定的道路
            if (roadBlock.previewRoadDirection != RoadDirection.Unknown)
            {
                continue;
            }

            var direction = (RoadDirection)i;
            switch (direction)
            {
                case RoadDirection.Top:
                    roadBlock.previewRoadDirection = RoadDirection.Down;
                    break;
                case RoadDirection.Down:
                    roadBlock.previewRoadDirection = RoadDirection.Top;
                    break;
                case RoadDirection.Left:
                    roadBlock.previewRoadDirection = RoadDirection.Right;
                    break;
                case RoadDirection.Right:
                    roadBlock.previewRoadDirection = RoadDirection.Left;
                    break;
            }
            
            aroundRoadDict.Add(direction, roadBlock);
            Debug.Log($"[{nameof(RoadBlock)}] 找到 {direction} 道路: {roadBlock.name}, {roadBlock.transform.GetGridKey()}");
        }
    }
}