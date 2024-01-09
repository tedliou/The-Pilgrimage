using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class SedanChairCamera : CustomBehaviour<SedanChairCamera>
{
    private CinemachineVirtualCamera m_camera;
    
    private void Start()
    {
        m_camera = GetComponent<CinemachineVirtualCamera>();

        // Check Sedan Chair Exist
        var sedanChair = FindObjectOfType<SedanChair>();
        if (sedanChair)
        {
            m_camera.Follow = sedanChair.transform;
            m_camera.LookAt = sedanChair.transform;
        }
        else
        {
            SedanChair.OnSedanChairCreate.AddListener(sedanChair =>
            {
                m_camera.Follow = sedanChair.transform;
                m_camera.LookAt = sedanChair.transform;
            });
        }
    }
}
