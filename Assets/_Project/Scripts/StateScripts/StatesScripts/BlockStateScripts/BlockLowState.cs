public class BlockLowState : BlockParentState
{
    private int _blockFrame;

    public override void Enter()
    {
        base.Enter();
        _playerAnimator.BlockLow();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _blockFrame++;
        if (_blockFrame == _blockAttack.hitStun)
        {
            ToCrouchState();
        }
    }

    private void ToCrouchState()
    {
        _stateMachine.ChangeState(_crouchState);
    }
}
