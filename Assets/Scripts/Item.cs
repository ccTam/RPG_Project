using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{

	new public string name = "New Item";
	[SerializeField]
	private int itemID = -1;
	public Sprite icon = null;
	public bool isDefaultItem = false, isUseable = false;
	public int weaponID = -1;
	public int maxStack = 0;
	public int amount = 0;

	public float minDam, maxDam;

	public int ID { get { return itemID; } }

	public virtual void Use()
	{
		Debug.Log("Used(" + name + ")");
	}
}
