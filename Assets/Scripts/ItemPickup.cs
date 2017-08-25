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
			//int K = CoroutineController.instance.GetK();
			//if (K == -1) { return; }
			//CoroutineController.instance.Inst[K] = StartCoroutine(PauseForPickUp(K));
		}
		else
		{
			Debug.Log("CANNOT Pick Up!");
		}
	}
	
	IEnumerator PauseForPickUp(int K)
	{
		yield return new WaitForSeconds(1f);
		PickUp();
		StopCoroutine(PauseForPickUp(K));
		CoroutineController.instance.RevokeK(K);
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
