using UnityEngine;

public class ParryState : State
{
	[SerializeField] private GameObject _parryEffect = default;
	private IdleState _idleState;
	private HurtState _hurtState;
	private AttackState _attackState;
	private AirborneHurtState _airborneHurtState;
	private GrabbedState _grabbedState;
	private BlockState _blockState;
	private bool _parried;

	private void Awake()
	{
		_idleState = GetComponent<IdleState>();
		_hurtState = GetComponent<HurtState>();
		_airborneHurtState = GetComponent<AirborneHurtState>();
		_grabbedState = GetComponent<GrabbedState>();
		_attackState = GetComponent<AttackState>();
		_blockState = GetComponent<BlockState>();
	}

	public override void Enter()
	{
		base.Enter();
		_audio.Sound("ParryStart").Play();
		_playerAnimator.Parry();
		_playerAnimator.OnCurrentAnimationFinished.AddListener(ToIdleState);
	}


	private void ToIdleState()
	{
		_stateMachine.ChangeState(_idleState);
	}

	public override bool ToHurtState(AttackSO attack)
	{
		if (_player.Parrying)
		{
			Parry(attack);
			return false;
		}
		_hurtState.Initialize(attack);
		_stateMachine.ChangeState(_hurtState);
		return true;
	}

	public override bool ToAirborneHurtState(AttackSO attack)
	{
		if (_player.Parrying)
		{
			Parry(attack);
			return false;
		}
		_airborneHurtState.Initialize(attack);
		_stateMachine.ChangeState(_airborneHurtState);
		return true;
	}

	public override bool ToAttackState(InputEnum inputEnum, InputDirectionEnum inputDirectionEnum)
	{
		if (_parried)
		{
			if (inputDirectionEnum == InputDirectionEnum.Down || _baseController.Crouch())
			{
				_attackState.Initialize(inputEnum, true, false);
			}
			else
			{
				_attackState.Initialize(inputEnum, false, false);
			}
			_stateMachine.ChangeState(_attackState);
			return true;
		}
		return false;
	}

	private void Parry(AttackSO attack)
	{
		_audio.Sound("Parry").Play();
		if (attack.isArcana)
		{
			_player.ArcanaGain(0.5f);
		}
		else
		{
			_player.ArcanaGain(0.1f);
		}
		_parried = true;
		GameManager.Instance.HitStop(0.15f);
		GameObject effect = Instantiate(_parryEffect);
		effect.transform.localPosition = attack.hurtEffectPosition;
		if (!attack.isProjectile)
		{
			_player.OtherPlayerMovement.Knockback(new Vector2(
				_player.transform.localScale.x, 0.0f), 2.0f, attack.knockbackDuration);
		}
		_inputBuffer.CheckInputBuffer();
	}

	public override bool ToBlockState(AttackSO attack)
	{
		if (_parried)
		{
			_blockState.Initialize(attack);
			_stateMachine.ChangeState(_blockState);
			return true;
		}
		return false;
	}

	public override bool ToParryState()
	{
		if (_parried)
		{
			_stateMachine.ChangeState(this);
			return true;
		}
		return false;
	}

	public override bool ToGrabbedState()
	{
		_stateMachine.ChangeState(_grabbedState);
		return true;
	}

	public override void UpdatePhysics()
	{
		base.UpdatePhysics();
		_rigidbody.velocity = Vector2.zero;
	}

	public override void Exit()
	{
		base.Exit();
		_parried = false;
		_playerAnimator.OnCurrentAnimationFinished.RemoveAllListeners();
	}
}