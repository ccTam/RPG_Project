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
	[SerializeField]
	Equipment[] curEquipment;
	Inventory inv;

	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;

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

	}
}
