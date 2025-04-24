using CharacterSelect;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerList", menuName = "CoOp/PlayerList")]
public class PlayerList : ScriptableObject
{
    [SerializeField] private List<PlayerInfo> _players = new List<PlayerInfo>();

    private void OnEnable()
    {
        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        #endif
    }

    private void OnDisable()
    {
        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        #endif
    }


    public List<PlayerInfo> Players => _players;
    public int Count => _players.Count;

    public void AddPlayer(PlayerInfo playerInfo)
    {
        if (!Contains(playerInfo)) _players.Add(playerInfo);
    }

    public void RemovePlayer(PlayerInfo playerInfo)
    {
        if (Contains(playerInfo)) _players.Remove(playerInfo);
    }

    public void Clear()
    {
        _players = new List<PlayerInfo>();
    }

    private bool Contains(PlayerInfo playerInfo)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].Index == playerInfo.Index) return true;
        }

        return false;
    }

    public PlayerInfo Get(int index)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].Index == index) return _players[i];
        }

        return null;
    }

    #if UNITY_EDITOR
    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!EditorApplication.isPlaying)
        {
            Clear();
        }
    }
    #endif

    private void OnApplicationQuit()
    {
        Clear();
    }
}
