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

    public Indicator[] playerIndicators;
    public Indicator clipIndicator;
    #endregion


    public static UnityEvent<Transform> onPlayerSpawn = new();
    

    private void Start()
    {
        // _gamePrefab.Init();
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerJoined);
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerLeft);
    }


    #region Events
    
    private void OnPlayerJoined(int id, Player player)
    {
        //SpawnPlayerObject(player);
    }
    
    private void OnPlayerLeft(int id, Player player)
    {
        //DespawnPlayerObject(player);
    }
    #endregion

    public void SpawnAllPlayers()
    {
        var players = PlayerManager.Instance.GetPlayers();
        for (var i = 0; i < 4; i++)
        {
            if (i < players.Count)
            {
                var obj = SpawnPlayerObject(players[i], i);
                playerIndicators[i].SetFollowTransform(obj.transform);
            }
            else
            {
                playerIndicators[i].SetFollowTransform(null);
            }
            
        }
    }

    private Vector3 m_lastSpawnPos = new Vector3(0, -99, 0);
    private GameObject SpawnPlayerObject(Player player, int index)
    {
        if (m_lastSpawnPos.y == -99)
        {
            m_lastSpawnPos = SedanChair.Instance.transform.position - new Vector3(0, 0, 1);
        }
        
        var spawnPos = m_lastSpawnPos;
        while (GridSystem.Find(spawnPos, CellType.Top))
        {
            spawnPos += new Vector3(1, 0, 0);
        }
        spawnPos.y = playerYPos;
        m_lastSpawnPos = spawnPos;
        
        var playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().Player = player;
        
        onPlayerSpawn.Invoke(playerObj.transform);

        return playerObj;
    }

    public void DespawnPlayerObject(Player player)
    {
        Debug.Log("Despawn //NOTHING");
    }

    public void ResetPlayerPos(PlayerController player)
    {
        var spawnPos = SedanChair.Instance.transform.position - new Vector3(0, 0, 1);
        while (GridSystem.Find(spawnPos, CellType.Top))
        {
            spawnPos += new Vector3(1, 0, 0);
        }
        spawnPos.y = playerYPos;
        player.Rigidbody.MovePosition(spawnPos);
        Log($"Reset {player.name}'s Position");
    }
}
