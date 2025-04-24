using UnityEngine;

public class AimState : PlayerBaseState
{
    public AimState(PlayerStateMachine currentContext, PlayerStateFactory factory) 
        : base(currentContext, factory) { }
    
    public override void EnterState()
    {
        if (_ctx.ThrowInput >= 0.1f && _ctx.Interactor.ObjectInHand)
        {
            Aim();
        }
    }
    
    public override void UpdateState()
    {
        CheckSwitchStates();
        
        Quaternion rotation = _ctx.Rigidbody.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(_ctx.LookDirection);
        rotation = Quaternion.Slerp(rotation, targetRotation, _ctx.CharacterDataSO.RotationSpeed * _ctx.CharacterDataSO.RotationSpeedMultiplier * Time.deltaTime);
        _ctx.Rigidbody.MoveRotation(rotation);
    }
    
    public override void ExitState()
    {
        _ctx.Rigidbody.linearVelocity = Vector3.one;
        _ctx.AimLine.GetComponent<MeshRenderer>().enabled = false;
    }
    
    public override void CheckSwitchStates()
    { 
        if (_ctx.ThrowInput == 0f && _ctx.Interactor.ObjectInHand)
        {
            _ctx.IsReadyToThrow = true;
            SwitchState(_factory.Throw());   
        }
        else if(_ctx.ThrowInput > 0f && _ctx.Interactor.ObjectInHand == false)
        {
            _ctx.IsReadyToThrow = false;
            SwitchState(_factory.Idle());
        }
    }
    
    private void Aim()
    {
        _ctx.Rigidbody.linearVelocity = Vector3.zero;
        _ctx.AimLine.GetComponent<MeshRenderer>().enabled = true;
    }
}
