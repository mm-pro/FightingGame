using UnityEngine;

public class JumpForwardState : AirParentState
{
    public override void UpdateLogic(PlayerNetwork player)
    {
        if (!player.enter)
        {
            player.animation = "JumpForward";
            player.enter = true;
            player.sound = "Jump";
            player.SetParticle(player.jumpDirection == 1 ? "JumpRight" : "JumpLeft", player.position);
            player.hasJumped = true;
            player.animationFrames = 0;
            player.velocity = new DemonicsVector2(2 * (DemonicsFloat)player.jumpDirection, player.playerStats.JumpForce);
            if (player.inputBuffer.CurrentInput().frame != 0)
            {
                player.isAir = true;
                player.inputBuffer.inputItems[player.inputBuffer.index].frame = 0;
                if (player.inputBuffer.CurrentInput().inputEnum == InputEnum.Special)
                {
                    Arcana(player, player.isAir);
                }
                else
                {
                    if (!(player.attackInput == InputEnum.Medium && player.isCrouch))
                    {
                        if (player.inputBuffer.CurrentInput().inputEnum != InputEnum.Throw)
                        {
                            if (!(player.attackInput == InputEnum.Heavy && !player.isCrouch && player.inputBuffer.CurrentInput().inputEnum == InputEnum.Heavy && player.direction.y >= 0))
                            {
                                Attack(player, player.isAir);
                            }
                        }
                    }
                }
            }
            return;
        }
        player.animationFrames++;
        player.velocity = new DemonicsVector2(player.velocity.x, player.velocity.y - (float)DemonicsPhysics.GRAVITY);
        ToFallState(player);
        base.UpdateLogic(player);
    }
    private void ToFallState(PlayerNetwork player)
    {
        if (player.velocity.y <= (DemonicsFloat)0)
        {
            EnterState(player, "Fall");
        }
    }
}