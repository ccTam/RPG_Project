using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ItemDatabase : ScriptableObject
{
	public static ItemDatabase instance;

	[SerializeField]
	private List<Item> database;
	[SerializeField]
	private List<int> ID;
	//private Dictionary<int, Item> itemDictionary;

	public List<int> GetIDList { get { return ID; } set { ID = value; } }

	void OnEnable()
	{
		if (database == null)
		{
			database = new List<Item>();
			ID = new List<int>();
		}
	}

	public List<Item> GetItemData()
	{
		return database;
	}

	public void Add(Item newItem)
	{
		database.Add(newItem);
		//itemDictionary.Add(newItem.ID, newItem);
		//Debug.Log("Dictionary Count: " + itemDictionary.Count);
		//Debug.Log("newItem.Name: " + itemDictionary[newItem.ID].Name);
	}

	public void Remove(Item item)
	{
		database.Remove(item);
		//itemDictionary.Remove(item.ID);
	}

	public void RemoveAt(int index)
	{
		if (ID == null) { Debug.LogError("ID not init"); return; }
		//Get the removed item's ID
		ID.Add(database[index].ID);
		database.RemoveAt(index);
		//itemDictionary.Remove(database[index].ID);
		//Debug.Log("=====0=====\n");
		//for (int i = 0; i < ID.Count; i++)
		//{
		//	Debug.Log(string.Format("ID[{0}] = {1}\n", i, ID[i]));
		//}
		//Debug.Log("0=========0");
	}

	public int GetNextID()
	{
		if (ID == null) { Debug.LogError("ID not init2"); return -1; }
		if (ID.Count == 0)
		{
			return database.Count;
		}
		else
		{
			int NextID = ID[0];
			return NextID;
		}
	}

	public void ClearNextID()
	{
		if (ID == null) { Debug.LogError("ID not init3"); return; }
		if (ID.Count > 0)
			ID.RemoveAt(0);
	}

	public int COUNT
	{
		get { return database.Count; }
	}

	public Item GetItem(int id)
	{
		return database[id];
	}

	//.ElementAt() requires the System.Linq
	//Get item ID by different types
	public Item GetItemByID(int id)
	{
		//ORG
		if (database.Count == 0) { Debug.LogError("ItemDatabse is empty"); return null; }
		for (int i = 0; i < database.Count; i++)
		{
			//Debug.Log("Searching on: " + i + " and its ID:" + database[i].ID);
			if (database[i].ID == id)
			{
				//Debug.Log("Item Found! -> " + i);
				return database[i];
			}
		}
		Debug.LogError("Item NOT Found!  -> NULL");
		return database[0];
		//#EO ORG
		//Item tempItem = null;
		//if (!itemDictionary.TryGetValue(id, out tempItem))
		//{
		//	Debug.LogError("Item NOT Found!  -> NULL");
		//	return itemDictionary[0];
		//}
		//else
		//	return tempItem;
	}

	public Consumable GetConsumableByID(int id)
	{
		for (int i = 0; i < database.Count; i++) 
		{
			if (database[i].ID == id)
			{
				return (Consumable)database[i];
			}
		}
		return null;
	}

	public Equipment GetEquipmentByID(int id)
	{
		for (int i = 0; i < database.Count; i++)
		{
			if (database[i].ID == id)
			{
				return (Equipment)database[i];
			}
		}
		return null;
	}

	//Sort by Names
	public void SortAlphabeticallyAtoZ()
	{
		database.Sort((x, y) => string.Compare(x.Name, y.Name));
	}
}
