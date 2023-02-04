using UnityEngine;

public class PlayerSimulation : MonoBehaviour
{
    [SerializeField] private Player _player = default;
    [SerializeField] private InputBuffer _inputBuffer = default;
    [SerializeField] private PlayerAnimator _playerAnimator = default;
    [SerializeField] private Audio _audio = default;
    [SerializeField] private CollisionVisualizer _hurtBoxVisualizer = default;
    [SerializeField] private CollisionVisualizer _hitBoxVisualizer = default;
    [SerializeField] private CollisionVisualizer _pushBoxVisualizer = default;
    [SerializeField] private DisconnectMenu _disconnectMenu = default;


    public void Simulate(PlayerNetwork playerGs, PlayerConnectionInfo info)
    {
        if (!string.IsNullOrEmpty(playerGs.soundGroup))
        {
            _audio.SoundGroup(playerGs.soundGroup).PlayInRandom();
            playerGs.soundGroup = "";
        }
        if (!string.IsNullOrEmpty(playerGs.sound))
        {
            _audio.Sound(playerGs.sound).Play();
            playerGs.sound = "";
        }
        if (!string.IsNullOrEmpty(playerGs.soundStop))
        {
            _audio.Sound(playerGs.soundStop).Stop();
            playerGs.soundStop = "";
        }
        if (info.state == PlayerConnectState.Disconnected && playerGs.health <= 0)
        {
            _player.PlayerUI.Disconnected();
        }
        _player.Simulate(playerGs, info);
        _player.PlayerUI.SetArcana(playerGs.arcanaGauge);
        _player.PlayerUI.SetComboTimerLock(playerGs.otherPlayer.comboLocked);
        _player.Assist.Simulate(playerGs);
        for (int i = 0; i < playerGs.inputBuffer.inputItems.Length; i++)
        {
            if (playerGs.inputBuffer.inputItems[i].pressed)
            {
                _inputBuffer.AddInputBufferItem(playerGs.inputBuffer.inputItems[i].inputEnum, playerGs.inputBuffer.inputItems[i].inputDirection);
            }
        }
        _playerAnimator.SetAnimation(playerGs.animation, playerGs.animationFrames);
        _playerAnimator.SetInvinsible(playerGs.invisible);
        _playerAnimator.SetSpriteOrder(playerGs.spriteOrder);
        _hitBoxVisualizer.ShowBox(playerGs.hitbox);
        _hurtBoxVisualizer.ShowBox(playerGs.hurtbox);
        _pushBoxVisualizer.ShowBox(playerGs.pushbox);
    }
}
