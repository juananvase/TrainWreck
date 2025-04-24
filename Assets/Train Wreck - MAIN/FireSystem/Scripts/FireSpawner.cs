using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

public class FireSpawner : FireBase
{
    [SerializeField] private List<FireSpawnPoint> _fireSpawnPoints;
    [SerializeField] private BoolEventAsset OnFireSpawnInitiated;
    [SerializeField] private BoolEventAsset OnAllFiresDestroyed;

    private bool _canSpawnFire = true;
    private void OnEnable()
    {
        OnFireSpawnInitiated.AddListener(InitFire);
        OnAllFiresDestroyed.AddListener(ReadyToSpawnFire);
    }

    private void OnDisable()
    {
        OnFireSpawnInitiated.RemoveListener(InitFire);
    }

    [Button]
    private void FindSpawnPoints()
    {
        _fireSpawnPoints = GetComponentsInChildren<FireSpawnPoint>().ToList();
    }

    private void ReadyToSpawnFire(bool canSpawnFire)
    {
        _canSpawnFire = canSpawnFire;
    }

    private void InitFire(bool spawnFireInitiated)
    {
        if (spawnFireInitiated && _canSpawnFire)
        {
            OnAllFiresDestroyed?.Invoke(false);
            StartCoroutine(SpawnFireOnTrain());
        }
    }
    
    private IEnumerator SpawnFireOnTrain()
    {
        yield return new WaitForSeconds(1f);
        
        if (_fireSpawnPoints.Count == 0) yield return null;
        
        foreach (FireSpawnPoint fire in _fireSpawnPoints)
        {
            if(fire != null || _fireSpawnPoints != null)
            {
                GameObject fireInstance = Instantiate(FireData.FirePrefab, fire.transform.position, fire.transform.rotation);
                PopulateList(fireInstance);
                yield return new WaitForSeconds(FireData.FireSpawnDuration);
            }
        }
        yield return null;
    }
}