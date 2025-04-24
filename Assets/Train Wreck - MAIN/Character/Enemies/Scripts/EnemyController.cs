using System;
using DebugMenu;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using GameEvents;

[RequireComponent(typeof(Targetable))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour, IBombDamageable
{
    [Header("Components")]
    [BoxGroup("Components")]
    [field: SerializeField] protected NavMeshAgent NavMeshAgent { get; set; }
    [field: SerializeField] protected Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Targetable Targetable { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Vision Vision { get; private set; }
    [field: SerializeField] public Animator ShooterAnimController { get; private set; }
    [field: SerializeField] public Animator BomberAnimController { get; private set; }
    [field: SerializeField] public EnemyCar EnemyCar { get; private set; }

    [Header("State Machine")]
    protected IEnumerator CurrentState;
    [ShowInInspector] public string _currentStateName;
    protected Transform TargetLocation;
    [SerializeField] public Transform _deathRetreatPoint;
    private bool _hasEnded = false;

    [Header("Attack State")]
    [field: SerializeField, InlineButton(nameof(FindWeapons), "Find")] public Weapon[] Weapons { get; private set; }
    [field: SerializeField] private Transform _aimPosition;

    [Header("Switching")]
    public GameObject Shooter;
    public GameObject Bomber;

    [Header("Datas")]
    [SerializeField] private TrainDataSO _trainData;
    [field: SerializeField, InlineEditor] public AudioSourcesSOData AudioData { get; private set; }

    [Header("VFX Assets")]
    [SerializeField] public Transform _smokeVFXSpawnPoint;
    [SerializeField] public Transform _explosionVFXSpawnPoint;
    [SerializeField] public GameObject _explosionVFXPrefab;
    [SerializeField] private GameObject _carSmokeVFXPrefab;

    [Header("Events")]
    [SerializeField] private BoolEventAsset _onLevelFinish;

    // private fields
    private FMOD.Studio.EventInstance _bomberAppearance;
    private EnemyManager _enemyManager;

    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
        _onLevelFinish.AddListener(RetreatEnemies);
    }

    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
        StopSFX(_bomberAppearance);
        _onLevelFinish.RemoveListener(RetreatEnemies);
    }

    private void Awake()
    {

    }

    private void Start()
    {
        Health.OnDeath.AddListener((DamageInfo info) => { OnEnemyDeath(); });
        Health.OnDamage.AddListener(Damaged);
        if (AudioData != null)
        {
            PlaySFX(AudioData.EnemyEntranceSFX);
        }

    }

    private void PlaySFX(EventReference sfxReference)
    {
        if (sfxReference.IsNull == false)
        {
            RuntimeManager.PlayOneShot(sfxReference, transform.position);
        }
    }

    private void Update()
    {
        if (!_hasEnded && Health.IsAlive) 
        {
            SpeedCheck();
        }
    }


    private void Damaged(DamageInfo damage)
    {
        EnemyCar.HitMovement();
    }

    //Adjust enemy speed based on the train's current stage
    private void SpeedCheck()
    {
        switch (_trainData.CurrentTrainSpeed)
        {
            case 0:
                SetEnemySpeed(GameManager.Instance.CurrentEnemyData.EnemyToWreckedTrainSpeed);
                break;
            case < 50:
                SetEnemySpeed(GameManager.Instance.CurrentEnemyData.EnemyToSlowTrainSpeed);
                break;
            case > 50 and < 100:
                SetEnemySpeed(GameManager.Instance.CurrentEnemyData.EnemyToNormalTrainSpeed);
                break;
            case > 100:
                SetEnemySpeed(GameManager.Instance.CurrentEnemyData.EnemyToFastTrainSpeed);
                break;
            default:
                break;
        }

    }
    private void SetEnemySpeed(float newSpeed)
    {
        NavMeshAgent.speed = newSpeed;
    }

    //Find all weapons available to the enemy (children components)
    private void FindWeapons()
    {
        Weapons = GetComponentsInChildren<Weapon>();
    }

    //Handle enemy death (triggered when health reaches zero)
    public void OnEnemyDeath()
    {
        ChangeState(DeathState());
        EnemyCar.ChangeState(EnemyCar.RetreatRoutine());
        
        PlaySFX(AudioData.EnemyExplosionSFX);
        PlaySFX(AudioData.EnemyDeathSFX);
    }

    public void TakeDamageFromBomb(Bomb bomb)
    {
        DamageInfo damageInfo = new DamageInfo(bomb.BombData.DamageToEnemies, gameObject, bomb.gameObject, bomb.gameObject, DamageType.Bomb);
        Health?.Damage(damageInfo);
    }

    //ENEMY AI STATES 
    //Initialize the enemy with a target location and a reference to the enemy manager
    public void Init(Transform destination, EnemyManager manager)
    {
        TargetLocation = destination;
        _enemyManager = manager;
        ChangeState(DrivingState());
        if (Shooter.activeInHierarchy) 
        {
            ShooterAnimController.SetBool("IsOutside", true);
        }
    }

    //Driving state: move towards the target location
    private IEnumerator DrivingState()
    {
        NavMeshAgent.SetDestination(TargetLocation.position);
        Rigidbody.isKinematic = true;
        EnemyCar.ChangeState(EnemyCar.DrivingMovementRoutine());
        //Have the enemy keep going until they reach destination, without this, the enemy will begin shooting before arriving
        while (Vector3.Distance(TargetLocation.position, transform.position) > NavMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        ChangeState(ShooterIsAttackingState());
    }

    private IEnumerator ShooterIsAttackingState()
    {
        yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.InitialShootDelay);
        Vision.GetVisibleTargets(1);
        EnemyCar.ChangeState(EnemyCar.AttackMovementRoutine());

        float targetCounter = 0;
        while (true)
        {
            foreach (Targetable target in Vision.targets)
            {
                if (Shooter.activeInHierarchy) 
                {
                    ShooterAnimController.SetBool("IsAttacking", true);
                }
                // Make sure the player is still alive
                if (Health.IsFullyCured)
                {
                    // Shooter Weapon
                    Weapon gun = Weapons[0];
                    gun.TryAttack(_aimPosition.position, gameObject, Targetable.Team, target.transform);
                }
                yield return new WaitForSeconds(0.5f);
                if (Shooter.activeInHierarchy) 
                {
                    ShooterAnimController.SetBool("IsAttacking", false);
                }
                yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.ShootDelay - 0.5f);
            }
            if (Vision.targets.Count == 0 && targetCounter <= 10)
            {
                targetCounter++;
                //Debug.LogWarning("Enemy did not detect any targetables, will retry");
                Vision.GetVisibleTargets(1);
            }
            yield return null;
        }
    }

    private IEnumerator BomberIsAttackingState()
    {
        //TODO: CHANGE (VISHNU)
        // Play sound effect and save it as an instance to stop it when changing scenes
        _bomberAppearance = FMODUnity.RuntimeManager.CreateInstance("event:/BomberAppearance");
        _bomberAppearance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        _bomberAppearance.start();
        _bomberAppearance.release();

        // Loop until the bomber is alive
        while (Health.IsFullyCured)
        {
            if (Bomber.activeInHierarchy) 
            { 
                BomberAnimController.SetBool("IsAttacking", true);
            }
            
            yield return new WaitForSeconds(0.5f);
            // Get the nearest window
            Transform nearestOpening = GetNearestOpening();

            if (nearestOpening != null)
            {
                // Bomb Thrower
                Weapon bomb = Weapons[1];
                bomb.TryAttack(_aimPosition.position, gameObject, 1, nearestOpening);  // Throw bomb towards the nearest window
            }
            yield return new WaitForSeconds(0.5f);
            if (Bomber.activeInHierarchy) 
            { 
                BomberAnimController.SetBool("IsAttacking", false);
            }
            
            yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.BombThrowDelay - 1.0f);
        }
    }

    private void StopSFX(FMOD.Studio.EventInstance SFX)
    {
        if (!IsPlayingSFX(SFX))
            return;

        SFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SFX.release();
    }

    protected bool IsPlayingSFX(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    public void ChangeState(IEnumerator newState)
    {
        if (CurrentState != null)
        {
            StopCoroutine(CurrentState);
        }
        CurrentState = newState;
        _currentStateName = CurrentState.ToString();
        StartCoroutine(CurrentState);
    }

    public IEnumerator SwitchAttackerState(bool switchToBomber)
    {
        if (switchToBomber)
        {
            // Switch animations
            if (Shooter.activeInHierarchy) 
            { 
                ShooterAnimController.SetBool("IsOutside", false);
            }
            

            // Scale down the Shooter
            StartCoroutine(ScaleObject(Shooter, 0, GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2));
            yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2);

            // Switch active states
            Shooter.SetActive(false);
            Bomber.SetActive(true);

            BomberAnimController.SetBool("IsOutside", true);
            StartCoroutine(ScaleObject(Bomber, 1, GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2));
            yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2);
            ChangeState(BomberIsAttackingState());
        }
        else
        {
            // Switch animations
            if (Bomber.activeInHierarchy) 
            { 
                BomberAnimController.SetBool("IsOutside", false);
            }
            

            // Scale down the Bomber
            StartCoroutine(ScaleObject(Bomber, 0, GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2));
            yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2);

            // Switch active states
            Shooter.SetActive(true);
            Bomber.SetActive(false);

            if (Shooter.activeInHierarchy) 
            { 
                ShooterAnimController.SetBool("IsOutside", true);
            }
            
            StartCoroutine(ScaleObject(Shooter, 1, GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2));
            yield return new WaitForSeconds(GameManager.Instance.CurrentEnemyData.SwitchingDuration / 2);
            ChangeState(ShooterIsAttackingState());
        }
    }

    // Coroutine to smoothly scale an object from current scale to target scale
    private IEnumerator ScaleObject(GameObject obj, float targetScale, float duration)
    {
        Vector3 initialScale = obj.transform.localScale;
        Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(initialScale, finalScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = finalScale;
    }

    //Death state: enemy retreats and dies
    private IEnumerator DeathState()
    {
        NavMeshAgent.speed = 15f;  // Adjust speed for fast exit
        //NavMeshAgent.acceleration = 50f;
        Rigidbody.isKinematic = false;

        GameObject explosionVFXinstance = Instantiate(_explosionVFXPrefab, _explosionVFXSpawnPoint.position, Quaternion.identity, transform);
        Destroy(explosionVFXinstance, 2f);
        if (Shooter.activeInHierarchy) 
        { 
            ShooterAnimController.SetBool("IsAlive", false);
        }
        

        if (BomberAnimController.gameObject.activeInHierarchy)
        {
            BomberAnimController.SetBool("IsAlive", false);
        }

        TargetLocation.TryGetComponent(out EnemyAttackPoint target);
        if (target != null)
        {
            _enemyManager.ReleaseAttackPoint(target);
        }
        NavMeshAgent.SetDestination(_deathRetreatPoint.position);
        yield return new WaitForSeconds(2f);
        GameObject smokeVFXinstance = Instantiate(_carSmokeVFXPrefab, _smokeVFXSpawnPoint.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(10f);
        Destroy(smokeVFXinstance);
        Destroy(gameObject);
    }

    private IEnumerator RetreatState()
    {
        NavMeshAgent.speed = 200f;  // Adjust speed for fast exit
        NavMeshAgent.acceleration = 500f;
        Rigidbody.isKinematic = false;

        TargetLocation.TryGetComponent(out EnemyAttackPoint target);
        if (target != null)
        {
            _enemyManager.ReleaseAttackPoint(target);
        }

        Vector3 retreatDirection = transform.position + Vector3.right * 100f; // Move 50 units to the right
        NavMeshAgent.SetDestination(retreatDirection);

        yield return new WaitForSeconds(5); // Adjust time as needed
        Destroy(gameObject);
    }

    private void RetreatEnemies(bool arg0)
    {
        _hasEnded = true;
        ChangeState(RetreatState());
        EnemyCar.ChangeState(EnemyCar.RetreatRoutine());
    }

    //Get the nearest window for the bomber to throw bombs
    private Transform GetNearestOpening()
    {
        if (TargetLocation != null)
        {
            return TargetLocation.parent;
        }
        return null;
    }
}
