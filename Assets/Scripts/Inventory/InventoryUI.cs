using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	public Transform itemParent;
	Inventory inv;
	InventorySlot[] slots;

	void Start()
	{
		inv = Inventory.instance;
		slots = itemParent.GetComponentsInChildren<InventorySlot>();
		inv.onItemChangedCallback += UpdateUI;
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i].slotIndex = i;
		}
	}

	void UpdateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (inv.Litems[i] != null && inv.Litems[i].ID == 0)
				slots[i].ClearSlot();
			else
			{
				slots[i].AddItem(inv.Litems[i]);
				slots[i].UseButton.interactable = inv.Litems[i].IsUsable;
				if (inv.Litems[i].MaxStack > 1) //if stackable
				{
					slots[i].amount.enabled = true;
					slots[i].amount.text = inv.SlotStack[i].ToString();
				}
				else
					slots[i].amount.enabled = false;
			}
		}
	}
}
