using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
	public Stats HP, MP, SP;
	public Stats HPR, MPR, SPR;
	public Stats HPR_Multiplier, MPR_Multiplier, SPR_Multiplier;
	public Stats Str, Dex, Int, Will, Luck;
	public Stats PAmin, PAmax, Balance, MA, CritRate, CritDamage, PDef, PPro, MDef, MPro, APen;
	public Stats AttackSpeed;

	public float lastCombatTime;
	public bool canControl, isAlive, isDeadly, isImmune, isStatic;
	public int Lv;

	[SerializeField]
	private GameObject HPBar, Prefab;
	[SerializeField]
	private GameObject WorldCanvas;

	void Start()
	{
		Initialize_HMSP(50, .4f, 50, .25f, 50f, 1.9f);
		Initialize_Attributes(0, 10f, 10f, 10f, 10f, 10f, 40f, 1.5f);
		HPBar = Instantiate(Prefab);
		HPBar.transform.SetParent(WorldCanvas.transform);
	}

	void Initialize_HMSP(float hp, float hpr, float mp, float mpr, float sp, float spr)
	{
		AttackSpeed = new Stats(1f, true);
		HP = new Stats(hp, true);
		MP = new Stats(mp, true);
		SP = new Stats(sp, true);
		HPR = new Stats(hpr);
		MPR = new Stats(mpr);
		SPR = new Stats(spr);
		HPR_Multiplier = new Stats(1f);
		MPR_Multiplier = new Stats(1f);
		SPR_Multiplier = new Stats(1f);
		isAlive = true;
		isImmune = false;
		canControl = true;
		isDeadly = false;
		isStatic = false;
	}

	void Initialize_Attributes(int apts, float str, float dex, float inte, float will, float luck, float critr, float critd)
	{
		Str = new Stats(str);
		Dex = new Stats(dex);
		Int = new Stats(inte);
		Will = new Stats(will);
		Luck = new Stats(luck);
		PAmin = new Stats(0, true);
		PAmax = new Stats(0, true);
		Balance = new Stats(0, true);
		MA = new Stats(0, true);
		CritRate = new Stats(critr, true);
		CritDamage = new Stats(critd, true);
		PDef = new Stats(0, true);
		PPro = new Stats(0, true);
		MDef = new Stats(0, true);
		MPro = new Stats(0, true);
		APen = new Stats(0, true);

		PAmax.CurValue = 10f;
		PAmin.CurValue = 5f;
		Balance.CurValue = 60f;
	}

	void Update()
	{
		if (isAlive && !isStatic)
		{
			UnitRegen();
		}
		HPBar.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
		HPBar.transform.LookAt(Camera.main.transform.position);
	}

	private void UnitRegen()
	{
		float _regenMultiplier;
		if (HP.CurValue > 0 && isDeadly)
		{
			isDeadly = false;
		}
		if (HP.CurValue < HP.FinalValue)
		{
			_regenMultiplier = Mathf.Clamp(HPR_Multiplier.FinalValue, 0, Mathf.Infinity);
			HP.CurValue += HPR.FinalValue * _regenMultiplier * Time.deltaTime;
			HP.CurValue = Mathf.Clamp(HP.CurValue, -HP.FinalValue, HP.FinalValue);
		}
		if (MP.CurValue < MP.FinalValue)
		{
			_regenMultiplier = Mathf.Clamp(MPR_Multiplier.FinalValue, 0, Mathf.Infinity);
			MP.CurValue += MPR.FinalValue * _regenMultiplier * Time.deltaTime;
			MP.CurValue = Mathf.Clamp(MP.CurValue, 0.0f, MP.FinalValue);
		}
		if (SP.CurValue < SP.FinalValue)
		{
			_regenMultiplier = Mathf.Clamp(SPR_Multiplier.FinalValue, 0, Mathf.Infinity);
			SP.CurValue += SPR.FinalValue * _regenMultiplier * Time.deltaTime;
			SP.CurValue = Mathf.Clamp(SP.CurValue, 0.0f, SP.FinalValue);
		}
	}

	public float GetAttackDamage()
	{
		float damage = (PAmax.CurValue - PAmin.CurValue) * Random.Range(Balance.CurValue / 100f, 1f) + PAmin.CurValue;
		if (Random.Range(0,100) < CritRate.CurValue)
		{
			Debug.Log("Attacked player for (CRIT):" + damage * CritDamage.CurValue);
			return damage * CritDamage.CurValue;
		}
		else
		{
			Debug.Log("Attacked player for :" + damage);
			return damage;
		}
	}
}
