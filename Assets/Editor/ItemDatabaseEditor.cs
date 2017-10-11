using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class ItemDatabaseEditor : EditorWindow
{
	private enum State { BLANK, EDIT, ADD }
	//private enum Filter { ALL, BASIC, CONSUMABLE, EQUIPMENT }

	private State state;
	private ItemType itemType;
	private WeaponType weaponType;
	private EquipSlot equipType;
	private int selectedItem = 0, itemMaxStack = 0, nextID = 0, goldValue = 0, resellValue = 0;
	private string itemName, itemTooltip;
	private float str = 0, dex = 0, inte = 0, will = 0, luck = 0,
				minDam = 0, maxDam = 0, bal = 0, crit = 0,
				pDef = 0, pPro = 0, mDef = 0, mPro = 0,
				hp = 0, mp = 0, sp = 0, dur = 0;
	private bool isDefaultItem, isUseable;
	private Sprite itemSprite = null;

	private const string DATABASE_PATH = @"Assets/Resources/Database/ItemDatabase.asset";
	private const string DATABASE_PATH_ITEMS = @"Assets/Resources/Items/";

	private ItemDatabase itemDatabase;
	private Vector2 _leftScrollPos, _rightScrollPos;

	[MenuItem("Database/Item Database %#w")]
	public static void Init()
	{
		ItemDatabaseEditor window = GetWindow<ItemDatabaseEditor>("Database");
		window.minSize = new Vector2(400, 300);
		window.Show();
	}

	void OnEnable()
	{
		if (itemDatabase == null)
			LoadDatabase();

		state = State.BLANK;
	}

	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		DisplayListArea();
		DisplayMainArea();
		EditorGUILayout.EndHorizontal();
	}

	void LoadDatabase()
	{
		itemDatabase = (ItemDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(ItemDatabase));
		if (itemDatabase == null)
		{
			Debug.Log("CREATE new DB");
			CreateDatabase();
		}
		else
		{
			Debug.Log("COUNT: " + itemDatabase.COUNT);
			for (int i = 0; i < itemDatabase.COUNT; i++)
			{
				Debug.Log(String.Format("[{0}]: ({1}){2}", i, itemDatabase.GetItem(i).name, itemDatabase.GetItem(i).Name));
			}
		}
	}

	void CreateDatabase()
	{
		itemDatabase = CreateInstance<ItemDatabase>();
		AssetDatabase.CreateAsset(itemDatabase, DATABASE_PATH);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	void DisplayListArea()
	{

		EditorGUILayout.BeginVertical(GUILayout.Width(250));
		EditorGUILayout.Space();

		_leftScrollPos = EditorGUILayout.BeginScrollView(_leftScrollPos, "box", GUILayout.ExpandHeight(true));
		for (int i = 0; i < itemDatabase.COUNT; i++)
		{
			Item tempItem = itemDatabase.GetItem(i);
			if (tempItem == null) { continue; }
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("-", GUILayout.Width(25)))
			{
				AssetDatabase.DeleteAsset(DATABASE_PATH_ITEMS + tempItem.ID.ToString("000") + ".asset");
				itemDatabase.RemoveAt(i);
				itemDatabase.SortAlphabeticallyAtoZ();
				EditorUtility.SetDirty(itemDatabase);
				state = State.BLANK;
				return;
			}
			string sMaxStack = tempItem.MaxStack > 0 ? tempItem.MaxStack.ToString("00") : "00";
			string sUseable = tempItem.IsUseable ? "*" : "-";
			if (GUILayout.Button(String.Format("#{0} - {1}\\{2}{3}", tempItem.ID.ToString("000"), sMaxStack, sUseable, tempItem.Name), "box", null))
			{
				selectedItem = i;
				state = State.EDIT;
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		EditorGUILayout.LabelField("Items: " + itemDatabase.COUNT, GUILayout.Width(100));

		if (GUILayout.Button("New Item"))
			state = State.ADD;

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
	}

	void DisplayMainArea()
	{
		EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
		EditorGUILayout.Space();

		switch (state)
		{
			case State.ADD:
				DisplayAddMainArea();
				break;
			case State.EDIT:
				DisplayEditMainArea();
				break;
			default:
				DisplayBlankMainArea();
				break;
		}

		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
	}

	void DisplayBlankMainArea()
	{
		EditorGUILayout.LabelField(
			"1) Click \"New Item\" to add new item\n" +
			"2) Select type of the new item\n" +
			"3) Adjust values of the item\n" +
			"4) Click \"Add\" to save the new item to the database\n" +
			"5) Click on an item to edit (updating is in real time)\n" +
			"6) Click the \"-\" button to delete an item",
			GUILayout.ExpandHeight(true));
	}

	void DisplayEditMainArea()
	{
		_rightScrollPos = EditorGUILayout.BeginScrollView(_rightScrollPos, "box", GUILayout.ExpandHeight(true));
		//Basic
		GUILayout.Label(new GUIContent("ID: " + itemDatabase.GetItem(selectedItem).ID.ToString()), EditorStyles.boldLabel);
		GUILayout.Label(new GUIContent("Type: " + itemDatabase.GetItem(selectedItem).ItemType.ToString()), EditorStyles.boldLabel);
		itemDatabase.GetItem(selectedItem).Icon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Sprite: "), itemDatabase.GetItem(selectedItem).Icon, typeof(Sprite), true);
		itemDatabase.GetItem(selectedItem).Name = EditorGUILayout.TextField(new GUIContent("Name: "), itemDatabase.GetItem(selectedItem).Name);
		itemDatabase.GetItem(selectedItem).MaxStack = int.Parse(EditorGUILayout.TextField(new GUIContent("Max Stack: "), itemDatabase.GetItem(selectedItem).MaxStack.ToString()));
		itemDatabase.GetItem(selectedItem).GoldValue = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Gold Value: "), itemDatabase.GetItem(selectedItem).GoldValue.ToString()));
		itemDatabase.GetItem(selectedItem).ResellValue = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Resell Value: "), itemDatabase.GetItem(selectedItem).ResellValue.ToString()));
		itemDatabase.GetItem(selectedItem).IsDefaultItem = EditorGUILayout.Toggle(new GUIContent("Default: "), itemDatabase.GetItem(selectedItem).IsDefaultItem);

		switch (itemDatabase.GetItem(selectedItem).ItemType)
		{
			case ItemType.BASIC:
				itemDatabase.GetItem(selectedItem).IsUseable = EditorGUILayout.Toggle(new GUIContent("Useable: "), itemDatabase.GetItem(selectedItem).IsUseable);
				break;
			case ItemType.CONSUMABLE:
				itemDatabase.GetItem(selectedItem).IsUseable = EditorGUILayout.Toggle(new GUIContent("Useable: "), true);
				Consumable conItem = (Consumable)itemDatabase.GetItem(selectedItem);
				conItem.HP = float.Parse(EditorGUILayout.TextField(new GUIContent("Total HP Regenerate: "), conItem.HP.ToString()));
				conItem.MP = float.Parse(EditorGUILayout.TextField(new GUIContent("Total MP Regenerate: "), conItem.MP.ToString()));
				conItem.SP = float.Parse(EditorGUILayout.TextField(new GUIContent("Total SP Regenerate: "), conItem.SP.ToString()));
				conItem.Dur = float.Parse(EditorGUILayout.TextField(new GUIContent("Effect Duration: "), conItem.Dur.ToString()));
				break;
			case ItemType.EQUIPMENT:
				itemDatabase.GetItem(selectedItem).IsUseable = EditorGUILayout.Toggle(new GUIContent("Useable: "), true);
				Equipment eqItem = (Equipment)itemDatabase.GetItem(selectedItem);
				eqItem.Str = float.Parse(EditorGUILayout.TextField(new GUIContent("Strength: "), eqItem.Str.ToString()));
				eqItem.Dex = float.Parse(EditorGUILayout.TextField(new GUIContent("Dexterity: "), eqItem.Dex.ToString()));
				eqItem.Inte = float.Parse(EditorGUILayout.TextField(new GUIContent("Intelligence: "), eqItem.Inte.ToString()));
				eqItem.Will = float.Parse(EditorGUILayout.TextField(new GUIContent("Will: "), eqItem.Will.ToString()));
				eqItem.Luck = float.Parse(EditorGUILayout.TextField(new GUIContent("Luck: "), eqItem.Luck.ToString()));
				eqItem.MinDam = float.Parse(EditorGUILayout.TextField(new GUIContent("Min.Damage: "), eqItem.MinDam.ToString()));
				eqItem.MaxDam = float.Parse(EditorGUILayout.TextField(new GUIContent("Max.Damage: "), eqItem.MaxDam.ToString()));
				eqItem.Bal = float.Parse(EditorGUILayout.TextField(new GUIContent("Balance: "), eqItem.Bal.ToString()));
				eqItem.CritR = float.Parse(EditorGUILayout.TextField(new GUIContent("Critical: "), eqItem.CritR.ToString()));
				eqItem.PDef = float.Parse(EditorGUILayout.TextField(new GUIContent("P.Defense: "), eqItem.PDef.ToString()));
				eqItem.PPro = float.Parse(EditorGUILayout.TextField(new GUIContent("P.Protection: "), eqItem.PPro.ToString()));
				eqItem.MDef = float.Parse(EditorGUILayout.TextField(new GUIContent("M.Defense: "), eqItem.MDef.ToString()));
				eqItem.MPro = float.Parse(EditorGUILayout.TextField(new GUIContent("M.Protection: "), eqItem.MPro.ToString()));

				break;

			default:
				Debug.LogError("Unrecognized ItemType");
				break;
		}

		//+Tooltip
		itemDatabase.GetItem(selectedItem).Tooltip = EditorGUILayout.TextField(new GUIContent("Tooltip: "), itemDatabase.GetItem(selectedItem).Tooltip, GUILayout.Height(100));
		EditorGUILayout.Space();

		//Done and Save to disk
		if (GUILayout.Button("Done", GUILayout.Width(100)))
		{
			itemDatabase.SortAlphabeticallyAtoZ();
			EditorUtility.SetDirty(itemDatabase);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			state = State.BLANK;
		}
		EditorGUILayout.EndScrollView();
	}

	void DisplayAddMainArea()
	{
		_rightScrollPos = EditorGUILayout.BeginScrollView(_rightScrollPos, "box", GUILayout.ExpandHeight(true));
		nextID = itemDatabase.GetNextID();
		GUILayout.Label(new GUIContent("ID: " + nextID.ToString()), EditorStyles.boldLabel);
		itemType = (ItemType)EditorGUILayout.EnumPopup(new GUIContent("Item Type: "), itemType, GUILayout.Width(300));

		itemSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Sprite: "), itemSprite, typeof(Sprite), true);
		itemName = EditorGUILayout.TextField(new GUIContent("Name: "), itemName);
		itemMaxStack = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Max Stack: "), itemMaxStack.ToString()));
		goldValue = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Gold Value: "), goldValue.ToString()));
		isDefaultItem = Convert.ToBoolean(EditorGUILayout.Toggle(new GUIContent("Default: "), isDefaultItem));

		switch (itemType)
		{
			case ItemType.BASIC:
				isUseable = Convert.ToBoolean(EditorGUILayout.Toggle(new GUIContent("Useable: "), isUseable));
				break;
			case ItemType.CONSUMABLE:
				isUseable = Convert.ToBoolean(EditorGUILayout.Toggle(new GUIContent("Useable: "), true));
				hp = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Total HP Regenerate: "), hp.ToString()));
				mp = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Total MP Regenerate: "), mp.ToString()));
				sp = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Total SP Regenerate: "), sp.ToString()));
				dur = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Effect Duration: "), dur.ToString()));
				break;
			case ItemType.EQUIPMENT:
				equipType = (EquipSlot)EditorGUILayout.EnumPopup(new GUIContent("Equip Type: "), equipType, GUILayout.Width(300));
				if (equipType == EquipSlot.Weapon)
					weaponType = (WeaponType)EditorGUILayout.EnumPopup(new GUIContent("Weapon Type: "), weaponType, GUILayout.Width(300));
				isUseable = Convert.ToBoolean(EditorGUILayout.Toggle(new GUIContent("Useable: "), true));
				str = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Strength: "), str.ToString()));
				dex = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Dexterity: "), dex.ToString()));
				inte = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Intelligence: "), inte.ToString()));
				will = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Will: "), will.ToString()));
				luck = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Luck: "), luck.ToString()));
				minDam = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Min.Damage: "), minDam.ToString()));
				maxDam = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Max.Damage: "), maxDam.ToString()));
				bal = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Balance: "), bal.ToString()));
				crit = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("Critical: "), crit.ToString()));
				pDef = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("P.Defense: "), pDef.ToString()));
				pPro = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("P.Protection: "), pPro.ToString()));
				mDef = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("M.Defense: "), mDef.ToString()));
				mPro = Convert.ToInt32(EditorGUILayout.TextField(new GUIContent("M.Protection: "), mPro.ToString()));
				break;

			default:
				Debug.LogError("Unrecognized ItemType");
				break;
		}
		itemTooltip = EditorGUILayout.TextField(new GUIContent("Tooltip: "), itemTooltip, GUILayout.Height(100));

		EditorGUILayout.Space();

		if (GUILayout.Button("Add", GUILayout.Width(100)))
		{
			itemDatabase.ClearNextID();
			switch (itemType)
			{
				case ItemType.BASIC:
					{
						Item newItem = (Item)CreateInstance(typeof(Item));
						newItem.ItemInit(nextID, itemName, itemSprite, isDefaultItem, isUseable, itemMaxStack, itemTooltip, goldValue);
						AssetDatabase.CreateAsset(newItem, DATABASE_PATH_ITEMS + String.Format("{0}.asset", nextID.ToString("000")));
						itemDatabase.Add(newItem);
					}
					break;
				case ItemType.CONSUMABLE:
					{
						Consumable newItem = (Consumable)CreateInstance(typeof(Consumable));
						newItem.ItemInit(nextID, itemName, itemSprite, isDefaultItem, isUseable, itemMaxStack, itemTooltip, goldValue, hp, mp, sp, dur);
						AssetDatabase.CreateAsset(newItem, DATABASE_PATH_ITEMS + String.Format("{0}.asset", nextID.ToString("000")));
						itemDatabase.Add(newItem);
					}
					break;
				case ItemType.EQUIPMENT:
					{
						Equipment newItem = (Equipment)CreateInstance(typeof(Equipment));
						newItem.ItemInit(nextID, itemName, itemSprite, isDefaultItem, equipType, weaponType, isUseable, itemMaxStack, itemTooltip, goldValue, str, dex, inte, will, luck, minDam, maxDam, bal, crit, pDef, pPro, mDef, mPro);
						AssetDatabase.CreateAsset(newItem, DATABASE_PATH_ITEMS + String.Format("{0}.asset", nextID.ToString("000")));
						itemDatabase.Add(newItem);
					}
					break;
				default:
					Debug.LogError("Unrecognized ItemType");
					break;
			}

			itemName = String.Format("NewItem({0})", itemDatabase.GetNextID().ToString("000"));
			itemMaxStack = 0;
			str = 0; dex = 0; inte = 0; will = 0; luck = 0;
			minDam = 0; maxDam = 0; bal = 0; crit = 0; pDef = 0; pPro = 0; mDef = 0; mPro = 0;
			hp = 0; mp = 0; sp = 0; dur = 0; goldValue = 0; resellValue = 0;
			isDefaultItem = false;
			isUseable = false;
			itemTooltip = "Tooltip MISSING!";
			itemSprite = null;
			state = State.BLANK;
			itemDatabase.SortAlphabeticallyAtoZ();
			EditorUtility.SetDirty(itemDatabase);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		EditorGUILayout.EndScrollView();
	}
}
