using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UIController : MonoBehaviour
{
	[SerializeField]
	GameObject IFCanvas, WorldCanvas, hpBar, InventoryWin, StatsWin;
	Slider MainHPSlider, MainHPBlur, MainMPSlider, MainSPSlider, HPSlider, MPSlider, SPSlider, LvSlider;
	Text Lv, Str, Dex, Int, Will, Luck, PA, Balance, MA, Crit, PDef, PPro, MDef, MPro, APen, APts;


	//[SerializeField]
	//GameObject tObj;
	//Transform player;
	private PlayerStats pStats;
	public Item testingItem;

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
		//player = GameObject.FindWithTag("Player").transform;
		//
		pStats = PlayerStats.instance;
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
		Balance = StatsWin.transform.Find("ItemsParent/BaseStats/Balance/Value").GetComponent<Text>();
		MA = StatsWin.transform.Find("ItemsParent/BaseStats/MagicAttack/Value").GetComponent<Text>();
		Crit = StatsWin.transform.Find("ItemsParent/BaseStats/Critical/Value").GetComponent<Text>();
		PDef = StatsWin.transform.Find("ItemsParent/BaseStats/P.Def/Value").GetComponent<Text>();
		PPro = StatsWin.transform.Find("ItemsParent/BaseStats/P.Pro/Value").GetComponent<Text>();
		MDef = StatsWin.transform.Find("ItemsParent/BaseStats/M.Def/Value").GetComponent<Text>();
		MPro = StatsWin.transform.Find("ItemsParent/BaseStats/M.Pro/Value").GetComponent<Text>();
		APen = StatsWin.transform.Find("ItemsParent/BaseStats/ArmorPierce/Value").GetComponent<Text>();
	}

	void Update()
	{
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
				MainHPBlur.value = Mathf.Lerp(MainHPBlur.value, MainHPSlider.value, 1.25f * Time.deltaTime);
		}
		else
			MainHPBlur.value = MainHPSlider.value;
	}

	void Update_HMSP()
	{
		HPSlider.maxValue = pStats.HP.FinalValue;
		MPSlider.maxValue = pStats.MP.FinalValue;
		SPSlider.maxValue = pStats.SP.FinalValue;
		HPSlider.value = pStats.HP.CurValue;
		MPSlider.value = pStats.MP.CurValue;
		SPSlider.value = pStats.SP.CurValue;
		MainHPSlider.maxValue = pStats.HP.FinalValue;
		MainHPBlur.maxValue = pStats.HP.FinalValue;
		MainMPSlider.maxValue = pStats.MP.FinalValue;
		MainSPSlider.maxValue = pStats.SP.FinalValue;
		MainHPSlider.value = pStats.HP.CurValue;
		MainMPSlider.value = pStats.MP.CurValue;
		MainSPSlider.value = pStats.SP.CurValue;

		if (pStats.bIsAlive)
		{
			if (pStats.bIsDeadly)
			{
				HPSlider.GetComponentInChildren<Text>().text = string.Format("<color=#B200BEFF>DEADLY</color> <color=#FF0000FF>({0})</color>", pStats.HP.CurValue.ToString("0"));
				MainHPSlider.GetComponentInChildren<Text>().text = string.Format("<color=#B200BEFF>DEADLY </color> <color=#FF0000FF>({0})</color>", pStats.HP.CurValue.ToString("0"));
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
		APts.text = pStats.curAPts.ToString("0");

		float t1, t2, t3, t4;
		t1 = pStats.Str.BaseValue; t2 = pStats.Str.FinalValue;
		Str.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.Dex.BaseValue; t2 = pStats.Dex.FinalValue;
		Dex.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.Int.BaseValue; t2 = pStats.Int.FinalValue;
		Int.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.Will.BaseValue; t2 = pStats.Will.FinalValue;
		Will.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.Luck.BaseValue; t2 = pStats.Luck.FinalValue;
		Luck.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.PAmin.BaseValue; t2 = pStats.PAmax.BaseValue;
		t3 = pStats.PAmin.CurValue; t4 = pStats.PAmax.CurValue;
		PA.text = (t1 != t3 || t2 != t4) ?
			string.Format("{0}<color=#FFD500FF>({2})</color> ~ {1}<color=#FFD500FF>({3})</color>", t1.ToString("0"), t2.ToString("0"), t3.ToString("0"), t4.ToString("0")) :
			string.Format("{0} ~ {1}", t1.ToString("0"), t2.ToString("0"));

		t1 = pStats.Balance.BaseValue; t2 = pStats.Balance.CurValue;
		Balance.text = t1 != t2 ? string.Format("{0}% <color=#FFD500FF>({1}%)</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}%", t1.ToString("0"));

		t1 = pStats.MA.BaseValue; t2 = pStats.MA.CurValue;
		MA.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		Crit.text = string.Format("{0}% <color=#FFB74BFF>(+{1}%)</color>", pStats.CritRate.CurValue.ToString("0"), ((pStats.CritDamage.CurValue - 1) * 100).ToString("0"));

		t1 = pStats.PDef.BaseValue; t2 = pStats.PDef.CurValue;
		PDef.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.PPro.BaseValue; t2 = pStats.PPro.CurValue;
		PPro.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.MDef.BaseValue; t2 = pStats.MDef.CurValue;
		MDef.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.MPro.BaseValue; t2 = pStats.MPro.CurValue;
		MPro.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

		t1 = pStats.APen.BaseValue; t2 = pStats.APen.CurValue;
		APen.text = t1 != t2 ? string.Format("{0} <color=#FFD500FF>({1})</color>", t1.ToString("0"), t2.ToString("0")) : string.Format("{0}", t1.ToString("0"));

	}
}