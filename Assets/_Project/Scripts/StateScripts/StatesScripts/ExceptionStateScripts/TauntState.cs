using System.Collections;
using UnityEngine;

public class TauntState : State
{
	private IdleState _idleState;
	private readonly float _tauntTime = 2.75f;

	private void Awake()
	{
		_idleState = GetComponent<IdleState>();
	}

	public override void Enter()
	{
		base.Enter();
		_playerAnimator.Taunt();
		StartCoroutine(TauntCoroutine());
	}

	IEnumerator TauntCoroutine()
	{
		yield return new WaitForSeconds(_tauntTime);
		ToIdleState();
	}

	private void ToIdleState()
	{
		_stateMachine.ChangeState(_idleState);
	}

	public override void UpdatePhysics()
	{
		base.UpdatePhysics();
		_rigidbody.velocity = Vector2.zero;
	}
}