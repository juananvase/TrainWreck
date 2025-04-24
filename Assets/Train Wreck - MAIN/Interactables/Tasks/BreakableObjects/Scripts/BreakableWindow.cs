using Sirenix.OdinInspector;
using UnityEngine;

public class BreakableWindow : BreakableObject
{
    //VFX
    [SerializeField, FoldoutGroup("VFX")] private GameObject _glassBreakingVFX;

    protected override void ConsumeResource(HealingInfo healingInfo)
    {
        base.ConsumeResource(healingInfo);

        //Play sfx on placed resource
        PlaySFX(BreakableObjectData.AudioData.PlaceWoodMaterial);
    }
    protected override void FullyRepairObject(HealingInfo healingInfo)
    {
        base.FullyRepairObject(healingInfo);

        //Allow the collider block other objects
        Collider.isTrigger = false;

        //Play sfx on repair object
        PlaySFX(BreakableObjectData.AudioData.RepairedSFX);
    }
    protected override void BreakObject(DamageInfo damageInfo)
    {
        base.BreakObject(damageInfo);

        //Make the collider NOT block other objects
        Collider.isTrigger = true;

        //Play sfx on broken object
        PlaySFX(BreakableObjectData.AudioData.BrokenWindowSFX);

        //Play VFX 
        if (_glassBreakingVFX != null)
        {
            GameObject glassInstance = Instantiate(_glassBreakingVFX, VFXSpawnPoint.position, VFXSpawnPoint.rotation);
            Destroy(glassInstance, 1f);
        }
    }
}
