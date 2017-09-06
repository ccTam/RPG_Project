using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

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

	Equipment[] curEquipment;
	Inventory inv;

	public delegate void OnequipmentChanged(Equipment newItem, Equipment oldItem);
	public OnequipmentChanged onequipmentChanged;

	void Start()
	{
		inv = Inventory.instance;

		int numSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
		curEquipment = new Equipment[numSlots];
	}

	public void Equip (Equipment newItem)
	{
		int equipSlotIndex = (int)newItem.equipSlot;
		Equipment oldItem = null;
		if (curEquipment[equipSlotIndex] != null)
		{
			oldItem = curEquipment[equipSlotIndex];
			inv.Add(oldItem);
		}
		curEquipment[equipSlotIndex] = newItem;
		if (onequipmentChanged != null)
		{
			onequipmentChanged(newItem, oldItem);
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
		if (onequipmentChanged != null)
		{
			onequipmentChanged(null, oldItem);
		}
	}

	void UnequipAll()
	{
		for (int i = 0; i < curEquipment.Length; i++)
		{
			Unequip(i);
		}
	}
}
