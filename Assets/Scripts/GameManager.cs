using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : CustomBehaviour<GameManager>
{
    public static GameManager Instance;
    
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

    [FormerlySerializedAs("toolPrefab")] public GameObject clipPrefab;
    public GameObject bombPrefab;
    public GameObject gasPrefab;
    public GameObject garbagePrefab;
    public GameObject roadPrefab;
    public GameObject bombTinyPrefab;

    public Indicator[] playerIndicators;
    public Indicator clipIndicator;
    public Indicator bombIndicator;
    public Indicator trayIndicator;
    public Indicator foodIndicator;
    public Indicator distanceIndicator;

    public TMP_Text distanceText;
    public TMP_Text speedText;
    public TMP_Text timeText;

    public AudioSource distanceSFX;

    public bool CanMove => foodAmount > 0;
    public int foodAmount = 100;
    public int stepFood = 5;
    #endregion


    public static UnityEvent<Transform> onPlayerSpawn = new();

    public DateTime startTime;

    private void Start()
    {
        Instance = this;
        // _gamePrefab.Init();
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerJoined);
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerLeft);

        startTime = DateTime.Now;
        
        SedanChair.OnMoved.AddListener(() =>
        {
            foodAmount -= stepFood;
            foodAmount = Mathf.Clamp(foodAmount, 0, 100);
        });
    }

    private void Update()
    {
        distanceText.text = $"{SedanChair.Instance.m_nodeIndex}M";
        var speed = SedanChair.Instance.currentSpeed * 100;
        speedText.text = $"{Convert.ToInt32(speed)}CM/s";

        var sec = (DateTime.Now - startTime).TotalSeconds;
        timeText.text = $"{(Convert.ToInt32(sec) / 60).ToString("00")}:{(Convert.ToInt32(sec) % 60).ToString("00")}";

        distanceIndicator.SetFollowTransform(SedanChair.Instance.transform);
        distanceIndicator.gameObject.SetActive(SedanChair.Instance.m_nodeIndex >= RoadBlock.Nodes.Count - 2);
        
        foodIndicator.SetFollowTransform(SedanChair.Instance.transform);
        if (foodAmount < 10)
        {
            foodIndicator.gameObject.SetActive(true);
            if (!distanceSFX.isPlaying)
            {
                distanceSFX.Play();
            }
        }
        else
        {
            foodIndicator.gameObject.SetActive(false);
            if (distanceSFX.isPlaying)
            {
                distanceSFX.Stop();
            }
        }
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
