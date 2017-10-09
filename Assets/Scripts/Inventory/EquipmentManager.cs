using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{

	#region Singleton

	public static EquipmentManager instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of EquipmentManager found!");
			return;
		}
		instance = this;
	}
	#endregion
	[SerializeField]
	Equipment[] curEquipment;
	Inventory inv;
	PlayerStats pStats;

	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;

	void Start()
	{
		inv = Inventory.instance;
		pStats = PlayerStats.instance;
		int numSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
		curEquipment = new Equipment[numSlots];
		onEquipmentChanged += ChangeStats;
	}

	public void Equip(Equipment newItem)
	{
		int equipSlotIndex = (int)newItem.equipSlot;
		Equipment oldItem = null;
		if (curEquipment[equipSlotIndex] != null)
		{
			oldItem = curEquipment[equipSlotIndex];
			inv.Add(oldItem);
		}
		curEquipment[equipSlotIndex] = newItem;
		if (onEquipmentChanged != null)
		{
			onEquipmentChanged(newItem, oldItem);
		}
	}

	public void Unequip(int equipSlotIndex)
	{
		Equipment oldItem = null;
		if (curEquipment[equipSlotIndex] != null)
		{
			oldItem = curEquipment[equipSlotIndex];
			inv.Add(oldItem);

			curEquipment[equipSlotIndex] = null;
		}
		if (onEquipmentChanged != null)
		{
			onEquipmentChanged(null, oldItem);
		}
	}

	void UnequipAll()
	{
		for (int i = 0; i < curEquipment.Length; i++)
			Unequip(i);
	}

	void ChangeStats(Equipment newItem, Equipment oldItem)
	{
		if (oldItem != null)
		{
			pStats.curStr -= oldItem.Str;
			pStats.curDex -= oldItem.Dex;
			pStats.curInt -= oldItem.Inte;
			pStats.curWill -= oldItem.Will;
			pStats.curLuck -= oldItem.Luck;
			pStats.curItemBasicAttributes[0] -= oldItem.Str;
			pStats.curItemBasicAttributes[1] -= oldItem.Dex;
			pStats.curItemBasicAttributes[2] -= oldItem.Inte;
			pStats.curItemBasicAttributes[3] -= oldItem.Will;
			pStats.curItemBasicAttributes[4] -= oldItem.Luck;

			pStats.curItemPDef -= oldItem.PDef;
			pStats.curItemPPro -= oldItem.PPro;
			pStats.curItemMDef -= oldItem.MDef;
			pStats.curItemMPro -= oldItem.MPro;

			pStats.curItemMin -= oldItem.MinDam;
			pStats.curItemMax -= oldItem.MaxDam;
			pStats.curItemBal -= oldItem.Bal;
			pStats.curItemCrit -= oldItem.Crit;
		}
		if (newItem != null)
		{
			pStats.curStr += newItem.Str;
			pStats.curDex += newItem.Dex;
			pStats.curInt += newItem.Inte;
			pStats.curWill += newItem.Will;
			pStats.curLuck += newItem.Luck;
			pStats.curItemBasicAttributes[0] += newItem.Str;
			pStats.curItemBasicAttributes[1] += newItem.Dex;
			pStats.curItemBasicAttributes[2] += newItem.Inte;
			pStats.curItemBasicAttributes[3] += newItem.Will;
			pStats.curItemBasicAttributes[4] += newItem.Luck;

			pStats.curItemPDef += newItem.PDef;
			pStats.curItemPPro += newItem.PPro;
			pStats.curItemMDef += newItem.MDef;
			pStats.curItemMPro += newItem.MPro;

			pStats.curItemMin += newItem.MinDam;
			pStats.curItemMax += newItem.MaxDam;
			pStats.curItemBal += newItem.Bal;
			pStats.curItemCrit += newItem.Crit;
		}
		if (pStats.OnStatsChangeCallback != null)
			pStats.OnStatsChangeCallback.Invoke();
	}
}
