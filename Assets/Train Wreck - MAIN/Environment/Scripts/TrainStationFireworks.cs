using System.Collections;
using FMODUnity;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

public class TrainStationFireworks : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private GameObject _fireworkSpawnPoint;
    [SerializeField] private GameObject _fireworkVFXPrefab;
    [SerializeField] private BoolEventAsset _onLevelWin;
    [SerializeField] private AudioSourcesSOData _audioData;

    private GameObject _instance;

    private void OnEnable()
    {
        _onLevelWin.AddListener(StartFireworks);
    }
    private void OnDisable()
    {
        _onLevelWin.RemoveListener(StartFireworks);
        Destroy(_instance);
    }

    [Button(Icon = SdfIconType.Apple)]
    public void StartFireworks(bool arg0)
    {
        StartCoroutine(SpawnFireworks());
    }

    private IEnumerator SpawnFireworks() 
    {
        yield return new WaitForSeconds(_delay);

        GameObject fireworkInstance = Instantiate(_fireworkVFXPrefab, _fireworkSpawnPoint.transform.position, _fireworkSpawnPoint.transform.rotation);
        fireworkInstance.transform.parent = _fireworkSpawnPoint.transform;

        RuntimeManager.PlayOneShot(_audioData.TrainExplosionSFX);
    }
}
