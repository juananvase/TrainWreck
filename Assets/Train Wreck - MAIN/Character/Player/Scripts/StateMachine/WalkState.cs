using System;
using Unity.VisualScripting;
using UnityEngine;

public class WalkState : PlayerBaseState
{
    public float FootprintSpacer = 1.5f;
    private Vector3 LastFootprint;
    private enumFoot WhichFoot;
    private Quaternion _rotation;

    public WalkState(PlayerStateMachine currentContext, PlayerStateFactory factory)
        : base(currentContext, factory) { }
    
    public override void EnterState()
    {
        //LastFootprint = _ctx.transform.position;
    }
    
    public override void UpdateState()
    {
        CheckSwitchStates();
        _ctx.IsGrounded = CheckGrounded();

        _rotation = _ctx.Rigidbody.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(_ctx.LookDirection);
        _rotation = Quaternion.Slerp(_rotation, targetRotation, _ctx.CharacterDataSO.RotationSpeed * _ctx.CharacterDataSO.RotationSpeedMultiplier * Time.deltaTime);
        _ctx.Rigidbody.MoveRotation(_rotation);
        
        // achieved target velocity only if the player can move 
        Vector3 targetVelocity = _ctx.MoveDirection * (_ctx.CharacterDataSO.MoveSpeed * _ctx.MoveSpeedMultiplier);
        targetVelocity += _ctx.SurfaceVelocity * (1f - Mathf.Abs(_ctx.MoveDirection.magnitude));
        
        Vector3 velocityDifference = targetVelocity - _ctx.Velocity;
        velocityDifference.y = 0f;  
        Vector3 acceleration = velocityDifference * _ctx.CharacterDataSO.Acceleration;
        
        acceleration += _ctx.GroundNormal * _ctx.CharacterDataSO.Gravity;    
        _ctx.Rigidbody.AddForce(acceleration*(_ctx.MoveSpeedMultiplier*_ctx.Rigidbody.mass));

        if (_ctx.Interactor.ObjectInHand != null)
        {
            PickupableContainer pickupable = _ctx.Interactor.ObjectInHand as PickupableContainer;
            if (pickupable != null)
            {
                if (pickupable.ItemInContainer != null)
                {
                    CheckFootprintDistance();
                }
            }
        }
    }

    private void CheckFootprintDistance()
    {
        float DistanceSinceLastFootprint = Vector3.Distance(LastFootprint, _ctx.transform.position);
        if (DistanceSinceLastFootprint >= FootprintSpacer)
        {
            if (WhichFoot == enumFoot.Left)
            {
                SpawnDecal(_ctx.LeftFootprint);
                WhichFoot = enumFoot.Right;
            }
            else if (WhichFoot == enumFoot.Right)
            {
                SpawnDecal(_ctx.RightFootprint);
                WhichFoot = enumFoot.Left;
            }
            LastFootprint = _ctx.transform.position;
        }
    }
    
    private void SpawnDecal(FootprintsVFX spawnPrefab)
    {
        Quaternion decalRotation = Quaternion.Euler(90f, _rotation.eulerAngles.y, 0f);
        PoolSystem.Instance.Get(spawnPrefab, new Vector3(_ctx.transform.position.x, _ctx.transform.position.y - 1.0f, _ctx.transform.position.z), decalRotation);
    }

    public override void ExitState()
    {
    }
    
    public override void CheckSwitchStates()
    {
        if (_ctx.MoveInput == Vector2.zero)
        {
            SwitchState(_factory.Idle());
        }

        if (_ctx.IsDashPressed && !_ctx.IsDashing)
        {
            _ctx.IsDashPressed = false;
            SwitchState(_factory.Dash());
        }
        
        if(_ctx.IsThrowPressed && !_ctx.IsDashing && _ctx.Interactor.ObjectInHand)
        {
            _ctx.IsThrowPressed = false;
            SwitchState(_factory.Aim());
        }
    }

    private bool CheckGrounded()
    {
        bool hit = Physics.Raycast(_ctx.GroundCheckDirection, -_ctx.transform.up, out RaycastHit hitInfo,
            _ctx.CharacterDataSO.GroundCheckDistance, _ctx.CharacterDataSO.GroundMask);   
        if (hit)
        {
            _ctx.GroundNormal = hitInfo.normal; // sets the ground normal based on ground surface, respects slopes
            if(hitInfo.rigidbody != null) _ctx.SurfaceVelocity = hitInfo.rigidbody.linearVelocity; // sustain the velocity if the ground surface has a rigidbody. for ex: moving platforms
        }
        else
        {
            _ctx.GroundNormal = Vector3.up;
            _ctx.SurfaceVelocity = Vector3.zero;
        }
        return hit;
    }

}

public enum enumFoot
{
    Left,
    Right,
}
