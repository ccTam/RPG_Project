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
	public const int SPACE = 20;
	public List<Item> Litems = new List<Item>();
	public int OnDragItemID = 0;
	public int OnDragSlot = -1;

	[SerializeField]
	Item EMPTY, TEST;

	private PlayerStats pStats;

	private void Start()
	{
		pStats = PlayerStats.instance;
		for (int i = 0; i < SPACE; i++)
		{
			Litems.Add(EMPTY);
		}
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	public bool Add(Item item)
	{
		if (!item.isDefaultItem)
		{
			bool hasRoom = false;
			for (int i = 0; i < SPACE; i++)
			{
				if (Litems[i].ID == 0)
				{
					Litems[i] = item;
					hasRoom = true;
					break;
				}
			}
			if (!hasRoom)
			{
				Debug.Log("INVENTORY is FULL");
				return false;
			}
			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
			return true;
		}
		return false;
	}
	public void Remove(int slotIndex)
	{
		Litems[slotIndex] = EMPTY;
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	public Item GetItemByID(int ID)
	{
		Item[] foundItems = (Item[])Resources.FindObjectsOfTypeAll(typeof(Item));
		foreach (Item resourceItem in foundItems)
		{
			if (resourceItem.ID == ID)
			{
				return resourceItem;
			}
		}
		return null;
	}
}
