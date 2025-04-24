using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] protected PlayerModels _playerPrefab;
    [SerializeField] protected Transform[] _spawnLocation;
    [SerializeField] private PlayerList _readyPlayers;
    [SerializeField] private PlayerList _joinPlayers;

    protected PlayerInputManager _playerInputManager;

    protected virtual void Awake()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
    }

    protected virtual void Start()
    {
        if (_playerInputManager.playerCount > 0) 
        {
            Debug.LogWarning("Player Prefab detected on scene " +
                 "Please ensure that the player prefab is removed from the scene before committing or building.");
            return; 
        }

        if (_readyPlayers.Count != 4)
        {
            for (int i = 0; i < _joinPlayers.Count; i++)
            {
                _playerInputManager.playerPrefab = _playerPrefab.PlayerPrefab[i];
                //Debug.Log($"Spawning {_playerPrefab.PlayerPrefab[i]}");
                PlayerInfo info = _joinPlayers.Get(i);
                if (info == null) return;
                if (info.GetDevices() == null) return;

                PlayerInput input = _playerInputManager.JoinPlayer(i, i, "Gamepad", info.GetDevices());

                if (input == null) return;
                GameObject player = input.gameObject;
                player.transform.position = _spawnLocation[i].position;
                player.GetComponent<Rigidbody>().position = _spawnLocation[i].position;
                player.transform.SetParent(transform, true);
            }
        }
        else 
        {
            for (int i = 0; i < _readyPlayers.Count; i++)
            {
                _playerInputManager.playerPrefab = _playerPrefab.PlayerPrefab[i];
                //Debug.Log($"Spawning {_playerPrefab.PlayerPrefab[i]}");
                PlayerInfo info = _readyPlayers.Get(i);
                if (info == null) return;
                if (info.GetDevices() == null) return;

                PlayerInput input = _playerInputManager.JoinPlayer(i, i, "Gamepad", info.GetDevices());

                if (input == null) return;
                GameObject player = input.gameObject;
                player.transform.position = _spawnLocation[i].position;
                player.GetComponent<Rigidbody>().position = _spawnLocation[i].position;
                player.transform.SetParent(transform, true);
            }
        }
    }

}
