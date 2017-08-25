using UnityEngine;

public class InventoryUI : MonoBehaviour {
	[SerializeField]
	private Transform itemParent;
	Inventory inventory;
	InventorySlot[] slots;

	void Start () {
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;

		slots = itemParent.GetComponentsInChildren<InventorySlot>();
		Debug.Log("SlotSize:"+slots.Length);
	}

	void UpdateUI () {
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.Litems.Count)
			{
				slots[i].AddItem(inventory.Litems[i]);
			}
			else
				slots[i].ClearSlot();
		}
	}
}
