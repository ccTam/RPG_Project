using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
	delegate void CombatStats();
	CombatStats OnStatsChangeCallback;

	private float HP, MP, SP;
	[SerializeField]
	private float cHP, cMP, cSP;
	[SerializeField]
	private float HPR, MPR, SPR;
	[SerializeField]
	private float HPR_Multiplier, MPR_Multiplier, SPR_Multiplier;
	[SerializeField]
	private float Str, Dex, Int, Will, Luck;
	[SerializeField]
	private float PAmin, PAmax, Balance, MA, CritRate, CritDamage, PDef, PPro, MDef, MPro, APen;
	[SerializeField]
	private float WeaMin, WeaMax, WeaBal, WeaCrit, lastCombatTime;

	[SerializeField]
	private bool canControl, isAlive, isDeadly, isImmune, isStatic;
	[SerializeField]
	private int weaponID, Exp, Lv, RemainExp, APts;

	public Leveling lving;
	private UIController uiController;

	#region //Get Set functions

	public float maxHP { get { return HP; } set { HP = value; } }
	public float curHP { get { return cHP; } set { cHP = value; } }
	public float maxMP { get { return MP; } set { MP = value; } }
	public float curMP { get { return cMP; } set { cMP = value; } }
	public float maxSP { get { return SP; } set { SP = value; } }
	public float curSP { get { return cSP; } set { cSP = value; } }

	public float curHPR { get { return HPR; } set { HPR = value; } }
	public float curHPR_M { get { return HPR_Multiplier; } set { HPR_Multiplier = value; } }
	public float curMPR { get { return MPR; } set { MPR = value; } }
	public float curMPR_M { get { return MPR_Multiplier; } set { MPR_Multiplier = value; } }
	public float curSMPR { get { return SPR; } set { SPR = value; } }
	public float curSPR_M { get { return SPR_Multiplier; } set { SPR_Multiplier = value; } }

	public int curWeaponID { get { return weaponID; } set { weaponID = value; } }
	public int curExp { get { return Exp; } }
	public int curLv { get { return Lv; } }
	public int curRemainExp { get { return RemainExp; } }
	public int curAPts { get { return APts; } }
	public float curStr { get { return Str; } set { Str = value; } }
	public float curDex { get { return Dex; } set { Dex = value; } }
	public float curInt { get { return Int; } set { Int = value; } }
	public float curWill { get { return Will; } set { Will = value; } }
	public float curLuck { get { return Luck; } set { Luck = value; } }

	public float curPAmin { get { return PAmin; } set { PAmin = value; } }
	public float curPAmax { get { return PAmax; } set { PAmax = value; } }
	public float curBalance { get { return Balance; } set { Balance = value; } }
	public float curMA { get { return MA; } set { MA = value; } }
	public float curCritRate { get { return CritRate; } set { CritRate = value; } }
	public float curCritDam { get { return CritDamage; } set { CritDamage = value; } }
	public float curPDef { get { return PDef; } set { PDef = value; } }
	public float curPPro { get { return PPro; } set { PPro = value; } }
	public float curMDef { get { return MDef; } set { MDef = value; } }
	public float curMPro { get { return MPro; } set { MPro = value; } }
	public float curAP { get { return APen; } set { APen = value; } }

	public float curWeaMin { get { return WeaMin; } set { WeaMin = value; } }
	public float curWeaMax { get { return WeaMax; } set { WeaMax = value; } }
	public float curWeaBal { get { return WeaBal; } set { WeaBal = value; } }
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
		WeaMin = 0;
		WeaMax = 0;
		Initialize_HMSP(100, .4f, 50, .2f, 30f, 1.3f);
		Initialize_Attributes(0, 10f, 10f, 10f, 10f, 10f, 0f, 50f);
		OnStatsChangeCallback += CalCombatStats;
	}

	void Update()
	{
		if (isAlive && !isStatic)
		{
			UnitRegen();
			if (OnStatsChangeCallback != null)
				OnStatsChangeCallback.Invoke();
		}
	}

	void Initialize_HMSP(float hp, float hpr, float mp, float mpr, float sp, float spr)
	{
		Exp = 0;
		Lv = 0;
		RemainExp = 400;
		HP = hp;
		MP = mp;
		SP = sp;
		cHP = hp;
		cMP = mp / 2;
		cSP = sp / 2;
		HPR = hpr;
		MPR = mpr;
		SPR = spr;
		HPR_Multiplier = 1f;
		MPR_Multiplier = 1f;
		SPR_Multiplier = 1f;
		isAlive = true;
		isImmune = false;
		canControl = true;
		isDeadly = false;
		isStatic = false;
	}

	void Initialize_Attributes(int apts, float str, float dex, float inte, float will, float luck, float critr, float critd)
	{
		APts = apts;
		Str = str;
		Dex = dex;
		Int = inte;
		Will = will;
		Luck = luck;
		CritRate = critr;
		CritDamage = critd;
	}

	private void CalCombatStats()
	{
		MA = (Int - 10) / 5f + cMP * .05f;
		PDef = Str / 10f;
		MDef = Will / 10f;
		MPro = Int / 20f;
		CritRate = (Will - 10f) / 10f + (Luck - 10f) / 5f;
		APen = (Dex - 10) / 15f;
		Balance = Mathf.Clamp(8.728944f * (Mathf.Log((Dex + 10) / 20, 2)), 0, 50) + WeaBal;
		switch (weaponID)
		{
			case (int)WeaponID.None:
			case (int)WeaponID.Melee:
				PAmin = Str / 3f;
				PAmax = Str / 2.5f;
				break;
			case (int)WeaponID.Archery:
				PAmin = Dex / 3.5f;
				PAmax = Dex / 2.5f;
				break;
			case (int)WeaponID.VoidMagic:
				PAmin = (Dex + Int) / 2 / 3f;
				PAmax = (Str + Dex + Int) / 3 / 2.5f;
				MA += cMP * .05f;
				break;
			case (int)WeaponID.DarkShield:
				PAmin = Str / 3f + HP * .05f;
				PAmax = Str / 2.5f + HP * .05f;
				PDef = Str / 10f + HP / 15f;
				break;
		}
	}

	private void UnitRegen()
	{
		if (cHP > 0 && isDeadly)
		{
			isDeadly = false;
		}
		if (cHP < HP)
		{
			cHP += HPR * HPR_Multiplier * Time.deltaTime;
			cHP = Mathf.Clamp(cHP, -HP, HP);
		}
		if (cMP < MP)
		{
			cMP += MPR * MPR_Multiplier * Time.deltaTime;
			cMP = Mathf.Clamp(cMP, 0.0f, MP);
		}
		if (cSP < SP)
		{
			cSP += SPR * SPR_Multiplier * Time.deltaTime;
			cSP = Mathf.Clamp(cSP, 0.0f, SP);
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
		HP += change * 1.75f;
		MP += change * .75f;
		SP += change * .6f;
		if (isAlive && !isStatic)
		{
			cHP += change * 1.75f;
			cMP += change * .75f;
			cSP += change * .6f;
		}

		HPR += change * .005f;
		MPR += change * .002f;
		SPR += change * .0025f;

		Str += change * .5f;
		Dex += change * .75f;
		Int += change * .5f;
		Will += change * .6f;
		Luck += change * .5f;
	}
	#endregion //Exp
}