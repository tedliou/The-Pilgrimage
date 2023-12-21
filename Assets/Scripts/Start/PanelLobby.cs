using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelLobby : GamePanel
{
    public static PanelLobby Instance;
    
    public GameObject playerInputManager;

    public Player[] players = new Player[4];
    
    [Serializable]
    public class Player
    {
        public GameObject avatar;
        public GameObject[] placeholders;
    }

    public bool isPressA = false;
    public Image progress;
    public float pressDuration = 3;
    private float _currentPressDuration = 0;
    
    private PlayerInputManager _playerInputManager;

    private void Start()
    {
        Instance = this;
    }

    private void OnPlayerJoin(PlayerInput playerInput)
    {
        var index = playerInput.playerIndex;
        players[index].avatar.SetActive(true);
        foreach (var e in players[index].placeholders)
        {
            e.SetActive(false);
        }
    }
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        var index = playerInput.playerIndex;
        players[index].avatar.SetActive(false);
        foreach (var e in players[index].placeholders)
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

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _playerInputManager = FindObjectOfType<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += OnPlayerJoin;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        if (_playerInputManager)
        {
            Destroy(_playerInputManager);
            _playerInputManager = null;
        }
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