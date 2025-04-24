using System.Collections;
using UnityEngine;

public class DashState : PlayerBaseState
{
    public DashState(PlayerStateMachine currentContext, PlayerStateFactory factory) 
        : base(currentContext, factory) { }
    
    public override void EnterState()
    {
        if (!_ctx.IsDashing)
        {
            Dash();
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
        if (!_ctx.IsDashing)
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
    
    public void Dash()
    {
        if (!_ctx.IsDashing && _ctx.CanDash)
        {
            _ctx.CanDash = false;
            _ctx.StartDashCooldown();
            _ctx.StartCoroutine(DashRoutine());
            if (_ctx.IsDashing)
            {
                _ctx.CharacterVocals.Bounce();
                PlayDashTrail();
            }
        } 
    }
    
    private IEnumerator DashRoutine()
    {
        _ctx.IsDashing = true;
        Vector3 targetPosition = _ctx.transform.position + _ctx.transform.forward *(_ctx.CharacterDataSO.DashDistance * _ctx.CharacterDataSO.DashSpeed);
        float elapsedTime = 0f;
        
        if (_ctx.IsGrounded)
        {
            while (elapsedTime < _ctx.CharacterDataSO.DashDuration)
            {
                elapsedTime += Time.deltaTime;
                var dashTime = elapsedTime / _ctx.CharacterDataSO.DashDuration;

                var horizontalPosition = Vector3.Lerp(_ctx.transform.position, targetPosition, dashTime);
                float parabolicCurve = _ctx.CharacterDataSO.DashHeight * (dashTime * (1f - dashTime));
                var verticalPosition = horizontalPosition + (_ctx.transform.up * parabolicCurve);
                if (CheckForWalls() == false)
                {
                    if (_ctx.Rigidbody.isKinematic == false)
                    {
                        _ctx.Rigidbody.linearVelocity = Vector3.zero;
                    }
                    _ctx.Rigidbody.MovePosition(verticalPosition);
                }
                else
                {
                    _ctx.CharacterDataSO.DashDistance = _ctx.CharacterDataSO.DefaultDashDistance;
                    break;
                }
                yield return null;
            }
            if (_ctx.Rigidbody.isKinematic == false)
            {
                _ctx.Rigidbody.linearVelocity = Vector3.one;
            }
        }
        
        _ctx.IsDashing = false;
        yield return new WaitForSeconds(_ctx.CharacterDataSO.DashCooldown);
        
        if(_ctx.CurrentState == this)
        {
            SwitchState(_factory.Idle());
        }
    }

    private bool CheckForWalls()
    {
        float radius = 1f;
        RaycastHit[] hits = new RaycastHit[10];
        int size = Physics.SphereCastNonAlloc(_ctx.transform.position, radius, _ctx.LookDirection, hits, _ctx.CharacterDataSO.DashToWallDistance, _ctx.CharacterDataSO.TrainLayerMask);    
        if (size == 0) return false;
    
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                _ctx.CurrentHitDistance = hit.distance;
                _ctx.CharacterDataSO.DashDistance = _ctx.CurrentHitDistance;
                return true;
            }   
        }   
        return false;
    }
    
    private void PlayDashTrail()
    {
        GameObject instance = GameObject.Instantiate(_ctx.DashTrailVFX, _ctx.DashTrailSpawnPoint.transform.position, _ctx.transform.rotation);
        instance.transform.SetParent(_ctx.transform);
        GameObject.Destroy(instance, _ctx.CharacterDataSO.DashTrailDuration);

    }
    
}
