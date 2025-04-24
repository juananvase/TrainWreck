using System.Net.NetworkInformation;
using UnityEngine;

public class ThrowState : PlayerBaseState
{
    public ThrowState(PlayerStateMachine currentContext, PlayerStateFactory factory) 
        : base(currentContext, factory) { }
    
    public override void EnterState()
    {
        Throw();
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
        if (!_ctx.IsReadyToThrow)
        {
            switch (_ctx.MoveInput.sqrMagnitude)
            {
                case > 0f:
                    SwitchState(_factory.Walk());
                    break;
                case <= 0f:
                    SwitchState(_factory.Idle());
                    break;
            }
        }
    }
    
    private void Throw()
    {
        _ctx.Interactor.TryThrowingItem();
        _ctx.IsReadyToThrow = false;
    }
}