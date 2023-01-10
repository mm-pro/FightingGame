public class GrabbedState : State
{
    public override void UpdateLogic(PlayerNetwork player)
    {
        if (!player.enter)
        {
            player.otherPlayer.enter = false;
            player.otherPlayer.state = "Throw";
            player.enter = true;
            player.animationFrames = 0;
            player.player.StopShakeCoroutine();
        }
        player.velocity = DemonicsVector2.Zero;
        player.animation = "HurtAir";
        player.animationFrames++;
        ThrowBreak(player);
    }

    private void ThrowBreak(PlayerNetwork player)
    {
        if (player.grabPress)
        {
            player.enter = false;
            player.otherPlayer.enter = false;
            player.state = "Knockback";
            player.otherPlayer.state = "Knockback";
        }
    }
}