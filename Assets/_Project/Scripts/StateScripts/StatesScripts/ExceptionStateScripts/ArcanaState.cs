using UnityEngine;

public class ArcanaState : State
{
	private IdleState _idleState;
	private FallState _fallState;
	private HurtState _hurtState;
	private GrabbedState _grabbedState;
	private ArcanaThrowState _arcanaThrowState;

	void Awake()
	{
		_idleState = GetComponent<IdleState>();
		_fallState = GetComponent<FallState>();
		_hurtState = GetComponent<HurtState>();
		_grabbedState = GetComponent<GrabbedState>();
		_arcanaThrowState = GetComponent<ArcanaThrowState>();
	}

	public override void Enter()
	{
		base.Enter();
		_playerAnimator.Arcana();
		_playerAnimator.OnCurrentAnimationFinished.AddListener(ArcanaEnd);
		_player.CurrentAttack = _playerComboSystem.GetArcana();
		_player.Arcana--;
		_playerUI.DecreaseArcana();
		_playerUI.SetArcana(_player.Arcana);
		_player.CurrentAttack = _playerComboSystem.GetArcana();
		_playerMovement.TravelDistance(new Vector2(
				_player.CurrentAttack.travelDistance * transform.root.localScale.x, 0.0f));
	}

	public override void UpdateLogic()
	{
		base.UpdateLogic();
	}

	private void ArcanaEnd()
	{
		if (_stateMachine.CurrentState == this)
		{
			if (_rigidbody.velocity.y <= 0.0f)
			{
				ToFallState();
			}
			else
			{
				ToIdleState();
			}
		}
	}

	private void ToIdleState()
	{
		_stateMachine.ChangeState(_idleState);
	}

	public void ToFallState()
	{
		_playerAnimator.Jump();
		_stateMachine.ChangeState(_fallState);
	}

	public override bool AssistCall()
	{
		_player.AssistAction();
		return true;
	}

	public override bool ToHurtState(AttackSO attack)
	{
		_player.OtherPlayerUI.DisplayNotification("Punish");
		_hurtState.Initialize(attack);
		_stateMachine.ChangeState(_hurtState);
		return true;
	}

	public override bool ToGrabbedState()
	{
		_stateMachine.ChangeState(_grabbedState);
		return true;
	}

	public override bool ToThrowState()
	{
		_stateMachine.ChangeState(_arcanaThrowState);
		return true;
	}


	public override void UpdatePhysics()
	{
		if (_player.CurrentAttack.travelDistance == 0.0f)
		{
			base.UpdatePhysics();
			_rigidbody.velocity = Vector2.zero;
		}
	}
}