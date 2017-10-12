using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
	private float baseValue = 0,
		itemModifierValue = 0,
		debuffValue = 0,
		currentValue = 0;

	private bool hasCurValue = false;

	public float BaseValue { get { return baseValue; } set { baseValue = value; } }
	public float ItemModValue { get { return itemModifierValue; } set { itemModifierValue = value; } }
	public float DebuffValue { get { return debuffValue; } set { debuffValue = value; } }
	public float CurValue
	{
		get { return hasCurValue ? currentValue : 0; }
		set { currentValue = value; }
	}
	public float FinalValue { get { return BaseValue + ItemModValue - DebuffValue; }}
	public float ModifiedValue { get { return ItemModValue - DebuffValue; } }

	public Stats()
	{
		this.baseValue = 0;
		this.itemModifierValue = 0;
		this.debuffValue = 0;
		this.CurValue = baseValue;
		this.hasCurValue = false;
	}

	public Stats(float baseValue)
	{
		this.baseValue = baseValue;
		this.itemModifierValue = 0;
		this.debuffValue = 0;
		this.CurValue = baseValue;
		this.hasCurValue = false;
	}

	public Stats(float baseValue, bool hasCurValue)
	{
		this.baseValue = baseValue;
		this.itemModifierValue = 0;
		this.debuffValue = 0;
		this.CurValue = baseValue;
		this.hasCurValue = hasCurValue;
	}
	public override string ToString()
	{
		return string.Format("Base:{0} ItemM:{1} Debuff:{2} CurValue:{3}", BaseValue, itemModifierValue, debuffValue, currentValue);
	}
}
