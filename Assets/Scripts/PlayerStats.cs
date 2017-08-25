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
	private float PAmin, PAmax, MA, CritRate, CritDamage, PDef, PPro, MDef, MPro, AP;
	[SerializeField]
	private float Wpmin, Wpmax, lastCombatTime;

	[SerializeField]
	private bool isAlive, isImmune, canControl, isDeadly, isStatic;
	[SerializeField]
	private int weaponID, Exp, Lv, RemainExp, APts;
	[SerializeField]
	private Transform bloodImages;

	public Leveling lving;
	private UIController uiController;

	#region //Get Set functions

	public float maxHP { get { return HP; } set { HP = value; } }
	public float curHP { get { return cHP; } }
	public float maxMP { get { return MP; } set { MP = value; } }
	public float curMP { get { return cMP; } }
	public float maxSP { get { return SP; } set { SP = value; } }
	public float curSP { get { return cSP; } }

	public float curHPR { get { return HPR; } set { HPR = value; } }
	public float curHPR_M { get { return HPR_Multiplier; } set { HPR_Multiplier = value; } }
	public float curMPR { get { return MPR; } set { MPR = value; } }
	public float curMPR_M { get { return MPR_Multiplier; } set { MPR_Multiplier = value; } }
	public float curSMPR { get { return SPR; } set { SPR = value; } }
	public float curSPR_M { get { return SPR_Multiplier; } set { SPR_Multiplier = value; } }

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
	public float curMA { get { return MA; } set { MA = value; } }
	public float curCritRate { get { return CritRate; } set { CritRate = value; } }
	public float curCritDam { get { return CritDamage; } set { CritDamage = value; } }
	public float curPDef { get { return PDef; } set { PDef = value; } }
	public float curPPro { get { return PPro; } set { PPro = value; } }
	public float curMDef { get { return MDef; } set { MDef = value; } }
	public float curMPro { get { return MPro; } set { MPro = value; } }
	public float curAP { get { return AP; } set { AP = value; } }

	public float curWpmin { get { return Wpmin; } }
	public float curWpmax { get { return Wpmax; } }
	public float curlastCombatTime { get { return lastCombatTime; } }

	public bool bCanControl { get { return canControl; } set { canControl = value; } }
	public bool bIsDeadly { get { return isDeadly; } }
	public bool bIsAlive { get { return isAlive; } }
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
		Initialize_HMSP(100, .4f, 50, .2f, 30f, 1.7f);
		Initialize_Attributes(23, 10f, 10f, 10f, 10f, 10f, 0f, 50f);
		OnStatsChangeCallback += CalCombatStats;
		//OnStatsChangeCallback += CalLevel;
	}

	void Update()
	{
		foreach (Transform T in bloodImages)
		{
			Image im = T.GetComponent<Image>();
			if (im.color != Color.clear) { im.color = Color.Lerp(im.color, Color.clear, 3f * Time.deltaTime); }
		}
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

	#region Weapon List
	//0 Bare Hand
	//1 Melee
	//2 Archery 
	//3 Void Magic
	//4 Dark Shield
	#endregion Weapon List
	private void CalCombatStats()
	{
		MA = (Int - 10) / 5f + cMP * .05f;
		PDef = Str / 10f;
		MDef = Will / 10f;
		MPro = Int / 20f;
		CritRate = (Will - 10f) / 15f + (Luck - 10f) / 10f;
		AP = (Dex - 10) / 15f;
		switch (weaponID)
		{
			case 0:
			case 1:
				PAmin = Str / 3f;
				PAmax = Str / 2.5f;
				break;
			case 2:
				PAmin = Dex / 3.5f;
				PAmax = Dex / 2.5f;
				break;
			case 3:
				PAmin = (Str + Dex + Int) / 3 / 3f;
				PAmax = (Str + Dex + Int) / 3 / 2.5f;
				break;
			case 4:
				PAmin = HP / 10f;
				PAmax = HP / 8.5f;
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
	#region //Damage
	private void Damage(float amount)
	{
		lastCombatTime = Time.time;
		if (!isImmune && isAlive)
		{
			if (isStatic) { return; }
			DrawBlood(amount);
			if (isDeadly) { Death(); return; }
			if (cHP >= HP * .5f && amount >= HP * .5f) { cHP -= amount; isDeadly = true; Debug.Log("Huge Damage Deadly"); return; }
			cHP -= amount;
			if (cHP <= 0)
			{
				if (Random.Range(0, 100) <= (30 + (Will + Luck - 20) * .07)) { isDeadly = true; Debug.Log("Lucky Deadly: " + (30 + (Will + Luck - 20) * .07)); return; }
				Death();
			}
		}
	}

	private void Damage(float damage, float mpReduce, float spReduce)
	{
		if (!isImmune && isAlive)
		{
			cMP -= mpReduce;
			cSP -= spReduce;
		}
		Damage(damage);
	}

	private void DrawBlood(float damage)
	{
		if (isAlive)
		{
			foreach (Transform T in bloodImages)
			{
				Image im = T.GetComponent<Image>();
				RectTransform recTran = T.GetComponent<RectTransform>();
				recTran.position = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
				im.color = new Vector4(1, 1, 1, Mathf.Clamp01(.2f + (damage / (HP * .25f))));
			}
		}
	}
	//Dot: Damage Over Time
	public void Dot(float damage, float damInterval, float dur)
	{
		if (dur == 0 || !isAlive) { Damage(damage, 0, 0); return; }
		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(DotCounter(K, damage, 0, 0, damInterval, dur));
	}
	public void Dot(float damage, float mpReduce, float spReduce, float damInterval, float dur)
	{
		if (dur == 0 || !isAlive) { Damage(damage, mpReduce, spReduce); return; }
		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(DotCounter(K, damage, mpReduce, spReduce, damInterval, dur));
	}

	IEnumerator DotCounter(int K, float damage, float mpReduce, float spReduce, float damInterval, float dur)
	{
		float count = dur / damInterval;
		while (true)
		{
			if (mpReduce == 0 && spReduce == 0)
				Damage(damage);
			else
				Damage(damage, mpReduce, spReduce);
			if (--count <= 0)
			{
				StopCoroutine(CoroutineController.instance.Inst[K]);
				CoroutineController.instance.RevokeK(K);
				Debug.Log((damage * dur / damInterval) + " damage taken over " + dur + "s");
			}
			if (isAlive)
				yield return new WaitForSeconds(damInterval);
			else
				yield return null;
		}
	}
	#endregion //Damage
	#region //Heal
	private void Heal(float hp, float mp, float sp)
	{
		if (isAlive)
		{
			cHP = Mathf.Clamp(cHP += hp, -HP, HP);
			cMP = Mathf.Clamp(cMP += hp, 0, MP);
			cSP = Mathf.Clamp(cSP += hp, 0, SP);
		}
	}

	//Hot: Heal Over Time
	public void Hot(float hp, float mp, float sp, float interva, float dur)
	{
		if (dur == 0) { Heal(hp, mp, sp); return; }
		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(HotCounter(K, hp, mp, sp, interva, dur));
	}

	IEnumerator HotCounter(int K, float hp, float mp, float sp, float healInterval, float dur)
	{
		float count = dur / healInterval;
		while (true)
		{
			if (mp == 0 && sp == 0)
				Heal(hp, 0, 0);
			else
				Heal(hp, mp, sp);
			if (--count <= 0)
			{
				StopCoroutine(CoroutineController.instance.Inst[K]);
				CoroutineController.instance.RevokeK(K);
				Debug.Log((hp * dur / healInterval) + " HP healed over " + dur + "s");
			}
			if (isAlive)
				yield return new WaitForSeconds(healInterval);
			else
				yield return null;
		}
	}
	#endregion //Heal

	#region Death
	private void Death()
	{
		if (isStatic) { return; }
		cHP = 0;
		cMP = 0;
		cSP = 0;
		isAlive = false;
		canControl = false;
		isImmune = true;
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		player.GetComponent<PlayerMotor>().MoveToPoint(player.transform.position);
		player.GetComponentInChildren<Animator>().SetBool("Dead", true);

		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(Reviving(K, 5f, HP * .1f, MP * .1f, SP * .1f));
	}
	#endregion //Death

	#region //Revive
	public void Revive(float hp, float mp, float sp)
	{
		cHP = hp;
		cMP = mp;
		cSP = sp;
		Revive();
	}
	private void Revive()
	{
		isAlive = true;
		canControl = true;
		isImmune = false;
		isDeadly = false;
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		player.GetComponent<PlayerMotor>().MoveToPoint(player.transform.position);
		player.GetComponentInChildren<Animator>().SetBool("Dead", false);
	}

	IEnumerator Reviving(int K, float t, float hp, float mp, float sp)
	{
		yield return new WaitForSeconds(t);
		if (!isAlive)
			Revive(hp, mp, sp);
		StopCoroutine(CoroutineController.instance.Inst[K]);
		CoroutineController.instance.RevokeK(K);
	}

	#endregion Revive
	
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
		HP += change * 1f;
		MP += change * .25f;
		SP += change * .5f;
		if (isAlive && !isStatic)
		{
			cHP += change * 1f;
			cMP += change * .25f;
			cSP += change * .5f;
		}

		HPR += change * .005f;
		MPR += change * .002f;
		SPR += change * .0025f;

		Str += change * .5f;
		Dex += change * .75f;
		Int += change * .5f;
		Will += change * .75f;
		Luck += change * .6f;

	}
	#endregion //Exp

}