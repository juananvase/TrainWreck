using System.Collections;
using DebugMenu;
using FMODUnity;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(CapsuleCollider))]
public class PlayerStateMachine : MonoBehaviour, IRespawnable, IKnockbackable
{
    [field: Header("Components")]
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; } 
    [field: SerializeField] public CapsuleCollider CapsuleCollider { get; private set; }
    [field: SerializeField] public Interactor Interactor { get; private set; }
    [field: SerializeField] public PlayerInput PlayerInput { get ; private set; } 
    [field: SerializeField] public MountedGun MountedGun { get ; private set; }
    [field: SerializeField] public CharacterVocals CharacterVocals { get ; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public GameObject FireOnPlayerVFXPrefab { get; private set; }
    [field: SerializeField] public VisualEffect FireOnPlayerVFX { get; private set; }
    [field: Header("Datas")]
    [field: SerializeField, InlineEditor] public CharacterDataSO CharacterDataSO {   get; private set; }
    [field: SerializeField, InlineEditor] public BulletDataSO BulletData { get; private set; }
    [field: SerializeField, InlineEditor] public MountedGunDataSO MountedGunData { get; private set; }
    [field: Header("Multipliers")]
    [field: SerializeField] public float MoveSpeedMultiplier { get; set; } = 1f;
    [field: Header("Location Points")]
    [field: SerializeField] public GameObject DashTrailSpawnPoint { get; private set; }
    [field: SerializeField] public GameObject SwirlingBirdsSpawnPoint { get; private set; }

    [field: Header("Audio SFX")]
    [field: SerializeField, InlineEditor] public AudioSourcesSOData AudioData { get; private set; }
    [field: Header("Visual Effects")]
    [field: SerializeField] public GameObject DashTrailVFX { get; private set; }
    [field: SerializeField] public GameObject StunnedVFX { get; private set; }
    [field: SerializeField] public GameObject AimLine { get; private set; }

    [field: Header("Decals")]
    [field: SerializeField] public FootprintsVFX RightFootprint;
    [field: SerializeField] public FootprintsVFX LeftFootprint;

    [field: Header("Game Events")]
    [SerializeField] private FloatEventAsset OnInteractionInput;
    [SerializeField] private FloatEventAsset OnPressedSelectKeyValue;
    [SerializeField] private BoolEventAsset OnPlayerCaughtOnFire;

    // properties 
    public PlayerBaseState CurrentState { get => _currentState;  set => _currentState = value; }
    public bool IsDashPressed { get => _isDashPressed; set => _isDashPressed = value; }
    public bool IsThrowPressed { get => _isThrowPressed; set => _isThrowPressed = value; }
    public bool IsDismountPressed { get => _isDismountPressed; set => _isDismountPressed = value; }
    public bool IsDashing { get =>_isDashing; set => _isDashing = value; }
    public float ThrowInput { get => _throwInput; set => _throwInput = value; }
    public float ShotInput { get => _shootingInput; set => _shootingInput = value; }
    public bool UsePressed { get => _usePressed; set => _usePressed = value; }
    public bool IsReloadingGun { get => _isReloading; set => _isReloading = value; }
    public bool IsReadyToThrow { get; set; }
    public bool CanShoot { get; set; } 
    public bool CanInteract { get; set; } = true;
    public bool CanDash { get; set; } = true;
    public bool IsOnFire { get; set; } = false;
    public bool IsStunned { get; set; } = false;
    public bool IsGrounded { get; set; }
    public bool IsMounted { get; set; }
    public bool IsMoving { get; set; }

    public float CurrentHitDistance { get { return _currentHitDistance; } set { _currentHitDistance = value; } }
    public float ElapsedShootingTime { get => _elapsedShootingTime; set => _elapsedShootingTime = value; }
    public Vector3 SurfaceVelocity { get { return _surfaceVelocity; } set { _surfaceVelocity = value; } }
    public Vector3 GroundNormal { get { return _groundNormal; } set { _groundNormal = value; } }
    public Vector2 AimInput { get { return _aimInput; } private set { _aimInput = value; } }
    public Vector2 MoveInput{ get { return _moveInput; } private set { _aimInput = value; } }   
    
    public Vector3 Velocity { get => Rigidbody.linearVelocity ; set => Rigidbody.linearVelocity = value; }
    public Vector3 GroundCheckDirection => transform.position + -transform.up * CharacterDataSO.GroundCheckOffset;
    public Vector3 LookDirection { get; set; } = Vector3.forward;
    public Vector3 MoveDirection { get; set; } = Vector3.zero;


    // private input variables
    private Vector2 _moveInput;
    private Vector2 _aimInput;
    private float _throwInput;
    private float _shootingInput;
    private bool _usePressed;
    private bool _isDashPressed;
    private bool _isThrowPressed;
    private bool _isDismountPressed;
    private bool _isReloading;
    private bool _isDashing;
    private bool _respawned;
    private float _elapsedTimeStunned;
    
    // private movement variables
    private Vector3 _surfaceVelocity;
    private Vector3 _groundNormal;
    private float _currentHitDistance;
    
    // player states 
    private PlayerBaseState _currentState;

    private Camera _camera;
    private float _elapsedShootingTime;
    private GameObject _fireInstance;
    private Vector3 spawnPosition;
    
    public PlayerStateFactory states;
    
    
    #region FUNCTIONS
    private void OnValidate()
    {
        if(Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();
        if(CapsuleCollider == null) CapsuleCollider = GetComponent<CapsuleCollider>();
        if(Interactor == null) Interactor = GetComponent<Interactor>();
        if(PlayerInput == null) PlayerInput = GetComponent<PlayerInput>();
        if(Animator == null) Animator = GetComponentInChildren<Animator>();
        if(CharacterVocals == null) CharacterVocals = GetComponentInChildren<CharacterVocals>();
            
        Rigidbody.freezeRotation = true;
        Rigidbody.useGravity = false;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        AimLine.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Awake()
    {
        states = new PlayerStateFactory(this);
        _currentState = states.Idle();
        _currentState.EnterState();
        
        Rigidbody.useGravity = true;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        Rigidbody.freezeRotation = true;
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);

        //Send PLayerInput component to the game manager
        CharacterDataSO.OnPassingPlayerInputComponent?.Invoke(PlayerInput);
    }

    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
    }

    private void Start()
    {
        spawnPosition = transform.position; // TODO: Players original spawn position
    }

    public void SetState(PlayerBaseState newState)
    {
        _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    private void OnReady(InputValue value)
    {
        OnPressedSelectKeyValue?.Invoke(value.Get<float>());
    }

    private void OnPause(InputValue value) 
    {
        CharacterDataSO.OnPausingGame?.Invoke(PlayerInput);
        RuntimeManager.PlayOneShot(AudioData.UISelectSFX);
    }

    private void OnUnpause(InputValue value)
    {
        CharacterDataSO.OnUnpausingGame?.Invoke(PlayerInput);
        RuntimeManager.PlayOneShot(AudioData.UISelectSFX);

    }

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void OnInteract(InputValue value)
    {
        OnInteractionInput?.Invoke(value.Get<float>());
        if (CanInteract)
        {
            Interactor.HandleInteraction();
        }
    }

    private void OnUsing(InputValue value)
    {
        _usePressed = value.isPressed;
        Interactor.TryUsingItem(_usePressed);
    }

    private void OnDrop(InputValue value)
    {
        Interactor.TryDrop();
    }

    private void OnDash(InputValue value)
    {
        _isDashPressed = value.isPressed;
    }

    private void OnThrow(InputValue value)
    {
        _isThrowPressed = value.isPressed;
        _throwInput = value.Get<float>();
    }
    
    private void OnUnMount(InputValue value)
    {
        _isDismountPressed = value.isPressed;
    }

    private void OnRotateGun(InputValue value)
    {
        _aimInput = value.Get<Vector2>();
    }

    private void OnShoot(InputValue value)
    {
        _shootingInput = value.Get<float>();
    }

    public void StartDashCooldown()
    {
        CanDash = false;
        StartCoroutine(StartDashCooldownRoutine());
    }
    private IEnumerator StartDashCooldownRoutine()
    {
        yield return new WaitForSeconds(CharacterDataSO.DashCooldown);
        CanDash = true;
    }
    
    public void InitFireOnPlayer()
    {
        if(_fireInstance == null)
            _fireInstance = Instantiate(FireOnPlayerVFXPrefab, transform.position, transform.rotation);
    
        CharacterVocals?.StartOnFireVocals();
        FireOnPlayerVFX.Play();
        _fireInstance.transform.SetParent(transform);
        PlayerOnFireState();
    }

    public void InitDestroyFireOnPlayer()
    { 
        ResetPlayerOnFireState();
        CharacterVocals?.StopOnFireVocals();
        FireOnPlayerVFX.Stop();
        Destroy(_fireInstance, CharacterDataSO.OnFireDestroyDuration);
    }

    private void PlayerOnFireState()
    {
        IsOnFire = true;
        OnPlayerCaughtOnFire.Invoke(IsOnFire);

        if (Interactor.ObjectInHand != null)
        {
            Interactor.TryDrop();
        }
        
        MoveSpeedMultiplier = 2f;
        CanDash = false;
    }
    
    private void ResetPlayerOnFireState()
    {
        OnPlayerCaughtOnFire.Invoke(IsOnFire);
        IsOnFire = false;
        MoveSpeedMultiplier = 1f;
        CanDash = true;
    }
    
    public void Respawn(Transform targetPos, float speed)
    {
        _respawned = false;
        StartCoroutine(MoveOutOfScreen(targetPos, speed));
    }

    private IEnumerator MoveOutOfScreen(Transform targetPos, float speed)
    {
        float elapsedTime = 0f;

        while (!_respawned)
        {
            elapsedTime += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPos.position, speed / elapsedTime);
            Rigidbody.linearVelocity = new Vector3(newPosition.x * (speed * Time.deltaTime), 0f, targetPos.position.z);
            
            if (Vector3.Distance(Rigidbody.transform.position, targetPos.position) < 1f)
            {
                _respawned = true;
            }
            
            if (elapsedTime >= CharacterDataSO.MaxRespawnTime)
            {
                Rigidbody.MovePosition(GameManager.Instance.RespawnPoint.position);
                _respawned = true;
                yield break;
            }
            yield return null;
        }
        Rigidbody.position = GameManager.Instance.RespawnPoint.position;
    }

    private void Update()
    {
        _currentState.UpdateState();
        Vector3 right = _camera.transform.right;
        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.Cross(right, up);
        Vector3 moveInput = forward * MoveInput.y + right * MoveInput.x;
        SetMoveInput(moveInput);
        SetLookDirection(moveInput);
    }

    private void SetMoveInput(Vector3 input)
    {
        input = Vector3.ClampMagnitude(input, 1f);
        MoveDirection = new Vector3(input.x, 0f, input.z).normalized;
        PlayerMoving(MoveDirection);
        Debug.DrawRay(transform.position, MoveDirection, Color.blue);  
    }


    private void SetLookDirection(Vector3 direction)
    {   
        if(direction.magnitude < 0.1f) return;
        LookDirection = new Vector3(direction.x, 0f, direction.z).normalized;
    }

    public void GetKnockedBack(Transform source)
    {
        Vector3 knockbackDirection = (transform.position - source.position).normalized;
        knockbackDirection += CharacterDataSO.UpwardDirection;
        Rigidbody.AddForce(knockbackDirection * CharacterDataSO.KnockbackForceToOther, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        // ground check 
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(GroundCheckDirection, -transform.up * CharacterDataSO.GroundCheckDistance);
        
        // nearby collider check for interactable(s)
        Gizmos.DrawCube(Interactor.PickupAnchor.position + CharacterDataSO.InteractionOffset, CharacterDataSO.InteractionRange);
        
        // dash distance checker
        Gizmos.DrawLine(transform.position, transform.position + LookDirection * _currentHitDistance);
        Gizmos.DrawWireSphere(transform.position + LookDirection * _currentHitDistance, 1f);
    }

    public bool PlayerMoving(Vector3 movement)
    {
        return IsMoving = ((movement.x > 0.1f || movement.x < -0.1f) || (movement.y > 0.1f || movement.y < -0.1f ) || (movement.z > 0.1f || movement.z < -0.1f)) ? true : false;
    }

    #endregion
    
    // DEBUG

    [DebugCommand("Respawn Player")]
    public void DebugRespawnPlayer()
    {
        this.transform.position = spawnPosition;
    }

}
