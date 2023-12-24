using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(PlayerInputHandler))]
public class Player : MonoBehaviour
{
    #region API

    public int Id
    {
        get
        {
            if (_id < 0)
            {
                _id = Input.playerIndex;
            }
            return _id;
        }
    }
    private int _id = -1;

    public PlayerInputHandler InputHandler
    {
        get
        {
            _inputHandler ??= GetComponent<PlayerInputHandler>();
            return _inputHandler;
        }
    }
    private PlayerInputHandler _inputHandler;

    #endregion
    
    
    #region Properties

    protected PlayerInput Input
    {
        get
        {
            _input ??= GetComponent<PlayerInput>();
            return _input;
        }
    }
    private PlayerInput _input;

    protected PlayerController Controller
    {
        get
        {
            _controller = GetComponent<PlayerController>();
            return _controller;
        }
    }
    private PlayerController _controller;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        name = $"[Player] {Id}";
    }

    #endregion

    
    #region Methods

    

    #endregion
}
