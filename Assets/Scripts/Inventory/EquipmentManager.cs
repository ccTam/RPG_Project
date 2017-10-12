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
			pStats.Str.ItemModValue -= oldItem.Str;
			pStats.Dex.ItemModValue -= oldItem.Dex;
			pStats.Int.ItemModValue -= oldItem.Inte;
			pStats.Will.ItemModValue -= oldItem.Will;
			pStats.Luck.ItemModValue -= oldItem.Luck;

			pStats.PDef.ItemModValue -= oldItem.PDef;
			pStats.PPro.ItemModValue -= oldItem.PPro;
			pStats.MDef.ItemModValue -= oldItem.MDef;
			pStats.MPro.ItemModValue -= oldItem.MPro;

			pStats.PAmin.ItemModValue -= oldItem.MinDam;
			pStats.PAmax.ItemModValue -= oldItem.MaxDam;
			pStats.Balance.ItemModValue -= oldItem.Bal;
			pStats.CritRate.ItemModValue -= oldItem.CritR;
		}
		if (newItem != null)
		{
			pStats.curWeaponID = newItem.WeaponID;
			pStats.Str.ItemModValue += newItem.Str;
			pStats.Dex.ItemModValue += newItem.Dex;
			pStats.Int.ItemModValue += newItem.Inte;
			pStats.Will.ItemModValue += newItem.Will;
			pStats.Luck.ItemModValue += newItem.Luck;

			pStats.PDef.ItemModValue += newItem.PDef;
			pStats.PPro.ItemModValue += newItem.PPro;
			pStats.MDef.ItemModValue += newItem.MDef;
			pStats.MPro.ItemModValue += newItem.MPro;

			pStats.PAmin.ItemModValue += newItem.MinDam;
			pStats.PAmax.ItemModValue += newItem.MaxDam;
			pStats.Balance.ItemModValue += newItem.Bal;
			pStats.CritRate.ItemModValue += newItem.CritR;
		}
		else
		{
			pStats.curWeaponID = WeaponType.None;
		}
		if (pStats.onStatsChangeCallback != null)
			pStats.onStatsChangeCallback.Invoke();
	}
}
