using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Unit/Enemy")]
public class Enemy : Interactable
{
	PlayerManager playerManager;
	private void Start()
	{
		playerManager = PlayerManager.instance;
	}

	public override void Interact()
	{
		base.Interact();

	}
	//public void Die()
	//{
	//	Destroy(gameObject);
	//}
}
