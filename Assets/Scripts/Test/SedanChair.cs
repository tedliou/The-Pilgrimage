using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SedanChair : BlockBase
{
    public Direction moveDirection = Direction.Stop;
    public Direction lastDirection = Direction.Stop;
    
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
    public bool isWaiting = false;

    private Coroutine m_moveJob;
    private List<Vector3> m_roads;

    [System.Serializable]
    public enum Direction
    {
        Top,
        Down,
        Right,
        Left,
        Stop
    }
    

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_child = transform.GetChild(0);
    }

    private void OnRoadUpdate()
    {
        StartCoroutine(FindPath());
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FindPath());
        RoadBlock.OnRoadUpdate.AddListener(OnRoadUpdate);
    }

    private void FixedUpdate()
    {
        return;
        
        if (RoadBlock.Nodes.Count == 1)
        {
            return;
        }
        
        if (!m_isTargetPosSet)
            return;
        
        var dir = m_targetPos - transform.position;
        var dirNormalized = dir.normalized;
        var velocity =  moveSpeed * dirNormalized;
        if (Vector3.Distance(transform.position, m_targetPos) < 0.02f)
        {
            velocity = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, m_targetPos, .5f);
            
            history.Add(m_currentRoad.transform.position);
            var pos = m_nextRoad.transform.position;
            m_targetPos = new Vector3(pos.x, 0, pos.z);

            var getNodeState = GetNextNode();
            isWaiting = !getNodeState;
            if (getNodeState)
            {
                Debug.Log("Success");
            }
        }
        
        m_rigidbody.velocity = velocity;
        
        if (dirNormalized.magnitude > 0)
        {
            m_child.forward = Vector3.Lerp(m_child.forward, dirNormalized, .5f);
        }
    }


    private void StartMoveJob(Vector3 targetPos)
    {
        if (m_moveJob != null)
            return;

        m_moveJob = StartCoroutine(MoveJob(targetPos));
    }

    private IEnumerator MoveJob(Vector3 targetPos)
    {
        Debug.Log($"[{nameof(SedanChair)}] Start Move Job, Target Pos: {targetPos}");
        
        var vector = targetPos - transform.position;
        var normal = vector.normalized;
        var param =  moveSpeed * normal * Time.fixedTime;
        
        while (true)
        {
            if (vector.magnitude < param.magnitude)
            {
                param = vector;
            }

            transform.position += param;
            isMoving = true;
            
            yield return new WaitForFixedUpdate();

            if (transform.position == targetPos)
            {
                break;
            }
        }

        isMoving = false;
        m_moveJob = null;
        
        Debug.Log($"[{nameof(SedanChair)}] End Move Job");
    }

    private IEnumerator ObserveMoveJob()
    {
        while (true)
        {
            yield return new WaitUntil(() => !isMoving);
            
            if (m_roads.Count > 0)
            {
                StartMoveJob(m_roads[0]);
            }

            yield return null;
        }
    }

    private IEnumerator FindPath()
    {
        Debug.Log("Path finder started");
        while (true)
        {
            var roadBlock = FindRoadBlock();
            roadBlock.NavigateAll();

            m_roads = RoadBlock.Nodes.Select(r => r.transform.position).ToList();
            if (m_roads.Count == 0 || m_roads.Count < 2)
            {
                yield return null;
                continue;
            }
            m_roads.RemoveAt(0);
            
            Debug.Log(m_roads.Count);
            Debug.Log(m_roads[0]);
            
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
            
            yield return new WaitForSeconds(.5f);
        }
    }

    private RoadBlock FindRoadBlock()
    {
        var downBlock = GridSystem.FindDownBlock(this);
        if (downBlock)
        {
            return downBlock.GetComponent<RoadBlock>();
        }

        Debug.Log("找不到底下的路");
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
