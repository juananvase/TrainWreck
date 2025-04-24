using System.Collections;
using CustomFMODFunctions;
using DebugMenu;
using EditorTools;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using GameEvents;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GUIMachineGunWarning))]
[DisallowMultipleComponent]
public class MountedGun : Task
{
    private readonly string PlayerTag = "Player";
    private readonly string MountedTag = "MountedGun";

    [field: Header("Components")]
    [field: SerializeField] public GameObject GunPrefab { get; set; }
    [field: SerializeField] public GameObject AimLine { get; set; }
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private GUIMachineGunWarning _warning;

    [field: Header("Location Points")]
    [field: SerializeField] public Transform ShootingLoc { get; set; }
    [field: SerializeField] public Transform BulletSpawnPoint { get; set; }
    [field: SerializeField] public Transform MuzzleFlashSpawnPoint { get; set; }

    [field: Header("Data's")]
    [field: SerializeField, InlineEditor] public BulletDataSO BulletData { get; private set; }
    [field: SerializeField, InlineEditor] public CameraDataSO CameraData { get; private set; }

    [field: Header("Cinemachine Impulse Source")]
    [field: SerializeField, Tooltip("Add a Cinemachine Impulse Source to this gameobject")] 
    public CinemachineImpulseSource ImpulseSource { get; private set; }
    public MountedGunDataSO MountedGunData => TaskData as MountedGunDataSO;

    public float LastShotTime { get; set; }
    public bool IsOverheating { get; set; }
    public float ElapsedTime { get; set; }
    public float NextTimeToShoot { get; set; }
    public bool IsOccupied { get; set; }
    public float OverheatAmount => ElapsedTime / LastShotTime;
    public int CurrentAmmoLoaded { get; set; } = 1;
    [field: SerializeField] public int CurrentBulletCount { get; set; }
    
    public bool FullyReloaded
    {
        get => CurrentBulletCount == 100;
        private set => CurrentBulletCount = value ? 100 : 0;
    }

    private PlayerStateMachine _player;
    private Interactor _interactor;
    private EventInstance _reloadingBulletInstance;
    public bool IsReloading { get; private set; }

    [SerializeField, FoldoutGroup("Events")] private IntEventAsset _onUpdateAmmoCountUI;
    [FoldoutGroup("Events")] public UnityEvent OnFinishReloading;
    [FoldoutGroup("Events")] public UnityEvent OnStartReloading;
    [FoldoutGroup("Events")] public UnityEvent OnMountGun;
    [FoldoutGroup("Events")] public UnityEvent OnDismountGun;
    
    private void OnValidate()
    {
        if (ImpulseSource != null)
        {
            return;
        }
        ImpulseSource = GetComponent<CinemachineImpulseSource>();
        ImpulseSource.ImpulseDefinition = CameraData.ImpulseSourceRecoil;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DebugMenuSystem.Instance.RegisterObject(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DebugMenuSystem.Instance.DeregisterObject(this);
        
        if (AudioInstanceHandler.CheckIfPlayingSFX(_reloadingBulletInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_reloadingBulletInstance);
        }
    }

    private void Start()
    {
        CurrentBulletCount = BulletData.InitialMaxBulletCount;
        UpdateAmmoCountUI(CurrentBulletCount);

        if (AimLine != null)
        {
            AimLine.SetActive(false);
        }
    }
    
    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        if (IsOccupied)
        {
            return;
        }
        
        _interactor = interactor;
        if (_interactor.TryGetComponent(out PlayerStateMachine player))
        {
            _player = player;
            
            if (_interactor.ObjectInHand == null && IsOccupied == false && IsReloading == false)
            {
                Mount();
            }

            CheckForReloading();
        }
        
        _warning.WarningOnInteract(interactor, FullyReloaded, CurrentBulletCount);
    }

