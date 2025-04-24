using CustomFMODFunctions;
using FMOD.Studio;
using FMODUnity;
using PrimeTween;
using UnityEngine;

public class MountedState : PlayerBaseState
{
    private readonly MountedGun _mountedGun;
    private readonly float _deadZone = 0.5f; 

    private float _lastShotTime;
    private float _elapsedTime;
    private bool _isShakingCamera;

    public float LastShotTime { get => _lastShotTime; set => _lastShotTime = value; }    

    public MountedState(PlayerStateMachine currentContext, PlayerStateFactory factory, MountedGun mountedGun)
        : base(currentContext, factory)
    {
        _mountedGun = mountedGun;
    }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (_ctx.IsMounted)
        {
            CalculateRotation();
        }

        if (_ctx.CanShoot && _ctx.IsMounted && _ctx.ShotInput >= _deadZone)
        {
            _ctx.ElapsedShootingTime += Time.deltaTime;
            if (Time.time > _mountedGun.NextTimeToShoot)
            {
                if (_mountedGun.CurrentBulletCount > 0)
                {
                    _mountedGun.CurrentBulletCount--;
                    _mountedGun.UpdateAmmoCountUI(_mountedGun.CurrentBulletCount);
                    FireBullet();
                    PlayShootingSFX();
                    
                    _ctx.Animator.SetBool("IsShooting", true);
                }
                _mountedGun.NextTimeToShoot = Time.time + _mountedGun.BulletData.FireRate;
            }
        }
        if (!_ctx.CanShoot || _ctx.ShotInput < _deadZone || _mountedGun.CurrentBulletCount <= 0 || !_ctx.IsMounted)
        {
            _ctx.Animator.SetBool("IsShooting", false);
        }
    }

    private void PlayShootingSFX()
    {
        RuntimeManager.PlayOneShot(_ctx.AudioData.MachineGunSFX, _mountedGun.transform.position);
    }

    public override void ExitState()
    {
        _mountedGun.Dismount();
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsDismountPressed)
        {
            if (_ctx.MoveInput.sqrMagnitude > 0f)
            {
                SwitchState(_factory.Walk());
            }
            if (_ctx.MoveInput == Vector2.zero)
            {
                SwitchState(_factory.Idle());
            }

            _mountedGun.IsOccupied = false;
        }
    }

    private void CalculateRotation()
    {
        var startRotation = _mountedGun.transform.rotation;

        if (_ctx.AimInput.sqrMagnitude < Mathf.Epsilon) return;

        float angle = Mathf.Atan2(_ctx.AimInput.x, _ctx.AimInput.y) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(angle, -_mountedGun.MountedGunData.RotationAngle, _mountedGun.MountedGunData.RotationAngle);

        var targetRotation = Quaternion.Euler(0f, clampedAngle, 0f);
        var smoothRotation = Quaternion.Slerp(startRotation, targetRotation, _mountedGun.MountedGunData.RotationSpeed * Time.deltaTime);
        _mountedGun.transform.rotation = smoothRotation;
    }



    private void FireBullet()
    {
        Bullet spawnedBullet = PoolSystem.Instance.Get(_mountedGun.BulletData.BulletPrefab, _mountedGun.BulletSpawnPoint.position, _mountedGun.transform.rotation) as Bullet;
        if (spawnedBullet)
        {
            spawnedBullet.ShootBullet(_mountedGun.BulletSpawnPoint.position);
            
            RecoilShake();
            
            PlayMuzzleFlashVFX();
            _lastShotTime = Time.time;
        }
    }

    private void RecoilShake()
    {
        Tween.ShakeLocalPosition(_mountedGun.GunPrefab.transform, _mountedGun.MountedGunData.RecoilSettings);

        if (!_mountedGun.ImpulseSource) 
            return;

        float minShakeForce = 0.1f;        
        float maxShakeForce = 1f;
        var impulseForce = _mountedGun.ImpulseSource.ImpulseDefinition.AmplitudeGain;
        _mountedGun.ImpulseSource.GenerateImpulseWithForce(Mathf.Clamp(impulseForce, minShakeForce, maxShakeForce));
    }

    private void PlayMuzzleFlashVFX()
    {
        PoolSystem.Instance.Get(_mountedGun.BulletData.MuzzleFlashVFX, _mountedGun.MuzzleFlashSpawnPoint.position, _mountedGun.MuzzleFlashSpawnPoint.rotation, _mountedGun.MuzzleFlashSpawnPoint);
    }
}