using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement = default;
    [SerializeField] private Audio _audio = default;

    public void UnlockMovement()
    {
        _playerMovement.SetLockMovement(false);
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    public void PlayerFootstepAnimationEvent()
    {
        _audio.SoundGroup("Footsteps").PlayInRandom();
    }
}
