using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
	public delegate void OnStatsChangeCallback();
	public OnStatsChangeCallback onStatsChangeCallback;

	public Stats HP, MP, SP;
	public Stats HPR, MPR, SPR;
	public Stats HPR_Multiplier, MPR_Multiplier, SPR_Multiplier;
	public Stats Str, Dex, Int, Will, Luck;
	public Stats PAmin, PAmax, Balance, MA, CritRate, CritDamage, PDef, PPro, MDef, MPro, APen;
	public Stats AttackSpeed;

	private float lastCombatTime;
	private bool canControl, isAlive, isDeadly, isImmune, isStatic;
	private int Exp, Lv, RemainExp, APts;
	private WeaponType weaponID;

	public Leveling lving;
	private UIController uiController;

	#region //Get Set functions

	public WeaponType curWeaponID { get { return weaponID; } set { weaponID = value; } }
	public int curExp { get { return Exp; } }
	public int curLv { get { return Lv; } }
	public int curRemainExp { get { return RemainExp; } }
	public int curAPts { get { return APts; } }

	public float curlastCombatTime { get { return lastCombatTime; } set { lastCombatTime = value; } }

	public bool bCanControl { get { return canControl; } set { canControl = value; } }
	public bool bIsAlive { get { return isAlive; } set { isAlive = value; } }
	public bool bIsDeadly { get { return isDeadly; } set { isDeadly = value; } }
	public bool bIsImmune { get { return isImmune; } set { isImmune = value; } }
	public bool bIsStatic { get { return isStatic; } set { isStatic = value; isImmune = isStatic; canControl = !isStatic; } }

	#endregion //Get Set functions
	#region Singleton

	public static PlayerStats instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of PlayerStats found!");
			return;
		}
		instance = this;
	}
	#endregion

	void Start()
	{
		lving = GetComponent<Leveling>();
		uiController = UIController.instance;
		weaponID = WeaponType.None;
		Initialize_HMSP(100, .4f, 50, .25f, 50f, 1.9f);
		Initialize_Attributes(0, 10f, 10f, 10f, 10f, 10f, 0f, 1.5f);
		onStatsChangeCallback += CalCombatStats;
	}

	void Initialize_HMSP(float hp, float hpr, float mp, float mpr, float sp, float spr)
	{
		Exp = 0;
		Lv = 0;
		RemainExp = 400;
		AttackSpeed = new Stats(2f, true);
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
		APts = apts;
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
	}

	void Update()
	{
		if (isAlive && !isStatic)
		{
			UnitRegen();
		}
		if (Input.GetKey(KeyCode.R))
		{
			GainExp((int)(400 * (Lv + .5f)));
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			Debug.Log("GetAttackDamage: " + GetAttackDamage());
		}

		if (onStatsChangeCallback != null)
			onStatsChangeCallback.Invoke();
	}

	private void CalCombatStats()
	{
		MA.BaseValue = (Int.BaseValue - 10) / 5f;
		MA.CurValue = (Int.FinalValue - 10) / 5f + MA.ModifiedValue;

		PDef.BaseValue = Str.BaseValue / 10f;
		PDef.CurValue = Str.FinalValue / 10f + PDef.ModifiedValue;

		PPro.BaseValue = Str.BaseValue / 50f;
		PPro.CurValue = Str.BaseValue / 50f + PPro.ModifiedValue;

		MDef.BaseValue = Will.BaseValue / 10f;
		MDef.CurValue = Will.FinalValue / 10f + MDef.ModifiedValue;

		MPro.BaseValue = Int.BaseValue / 20f;
		MPro.CurValue = Int.FinalValue / 20f + MPro.ModifiedValue;

		CritRate.BaseValue = (Will.BaseValue - 10f) / 15f + (Luck.BaseValue - 10f) / 5f;
		CritRate.CurValue = (Will.BaseValue - 10f) / 15f + (Luck.BaseValue - 10f) / 5f + CritRate.ModifiedValue;

		CritDamage.CurValue = CritDamage.FinalValue;

		APen.BaseValue = (Dex.BaseValue - 10) / 15f;
		APen.CurValue = (Dex.FinalValue - 10) / 15f + APen.ModifiedValue;

		Balance.BaseValue = Mathf.Clamp(8.728944f * (Mathf.Log((Dex.BaseValue + 10) / 20, 2)), 0, 50);
		Balance.CurValue = Mathf.Clamp(
			Mathf.Clamp(8.728944f * (Mathf.Log((Dex.FinalValue + 10) / 20, 2)), 0, 50) + Balance.ModifiedValue, 0, 80);

		switch (weaponID)
		{
			case WeaponType.None:
			case WeaponType.Melee:
				PAmin.BaseValue = Str.BaseValue / 3f;
				PAmin.CurValue = Str.FinalValue / 3f + PAmin.ModifiedValue;
				PAmax.BaseValue = Str.BaseValue / 2.5f;
				PAmax.CurValue = Str.FinalValue / 2.5f + PAmax.ModifiedValue;
				break;
			case WeaponType.Archery:
				PAmin.BaseValue = Dex.BaseValue / 3.5f;
				PAmin.CurValue = Dex.FinalValue / 3.5f + PAmin.ModifiedValue;
				PAmax.BaseValue = Dex.BaseValue / 2.5f;
				PAmax.CurValue = Dex.FinalValue / 2.5f + PAmax.ModifiedValue;
				break;
			case WeaponType.VoidMagic:
				PAmin.BaseValue = (Dex.BaseValue + Int.BaseValue) / 2 / 3f;
				PAmin.CurValue = (Dex.FinalValue + Int.FinalValue) / 2 / 3f + PAmin.ModifiedValue;
				PAmax.BaseValue = (Str.BaseValue + Dex.BaseValue + Int.BaseValue) / 3 / 2.5f;
				PAmax.CurValue = (Str.FinalValue + Dex.FinalValue + Int.FinalValue) / 3 / 2.5f + PAmax.ItemModValue;
				break;
			case WeaponType.DarkShield:
				PAmin.BaseValue = Str.BaseValue / 3f + HP.BaseValue * .05f;
				PAmin.CurValue = Str.FinalValue / 3f + HP.FinalValue * .05f + PAmin.ModifiedValue;
				PAmax.BaseValue = Str.BaseValue / 2.5f + HP.BaseValue * .05f;
				PAmax.CurValue = Str.FinalValue / 2.5f + HP.FinalValue * .05f + PAmax.ModifiedValue;
				PDef.BaseValue = Str.BaseValue / 10f + HP.BaseValue / 15f;
				PDef.CurValue = Str.FinalValue / 10f + HP.FinalValue / 15f + PDef.ModifiedValue;
				break;
		}
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
		float damage = PAmax.CurValue - PAmin.CurValue;
		return damage * Random.Range(Balance.CurValue / 100f, 1f) + PAmin.CurValue;
	}

	public bool canAttack()
	{
		return (isAlive && !isStatic);
	}
	#region //Exp
	public void GainExp(int exp)
	{
		if (Lv >= 200)
			return;
		int tLv = Lv;
		Exp += exp;
		Lv = lving.ExpToLevel(Exp, Lv);
		if (Lv > tLv)
		{
			OnLevelStatsChange(Lv - tLv);
		}
		RemainExp = lving.GetRemainExp(Exp, Lv);
		uiController.Update_Exp();
	}
	private void OnLevelStatsChange(int change)
	{
		APts += change * 1;
		HP.BaseValue += change * 1.75f;
		MP.BaseValue += change * .75f;
		SP.BaseValue += change * .6f;
		if (isAlive && !isStatic)
		{
			HP.CurValue += change * 1.75f;
			MP.CurValue += change * .75f;
			SP.CurValue += change * .6f;
		}

		HPR.BaseValue += change * .005f;
		MPR.BaseValue += change * .002f;
		SPR.BaseValue += change * .0025f;

		Str.BaseValue += change * .5f;
		Dex.BaseValue += change * .75f;
		Int.BaseValue += change * .5f;
		Will.BaseValue += change * .6f;
		Luck.BaseValue += change * .5f;

		if (onStatsChangeCallback != null)
			onStatsChangeCallback.Invoke();
	}
	#endregion //Exp
}