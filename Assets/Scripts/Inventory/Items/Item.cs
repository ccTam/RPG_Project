using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
	new public string name = "New Item";
	[SerializeField]
	private int itemID = 0;
	[SerializeField]
	private Sprite icon = null;
	[SerializeField]
	private bool isDefaultItem = false, isUseable = false;
	[SerializeField]
	private int maxStack = 0;
	[SerializeField]
	[TextArea(3, 10)]
	private string tooltip = "Tooltip MISSING!";

	public int ID { get { return itemID; } }
	public int MaxStack { get { return maxStack; } }

	public bool IsDefaultItem { get { return isDefaultItem; } }
	public bool IsUseable { get { return isUseable; } }

	public Sprite Icon { get { return icon; } }

	public virtual void Use()
	{
		Debug.Log("Used(" + name + ")");
	}
}
