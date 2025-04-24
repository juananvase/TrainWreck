using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DebugMenu;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [field: SerializeField] public Transform RespawnPoint { get; private set; }
    [SerializeField] private GameObject _debugPlayer;

    [Header("Data")]
    [field: SerializeField, InlineEditor] public GameManagerDataSO GameManagerData { get; private set; }
    [field: SerializeField, InlineEditor] public EnemyDataConfigSO EnemyConfigs { get; private set; }

    [Header("Tutorial")]
    [field: SerializeField] public Sprite LevelTutorialImage { get; private set; }
    [field: SerializeField] public bool ShowTutorial { get; private set; } = false;

    [Header("Player Inputs")]
    [field: SerializeField] public List<PlayerInput> PlayerInputs { get; private set; } = new(); //A list of all players input, we get the items on start

    public EnemyData CurrentEnemyData { get; private set; } // enemy data based on the level 
    public static GameManager Instance { get; private set; }

     private void Awake()
     {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameManagerData.OnReceivingPlayerInputComponent.AddListener(AddToPlayerInputList);
        AssignEnemyData();
     }

    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
        
        GameManagerData.OnReceivingImageComponent.AddListener(SetTutorialImage);
    }
    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
        
        GameManagerData.OnReceivingPlayerInputComponent.RemoveListener(AddToPlayerInputList);
        GameManagerData.OnReceivingImageComponent.RemoveListener(SetTutorialImage);
        CleanPlayerInputList();
    }

    private void AssignEnemyData()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        
        switch (currentIndex)
        {
            case 2:
                CurrentEnemyData = EnemyConfigs.enemyConfigs[0];
                break;
            case 3:
                CurrentEnemyData = EnemyConfigs.enemyConfigs[1];
                break;
            case 4:
                CurrentEnemyData = EnemyConfigs.enemyConfigs[2];
                break;
            case 5:
                CurrentEnemyData = EnemyConfigs.enemyConfigs[3];
                break;
        }
    }

    private void SetTutorialImage(Image imageComponent)
    {
        imageComponent.sprite = LevelTutorialImage;
    }
    private void AddToPlayerInputList(PlayerInput playerInput)
    {
        PlayerInputs.Add(playerInput);
    }
    private void CleanPlayerInputList()
    {
        PlayerInputs.Clear();
    }
    
    // DEBUG
    #region DEBUG FUNCTIONS
    [DebugCommand("Spawn Player")]
    public void DebugSpawnPlayer()
    {
        Instantiate(_debugPlayer, RespawnPoint.position, Quaternion.identity);
        if (_debugPlayer != null)
        {
            if (_debugPlayer.TryGetComponent<PlayerInput>(out var debugPlayerInput))
            {
                AddToPlayerInputList(debugPlayerInput);
            }
        }
    }

    [DebugCommand("Break All Windows")]
    public void DebugBreakAllWindows()
    {
        BreakableWindow[] windows = FindObjectsByType<BreakableWindow>(FindObjectsSortMode.None); //TODO: change deprecated function
        if(windows != null)
        {
            for (int i = 0; i < windows.Length; i++)
            {
                if(windows[i].TryGetComponent(out Health health))
                {
                    DamageInfo damageInfo = new DamageInfo(100f, gameObject, gameObject, gameObject, DamageType.Debug);
                    health.Damage(damageInfo);
                }
            }
        }
    }
    
    [DebugCommand("Break All Pipes")]
    public void DebugBreakAllPipes()
    {
        BreakablePipe[] pipes = FindObjectsByType<BreakablePipe>(FindObjectsSortMode.None); //TODO: change deprecated function
        if(pipes != null)
        {
            for (int i = 0; i < pipes.Length; i++)
            {
                if(pipes[i].TryGetComponent(out Health health))
                {
                    DamageInfo damageInfo = new DamageInfo(100f, gameObject, gameObject, gameObject, DamageType.Debug);
                    health.Damage(damageInfo);
                }
            }
        }
    }
    #endregion
}
