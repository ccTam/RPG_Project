using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{

	public float radius = 3f;
	public Transform interactionTransform;
	bool isFocus = false;
	bool isInteracting = false;
	Transform player;
	public bool canPickUp = true;

	public virtual void Interact()
	{
		Debug.Log("INTERACTING with: " + transform.name);
	}

	private void Start()
	{
		StartCoroutine(LookForInteract());
	}

	IEnumerator LookForInteract()
	{
		while (true)
		{
			if (isFocus && !isInteracting)
			{
				float distance = Vector3.Distance(player.position, interactionTransform.position);
				if (distance <= radius)
				{
					Interact();
					isInteracting = true;
					//if (canPickUp)
					//{
					//	pStats.bCanControl = false;
					//}
				}
			}
			yield return new WaitForSeconds(.03f);
		}
	}

	public void OnFocused(Transform playerTransform)
	{
		isFocus = true;
		player = playerTransform;
		isInteracting = false;
	}

	public void OnDeFocus()
	{
		isFocus = false;
		player = null;
		isInteracting = false;
	}

	private void OnDrawGizmosSelected()
	{
		if (interactionTransform == null)
		{
			interactionTransform = transform;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(interactionTransform.position, radius);
	}
}
