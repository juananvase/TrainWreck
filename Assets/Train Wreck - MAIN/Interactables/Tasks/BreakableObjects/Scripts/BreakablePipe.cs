using CustomFMODFunctions;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.VFX;

public class BreakablePipe : BreakableObject
{
    [Header("Pipe Componenents")]
    [SerializeField] private GameObject _fixingPipeVFXPrefab;
    [SerializeField] private float _steamVFXFadeOutTime;
    
    public BreakablePipeDataSO BreakablePipeData => TaskData as BreakablePipeDataSO; //Cast to use TaskData data as BreakablePipeDataSO
    
    //SFX
    private EventInstance _steamSFXinstance;
    //VFX
    private GameObject _steamPrefabInstance;
    private VisualEffect _steamVFX;

    protected override void OnDisable()
    {
        base.OnDisable();

        //Stop SFX on disable to avoid passing through scenes since is looping
        if (AudioInstanceHandler.CheckIfPlayingSFX(_steamSFXinstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_steamSFXinstance);
        }
    }

    protected override void FullyRepairObject(HealingInfo healingInfo)
    {
        base.FullyRepairObject(healingInfo);
        //Stop the steam vfx
        EndSteam();
        //Play repair sfx
        PlaySFX(BreakableObjectData.AudioData.RepairedSFX);
    }

    protected override void Fixing(Interactor interactor, HealingInfo healingInfo)
    {
        _health?.Heal(healingInfo);

        if (_fixingVFXInstance == null)
            _fixingVFXInstance = Instantiate(_fixingPipeVFXPrefab, VFXSpawnPoint.position, Quaternion.identity);
        StartCoroutine(FillOnHeld(interactor, BreakableObjectData.TimeToFinishRepairing));
    }

    private void EndSteam()
    {
        //Check if there is an instance playing
        if (_steamPrefabInstance != null)
        {
            _steamVFX = _steamPrefabInstance.GetComponent<VisualEffect>();
            StartCoroutine(FadeOut(startAlpha, endAlpha, _steamVFXFadeOutTime, _steamVFX));
        }
        AudioInstanceHandler.StopAndReleaseSFXInstance(_steamSFXinstance);
    }


    protected override void BreakObject(DamageInfo damageInfo)
    {
        base.BreakObject(damageInfo);
        //Start steam vfx
        StartSteam();

        //Play repair broken
        PlaySFX(BreakableObjectData.AudioData.BrokenPipeSFX);
    }
    protected void StartSteam()
    {
        //Check if there is an instance playing
        if (BreakablePipeData.SteamVFXPrefab != null)
        {
            _steamPrefabInstance = Instantiate(BreakablePipeData.SteamVFXPrefab, VFXSpawnPoint.position, VFXSpawnPoint.rotation, transform);
        }

        //Store an instance of the steam sfx and play it
        _steamSFXinstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_steamSFXinstance,  BreakableObjectData.AudioData.SteamSFX);
        _steamSFXinstance.set3DAttributes(transform.position.To3DAttributes());
    }

    protected override void ConsumeResource(HealingInfo healingInfo)
    {
        base.ConsumeResource(healingInfo);

        //Play fixing sfx
        PlaySFX(BreakableObjectData.AudioData.PlaceMetalMaterial);
    }
}
