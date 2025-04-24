using UnityEngine;
using UnityEngine.VFX;

public class FoamVFX : PooledObject
{
    private float _duration = 1f;
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
        _visualEffect.transform.position = transform.forward;
    }

    private void CleanUp()
    {
        ReturnToPool();
    }
}
