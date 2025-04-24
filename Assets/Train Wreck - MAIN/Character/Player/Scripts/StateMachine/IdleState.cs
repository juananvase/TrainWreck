using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : PlayerBaseState
{
    private PlayerInput _playerInput;
    private readonly string PlayerHash = "Player";

    public IdleState(PlayerStateMachine currentContext, PlayerStateFactory factory)
        : base(currentContext, factory) { }
    
    public override void EnterState()
    {
        _ctx.Rigidbody.linearVelocity = Vector3.zero;
        if (_ctx.PlayerInput != null)
        {
            _playerInput = _ctx.PlayerInput;
            if (_ctx.PlayerInput.currentActionMap != _playerInput.currentActionMap)
            {
               _playerInput.SwitchCurrentActionMap(PlayerHash);
            }
        }
    }
    
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    
    public override void ExitState()
    {
    }
    
    public override void CheckSwitchStates()
    {
        if (_ctx.MoveInput.sqrMagnitude > 0f)
        {
            SwitchState(_factory.Walk());
        }

        if (_ctx.IsDashPressed && !_ctx.IsDashing)
        {
            _ctx.IsDashPressed = false;
            SwitchState(_factory.Dash());
        }
        
        if(_ctx.IsThrowPressed && _ctx.Interactor.ObjectInHand && !_ctx.IsDashing)  
        {
            _ctx.IsThrowPressed = false;
            SwitchState(_factory.Aim());
        }
    }
}
