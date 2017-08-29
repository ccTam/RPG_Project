using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{

	new public string name = "New Item";
	[SerializeField]
	private int itemID = -1;
	[SerializeField]
	private Sprite icon = null;
	[SerializeField]
	private bool isDefaultItem = false, isUseable = false;
	[SerializeField]
	private int weaponID = -1, maxStack = 0;
	[SerializeField]
	private float minDam = 0, maxDam = 0, bal = 0, crit = 0;

	public int ID { get { return itemID; } }
	public int MaxStack { get { return maxStack; } }
	public int WeaponID { get { return weaponID; } }
	public bool IsDefaultItem { get { return isDefaultItem; } }
	public bool IsUseable { get { return isUseable; } }

	public Sprite Icon { get { return icon; } }

	public float MinDam { get { return minDam; } }
	public float MaxDam { get { return maxDam; } }
	public float Bal { get { return bal; } }
	public float Crit { get { return crit; } }

	public virtual void Use()
	{
		Debug.Log("Used(" + name + ")");
	}
}
