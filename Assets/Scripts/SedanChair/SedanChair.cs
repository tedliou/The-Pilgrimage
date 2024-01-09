using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SedanChair : Singleton<SedanChair>
{
    public static UnityEvent<SedanChair> OnSedanChairCreate = new();
    
    public float moveSpeed = .2f;

    public List<Vector3> history = new List<Vector3>();
    
    public Rigidbody m_rigidbody;
    private Vector3 m_targetPos;
    private bool m_isTargetPosSet = false;

    public int m_currentNodeIndex;
    public RoadBlock m_currentRoad;
    public RoadBlock m_nextRoad;
    private Transform m_child;

    public bool isMoving = false;
    public bool isFinding = false;
    public bool isRoadUpdated = false;

    private Coroutine m_moveJob;
    [SerializeField]private List<RoadBlock> m_roads;

    [System.Serializable]
    public enum Direction
    {
        Top,
        Down,
        Right,
        Left,
        Stop
    }

    protected override void OnStart()
    {
        base.OnStart();
        
        m_child = transform.GetChild(0);
        StartCoroutine(FindPath());
        StartCoroutine(ObserveMoveJob());
        RoadBlock.OnRoadUpdate.AddListener(OnRoadUpdate);
        
        OnSedanChairCreate.Invoke(this);
    }

    private void OnRoadUpdate()
    {
        isRoadUpdated = true;
        if (!isFinding && !isMoving)
        {
            StartCoroutine(FindPath());
        }
    }


    [Button]
    private void StartMoveJob(Vector3 targetPos)
    {
        if (m_moveJob != null)
            return;

        m_moveJob = StartCoroutine(MoveJob(targetPos));
    }

    private IEnumerator MoveJob(Vector3 targetPos)
    {
        Debug.Log($"[{nameof(SedanChair)}] Start Move Job, Target Pos: {targetPos}");
        
        var param =  moveSpeed * Time.deltaTime;
        
        while (true)
        {
            if (transform.position == targetPos)
            {
                break;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, param);
            m_child.forward = Vector3.Lerp(m_child.forward, (targetPos - transform.position).normalized, .2f);
            isMoving = true;
            
            yield return null;

        }

        if (m_roads[0].isEndPoint)
        {
            Debug.Log("Reach EndPoint!");
            m_roads[0].isEndPoint = false;
            EnvSpawner.current.GenerateNewMap();
        }
        m_roads.RemoveAt(0);
        isMoving = false;
        m_moveJob = null;
        
        Debug.Log($"[{nameof(SedanChair)}] End Move Job");

        if (isRoadUpdated)
        {
            StartCoroutine(FindPath());
        }
    }

    private IEnumerator ObserveMoveJob()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            yield return new WaitUntil(() => !isMoving);
            
            
            if (m_roads.Count > 0)
            {
                if (m_roads[0].transform.GetGridKey() != transform.GetGridKey())
                {
                    m_roads[0].isPassed = true;
                    StartMoveJob(m_roads[0].transform.position);
                }
            }

            yield return null;
        }
    }

    private IEnumerator FindPath()
    {
        Debug.Log($"[{nameof(SedanChair)}] Start Path Finder");
        
        isFinding = true;
        
        while (true)
        {
            var roadBlock = FindRoadBlock();
            if (roadBlock)
            {
                roadBlock.NavigateAll();
                m_roads = RoadBlock.Nodes.ToList();
                if (m_roads.Count == 0 || m_roads.Count < 2)
                {
                    yield return null;
                    continue;
                }

                m_roads[0].isPassed = true;
                m_roads.RemoveAt(0);
                break;
            }

            
            
            
            // New Nodes
            /// index 0 is current
            
            
            // m_currentNodeIndex = 0;
            // m_currentRoad = RoadBlock.Nodes[m_currentNodeIndex];
            // m_currentRoad.isPassed = true;
            //
            // if (RoadBlock.Nodes.Count > 1)
            // {
            //     m_nextRoad = RoadBlock.Nodes[m_currentNodeIndex + 1];
            //     m_nextRoad.isPassed = true;
            //         
            //     if (!m_isTargetPosSet)
            //     {
            //         m_targetPos = m_nextRoad.transform.position;
            //         m_isTargetPosSet = true;
            //     }
            //     
            //     Debug.Log("Path finder ended");
            //     
            //     yield break;
            // }
            
            yield return null;
        }

        isFinding = false;
        
        Debug.Log($"[{nameof(SedanChair)}] Stop Path Finder");
    }

    private RoadBlock FindRoadBlock()
    {
        // var downBlock = GridSystem.FindDownBlock(this);
        // if (downBlock)
        // {
        //     return downBlock.GetComponent<RoadBlock>();
        // }
        //
        // Debug.Log("找不到底下的路");
        return null;
    }

    private bool GetNextNode()
    {
        m_currentNodeIndex += 1;
        if (m_currentNodeIndex < RoadBlock.Nodes.Count - 1)
        {
            m_currentRoad = RoadBlock.Nodes[m_currentNodeIndex];
            m_nextRoad = RoadBlock.Nodes[m_currentNodeIndex + 1];
            m_targetPos = m_nextRoad.transform.position;
            Debug.Log(nameof(GetNextNode));
            return true;
        }

        return false;
    }
}
