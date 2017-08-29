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
	public int OnDragItemID = 0, OnDragSlot = -1;
	public int[] SlotStack;

	[SerializeField]
	Item EMPTY, TEST;

	private void Start()
	{
		SlotStack = new int[SPACE];
		for (int i = 0; i < SPACE; i++)
		{
			Litems.Add(EMPTY);
			SlotStack[i] = 0;
		}
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}


	public bool Add(Item item)
	{
		bool hasRoom = false;
		for (int i = 0; i < SPACE; i++)
		{
			if (Litems[i].ID == 0)
			{
				Litems[i] = item;
				SlotStack[i] = 1;
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
	public void Remove(int slotIndex)
	{
		Litems[slotIndex] = EMPTY;
		SlotStack[slotIndex] = 0;
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	public Item GetItemByID(int ID)
	{
		Item[] foundItems = (Item[])Resources.FindObjectsOfTypeAll(typeof(Item));
		foreach (Item resourceItem in foundItems)
			if (resourceItem.ID == ID)
				return resourceItem;
		return null;
	}
}
