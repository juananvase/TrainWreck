using FMODUnity;
using PrimeTween;
using UnityEngine;

public class StunnedState : PlayerBaseState
{
    private float _stunTime;
    
    public StunnedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _ctx.IsStunned = true;
        _ctx.Rigidbody.isKinematic = true;
        _ctx.CanShoot = false;
        _stunTime = _ctx.CharacterDataSO.StunDuration;
        PlaySFX();
        PlayStunnedVFX();
    }
    
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    
    public override void ExitState()
    {
        _ctx.Rigidbody.isKinematic = false;
        _ctx.CanShoot = true;
    }

    public override void CheckSwitchStates()
    {
        _stunTime -= Time.deltaTime;
            
        if (_stunTime <= 0)
        {
            _ctx.IsStunned = false;
            
            if (_ctx.MoveInput.sqrMagnitude > 0f)
            {
                SwitchState(_factory.Idle());
            }
            
            if (_ctx.MoveInput == Vector2.zero)
            {
                SwitchState(_factory.Idle());
            }
        }
        
    }
    
    private void PlayStunnedVFX()
    {
        GameObject vfxInstance = GameObject.Instantiate(_ctx.StunnedVFX, _ctx.SwirlingBirdsSpawnPoint.transform.position, Quaternion.identity);
        vfxInstance.transform.SetParent(_ctx.transform);
        GameObject.Destroy(vfxInstance, _ctx.CharacterDataSO.StunDuration);
    }
    
    private void PlaySFX()
    {
        _ctx.CharacterVocals?.Stunned();
    }
}
