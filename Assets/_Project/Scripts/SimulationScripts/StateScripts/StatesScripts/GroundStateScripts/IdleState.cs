using UnityEngine;

public class IdleState : GroundParentState
{
    public override void UpdateLogic(PlayerNetwork player)
    {
        if (!player.enter)
        {
            player.enter = true;
            player.animationFrames = 0;
        }
        player.animation = "Idle";
        player.animationFrames++;
        player.velocity = DemonicsVector2.Zero;
        CheckFlip(player);
        base.UpdateLogic(player);
        ToWalkState(player);
        ToJumpState(player);
        ToJumpForwardState(player);
        ToCrouchState(player);
        ToDashState(player);
        ToAttackState(player);
        ToArcanaState(player);
    }

    private void ToCrouchState(PlayerNetwork player)
    {
        if (player.direction.y < 0)
        {
            player.enter = false;
            player.state = "Crouch";
        }
    }
    private void ToJumpState(PlayerNetwork player)
    {
        if (player.direction.y > 0)
        {
            player.enter = false;
            player.state = "Jump";
        }
    }
    private void ToJumpForwardState(PlayerNetwork player)
    {
        if (player.direction.y > 0 && player.direction.x != 0)
        {
            player.jumpDirection = (int)player.direction.x;
            player.enter = false;
            player.state = "JumpForward";
        }
    }
    private void ToWalkState(PlayerNetwork player)
    {
        if (player.direction.x != 0)
        {
            player.enter = false;
            player.state = "Walk";
        }
    }
    private void ToDashState(PlayerNetwork player)
    {
        if (player.dashDirection != 0)
        {
            player.enter = false;
            player.state = "Dash";
        }
    }
    public void ToAttackState(PlayerNetwork player)
    {
        if (player.attackPress)
        {
            Attack(player);
        }
    }
    public void ToArcanaState(PlayerNetwork player)
    {
        if (player.arcanaPress)
        {
            Arcana(player);
        }
    }
}