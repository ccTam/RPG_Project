using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
	public EquipSlot equipSlot;
	[SerializeField]
	private WeaponType weaponID;
	[SerializeField]
	private float str = 0, dex = 0, inte = 0, will = 0, luck = 0,
					minDam = 0, maxDam = 0, bal = 0, critr = 0, critd = 0,
					pDef = 0, pPro = 0, mDef = 0, mPro = 0;	

	public WeaponType WeaponID { get { return weaponID; } }

	public float Str { get { return str; } set { str = value; } }
	public float Dex { get { return dex; } set { dex = value; } }
	public float Inte { get { return inte; } set { inte = value; } }
	public float Will { get { return will; } set { will = value; } }
	public float Luck { get { return luck; } set { luck = value; } }

	public float MinDam { get { return minDam; } set { minDam = value; } }
	public float MaxDam { get { return maxDam; } set { maxDam = value; } }
	public float Bal { get { return bal; } set { bal = value; } }
	public float CritR { get { return critr; } set { critr = value; } }
	public float CritD { get { return critd; } set { critd = value; } }

	public float PDef { get { return pDef; } set { pDef = value; } }
	public float PPro { get { return pPro; } set { pPro = value; } }
	public float MDef { get { return mDef; } set { mDef = value; } }
	public float MPro { get { return mPro; } set { mPro = value; } }

	public override void Use()
	{
		base.Use();
		EquipmentManager.instance.Equip(this);
	}

	public void ItemInit(int ID, string name, Sprite icon, bool isDefaultItem, EquipSlot equipSlot, WeaponType weaponID, bool isUsable, int maxStack, string tooltip, int goldValue, float str, float dex, float inte, float will, float luck, float minDam, float maxDam, float bal, float crit, float pDef, float pPro, float mDef, float mPro)
	{
		this.itemID = ID;
		this.itemType = ItemType.EQUIPMENT;
		this._name = name;
		this.name = "I" + name;
		this.icon = icon;
		this.isDefaultItem = isDefaultItem;
		this.equipSlot = equipSlot;
		this.weaponID = weaponID;
		this.isUsable = isUsable;
		this.maxStack = maxStack;
		this.tooltip = tooltip;
		this.str = str;
		this.dex = dex;
		this.inte = inte;
		this.will = will;
		this.luck = luck;
		this.minDam = minDam;
		this.maxDam = maxDam;
		this.bal = bal;
		this.critr = crit;
		this.pDef = pDef;
		this.pPro = pPro;
		this.mDef = mDef;
		this.mPro = mPro;
		this.goldValue = goldValue;
		this.resellValue = (int)(this.goldValue * .1f);
	}
}
public enum EquipSlot { Head, Chest, Legs, Feet, Weapon, OffHand }
public enum WeaponType { None, Melee, Archery, VoidMagic, DarkShield }