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
	//[SerializeField]
	//private float[] ItemBasicAttributes = new float[5];
	public Stats PAmin, PAmax, Balance, MA, CritRate, CritDamage, PDef, PPro, MDef, MPro, APen;
	[SerializeField]
	private float lastCombatTime;

	[SerializeField]
	private bool canControl, isAlive, isDeadly, isImmune, isStatic;
	[SerializeField]
	private int weaponID, Exp, Lv, RemainExp, APts;

	public Leveling lving;
	private UIController uiController;

	#region //Get Set functions

	//public float maxHP { get { return HP; } set { HP = value; } }
	//public float curHP { get { return cHP; } set { cHP = value; } }
	//public float maxMP { get { return MP; } set { MP = value; } }
	//public float curMP { get { return cMP; } set { cMP = value; } }
	//public float maxSP { get { return SP; } set { SP = value; } }
	//public float curSP { get { return cSP; } set { cSP = value; } }

	//public float curHPR { get { return HPR; } set { HPR = value; } }
	//public float curHPR_M { get { return HPR_Multiplier; } set { HPR_Multiplier = value; } }
	//public float curMPR { get { return MPR; } set { MPR = value; } }
	//public float curMPR_M { get { return MPR_Multiplier; } set { MPR_Multiplier = value; } }
	//public float curSMPR { get { return SPR; } set { SPR = value; } }
	//public float curSPR_M { get { return SPR_Multiplier; } set { SPR_Multiplier = value; } }

	public int curWeaponID { get { return weaponID; } set { weaponID = value; } }
	public int curExp { get { return Exp; } }
	public int curLv { get { return Lv; } }
	public int curRemainExp { get { return RemainExp; } }
	public int curAPts { get { return APts; } }
	//public float curStr { get { return Str; } set { Str = value; } }
	//public float curDex { get { return Dex; } set { Dex = value; } }
	//public float curInt { get { return Int; } set { Int = value; } }
	//public float curWill { get { return Will; } set { Will = value; } }
	//public float curLuck { get { return Luck; } set { Luck = value; } }

	//public float curPAmin { get { return PAmin; } set { PAmin = value; } }
	//public float curPAmax { get { return PAmax; } set { PAmax = value; } }
	//public float curBalance { get { return Balance; } set { Balance = value; } }
	//public float curMA { get { return MA; } set { MA = value; } }
	//public float curCritRate { get { return CritRate; } set { CritRate = value; } }
	//public float curCritDam { get { return CritDamage; } set { CritDamage = value; } }
	//public float curPDef { get { return PDef; } set { PDef = value; } }
	//public float curPPro { get { return PPro; } set { PPro = value; } }
	//public float curMDef { get { return MDef; } set { MDef = value; } }
	//public float curMPro { get { return MPro; } set { MPro = value; } }
	//public float curAP { get { return APen; } set { APen = value; } }

	//public float[] curItemBasicAttributes { get { return ItemBasicAttributes; } set { ItemBasicAttributes = value; } }
	//public float curItemMin { get { return ItemMin; } set { ItemMin = value; } }
	//public float curItemMax { get { return ItemMax; } set { ItemMax = value; } }
	//public float curItemBal { get { return ItemBal; } set { ItemBal = value; } }
	//public float curItemCrit { get { return ItemCrit; } set { ItemCrit = value; } }
	//public float curItemPDef { get { return ItemPDef; } set { ItemPDef = value; } }
	//public float curItemPPro { get { return ItemPPro; } set { ItemPPro = value; } }
	//public float curItemMDef { get { return ItemMDef; } set { ItemMDef = value; } }
	//public float curItemMPro { get { return ItemMPro; } set { ItemMPro = value; } }
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
		weaponID = 0;
		Initialize_HMSP(100, .4f, 50, .25f, 50f, 1.9f);
		Initialize_Attributes(0, 10f, 10f, 10f, 10f, 10f, 0f, 1.5f);
		onStatsChangeCallback += CalCombatStats;

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
		if (onStatsChangeCallback != null)
			onStatsChangeCallback.Invoke();
	}

	void Initialize_HMSP(float hp, float hpr, float mp, float mpr, float sp, float spr)
	{
		Exp = 0;
		Lv = 0;
		RemainExp = 400;
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
		Balance.CurValue = Mathf.Clamp(8.728944f * (Mathf.Log((Dex.FinalValue + 10) / 20, 2)), 0, 50) + Balance.ModifiedValue;

		switch (weaponID)
		{
			case (int)WeaponType.None:
			case (int)WeaponType.Melee:
				PAmin.BaseValue = Str.BaseValue / 3f;
				PAmin.CurValue = Str.FinalValue / 3f + PAmin.ModifiedValue;
				PAmax.BaseValue = Str.BaseValue / 2.5f;
				PAmax.CurValue = Str.FinalValue / 2.5f + PAmax.ModifiedValue;
				break;
			case (int)WeaponType.Archery:
				PAmin.BaseValue = Dex.BaseValue / 3.5f;
				PAmin.CurValue = Dex.FinalValue / 3.5f + PAmin.ModifiedValue;
				PAmax.BaseValue = Dex.BaseValue / 2.5f;
				PAmax.CurValue = Dex.FinalValue / 2.5f + PAmax.ModifiedValue;
				break;
			case (int)WeaponType.VoidMagic:
				PAmin.BaseValue = (Dex.BaseValue + Int.BaseValue) / 2 / 3f;
				PAmin.CurValue = (Dex.FinalValue + Int.FinalValue) / 2 / 3f + PAmin.ModifiedValue;
				PAmax.BaseValue = (Str.BaseValue + Dex.BaseValue + Int.BaseValue) / 3 / 2.5f;
				PAmax.CurValue = (Str.FinalValue + Dex.FinalValue + Int.FinalValue) / 3 / 2.5f + PAmax.ItemModValue;
				break;
			case (int)WeaponType.DarkShield:
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
		if (HP.CurValue > 0 && isDeadly)
		{
			isDeadly = false;
		}
		if (HP.CurValue < HP.FinalValue)
		{
			HP.CurValue += HPR.FinalValue * HPR_Multiplier.FinalValue * Time.deltaTime;
			HP.CurValue = Mathf.Clamp(HP.CurValue, -HP.FinalValue, HP.FinalValue);
		}
		if (MP.CurValue < MP.FinalValue)
		{
			MP.CurValue += MPR.FinalValue * MPR_Multiplier.FinalValue * Time.deltaTime;
			MP.CurValue = Mathf.Clamp(MP.CurValue, 0.0f, MP.FinalValue);
		}
		if (SP.CurValue < SP.FinalValue)
		{
			SP.CurValue += SPR.FinalValue * SPR_Multiplier.FinalValue * Time.deltaTime;
			SP.CurValue = Mathf.Clamp(SP.CurValue, 0.0f, SP.FinalValue);
		}
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