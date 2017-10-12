using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
	[SerializeField]
	protected string _name = string.Empty;
	[SerializeField]
	protected int itemID = 0, maxStack = 0;
	[SerializeField]
	protected Sprite icon = null;
	[SerializeField]
	protected bool isDefaultItem = false, isUsable = false;
	[SerializeField]
	[TextArea(3, 10)]
	protected string tooltip = string.Empty;
	[SerializeField]
	protected ItemType itemType;
	[SerializeField]
	protected int resellValue = 0;
	[SerializeField]
	protected int goldValue = 0;

	public int ID { set { itemID = value; } get { return itemID; } }
	public int MaxStack { set { maxStack = value; } get { return maxStack; } }

	public Sprite Icon { set { icon = value; } get { return icon; } }

	public bool IsDefaultItem { set { isDefaultItem = value; } get { return isDefaultItem; } }
	public bool IsUsable { set { isUsable = value; } get { return isUsable; } }

	public string Name { get { return _name; } set { _name = value; } }
	public string Tooltip { get { return tooltip; } set { tooltip = value; } }
	public ItemType ItemType { get { return itemType; } }

	public int GoldValue { get { return goldValue; } set { goldValue = value; } }
	public int ResellValue { get { return resellValue; } set { resellValue = value; } }

	public void ItemInit(int ID, string name, Sprite icon, bool isDefaultItem, bool isUseable, int maxStack, string tooltip, int goldValue)
	{
		this.itemID = ID;
		this.itemType = ItemType.BASIC;
		this._name = name;
		this.name = "I" + name;
		this.icon = icon;
		this.isDefaultItem = isDefaultItem;
		this.isUsable = IsUsable;
		this.maxStack = maxStack;
		this.tooltip = tooltip;
		this.goldValue = goldValue;
		this.resellValue = (int)(this.goldValue * .1f);
	}

	public virtual void Use()
	{
		Debug.Log("Used(" + _name + ")");
	}

}
public enum ItemType { BASIC, CONSUMABLE, EQUIPMENT }
