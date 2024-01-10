using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput), typeof(MultiplayerEventSystem))]
public class PlayerInputHandler : CustomBehaviour<PlayerInputHandler>
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
    public UnityEvent OnPlayerGet;
    public UnityEvent OnPlayerGetCancel;
    public UnityEvent onPlayerFire;
    public UnityEvent onPlayerFireCancel;
    public UnityEvent onPlayerReset = new();
    public UnityEvent onPlayerBack = new();

    public bool isLooking;
    #endregion

    private float m_lastAngle;

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
        Input.currentActionMap.FindAction("Reset").performed += OnReset;
        Input.currentActionMap.FindAction("Back").performed += OnBack;

    }
    #endregion

    #region Events
    private void OnMove(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        onPlayerMove.Invoke(dir);
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        // var rot = ctx.ReadValue<Vector2>();
        // // if (rot.magnitude >= .9)
        // // {
        // //     isLooking = true;
        // //     m_lastAngle = Mathf.Atan2(rot.x, rot.y) * Mathf.Rad2Deg;
        // // }
        // // else
        // // {
        // //     isLooking = false;
        // // }
        //
        // isLooking = true;
        // m_lastAngle = Mathf.Atan2(rot.x, rot.y) * Mathf.Rad2Deg;
        // Log(rot);
        //
        // onPlayerLook.Invoke(m_lastAngle);
    }
    
    
    private void OnGet(InputAction.CallbackContext ctx)
    {
        OnPlayerGet.Invoke();
    }
    private void OnGetCancel(InputAction.CallbackContext ctx)
    {
        OnPlayerGetCancel.Invoke();
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        onPlayerFire.Invoke();
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
    
    private void OnReset(InputAction.CallbackContext _)
    {
        onPlayerReset.Invoke();
    }
    private void OnBack(InputAction.CallbackContext _)
    {
        onPlayerBack.Invoke();
    }
    #endregion
}
