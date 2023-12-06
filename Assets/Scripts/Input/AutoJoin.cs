using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class AutoJoin : MonoBehaviour
{
    
    
    private PlayerInputManager _playerInputManager;
    
    private void Awake()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        _playerInputManager.JoinPlayer();
        
    }
}
