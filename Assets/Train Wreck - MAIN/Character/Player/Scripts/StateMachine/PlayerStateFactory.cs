public class PlayerStateFactory
{
    private PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new IdleState(_context, this);
    }

    public PlayerBaseState Walk()
    {
        return new WalkState(_context, this);
    }

    public PlayerBaseState Dash()
    {
        return new DashState(_context, this);
    }
    
    public PlayerBaseState Aim()
    {
        return new AimState(_context, this);
    }
    
    public PlayerBaseState Throw()
    {
        return new ThrowState(_context, this);
    }
}
