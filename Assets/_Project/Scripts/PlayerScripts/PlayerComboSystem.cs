using UnityEngine;

public class PlayerComboSystem : MonoBehaviour
{
	private PlayerStats _playerStats;

	void Awake()
	{
		_playerStats = GetComponent<PlayerStats>();
	}

	public AttackSO GetComboAttack(InputEnum inputEnum, bool isCrouching, bool isAir)
	{
		if (inputEnum == InputEnum.Throw)
		{
			return _playerStats.PlayerStatsSO.mThrow;
		}
		if (isCrouching)
		{
			return GetCrouchingAttackType(inputEnum);
		}
		else
		{
			if (isAir)
			{
				return _playerStats.PlayerStatsSO.jumpL;
			}
			else
			{
				return GetStandingAttackType(inputEnum);
			}
		}
	}

	private AttackSO GetCrouchingAttackType(InputEnum inputEnum)
	{
		if (inputEnum == InputEnum.Light)
		{
			return _playerStats.PlayerStatsSO.m2L;
		}
		else if (inputEnum == InputEnum.Medium)
		{
			return _playerStats.PlayerStatsSO.m2M;
		}
		else
		{
			return _playerStats.PlayerStatsSO.m2H;
		}
	}

	private AttackSO GetStandingAttackType(InputEnum inputEnum)
	{
		if (inputEnum == InputEnum.Light)
		{
			return _playerStats.PlayerStatsSO.m5L;
		}
		else if (inputEnum == InputEnum.Medium)
		{
			return _playerStats.PlayerStatsSO.m5M;
		}
		else
		{
			return _playerStats.PlayerStatsSO.m5H;
		}
	}

	public AttackSO GetThrow()
	{
		return _playerStats.PlayerStatsSO.mThrow;
	}

	public ArcanaSO GetArcana()
	{
		return _playerStats.PlayerStatsSO.arcana;
	}
}
