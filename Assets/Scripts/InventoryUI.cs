using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	[SerializeField]
	private Transform itemParent;
	Inventory inventory;
	InventorySlot[] slots;

	void Start()
	{
		inventory = Inventory.instance;
		slots = itemParent.GetComponentsInChildren<InventorySlot>();
		inventory.onItemChangedCallback += UpdateUI;
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i].slotIndex = i;
		}
	}

	void UpdateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (inventory.Litems[i].ID == 0)
				slots[i].ClearSlot();
			else
			{
				slots[i].AddItem(inventory.Litems[i]);
				slots[i].UseButton.interactable = inventory.Litems[i].IsUseable;
				if (inventory.Litems[i].MaxStack > 1) //if stackable
				{
					slots[i].amount.enabled = true;
					slots[i].amount.text = inventory.SlotStack[i].ToString();
				}
				else
					slots[i].amount.enabled = false;
			}
		}
	}
}
