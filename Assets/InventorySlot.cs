﻿using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
	[SerializeField]
	Image icon;
	[SerializeField]
	Button removeButton;
	[SerializeField]
	Item item;

	public void AddItem(Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
		removeButton.interactable = true;
	}
	public void ClearSlot()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		removeButton.interactable = false;
	}
	public void OnRemoveButton()
	{
		Debug.Log("Remove(" + item.name+")");
		Inventory.instance.Remove(item);
	}

	public void UseItem()
	{
		if (item!= null)
		{
			item.Use();
		}
	}
}
