using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    #region API

    public BlockBase GetPrefab(string id) => _gamePrefab.GetPrefab(id);

    #endregion
    
    
    #region Properties
    
    protected PlayerManager PlayerManager
    {
        get
        {
            _playerManager ??= PlayerManager.Instance;
            return _playerManager;
        }
    }
    private PlayerManager _playerManager;
    
    protected PlayerInputManager PlayerInputManager
    {
        get
        {
            _playerInputManager ??= FindObjectOfType<PlayerInputManager>();
            return _playerInputManager;
        }
    }
    private PlayerInputManager _playerInputManager;

    #endregion

    #region Inspector

    [SerializeField] private GamePrefab _gamePrefab;
    
    public int targetFrameRate = 30;

    [Header("Position Settings")]
    public float playerYPos = 0;
    public float floorYPos = .5f;
    public float propsYPos = 0;

    [Header("Player Object")] [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] private Vector3 playerSpawnPos;
    #endregion


    public static UnityEvent<Transform> onPlayerSpawn = new();
    
    #region Unity Messages

    private void Start()
    {
        _gamePrefab.Init();
        Application.targetFrameRate = targetFrameRate;
        PlayerManager.OnPlayerJoined.AddListener(OnPlayerJoined);
        PlayerManager.OnPlayerJoined.AddListener(OnPlayerLeft);
    }

    #endregion

    #region Events
    // private void OnPlayerJoin(PlayerInput playerInput)
    // {
    //     var playerIndex = playerInput.playerIndex;
    //     var inputHandler = playerInput.GetComponent<PlayerInputHandler>();
    //     SpawnPlayerObject(playerIndex, inputHandler);
    // }

    private void OnPlayerJoined(int id, Player player)
    {
        SpawnPlayerObject(player);
    }
    
    private void OnPlayerLeft(int id, Player player)
    {
        DespawnPlayerObject(player);
    }
    #endregion

    private void SpawnPlayerObject(Player player)
    {
        var spawnPos = playerSpawnPos;
        spawnPos.y = playerYPos;
        
        var playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().Player = player;
        
        onPlayerSpawn.Invoke(playerObj.transform);
    }

    private void DespawnPlayerObject(Player player)
    {
        Debug.Log("Despawn //NOTHING");
    }
}
