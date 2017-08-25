using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

	#region Singleton

	public static Inventory instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Inventory found!");
			return;
		}
		instance = this;
	}
	#endregion

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;
	public const int space = 20;
	public List<Item> Litems = new List<Item>();

	private PlayerStats pStats;

	private void Start()
	{
		pStats = PlayerStats.instance;	
	}

	public bool Add(Item item)
	{
		if (!item.isDefaultItem)
		{
			if (Litems.Count >= space)
			{
				Debug.Log("Inventory is FULL");
				return false;
			}
			if (item.isWeapon)
			{
				if (pStats.curWeaponID == item.weaponID)
				{
					pStats.curWeaMin += item.minDam;
					pStats.curWeaMax += item.maxDam;
				}
			}
			Litems.Add(item);
			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
			return true;
		}
		return false;
	}
	public void Remove(Item item)
	{
		Litems.Remove(item);
		if (item.isWeapon)
		{
			if (pStats.curWeaponID == item.weaponID)
			{
				pStats.curWeaMin -= item.minDam;
				pStats.curWeaMax -= item.maxDam;
			}
		}
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}
}
