using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{

	public EquipSlot equipSlot;
	[SerializeField]
	private WeaponID weaponID;
	[SerializeField]
	private float str = 0, dex = 0, inte = 0, will = 0, luck = 0;
	[SerializeField]
	private float minDam = 0, maxDam = 0, bal = 0, crit = 0;
	[SerializeField]
	private float pDef = 0, pPro = 0, mDef = 0, mPro = 0;
	[SerializeField]
	

	public int WeaponID { get { return (int)weaponID; } }

	public float Str { get { return str; } /*set { str = value; }*/ }
	public float Dex { get { return dex; } /*set { dex = value; }*/ }
	public float Inte { get { return inte; } /*set { inte = value; }*/ }
	public float Will { get { return will; } /*set { will = value; }*/ }
	public float Luck { get { return luck; } /*set { luck = value; }*/ }

	public float MinDam { get { return minDam; } }
	public float MaxDam { get { return maxDam; } }
	public float Bal { get { return bal; } }
	public float Crit { get { return crit; } }

	public float PDef { get { return pDef; } }
	public float PPro { get { return pPro; } }
	public float MDef { get { return mDef; } }
	public float MPro { get { return mPro; } }



	public override void Use()
	{
		base.Use();
		EquipmentManager.instance.Equip(this);
	}
}

public enum EquipSlot { Head, Chest, Legs, Feet, Weapon, OffHand }
public enum WeaponID { None, Melee, Archery, VoidMagic, DarkShield }