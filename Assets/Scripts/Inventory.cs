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

	public bool Add(Item item)
	{
		if (!item.isDefaultItem)
		{
			if (Litems.Count >= space)
			{
				Debug.Log("Inventory is FULL");
				return false;
			}
			Litems.Add(item);
			if (onItemChangedCallback != null)
			{
				onItemChangedCallback.Invoke();
			}
			return true;
		}
		return false;
	}
	public void Remove(Item item)
	{
		Litems.Remove(item);
		if (onItemChangedCallback != null)
		{
			onItemChangedCallback.Invoke();
		}
	}

}
