using System.Collections;
using DebugMenu;
using UnityEngine;

public class TrainBase : MonoBehaviour, IBombDamageable
{
    [Header("Data's")]
    [SerializeField] protected TrainDataSO trainData;
    [Header("Components")]
    [SerializeField] protected Health trainHealth;

    private bool _hasTakenDamageFromBomb;
    protected virtual void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
    }
    protected virtual void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
    }

    public void TakeDamageFromBomb(Bomb bomb)
    {
        if (_hasTakenDamageFromBomb)
            return;
        
        _hasTakenDamageFromBomb = true;
        DamageInfo damageInfo = new DamageInfo(bomb.BombData.DamageToTrain, gameObject, bomb.gameObject, bomb.gameObject, DamageType.Bomb);
        trainHealth?.Damage(damageInfo);
        StartCoroutine(ResetDamageFlagAfterDelay());
    }

    private IEnumerator ResetDamageFlagAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        _hasTakenDamageFromBomb = false;
    }

    // DEBUG
    
    [DebugCommand("Damage (10) Train")]
    public void DebugDamageTrain()
    {
        DamageInfo damageInfo = new DamageInfo(10f, gameObject, gameObject, gameObject, DamageType.Debug);
        trainHealth?.Damage(damageInfo);
    }

    [DebugCommand("Heal (10) Train")]
    public void DebugHealTrain()
    {
        HealingInfo healingInfo = new HealingInfo(10f, gameObject, gameObject, gameObject, HealType.Debug);
        trainHealth?.Heal(healingInfo);
    }
}
