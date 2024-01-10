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
    public static UnityEvent OnMoved = new();
    
    public float moveSpeed = .2f;
    public float currentSpeed;
    public Vector3 lastPos;
    
    public List<Vector3> history = new List<Vector3>();
    
    public Rigidbody m_rigidbody;
    private Vector3 m_targetPos;
    private bool m_isTargetPosSet = false;

    public int m_nodeIndex;
    
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
    public AudioSource sfx;

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
        m_nodeIndex = 0;
        StartCoroutine(ObserveMoveJob());
        
        // RoadBlock.OnRoadUpdate.AddListener(OnRoadUpdate);
        
        //OnSedanChairCreate.Invoke(this);
        lastPos = transform.position;
        StartCoroutine(GetSpeed());
    }

    private void Update()
    {
        if (isMoving)
        {
            if (!sfx.isPlaying)
            {
                sfx.Play();
            }
        }
        else
        {
            if (sfx.isPlaying)
            {
                sfx.Stop();
            }    
        }
    }

    private IEnumerator GetSpeed()
    {
        yield return null;
        while (true)
        {
            currentSpeed = Vector3.Distance(transform.position, lastPos);
            lastPos = transform.position;
            yield return new WaitForSeconds(1);
        }
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
            EnvSpawner.Instance.Generate();
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

        m_nodeIndex = EnvSpawner.Instance.prebuildRoadLength;
        while (true)
        {
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitUntil(() => RoadBlock.Nodes.Count > m_nodeIndex);

            RoadBlock.Nodes[m_nodeIndex].isPassed = true;
            m_nodeIndex++;
            yield return new WaitUntil(() => m_nodeIndex < RoadBlock.Nodes.Count);
            yield return new WaitUntil(() => GameManager.Instance.CanMove);
            
            targetRoadBlock = RoadBlock.Nodes[m_nodeIndex];
            StartMoveJob(targetRoadBlock);
            OnMoved.Invoke();

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
        m_nodeIndex += 1;
        if (m_nodeIndex < RoadBlock.Nodes.Count - 1)
        {
            m_currentRoad = RoadBlock.Nodes[m_nodeIndex];
            m_nextRoad = RoadBlock.Nodes[m_nodeIndex + 1];
            m_targetPos = m_nextRoad.transform.position;
            Debug.Log(nameof(GetNextNode));
            return true;
        }

        return false;
    }
}
