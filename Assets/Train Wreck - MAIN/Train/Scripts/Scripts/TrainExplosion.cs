using FMODUnity;
using UnityEngine;

public class TrainExplosion : TrainHealthStages
{
    [SerializeField] private GameObject _explsionSpawnPoint;
    [SerializeField] private GameObject _explsionVFXPrefab;
    [SerializeField] private AudioSourcesSOData _audioData;
    
    private GameObject _instance;

    protected override void OnTrainHealthUpdated(Vector2 health)
    {
        base.OnTrainHealthUpdated(health);

        if (trainHealth <= 0)
        {
            _instance = Instantiate(_explsionVFXPrefab, _explsionSpawnPoint.transform.position, _explsionSpawnPoint.transform.rotation);
            RuntimeManager.PlayOneShot(_audioData.TrainExplosionSFX);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(_instance != null)
            Destroy(_instance);
    }
}
