using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {

	public EquipSlot equipSlot;
	[SerializeField]
	private float minDam = 0, maxDam = 0, bal = 0, crit = 0;
	[SerializeField]
	private int weaponID = -1;

	public int WeaponID { get { return weaponID; } }

	public float MinDam { get { return minDam; } }
	public float MaxDam { get { return maxDam; } }
	public float Bal { get { return bal; } }
	public float Crit { get { return crit; } }

	public override void Use()
	{
		base.Use();
		EquipmentManager.instance.Equip(this);
	}
}

public enum EquipSlot { Head, Chest, Lets, Weapon, OffHand, Feet}