using Demonics.Sounds;
using UnityEngine;

public class State : MonoBehaviour
{
    public string stateName;
    protected StateMachine _stateMachine;
    protected Rigidbody2D _rigidbody;
    protected PlayerAnimator _playerAnimator;
    protected Player _player;
    protected PlayerMovement _playerMovement;
    protected PlayerUI _playerUI;
    protected PlayerStats _playerStats;
    protected PlayerController _playerController;
    protected PlayerComboSystem _playerComboSystem;
    protected Audio _audio;


    public void Initialize(StateMachine stateMachine, Rigidbody2D rigidbody, PlayerAnimator playerAnimator, Player player, PlayerMovement playerMovement,
        PlayerUI playerUI, PlayerStats playerStats, PlayerController playerController, PlayerComboSystem playerComboSystem, Audio audio)
    {
        _stateMachine = stateMachine;
        _rigidbody = rigidbody;
        _playerAnimator = playerAnimator;
        _player = player;
        _playerMovement = playerMovement;
        _playerUI = playerUI;
        _playerStats = playerStats;
        _playerController = playerController;
        _playerComboSystem = playerComboSystem;
        _audio = audio;
    }

    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }
    public virtual bool ToAttackState(InputEnum inputEnum) { return false; }
    public virtual bool ToArcanaState() { return false; }
}