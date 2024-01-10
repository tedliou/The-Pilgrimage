using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Car : BlockBase
{
    public bool isGenerator = false;
    public Car extendCar;
    public int gasProp;
    public int garbageProp;
    public PropBlock gasObj;
    public PropBlock garbageObj;
    public PropBlock roadObj;
    
    public SedanChair sedanChair;
    public RoadBlock targetRoadBlock;
    public bool isMoving;
    
    private Transform m_child;
    private Coroutine m_moveJob;
    public int m_nodeIndex;
    public int offset = 3;

    public AudioSource sfx;

    private void Awake()
    {
        m_child = transform.GetChild(0);
    }

    private void Start()
    {
        StartCoroutine(ObserveMoveJob());
        if (isGenerator)
        {
            
            gasObj.SetAmount(0);
            garbageObj.SetAmount(0);
            StartCoroutine(GeneratorJob());
        }
        else
        {
            roadObj.SetAmount(0);
        }
    }

    public void PlaceGas(int amount)
    {
        gasObj.Place(amount);
    }

    public void PlaceGarbage(int amount)
    {
        garbageObj.Place(amount);
    }

    private IEnumerator GeneratorJob()
    {
        while (true)
        {
            yield return new WaitUntil(() => gasObj.m_amount > 0 && garbageObj.m_amount > 0);

            extendCar.roadObj.Place(5);
            gasObj.Pickup(1, false);
            garbageObj.Pickup(1, false);
            Log("生成 5 祈路");
            sfx.Play();
            
            yield return new WaitForSeconds(1);
        }
    }

    private void StartMoveJob(RoadBlock roadBlock)
    {
        if (m_moveJob != null)
            return;

        m_moveJob = StartCoroutine(MoveJob(roadBlock));
    }

    private IEnumerator MoveJob(RoadBlock roadBlock)
    {
        var param =  sedanChair.moveSpeed * Time.deltaTime;
        
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

        targetRoadBlock = null;
        isMoving = false;
        m_moveJob = null;
    }

    private IEnumerator ObserveMoveJob()
    {
        yield return new WaitForSeconds(1);

        sedanChair = FindObjectOfType<SedanChair>();
        
        while (true)
        {
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitUntil(() => RoadBlock.Nodes.Count > 1);

            if (m_nodeIndex >= sedanChair.m_nodeIndex - offset)
            {
                // targetRoadBlock = RoadBlock.Nodes[1];
                // StartMoveJob(targetRoadBlock);
            }
            else
            {
                m_nodeIndex++;
                targetRoadBlock = RoadBlock.Nodes[m_nodeIndex];
                StartMoveJob(targetRoadBlock);
            }

            yield return null;
        }
    }
}
