using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using GameEvents;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    public float MaxHealth { get; private set; } = 1f;
    [BoxGroup("Debug"), ShowInInspector] private float _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    //Important data and SOs
    [SerializeField, FoldoutGroup("Data")] private ScriptableObject _healthSO;
    [BoxGroup("Debug"), ShowInInspector, FoldoutGroup("Data")] public float Percentage => _currentHealth / MaxHealth;
    [BoxGroup("Debug"), ShowInInspector, FoldoutGroup("Data")] public bool IsFullyCured => _currentHealth >= MaxHealth;
    [BoxGroup("Debug"), ShowInInspector, FoldoutGroup("Data")] public bool IsAlive => Percentage >= 0.01f;
    [BoxGroup("Debug"), ShowInInspector, FoldoutGroup("Data")] public bool IsDeath => Percentage <= 0f;
    [field: SerializeField, FoldoutGroup("Data")] public bool isAlwaysAffectable { get; private set; } = false;
    [field: SerializeField, FoldoutGroup("Data")] public bool CanBeDamaged { get; private set; } = true;
    [field: SerializeField, FoldoutGroup("Data")] public bool CanBeHealed { get; private set; } = true;

    //Events
    [FoldoutGroup("Events")] public UnityEvent<DamageInfo> OnDamage;
    [FoldoutGroup("Events")] public UnityEvent<HealingInfo> OnHealing;

    //Note: This Can be unity since are being used bu UI
    [FoldoutGroup("Events")] public UnityEvent<DamageInfo> OnDeath;
    [FoldoutGroup("Events")] public UnityEvent<HealingInfo> OnAlive;
    [FoldoutGroup("Events")] public UnityEvent<HealingInfo> OnFullyCured;
    [field: SerializeField, FoldoutGroup("Events")] public Vector2EventAsset OnHealthUpdated { get; private set; } //GameEvent

    private void Awake()
    {
        if (_healthSO is IHealthDataAdapter healthInfo)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
            MaxHealth = healthInfo.Health;
        }
        if (_healthSO is EnemyDataConfigSO enemies)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            switch (currentSceneIndex)
            {
                case 2:
                    MaxHealth = enemies.enemyConfigs[0].Health;
                    break;
                case 3:
                    MaxHealth = enemies.enemyConfigs[1].Health;
                    break;
                case 4:
                    MaxHealth = enemies.enemyConfigs[2].Health;
                    break;
                case 5:
                    MaxHealth = enemies.enemyConfigs[3].Health;
                    break;
            }
        }
        _currentHealth = MaxHealth;
    }

    public void Damage(DamageInfo damageInfo)
    {
        if (!CanBeDamaged)
            return;

        if (damageInfo.Amount < 1f)
            return;

        _currentHealth -= damageInfo.Amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);


        damageInfo.FinalPercentage = Percentage;
        OnDamage.Invoke(damageInfo);
        OnHealthUpdated?.Invoke(new Vector2 (_currentHealth, MaxHealth));

        CheckHealthState(damageInfo);
    }
    public void Heal(HealingInfo healingInfo)
    {
        if (!CanBeHealed) 
            return;

        if (healingInfo.Amount < 0f) 
            return;

        _currentHealth += healingInfo.Amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        healingInfo.FinalPercentage = Percentage;
        OnHealing.Invoke(healingInfo);

        OnHealthUpdated?.Invoke(new Vector2(_currentHealth, MaxHealth));

        CheckHealthState(healingInfo);
    }

    private void CheckHealthState(HealingInfo healingInfo) 
    {
        if (IsAlive && !IsFullyCured)
        {
            OnAlive.Invoke(healingInfo);

            if (!isAlwaysAffectable)
                CanBeHealed = true;
        }

        if (IsFullyCured)
        {
            OnFullyCured.Invoke(healingInfo);

            if (!isAlwaysAffectable)
            {
                CanBeHealed = false;
                CanBeDamaged = true;
            }
        }
    }
    private void CheckHealthState(DamageInfo damageInfo)
    {
        if (IsDeath)
        {
            OnDeath.Invoke(damageInfo);
            CanBeDamaged = false;

            if (!isAlwaysAffectable)
            {
                CanBeHealed = true;
                return;
            }

            CanBeHealed = false;
        }
    }

    public void ActivateInvulnerabilityDuringTime(float seconds) 
    {
        StartCoroutine(InvulnerabilityDuringTime(seconds));
    }
    private IEnumerator InvulnerabilityDuringTime(float seconds)
    {
        CanBeDamaged = false;
        yield return new WaitForSeconds(seconds);
        CanBeDamaged = true;
    }

    [ContextMenu("Damage Test"), Button("Damage Test")]
    public void DamageTest()
    {
        DamageInfo damageInfo = new DamageInfo(10, gameObject, gameObject, gameObject, DamageType.Debug);
        Damage(damageInfo);
    } 
    [ContextMenu("Healing Test"), Button("Healing Test")]
    public void HealingTest()
    {
        HealingInfo healingInfo = new HealingInfo(10, gameObject, gameObject, gameObject, HealType.Debug);
        Heal(healingInfo);
    }
}


public class DamageInfo
{
    public DamageInfo(float amount, GameObject victim, GameObject instigator, GameObject source, DamageType damageType)
    {
        Amount = amount;
        Victim = victim;
        Instigator = instigator;
        Source = source;
        DamageType = damageType;
    }

    public float Amount { get; set; }
    public GameObject Victim { get; set; }
    public GameObject Source { get; set; }
    public GameObject Instigator { get; set; }
    public DamageType DamageType { get; set; }
    public float FinalPercentage { get; set; }
}
public enum DamageType
{
    Gun,
    Object,
    Bomb,
    Fire,
    Foam,
    Debug
}

public class HealingInfo
{
    public float Amount { get; set; }
    public GameObject Target { get; set; }
    public GameObject Source { get; set; }
    public GameObject Instigator { get; set; }
    public HealType HealType { get; set; }
    public float FinalPercentage { get; set; }

    public HealingInfo(float amount, GameObject target, GameObject instigator, GameObject source, HealType healType)
    {
        Amount = amount;
        Target = target;
        Instigator = instigator;
        Source = source;
        HealType = healType;

    }
}
public enum HealType
{
    Fixing,
    Debug
    //If we need other types of healing
}