using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelLobby : GamePanel
{
    public static PanelLobby Instance;
    
    public GameObject playerInputManager;

    public PlayerSetting[] players = new PlayerSetting[4];
    
    [Serializable]
    public class PlayerSetting
    {
        public GameObject avatar;
        public GameObject[] placeholders;
    }

    public bool isPressA = false;
    public Image progress;
    public float pressDuration = 3;
    private float _currentPressDuration = 0;

    private Dictionary<int, Player> _players;
    
    // private PlayerInputManager _playerInputManager;

    private void Start()
    {
        Instance = this;
    }
    
    
    /// 拿到所有玩家
    /// 在玩家加入和離開時更新資料
    

    protected override void OnEnable()
    {
        base.OnEnable();

        LoadPlayers();
        PlayerManager.Instance.OnPlayerJoined.AddListener(OnPlayerJoined);
        PlayerManager.Instance.OnPlayerLeft.AddListener(OnPlayerLeft);
        PlayerManager.Instance.GetPlayer(0)?.InputHandler.OnPlayerGet.AddListener(SetPressActive);
        PlayerManager.Instance.GetPlayer(0)?.InputHandler.OnPlayerGetCancel.AddListener(SetPressCancel);

        // _playerInputManager = FindObjectOfType<PlayerInputManager>();
        // _playerInputManager.onPlayerJoined += OnPlayerJoin;
        // _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerJoined.RemoveListener(OnPlayerJoined);
        PlayerManager.Instance.OnPlayerLeft.RemoveListener(OnPlayerLeft);
        PlayerManager.Instance.GetPlayer(0)?.InputHandler.OnPlayerGet.RemoveListener(SetPressActive);
        PlayerManager.Instance.GetPlayer(0)?.InputHandler.OnPlayerGetCancel.RemoveListener(SetPressCancel);
        // if (_playerInputManager)
        // {
        //     Destroy(_playerInputManager);
        //     _playerInputManager = null;
        // }
    }

    private void SetPressActive()
    {
        isPressA = true;
    }

    private void SetPressCancel()
    {
        isPressA = false;
    }

    private void LoadPlayers()
    {
        foreach (var e in players)
        {
            e.avatar.SetActive(false);
            foreach (var f in e.placeholders)
            {
                f.SetActive(true);
            }
        }
        Debug.Log(PlayerManager.Instance.GetPlayers().Count);
        foreach (var e in PlayerManager.Instance.GetPlayers())
        {
            players[e.Id].avatar.SetActive(true);
            foreach (var f in players[e.Id].placeholders)
            {
                f.SetActive(false);
            }
        }
    }
    
    // 之後要改成設定檔來定義 Avatar 圖示
    private void OnPlayerJoined(int id, Player player)
    {
        players[id].avatar.SetActive(true);
        foreach (var e in players[id].placeholders)
        {
            e.SetActive(false);
        }
    }
    
    private void OnPlayerLeft(int id, Player player)
    {
        players[id].avatar.SetActive(false);
        foreach (var e in players[id].placeholders)
        {
            e.SetActive(true);
        }
    }

    protected override void OnPanelAction(PanelOption option)
    {
        base.OnPanelAction(option);
        SetDisplay(option);
    }

    private void SetDisplay(PanelOption option)
    {
        gameObject.SetActive(option == PanelOption.Lobby);
    }

    private void Update()
    {
        if (isPressA)
        {
            _currentPressDuration += Time.deltaTime;
            var p = _currentPressDuration / pressDuration;
            progress.fillAmount = p;
            if (p > 1)
            {
                Debug.Log("Game Start!");
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            _currentPressDuration = 0;
            progress.fillAmount = 0;
        }
    }
}