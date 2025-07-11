using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : Singleton<LobbyUI>
{
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
    
    
    /// 拿到所有玩家
    /// 在玩家加入和離開時更新資料
    

    protected void OnEnable()
    {
        if (PlayerManager.Instance.GetPlayers().Count == 0)
        {
            return;
        }
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
        if (PlayerManager.Instance.GetPlayers().Count == 0)
        {
            return;
        }
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

    public void Show()
    {
        gameObject.SetActive(true);
        HomeUI.Instance.Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnInit()
    {
        base.OnInit();
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
                StartCoroutine(LoadScene());
            }
        }
        else
        {
            _currentPressDuration = 0;
            progress.fillAmount = 0;
        }
    }

    private IEnumerator LoadScene()
    {
        FadeImage.Instance.Hide();
        yield return new WaitForSeconds(2);
        LobbyUI.Instance.Hide();
        SceneManager.LoadSceneAsync(1);
    }
}