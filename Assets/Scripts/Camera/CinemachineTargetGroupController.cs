using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTargetGroupController : MonoBehaviour
{
    #region Private

    private CinemachineTargetGroup _targetGroup;

    #endregion
    
    #region Unity Messages
    private void Awake()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
        GameManager.onPlayerSpawn.AddListener(OnPlayerJoined);
    }

    #endregion

    #region Events
    private void OnPlayerJoined(Transform playerObject)
    {
        _targetGroup.AddMember(playerObject, 1, 0);
    }
    #endregion
}
