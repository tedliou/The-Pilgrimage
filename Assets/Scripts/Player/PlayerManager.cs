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
        if (!MPlayers.ContainsKey(id))
        {
            Debug.LogError($"玩家 {id} 不存在");
            return null;
        }

        return MPlayers[id];
    }
    
    public List<Player> GetPlayers()
    {
        return MPlayers.Values.Where(p => !p.IsKeyboardMouse).ToList();
    }

    #endregion

    public PlayerInputManager MInputManager
    {
        get
        {
            m_inputManager ??= GetComponent<PlayerInputManager>();
            return m_inputManager;
        }
    }
    private PlayerInputManager m_inputManager;

    protected Dictionary<int, Player> MPlayers
    {
        get
        {
            m_players ??= new Dictionary<int, Player>();
            return m_players;
        }
    }
    private Dictionary<int, Player> m_players;


    private void Start()
    {
        RegisterEvents();
    }




    private void RegisterEvents()
    {
        MInputManager.onPlayerJoined += OnJoined;
        MInputManager.onPlayerLeft += OnLeft;
    }

    private void OnJoined(PlayerInput playerInput)
    {
        var id = playerInput.playerIndex;
        var player = playerInput.GetComponent<Player>();
        Add(id, player);
        Log($"{playerInput.name} Joined");
    }

    private void OnLeft(PlayerInput playerInput)
    {
        var id = playerInput.playerIndex;
        Remove(id);
    }

    private void Add(int id, Player player)
    {
        if (!MPlayers.TryAdd(id, player))
        {
            Debug.LogError($"玩家 {0} 已存在");
            return;
        }
        
        OnPlayerJoined.Invoke(id, player);
    }

    private void Remove(int id)
    {
        if (!MPlayers.ContainsKey(id))
        {
            Debug.LogError($"玩家 {id} 不存在");
            return;
        }
        OnPlayerJoined.Invoke(id, MPlayers[id]);
        MPlayers.Remove(id);
    }
}
