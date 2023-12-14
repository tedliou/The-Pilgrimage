using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    
    #region Public
    public UnityEvent<Vector2> onPlayerMove;
    public UnityEvent<float> onPlayerLook;
    public UnityEvent onPlayerGet;
    public UnityEvent onPlayerFire;
    #endregion
    
    #region Private
    private PlayerInput _playerInput;
    #endregion

    #region Unity Messages
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
        _playerInput.currentActionMap.FindAction("Move").performed += OnMove;
        _playerInput.currentActionMap.FindAction("Move").canceled += OnMove;
        _playerInput.currentActionMap.FindAction("Look").performed += OnLook;
        _playerInput.currentActionMap.FindAction("Get").performed += OnGet;
        _playerInput.currentActionMap.FindAction("Fire").performed += OnFire;
    }
    #endregion

    #region Events
    private void OnMove(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        onPlayerMove.Invoke(dir);
        ConsoleProDebug.Watch("Move", $"Player {_playerInput.playerIndex}: {dir}");
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        var angle = 0f;
        var rot = ctx.ReadValue<Vector2>();
        if (rot.magnitude >= 1)
        {
            angle = Mathf.Atan2(rot.x, rot.y) * Mathf.Rad2Deg;
        }
        onPlayerLook.Invoke(angle);
        ConsoleProDebug.Watch("Rotate", $"Player {_playerInput.playerIndex}: {angle}");
    }
    
    
    private void OnGet(InputAction.CallbackContext ctx)
    {
        onPlayerGet.Invoke();
        ConsoleProDebug.Watch("Interactive", $"Player {_playerInput.playerIndex}: {ctx.performed}");
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        onPlayerFire.Invoke();
        ConsoleProDebug.Watch("Interactive", $"Player {_playerInput.playerIndex}: {ctx.performed}");
    }
    #endregion
}