    private void CheckForReloading()
    {
        if (_interactor.ObjectInHand != null && FullyReloaded == false)
        {
            if (CurrentBulletCount < BulletData.InitialMaxBulletCount &&
                _interactor.ObjectInHand.PickupableData == MountedGunData.RequireItem)
            {
                
                PlaceAmmo();
                ReloadingAmmo();
            }
        }

        else if (_interactor.ObjectInHand == null && IsReloading)
        {
            ReloadingAmmo();
        }
    }

    private void PlaceAmmo()
    {
        _interactor.TryCleanUp();
        IsReloading = true;
    }

    private void ReloadingAmmo()
    {
        if (IsOccupied)
        {
            return;
        }

        _player.IsReloadingGun = true;
        CurrentAmmoLoaded = CurrentBulletCount;
        OnStartReloading?.Invoke();
        StartCoroutine(ReloadAmmo());
        PlayReloadingBulletsSFX();
    }

    public void UpdateAmmoCountUI(int ammoCount)
    {
        _warning.UpdateAmmunitionCountMessage(ammoCount);
        _onUpdateAmmoCountUI.Invoke(ammoCount);
    }

    private IEnumerator ReloadAmmo()
    {        
        while (interactInput > 0 && CurrentBulletCount < BulletData.InitialMaxBulletCount)
        {
            Tween.ShakeLocalPosition(GunPrefab.transform, MountedGunData.RecoilSettings);

            //Check that the player is close to avoid continuing to interact when he is far away
            if (Vector3.Distance(_interactor.transform.position, transform.position) > TaskData.ContinuedInteractionDistance)
                break;

            IsReloading = true;
            if (CurrentBulletCount < BulletData.InitialMaxBulletCount)
            {
                CurrentBulletCount += BulletData.ReloadAmount;
                CurrentBulletCount = Mathf.Min(CurrentBulletCount, BulletData.InitialMaxBulletCount);
            }
            if (CurrentBulletCount >= BulletData.InitialMaxBulletCount)
            {
                IsReloading = false;
                OnFinishReloading?.Invoke();
            }
            UpdateAmmoCountUI(CurrentBulletCount);
            yield return new WaitForSeconds(BulletData.ReloadTime);
        }
        StopReloadingBulletsSFX();
        _player.IsReloadingGun = false;
        yield return null;
    }
    
    private void PlayReloadingBulletsSFX()
    {
        _reloadingBulletInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_reloadingBulletInstance, TaskData.AudioData.ReloadingBulletsSFX);
        _reloadingBulletInstance.set3DAttributes(transform.position.To3DAttributes());
    }
    
    private void StopReloadingBulletsSFX()
    {
        if (AudioInstanceHandler.CheckIfPlayingSFX(_reloadingBulletInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_reloadingBulletInstance);
        }
    }

    private void Mount()
    {
        _player.PlayerInput?.SwitchCurrentActionMap(MountedTag);

        AimLine.SetActive(true);
        IsOccupied = true;
        _player.IsMounted = true;
        _player.Rigidbody.isKinematic = true;
        _player.transform.SetParent(transform);
        _player.transform.position = ShootingLoc.position;
        _player.transform.LookAt(BulletSpawnPoint.position);
        _player.CanShoot = true;

        PlayerStateFactory state = _player?.states;
        _player?.SetState(new MountedState(_player, state, this));

        OnMountGun.Invoke();
    }

    public void Dismount()
    {
        AimLine.SetActive(false);
        IsOccupied = false;
        _player.IsMounted = false;
        _player.IsDismountPressed = false;
        _player.Rigidbody.isKinematic = false;
        _player.transform.SetParent(null);
        _player.PlayerInput?.SwitchCurrentActionMap(PlayerTag);

        OnDismountGun.Invoke();
    }

    
    //DEBUG

    [DebugCommand("Reload Ammo")]
    public void DebugReloadAmmo()
    {
        CurrentBulletCount = BulletData.InitialMaxBulletCount;
        if (IsReloading)
        {
            OnFinishReloading?.Invoke();
        }
    }

}