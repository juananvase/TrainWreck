using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using CharacterSelect;
using Unity.VisualScripting;

public class JoinPlayer : MonoBehaviour
{
    public List<PlayerInput> playerList = new List<PlayerInput>(); //Store players information
    public GameObject[] playerModel = new GameObject[4]; //Store colors for players
    [SerializeField] protected Transform[] _spawnLocation;
    private Vector3 spawnPlayer = new Vector3(0, 0, 0);

    [SerializeField] InputAction joinAction; //Join action input reference
    private const int maxPlayers = 4; //Set max amount of players

    //Track active players using a list of PlayerInput components
    private List<PlayerInput> activePlayers = new List<PlayerInput>();

    public PlayerInputManager playerInputManager;

    private void Awake()
    {
        joinAction.Enable();
    }
    private void Start()
    {
        //Ensure the player list is empty at the start
        activePlayers.Clear();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnJoin;
    }

    void OnJoin(PlayerInput playerInput)
    {
        TryJoinPlayer(playerInput);
        AssignCharacter(playerInput);
        SpawnOnLocation(playerInput);

    }

    public void TryJoinPlayer(PlayerInput playerInput)
    {
        if (activePlayers.Count < maxPlayers)
        {
            if (!activePlayers.Contains(playerInput))
            {
                activePlayers.Add(playerInput);
            }
        }
    }

    public void RemovePlayer(PlayerInput playerInput)
    {
        if (activePlayers.Contains(playerInput))
        {
            activePlayers.Remove(playerInput);
        }
    }

    private void AssignCharacter(PlayerInput playerInput)
    {
        // Get the player index
        int playerIndex = playerInput.playerIndex;

        // Ensure the player index is within a valid range
        if (playerIndex >= 0 && playerIndex < playerModel.Length)
        {
            // If the player already has a model, destroy it first (if you want to replace the model)
            if (playerInput.transform.childCount > 0)
            {
                Transform firstChild = playerInput.transform.GetChild(0); // Get the first child

                Destroy(firstChild.gameObject); // Destroy previous model
            }

            // Instantiate the new player model and parent it to the player
            GameObject model = Instantiate(playerModel[playerIndex], new Vector3(playerInput.transform.position.x, playerInput.transform.position.y - 0.94f, playerInput.transform.position.z), Quaternion.identity);
            model.transform.SetParent(playerInput.transform);  // Set the model as a child of the player
        }
    }

    private void SpawnOnLocation(PlayerInput playerInput)
    {
        playerInput.transform.position = _spawnLocation[playerInput.playerIndex].position;
        playerInput.GetComponent<Rigidbody>().position = _spawnLocation[playerInput.playerIndex].position;
    }
}
