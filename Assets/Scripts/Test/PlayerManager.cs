using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : Singleton<PlayerManager>
{
    #region API

    public UnityEvent<int, Player> OnPlayerJoined;
    public UnityEvent<int, Player> OnPlayerLeft;

    public Player GetPlayer(int id)
    {
        if (!Players.ContainsKey(id))
        {
            Debug.LogError($"玩家 {id} 不存在");
            return null;
        }

        return Players[id];
    }
    
    public List<Player> GetPlayers()
    {
        return Players.Values.ToList();
    }

    #endregion
    
    
    #region Properties

    protected PlayerInputManager InputManager
    {
        get
        {
            _inputManager ??= GetComponent<PlayerInputManager>();
            return _inputManager;
        }
    }
    private PlayerInputManager _inputManager;

    protected Dictionary<int, Player> Players
    {
        get
        {
            _players ??= new Dictionary<int, Player>();
            return _players;
        }
    }
    private Dictionary<int, Player> _players;

    #endregion
    

    #region Unity Messages

    private void Awake()
    {
        RegisterEvents();
    }

    private void Start()
    {
        
    }

    #endregion


    #region Methods

    private void RegisterEvents()
    {
        InputManager.onPlayerJoined += OnJoined;
        InputManager.onPlayerLeft += OnLeft;
    }

    private void OnJoined(PlayerInput playerInput)
    {
        var id = playerInput.playerIndex;
        var player = playerInput.GetComponent<Player>();
        Add(id, player);
    }

    private void OnLeft(PlayerInput playerInput)
    {
        var id = playerInput.playerIndex;
        Remove(id);
    }

    private void Add(int id, Player player)
    {
        if (!Players.TryAdd(id, player))
        {
            Debug.LogError($"玩家 {0} 已存在");
            return;
        }
        
        OnPlayerJoined.Invoke(id, player);
    }

    private void Remove(int id)
    {
        if (!Players.ContainsKey(id))
        {
            Debug.LogError($"玩家 {id} 不存在");
            return;
        }
        OnPlayerJoined.Invoke(id, Players[id]);
        Players.Remove(id);
    }

    #endregion
}
