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

    public bool isStarted = false;
    public bool isMoving = false;
    public bool isFinding = false;
    public bool isRoadUpdated = false;

    public RoadBlock targetRoadBlock;

    private Coroutine m_moveJob;
    [SerializeField]private List<RoadBlock> m_roads;

    public Direction direction = Direction.Right;

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
        // StartCoroutine(FindPath());
        StartCoroutine(ObserveMoveJob());
        // RoadBlock.OnRoadUpdate.AddListener(OnRoadUpdate);
        
        //OnSedanChairCreate.Invoke(this);
    }
    
    

    // private void OnRoadUpdate()
    // {
    //     isRoadUpdated = true;
    //     if (!isFinding && !isMoving)
    //     {
    //         StartCoroutine(FindPath());
    //     }
    // }


    [Button]
    private void StartMoveJob(RoadBlock roadBlock)
    {
        if (m_moveJob != null)
            return;

        m_moveJob = StartCoroutine(MoveJob(roadBlock));
    }

    private IEnumerator MoveJob(RoadBlock roadBlock)
    {
        Debug.Log($"[{nameof(SedanChair)}] Start Move Job, Target Pos: {roadBlock}");
        
        var param =  moveSpeed * Time.deltaTime;
        
        while (true)
        {
            if (transform.position == roadBlock.transform.position)
            {
                break;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, roadBlock.transform.position, param);
            m_child.forward = Vector3.Lerp(m_child.forward, (roadBlock.transform.position - transform.position).normalized, .2f);
            isMoving = true;
            
            yield return null;

        }

        if (targetRoadBlock.isEndPoint)
        {
            Debug.Log("Reach EndPoint!");
            targetRoadBlock.isEndPoint = false;
            EnvSpawner.Instance.GenerateNewMap();
        }

        targetRoadBlock = null;
        isMoving = false;
        m_moveJob = null;
        
        Debug.Log($"[{nameof(SedanChair)}] End Move Job");

        if (isRoadUpdated)
        {
            //StartCoroutine(FindPath());
        }
    }

    private IEnumerator ObserveMoveJob()
    {
        yield return new WaitForSeconds(1);
        
        while (true)
        {
            yield return new WaitUntil(() => !isMoving);
            
            while (!FindRoadBlock())
            {
                yield return null;
            }

            var road = FindRoadBlock();
            while (road.nextRoad == null)
            {
                yield return null;
            }

            road.isPassed = true;
            targetRoadBlock = road.nextRoad;
            StartMoveJob(targetRoadBlock);

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
        var block = GridSystem.FindDownBlock(transform.position);
        if (block)
        {
            return block.GetComponent<RoadBlock>();
        }
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
