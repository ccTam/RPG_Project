using System.Collections;
using UnityEngine;

public class ItemPickup : Interactable {

	public Item item;

	public override void Interact()
	{
		base.Interact();
		if (canPickUp)
		{
			PickUp();
		}
		else
			Debug.Log("CANNOT Pick Up!");
	}

	void PickUp()
	{
		Debug.Log("Picked up: " + item.name);
		if (Inventory.instance.Add(item))
		{
			Destroy(gameObject);
		}
	}
}
