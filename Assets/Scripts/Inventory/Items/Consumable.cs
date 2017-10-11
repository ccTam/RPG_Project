using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Potion")]
public class Consumable : Item
{
	[SerializeField]
	private float hp = 0, mp = 0, sp = 0, dur = 0;

	public float HP { get { return hp; } set { hp = value; } }
	public float MP { get { return mp; } set { mp = value; } }
	public float SP { get { return sp; } set { sp = value; } }
	public float Dur { get { return dur; } set { dur = value; } }

	public override void Use()
	{
		base.Use();
		float interval = .5f;
		Combat.instance.Hot(hp * interval / dur, mp * interval / dur, sp * interval / dur, interval, dur);
	}

	public void ItemInit(int ID, string name, Sprite icon, bool isDefaultItem, bool isUseable, int maxStack, string tooltip, int goldValue, float hp, float mp, float sp, float dur)
	{
		this.itemID = ID;
		this.itemType = ItemType.CONSUMABLE;
		this._name = name;
		this.name = "I" + name;
		this.icon = icon;
		this.isDefaultItem = isDefaultItem;
		this.isUseable = IsUseable;
		this.maxStack = maxStack;
		this.tooltip = tooltip;
		this.hp = hp;
		this.mp = mp;
		this.sp = sp;
		this.dur = dur;
		this.goldValue = goldValue;
		this.resellValue = (int)(this.goldValue * .1f);
	}
}
