using System.Collections;
using CustomFMODFunctions;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Extinguisher : Pickupable, IActivatable, IRespawnable
{
    [SerializeField] private GameObject _foamSpawnPoint;
    [SerializeField] private float _foamDestroyTime = 2f;
    [SerializeField] private Foam _foam;

    private bool _stillActive = false;
    private EventInstance _extinguisherInstance;
    public void Activate(bool value)
    {
        _stillActive = value;
        StartCoroutine(InitExtinguish());
    }

    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);
        RuntimeManager.PlayOneShot(AudioData.ExtinguisherPickupSFX, pickupAnchor.position);
    }

    public override void Drop()
    {
        base.Drop();
        RuntimeManager.PlayOneShot(AudioData.DropExtinguisherSFX, transform.position);
    }

    private IEnumerator InitExtinguish()
    {
        if (AudioInstanceHandler.CheckIfPlayingSFX(_extinguisherInstance) == false)
        {
            _extinguisherInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_extinguisherInstance, AudioData.ExtinguisherSFX);
        }
        while (_stillActive && IsBeingHeld)
        {
            Foam spawnedFoam = PoolSystem.Instance.Get(_foam, _foamSpawnPoint.transform.position, transform.rotation) as Foam;
            spawnedFoam?.ShootFoam(_foamSpawnPoint.transform);
            yield return null;
            StartCoroutine(InitializeCleanUp(spawnedFoam));   
        }
        AudioInstanceHandler.StopAndReleaseSFXInstance(_extinguisherInstance);
    }

    private IEnumerator InitializeCleanUp(Foam spawnedFoam)
    {
        yield return new WaitForSeconds(_foamDestroyTime);
        spawnedFoam.CleanUp();
    }

    private void OnDisable()
    {
        if(AudioInstanceHandler.CheckIfPlayingSFX(_extinguisherInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_extinguisherInstance);
        }
    }
}
