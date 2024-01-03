using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

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
    public static UnityEvent OnRoadUpdate = new UnityEvent();
    public static UnityEvent OnResetRoad = new UnityEvent();
    public static int CurrentDepth = 0;
    public static List<RoadBlock> Nodes = new List<RoadBlock>();
    
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
    public RoadDirection selectRoadDirection = RoadDirection.Unknown;
    public int[] roadScores = new int[4];

    private void Awake()
    {
        OnResetRoad.AddListener(ResetRoad);
        isPassed = false;
        ResetRoad();
    }

    private void ResetRoad()
    {
        if (isPassed)
        {
            return;
        }
        previewRoadDirection = RoadDirection.Unknown;
        selectRoadDirection = RoadDirection.Unknown;
        selfScore = 0;
        isBranchRoad = false;
        roadScores = new int[4];
        aroundRoadDict.Clear();
        CurrentDepth = 0;
        if (Nodes.Count > 0)
        {
            Nodes.Clear();
        }
        SetRoadMode(RoadMode.Unknown);
    }

    protected override void Start()
    {
        base.Start();
        SetRoadMode(RoadMode.Unknown);
        OnRoadUpdate.Invoke();
    }

    public void SetRoadMode(RoadMode roadMode)
    {
        indicatorHorizontal.SetActive(false);
        indicatorVertical.SetActive(false);
        indicatorTopToRight.SetActive(false);
        indicatorTopToLeft.SetActive(false);
        indicatorBottomToRight.SetActive(false);
        indicatorBottomToLeft.SetActive(false);
        
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
            case RoadMode.DownToRight:
                indicatorBottomToRight.SetActive(true);
                break;
            case RoadMode.DownToLeft:
                indicatorBottomToLeft.SetActive(true);
                break;
            default:
                break;
        }
    }

    [Button("尋路")]
    public void NavigateAll(int depth = -1)
    {
        // 重置所有道路
        OnResetRoad.Invoke();
        
        // 執行尋路
        Navigate(ref selfScore, depth);
        
        // 建立節點庫與完整路徑外觀
        UpdateRoadState();
        
        Debug.Log($"[{nameof(RoadBlock)}] 完成尋路：路徑共長 {Nodes.Count}");
    }

    public void Navigate(ref int score, int depth)
    {
        FindAround();
        score += 1;
        CurrentDepth += 1;
        // if (CurrentDepth > depth)
        // {
        //     return;
        // }
        

        if (aroundRoadDict.Count == 0)
        {
            isBranchRoad = false;
            selfScore = score;
        }
        else if (aroundRoadDict.Count == 1)
        {
            isBranchRoad = false;
            aroundRoadDict.First().Value.Navigate(ref score, depth);
            selectRoadDirection = aroundRoadDict.First().Key;
            selfScore = score;
        }
        else
        {
            selfScore = score;
            isBranchRoad = true;
            for (var i = 0; i < 4; i++)
            {
                var key = (RoadDirection)i;
                if (!aroundRoadDict.ContainsKey(key))
                {
                    continue;
                }
                roadScores[i] = selfScore;
                aroundRoadDict[(RoadDirection)i].Navigate(ref roadScores[i], depth);
            }

            var selectedIndex = 0;
            for (var i = 1; i < 4; i++)
            {
                if (roadScores[selectedIndex] > roadScores[i])
                {
                    continue;
                }

                selectedIndex = i;
            }

            selectRoadDirection = (RoadDirection)selectedIndex;
        }
    }

    public void UpdateRoadState()
    {
        Nodes.Add(this);
        if (previewRoadDirection == RoadDirection.Unknown || aroundRoadDict.Count == 0)
        {
            if (aroundRoadDict.Count == 0)
            {
                switch (previewRoadDirection)
                {
                    case RoadDirection.Top:
                        SetRoadMode(RoadMode.Vertical);
                        break;
                    case RoadDirection.Down:
                        SetRoadMode(RoadMode.Vertical);
                        break;
                    case RoadDirection.Left:
                        SetRoadMode(RoadMode.Horizontal);
                        break;
                    case RoadDirection.Right:
                        SetRoadMode(RoadMode.Horizontal);
                        break;
                }
            }
            else
            {
                var firstDir = isBranchRoad ? selectRoadDirection : aroundRoadDict.First().Value.previewRoadDirection;
                switch (firstDir)
                {
                    case RoadDirection.Top:
                        SetRoadMode(RoadMode.Vertical);
                        break;
                    case RoadDirection.Down:
                        SetRoadMode(RoadMode.Vertical);
                        break;
                    case RoadDirection.Left:
                        SetRoadMode(RoadMode.Horizontal);
                        break;
                    case RoadDirection.Right:
                        SetRoadMode(RoadMode.Horizontal);
                        break;
                }
                aroundRoadDict.First().Value.UpdateRoadState();
            }
        }
        else
        {
            if ((previewRoadDirection == RoadDirection.Top && selectRoadDirection == RoadDirection.Down) ||
                (previewRoadDirection == RoadDirection.Down && selectRoadDirection == RoadDirection.Top))
            {
                SetRoadMode(RoadMode.Vertical);
            }
            else if ((previewRoadDirection == RoadDirection.Left && selectRoadDirection == RoadDirection.Right) ||
                     (previewRoadDirection == RoadDirection.Right && selectRoadDirection == RoadDirection.Left))
            {
                SetRoadMode(RoadMode.Horizontal);
            }
            else if ((previewRoadDirection == RoadDirection.Top && selectRoadDirection == RoadDirection.Right) ||
                     (previewRoadDirection == RoadDirection.Right && selectRoadDirection == RoadDirection.Top))
            {
                SetRoadMode(RoadMode.TopToRight);
            }
            else if ((previewRoadDirection == RoadDirection.Top && selectRoadDirection == RoadDirection.Left) ||
                     (previewRoadDirection == RoadDirection.Left && selectRoadDirection == RoadDirection.Top))
            {
                SetRoadMode(RoadMode.TopToLeft);
            }
            else if ((previewRoadDirection == RoadDirection.Down && selectRoadDirection == RoadDirection.Right) ||
                     (previewRoadDirection == RoadDirection.Right && selectRoadDirection == RoadDirection.Down))
            {
                SetRoadMode(RoadMode.DownToRight);
            }
            else if ((previewRoadDirection == RoadDirection.Down && selectRoadDirection == RoadDirection.Left) ||
                     (previewRoadDirection == RoadDirection.Left && selectRoadDirection == RoadDirection.Down))
            {
                SetRoadMode(RoadMode.DownToLeft);
            }
            
            aroundRoadDict[selectRoadDirection].UpdateRoadState();
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

            // 過濾上一動
            if ((RoadDirection)i == previewRoadDirection)
            {
                continue;
            }

            // 取得道路
            var roadBlock = aroundBlocks[i].GetComponent<RoadBlock>();

            // 神轎已經過的道路不可再使用
            if (roadBlock.isPassed)
            {
                continue;
            }
            
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
            //Debug.Log($"[{nameof(RoadBlock)}] 找到 {direction} 道路: {roadBlock.name}, {roadBlock.transform.GetGridKey()}");
        }
    }
}