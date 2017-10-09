using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

	public ItemDatabase itemDB;
	public string[] itemName;
	private const string DATABASE_PATH = @"Database/ItemDatabase";

	private void Start()
	{
		//itemJsonString = File.ReadAllText(Application.dataPath + "/Resources/Items.json");
		//Debug.Log(itemJsonString);
		itemDB = (ItemDatabase)Resources.Load(DATABASE_PATH, typeof(ItemDatabase));
		//TESTING
		if (itemDB == null) { Debug.LogWarning("itemDB is empty"); return; }
		//Debug.LogWarning("Item COUNT = " + itemDB.COUNT);
		//Debug.LogWarning("Item at (4) = " + itemDB.GetItem(4).Name + " ObjName: " + itemDB.GetItem(4).name);
		//Debug.LogWarning("Item with ID(4) = " + itemDB.GetItemByID(4).Name + " ObjName: " + itemDB.GetItemByID(4).name);
		itemName = new string[SPACE];
		//EO TESTING
		SlotStack = new int[SPACE];
		for (int i = 0; i < SPACE; i++)
		{
			Litems.Add(itemDB.GetItemByID(0));
			SlotStack[i] = 0;
		}
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			//Debug.Log("itemDB.COUNT: " + itemDB.COUNT);
			int randomID = Random.Range(1, itemDB.COUNT);
			Debug.Log("randomItemID: " + randomID);
			Item testingItem = itemDB.GetItemByID(randomID);
			//Debug.Log(testingItem.Name);
			Add(testingItem);
		}
	}

	public bool Add(Item item)
	{
		bool hasRoom = false;
		for (int i = 0; i < SPACE; i++)
		{
			if (Litems[i] != null && Litems[i].ID == 0)
			{
				Debug.Log(string.Format("Adding \"{2}\" at itemSlot-{0} ID: {1}", i, Litems[i].ID, item.Name));
				Litems[i] = item;
				SlotStack[i] = 1;
				hasRoom = true;
				break;
			}
		}
		if (!hasRoom)
		{
			Debug.LogWarning("INVENTORY is FULL");
			return false;
		}
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
		return true;
	}

	public void Remove(int slotIndex)
	{
		Litems[slotIndex] = itemDB.GetItemByID(0);
		SlotStack[slotIndex] = 0;
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

}
