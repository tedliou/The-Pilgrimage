using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput), typeof(MultiplayerEventSystem))]
public class PlayerInputHandler : MonoBehaviour
{
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
    
    protected InputSystemUIInputModule InputModule
    {
        get
        {
            _inputModule ??= GetComponent<InputSystemUIInputModule>();
            return _inputModule;
        }
    }
    private InputSystemUIInputModule _inputModule;
    
    protected MultiplayerEventSystem EventSystem
    {
        get
        {
            _eventSystem ??= GetComponent<MultiplayerEventSystem>();
            return _eventSystem;
        }
    }
    private MultiplayerEventSystem _eventSystem;

    #endregion
    
    
    #region API
    public UnityEvent<Vector2> onPlayerMove;
    public UnityEvent<float> onPlayerLook;
    [FormerlySerializedAs("onPlayerGet")] public UnityEvent OnPlayerGet;
    [FormerlySerializedAs("onPlayerGetCancel")] public UnityEvent OnPlayerGetCancel;
    public UnityEvent onPlayerFire;
    public UnityEvent onPlayerFireCancel;
    #endregion

    #region Unity Messages
    private void Awake()
    {
        InputModule.actionsAsset = Input.actions;
        Input.SwitchCurrentActionMap("Player");
        Input.currentActionMap.FindAction("Move").performed += OnMove;
        Input.currentActionMap.FindAction("Move").canceled += OnMove;
        Input.currentActionMap.FindAction("Look").performed += OnLook;
        Input.currentActionMap.FindAction("Get").performed += OnGet;
        Input.currentActionMap.FindAction("Get").canceled += OnGetCancel;
        Input.currentActionMap.FindAction("Fire").performed += OnFire;
        Input.currentActionMap.FindAction("Fire").canceled += OnFireCancel;
        Input.currentActionMap.FindAction("Active").performed += OnActive;
    }
    #endregion

    #region Events
    private void OnMove(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        onPlayerMove.Invoke(dir);
        ConsoleProDebug.Watch("Move", $"Player {Input.playerIndex}: {dir}");
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
        ConsoleProDebug.Watch("Rotate", $"Player {Input.playerIndex}: {angle}");
    }
    
    
    private void OnGet(InputAction.CallbackContext ctx)
    {
        OnPlayerGet.Invoke();
        ConsoleProDebug.Watch("Interactive", $"Player {Input.playerIndex}: {ctx.performed}");
    }
    private void OnGetCancel(InputAction.CallbackContext ctx)
    {
        OnPlayerGetCancel.Invoke();
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        onPlayerFire.Invoke();
        ConsoleProDebug.Watch("Interactive", $"Player {Input.playerIndex}: {ctx.performed}");
    }
    private void OnFireCancel(InputAction.CallbackContext ctx)
    {
        onPlayerFireCancel.Invoke();
    }

    private void OnActive(InputAction.CallbackContext _)
    {
        if (!EventSystem.currentSelectedGameObject)
        {
            EventSystem.SetSelectedGameObject(FirstSelectObject.Find());
        }
    }
    #endregion
}
