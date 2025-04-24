using System;
using DebugMenu;
using UnityEngine;
using UnityEngine.InputSystem;
using GameEvents;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInputManager))]
public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] protected PlayerColors _playerColors;
    [SerializeField] protected PlayerNames _playerNames;
    [SerializeField] protected PlayerIcons _playerIcon;
    [SerializeField] protected CharacSelectMesh _playerSelectMesh;
    [SerializeField] protected PlayerInfoEventAsset _playerJoinedEvent;
    [SerializeField] private BoolEventAsset OnAllPlayersReady;
    [SerializeField] private BoolEventAsset OnSelectedSinglePlayerMode;
    [SerializeField] protected PlayerList _joinedList; // List to keep track of players who joined
    [SerializeField] protected PlayerList _readyList;  // List to keep track of players who are ready
    [SerializeField] protected string _gameplayScene;  // The name of the gameplay scene
    [SerializeField] protected PlayerModels _playerPrefabs;  // Player prefabs for instantiation

    [SerializeField] private List<GameObject> _playersUIPositions = new();

    [SerializeField] public int RequiredNumberOfPlayers;

    protected virtual void Awake()
    {
        _joinedList.Clear();  // Clear any previously stored players
        _readyList.Clear();   // Clear ready players list
    }

    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
        OnSelectedSinglePlayerMode.AddListener(SetSinglePlayerMode);
    }

    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
        OnSelectedSinglePlayerMode.RemoveListener(SetSinglePlayerMode);
    }

    private void SetSinglePlayerMode(bool value)
    {
        RequiredNumberOfPlayers = value ? 1 : 4;
    }


    public virtual void OnPlayerJoined(PlayerInput playerInput)
    {
        //playerInput.transform.SetParent(transform, false);

        Transform playerUIPosition = null;
        foreach (GameObject uiPosition in _playersUIPositions) 
        {
            if (uiPosition.transform.childCount > 0) 
                continue;

            playerUIPosition = uiPosition.transform;
        }
        playerInput.transform.SetParent(playerUIPosition, false);
        playerInput.transform.position = playerUIPosition.position;

        int playerIndex = playerInput.playerIndex; 
        Color color = _playerColors.Colors[playerIndex];
        Sprite icon = _playerIcon.Icons[playerIndex];
        string name = _playerNames.PlayerName[playerIndex];
        CharacterSelectPlayer mesh = _playerSelectMesh.CharacterSelectPlayer[playerIndex];
        playerInput.GetComponent<PlayerMenu>().Init(color, icon, name, mesh);
        PlayerInfo playerInfo = new PlayerInfo(playerIndex, color, playerInput.devices);



        _joinedList.AddPlayer(playerInfo);
        _playerJoinedEvent.Invoke(playerInfo);
    }

    public void MarkPlayerReady(int playerIndex)
    {
        // Find the player in the _joinedList by their index
        PlayerInfo player = _joinedList.Get(playerIndex);

        if (player != null)
        {
            // Mark the player as ready
            player.SetReady(true);
        }

        // Check if all players are ready
        CheckQueue();
    }

    public virtual void CheckQueue()
    {
        // Check if every player in the joined list is ready
        bool allPlayersReady = false;
        if (_joinedList.Count == RequiredNumberOfPlayers) 
        {
            if (_readyList.Count == RequiredNumberOfPlayers) 
            { 
                allPlayersReady = true;
            }
        }

        // If all players are ready, transition to the gameplay scene
        if (allPlayersReady)
        {
            OnAllPlayersReady.Invoke(true);  // Load the gameplay scene
        }
    }
    
    // DEBUG
    [DebugCommand("Skip Ready Players")]
    public void DebugReadyAllPlayers()
    {
        OnAllPlayersReady.Invoke(true);
    }
}
