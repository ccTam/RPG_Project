using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[SerializeField]
	GameObject IFCanvas, WorldCanvas, InventoryWin, StatsWin;
	Slider MainHPSlider, MainHPBlur, MainMPSlider, MainSPSlider, HPSlider, MPSlider, SPSlider, LvSlider;
	Text Lv, Str, Dex, Int, Will, Luck, PA, MA, Crit, PDef, PPro, MDef, MPro, AP, APts;
	[SerializeField]
	private int exp = 10000;

	[SerializeField]
	Transform player, hpBar;
	PlayerStats pStats;
	Combat combat;

	#region Singleton

	public static UIController instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of UIController found!");
			return;
		}
		instance = this;
	}
	#endregion

	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
		//
		pStats = PlayerStats.instance;
		combat = Combat.instance;
		InventoryWin = IFCanvas.transform.Find("Inventory").gameObject;
		StatsWin = IFCanvas.transform.Find("Stats").gameObject;

		MainHPSlider = IFCanvas.transform.Find("User Canvas/HP").GetComponent<Slider>();
		MainHPBlur = IFCanvas.transform.Find("User Canvas/HP Blur").GetComponent<Slider>();
		MainMPSlider = IFCanvas.transform.Find("User Canvas/MP").GetComponent<Slider>();
		MainSPSlider = IFCanvas.transform.Find("User Canvas/SP").GetComponent<Slider>();

		LvSlider = StatsWin.transform.Find("ItemsParent/BaseStats/Level/LvSlider").GetComponent<Slider>();
		APts = StatsWin.transform.Find("ItemsParent/BaseStats/AbilityPoints/Value").GetComponent<Text>();
		HPSlider = StatsWin.transform.Find("ItemsParent/BaseStats/HP/HPSlider").GetComponent<Slider>();
		MPSlider = StatsWin.transform.Find("ItemsParent/BaseStats/MP/MPSlider").GetComponent<Slider>();
		SPSlider = StatsWin.transform.Find("ItemsParent/BaseStats/SP/SPSlider").GetComponent<Slider>();

		Lv = IFCanvas.transform.Find("User Canvas/Lv").GetComponent<Text>();
		Str = StatsWin.transform.Find("ItemsParent/BaseStats/Str/Value").GetComponent<Text>();
		Dex = StatsWin.transform.Find("ItemsParent/BaseStats/Dex/Value").GetComponent<Text>();
		Int = StatsWin.transform.Find("ItemsParent/BaseStats/Int/Value").GetComponent<Text>();
		Will = StatsWin.transform.Find("ItemsParent/BaseStats/Will/Value").GetComponent<Text>();
		Luck = StatsWin.transform.Find("ItemsParent/BaseStats/Luck/Value").GetComponent<Text>();
		PA = StatsWin.transform.Find("ItemsParent/BaseStats/PhysicalAttack/Value").GetComponent<Text>();
		MA = StatsWin.transform.Find("ItemsParent/BaseStats/MagicAttack/Value").GetComponent<Text>();
		Crit = StatsWin.transform.Find("ItemsParent/BaseStats/Critical/Value").GetComponent<Text>();
		PDef = StatsWin.transform.Find("ItemsParent/BaseStats/P.Def/Value").GetComponent<Text>();
		PPro = StatsWin.transform.Find("ItemsParent/BaseStats/P.Pro/Value").GetComponent<Text>();
		MDef = StatsWin.transform.Find("ItemsParent/BaseStats/M.Def/Value").GetComponent<Text>();
		MPro = StatsWin.transform.Find("ItemsParent/BaseStats/M.Pro/Value").GetComponent<Text>();
		AP = StatsWin.transform.Find("ItemsParent/BaseStats/ArmorPierce/Value").GetComponent<Text>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			combat.Dot(pStats.maxHP * .08f, 3f, 12f);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			combat.Dot(pStats.maxHP * .00f, 50f, 40f, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			combat.Hot(pStats.maxHP * .01f, pStats.maxMP * .01f, pStats.maxSP * .01f, .3f, 5f);
		}
		if (Input.GetKey(KeyCode.R))
		{
			pStats.GainExp(exp);
		}
		if (Input.GetButtonDown("Inventory"))
			InventoryWin.SetActive(!InventoryWin.activeSelf);
		if (Input.GetButtonDown("Stats"))
			StatsWin.SetActive(!StatsWin.activeSelf);
		UI_Update();
	}
	void UI_Update()
	{
		Update_HMSP();
		Update_Attributes();
		if (MainHPBlur.value > MainHPSlider.value)
		{
			if (Time.time - pStats.curlastCombatTime >= 2f || !pStats.bIsAlive)
				MainHPBlur.value = Mathf.Lerp(MainHPBlur.value, MainHPSlider.value, 1.4f * Time.deltaTime);
		}
		else
			MainHPBlur.value = MainHPSlider.value;
		hpBar = WorldCanvas.transform.Find("HP Bar");
		hpBar.position = new Vector3(player.position.x, player.position.y + 1.7f, player.position.z);
		hpBar.LookAt(Camera.main.transform.position);
	}

	void Update_HMSP()
	{
		HPSlider.maxValue = pStats.maxHP;
		MPSlider.maxValue = pStats.maxMP;
		SPSlider.maxValue = pStats.maxSP;
		HPSlider.value = pStats.curHP;
		MPSlider.value = pStats.curMP;
		SPSlider.value = pStats.curSP;
		MainHPSlider.maxValue = pStats.maxHP;
		MainHPBlur.maxValue = pStats.maxHP;
		MainMPSlider.maxValue = pStats.maxMP;
		MainSPSlider.maxValue = pStats.maxSP;
		MainHPSlider.value = pStats.curHP;
		MainMPSlider.value = pStats.curMP;
		MainSPSlider.value = pStats.curSP;

		if (pStats.bIsAlive)
		{
			if (pStats.bIsDeadly)
			{
				HPSlider.GetComponentInChildren<Text>().text = string.Format("<color=#B200BEFF>DEADLY</color> <color=#FF0000FF>({0})</color>", pStats.curHP.ToString("0"));
				MainHPSlider.GetComponentInChildren<Text>().text = string.Format("<color=#B200BEFF>DEADLY </color> <color=#FF0000FF>({0})</color>", pStats.curHP.ToString("0"));
			}
			else
			{
				HPSlider.GetComponentInChildren<Text>().text = HPSlider.value.ToString("0") + "/" + HPSlider.maxValue.ToString("0");
				MainHPSlider.GetComponentInChildren<Text>().text = MainHPSlider.value.ToString("0") + "/" + MainHPSlider.maxValue.ToString("0");
			}
		}
		else
		{
			HPSlider.GetComponentInChildren<Text>().text = string.Format("<color=#B200BEFF>DEAD</color>", HPSlider.value.ToString("0"), HPSlider.maxValue.ToString("0"));
			MainHPSlider.GetComponentInChildren<Text>().text = string.Format("<color=#B200BEFF>DEAD</color>", MainHPSlider.value.ToString("0"), MainHPSlider.maxValue.ToString("0"));
		}

		MPSlider.GetComponentInChildren<Text>().text = MPSlider.value.ToString("0") + "/" + MPSlider.maxValue.ToString("0");
		SPSlider.GetComponentInChildren<Text>().text = SPSlider.value.ToString("0") + "/" + SPSlider.maxValue.ToString("0");

		MainMPSlider.GetComponentInChildren<Text>().text = MainMPSlider.value.ToString("0") + "/" + MainMPSlider.maxValue.ToString("0");


	}
	//Exp
	public void Update_Exp()
	{
		float percent = 100f - 100f * ((float)pStats.curRemainExp / ((float)pStats.lving.LvRequiredExp(pStats.curLv + 1) - (float)pStats.lving.LvRequiredExp(pStats.curLv)));
		if (pStats.curLv >= 200)
		{
			LvSlider.GetComponentInChildren<Text>().text = string.Format("Lv.200 (MAX)");
			LvSlider.value = 100;
		}
		else
		{
			LvSlider.GetComponentInChildren<Text>().text = string.Format("Lv.{0} ({1}%)", pStats.curLv, percent.ToString("0.0"));
			LvSlider.value = percent;
		}
		Lv.gameObject.GetComponentInChildren<Slider>().value = LvSlider.value;
		Lv.text = string.Format("Lv. {0}", pStats.curLv.ToString("0"));
	}

	void Update_Attributes()
	{
		Str.text = string.Format("{0} ({1})", pStats.curStr.ToString("0"), pStats.curStr.ToString("0"));
		Dex.text = string.Format("{0} ({1})", pStats.curDex.ToString("0"), pStats.curDex.ToString("0"));
		Int.text = string.Format("{0} ({1})", pStats.curInt.ToString("0"), pStats.curInt.ToString("0"));
		Will.text = string.Format("{0} ({1})", pStats.curWill.ToString("0"), pStats.curWill.ToString("0"));
		Luck.text = string.Format("{0} ({1})", pStats.curLuck.ToString("0"), pStats.curLuck.ToString("0"));

		PA.text = string.Format("{0}<color=#FFD500FF>({2})</color> ~ {1}<color=#FFD500FF>({3})</color>",
			(pStats.curPAmin + pStats.curWeaMin).ToString("0"),
			(pStats.curPAmax + pStats.curWeaMax).ToString("0"),
			pStats.curPAmin.ToString("0"),
			pStats.curPAmax.ToString("0"));
		MA.text = string.Format("{0} ({1})", pStats.curMA.ToString("0"), pStats.curMA.ToString("0"));
		Crit.text = string.Format("{0}% (+{1}%)", pStats.curCritRate.ToString("0"), pStats.curCritDam.ToString("0"));
		PDef.text = pStats.curPDef.ToString("0");
		PPro.text = pStats.curPPro.ToString("0");
		MDef.text = pStats.curMDef.ToString("0");
		MPro.text = pStats.curMPro.ToString("0");
		AP.text = string.Format("{0} ({1})", pStats.curAP.ToString("0"), pStats.curAP.ToString("0"));
		APts.text = pStats.curAPts.ToString("0");
	}
}
