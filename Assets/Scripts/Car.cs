using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Car : BlockBase
{
    public SedanChair sedanChair;
    [Range(1, 10)]
    public int historyIndexOffset = 1;
    public int currentHistoryIndex = -1;

    private Rigidbody m_rigidbody;
    private Transform m_child;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_child = transform.GetChild(0);
    }

    protected override void Start()
    {
        StartCoroutine(FindSedanChair());
        
        transform.localScale = Vector3.zero;
    }

    private void FixedUpdate()
    {
        return;
        if (sedanChair.history.Count > historyIndexOffset)
        {
            if (currentHistoryIndex == -1)
            {
                currentHistoryIndex = sedanChair.history.Count - historyIndexOffset - 1;
            }
            var m_targetPos = sedanChair.history[currentHistoryIndex];
            
            var dir = m_targetPos - transform.position;
            var dirNormalized = dir.normalized;
            var velocity =  sedanChair.moveSpeed * dirNormalized;

            if (sedanChair.m_rigidbody.velocity.magnitude > 0)
            {
                if (Vector3.Distance(transform.position, m_targetPos) < .05f)
                {
                    velocity = Vector3.zero;
                    transform.position = m_targetPos;
                    currentHistoryIndex++;
                }
            
                if (dirNormalized.magnitude > 0)
                {
                    m_child.forward = Vector3.Lerp(m_child.forward, dirNormalized, .5f);
                }
            }
            else
            {
                velocity = Vector3.zero;
            }
            
        
            m_rigidbody.velocity = velocity;

            if (velocity.magnitude > 0)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, .5f);
            }
        }
    }

    private IEnumerator FindSedanChair()
    {
        while (true)
        {
            sedanChair = FindObjectOfType<SedanChair>();
            if (sedanChair)
            {
                transform.position = sedanChair.transform.position;
                
                yield break;
            }
        }
    }
}
