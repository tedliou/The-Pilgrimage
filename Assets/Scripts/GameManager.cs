using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager current;
    #endregion

    #region Inspector
    public int targetFrameRate = 30;

    [Header("Position Settings")]
    public float playerYPos = 0;
    public float floorYPos = .5f;
    public float propsYPos = 0;

    [Header("Player Object")] [SerializeField]
    private GameObject playerPrefab;
    #endregion

    #region Private
    private PlayerInputManager _playerInputManager;
    

    #endregion

    #region Unity Messages
    private void Awake()
    {
        current = this;
        Application.targetFrameRate = targetFrameRate;
        _playerInputManager = FindObjectOfType<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += OnPlayerJoin;
    }
    #endregion

    #region Events
    private void OnPlayerJoin(PlayerInput playerInput)
    {
        var playerIndex = playerInput.playerIndex;
        var inputHandler = playerInput.GetComponent<PlayerInputHandler>();
        SpawnPlayerObject(playerIndex, inputHandler);
    }
    #endregion

    private void SpawnPlayerObject(int playerID, PlayerInputHandler inputHandler)
    {
        Debug.Log($"Spawn Player: P{playerID}");
        var spawnPos = Vector3.zero;
        var playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().SetInputHandler(inputHandler);
    }
}
