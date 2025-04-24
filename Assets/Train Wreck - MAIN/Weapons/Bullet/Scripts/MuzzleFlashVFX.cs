using UnityEngine;
using UnityEngine.VFX;

public class MuzzleFlashVFX : PooledObject
{
    private float _duration = 3f;
    private float _elapsedTime = 0f;
    [SerializeField] private VisualEffect _visualEffect;
    
    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _duration)
        {
            CleanUp();
        }
    }

    private void OnEnable()
    {
        _elapsedTime = 0f;
        _visualEffect.Play();
    }

    private void CleanUp()
    {
        _visualEffect.Stop();
        ReturnToPool();
    }
}
