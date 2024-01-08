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

    public GameObject toolPrefab;
    public GameObject bombPrefab;
    public GameObject garbagePrefab;
    public GameObject gasPrefab;
    public GameObject roadPrefab;
    #endregion


    public static UnityEvent<Transform> onPlayerSpawn = new();
    
    #region Unity Messages

    private void Start()
    {
        // _gamePrefab.Init();
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerJoined);
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerLeft);
    }

    #endregion

    #region Events
    
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
        var sedanChair = FindObjectOfType<SedanChair>();
        spawnPos = sedanChair.transform.position - new Vector3(0, 0, 1);
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
