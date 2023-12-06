using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        dir.y = 0;
        ConsoleProDebug.Watch("Move", $"Player {_playerInput.playerIndex}: {dir}");
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        var angle = 0f;
        var rot = ctx.ReadValue<Vector2>();
        if (rot.magnitude >= 1)
        {
            angle = Mathf.Atan2(rot.x, rot.y) * Mathf.Rad2Deg;
        }
        ConsoleProDebug.Watch("Rotate", $"Player {_playerInput.playerIndex}: {angle}");
    }

    public void OnInteractive(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        
        ConsoleProDebug.Watch("Interactive", $"Player {_playerInput.playerIndex}: {ctx.performed}");
    }
}
