using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//IMPORTANT
using Random = UnityEngine.Random;
using GameEvents;
using DebugMenu;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemies")]
    [field: SerializeField] public Transform _enemySpawnLocation; //Transform where enemies will spawn
    [SerializeField] private bool _hasStartedSpawning = false;
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] private EnemyController _enemyPrefab;

    [Header("Data")]
    [SerializeField] private TrainDataSO _trainData; //Total duration of the level

    //Serialized list of windows in the scene
    [Header("Windows")]
    [SerializeField] private List<EnemyMonitor> enemyMonitors = new List<EnemyMonitor>(); //List of windows in the scene, MUST fill out
    private int currentMonitorIndex = 0;  //Index to track which monitor to spawn enemy at

    [Header("Listening Events")]
    [SerializeField] private GameObjectEventAsset OnBreakObject;
    [SerializeField] private GameObjectEventAsset OnRepairObject;
    [SerializeField] private BoolEventAsset OnInitSpawningEnemies;
    [SerializeField] private BoolEventAsset OnLevelWon;

    private void Start()
    {
        FindWindows();
    }

    private void FindWindows() 
    {
        var enemyWindows = GetComponentsInChildren<EnemyMonitor>();
        if (enemyWindows.Length == 0) 
        {
            Debug.LogWarning("No windows found, make sure to attach EnemyMonitor script to windows");
            return;
        }

        foreach (EnemyMonitor window in enemyWindows)
        {
            if (window != null)
            {
                enemyMonitors.Add(window);      
            }
        }
    }

    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
        OnInitSpawningEnemies.AddListener(InitSpawningEnemies);
        OnLevelWon.AddListener(StopSpawningEnemies);
    }

    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
        OnInitSpawningEnemies.RemoveListener(InitSpawningEnemies);
        OnLevelWon.RemoveListener(StopSpawningEnemies);
    }

    private void InitSpawningEnemies(bool addedFuel)
    {
        // Debug.Log(addedFuel);
        if (addedFuel && !_hasStartedSpawning)
        {
            
            StartCoroutine(SpawnEnemyRoutine());
            _hasStartedSpawning = true;
            _spawnEnemies = true;
        }
    }

    private void StopSpawningEnemies(bool levelEnd) 
    { 
        _hasStartedSpawning = false;
        _spawnEnemies = false;
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.InitialDelay);
        while (_spawnEnemies)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.EnemySpawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        //Get assigned monitor for the current index
        EnemyMonitor availableMonitor = GetAvailableMonitorForIndex(currentMonitorIndex);

        //Debug.Log($"Spawning Enemies at {currentWindowIndex}");
        if (availableMonitor != null)
        {
            EnemyAttackPoint spawnPoint = availableMonitor.GetRandomAvailablePoint();
            //If a spawn point is found, instantiate and assign the enemy
            if (spawnPoint != null)
            {
                EnemyController spawnedEnemy = Instantiate(_enemyPrefab, _enemySpawnLocation.position, _enemySpawnLocation.rotation, transform);
                availableMonitor.AssignEnemy(spawnedEnemy, spawnPoint); //Assign the spawned enemy to the monitor point
                spawnedEnemy.Init(spawnPoint.transform, this); //Call Init function in Enemy Controller to start State Machine
            }

        }

        if (enemyMonitors.Count > 0)
        {
            currentMonitorIndex = (currentMonitorIndex + 1) % enemyMonitors.Count;
        }
        else
        {
            Debug.Log("No enemy monitors available");
        }
    }

    public void ReleaseAttackPoint(EnemyAttackPoint enemyPoint)
    {
        foreach (var monitor in enemyMonitors)
        {
            if (monitor.ContainsPoint(enemyPoint))
            {
                monitor.RemoveEnemyFromPoint(enemyPoint);
                break;
            }
        }
    }

    private EnemyMonitor GetAvailableMonitorForIndex(int index)
    {
        foreach (var monitor in enemyMonitors)
        {
            if (monitor.HasAvailablePoints())
            {
                return monitor; //Return the monitor when the count matches the index
            }
        }
        return null; //Return null if no available monitor is found
    }

    //DEBUG
    [DebugCommand("Spawn Enemies")]
    public void DebugResetEnemySpawnDelay()
    {
        InitSpawningEnemies(true);
    }
}
