using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler{
	[SerializeField]
	Image icon;
	[SerializeField]
	Button useButton = null, removeButton = null;
	[SerializeField]
	Item EMPTY;
	[SerializeField]
	Item item;
	[SerializeField]
	public Text amount;
	public int slotIndex;
	private Vector2 imageOriginalPosition;


	[SerializeField]
	private Transform originalParent;
	private Vector2 offset;
	private Inventory inv;
	private ItemDatabase itemDB;

	public Button UseButton { get { return useButton; } }

	private void Start()
	{
		inv = Inventory.instance;
		itemDB = inv.itemDB;
		imageOriginalPosition = icon.transform.position;
		EMPTY = itemDB.GetItemByID(0);
	}

	public void AddItem(Item newItem)
	{
		if (newItem == null) {Debug.LogWarning("**NULL ITEM**"); return; }
		item = newItem;
		icon.sprite = item.Icon;
		icon.enabled = true;
		removeButton.interactable = true;
	}
	public void ClearSlot()
	{
		item = EMPTY;
		icon.sprite = null;
		icon.enabled = false;
		useButton.interactable = false;
		removeButton.interactable = false;
		amount.enabled = false;
	}
	public void OnRemoveButton()
	{
		Debug.Log("Removed(" + item.name + ")");
		inv.Remove(slotIndex);
	}

	public void UseItem()
	{
		if (item != EMPTY)
		{
			item.Use();
			inv.SlotStack[slotIndex]--;
			if (inv.SlotStack[slotIndex] == 0)
				inv.Remove(slotIndex);
		}
		if (inv.onItemChangedCallback != null)
			inv.onItemChangedCallback();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (item != EMPTY)
		{
			//offset = eventData.position - (Vector2)icon.transform.position;
			icon.transform.SetParent(transform.parent);
			icon.transform.position = eventData.position /*- offset*/;
			icon.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
			removeButton.interactable = false;
			inv.OnDragItemID = inv.Litems[slotIndex].ID;
			inv.OnDragSlot = slotIndex;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (item != EMPTY)
		{
			icon.transform.position = eventData.position /*- offset*/;
		}
	}

	//slotIndex = Target, OnDragSlot = The slot that was dragged from, OnDragItemID = The item that was dragged
	public void OnDrop(PointerEventData eventData)
	{
		if (inv.OnDragSlot == -1)
			return;
		if (inv.Litems[slotIndex].ID == 0 && inv.OnDragSlot != slotIndex)
		{
			//Debug.Log("Moved Item");
			inv.SlotStack[slotIndex] = inv.SlotStack[inv.OnDragSlot];
			inv.Litems[slotIndex] = itemDB.GetItemByID(inv.OnDragItemID);
			inv.Remove(inv.OnDragSlot);
		}
		else
		{
			//if item are same, can stack, amount is small than maxStack
			if (inv.Litems[inv.OnDragSlot] == inv.Litems[slotIndex] &&
				inv.Litems[slotIndex].MaxStack > 1 &&
				inv.OnDragSlot != slotIndex)
			{
				if (inv.SlotStack[slotIndex] + inv.SlotStack[inv.OnDragSlot] <= inv.Litems[slotIndex].MaxStack)
				{
					inv.SlotStack[slotIndex] += inv.SlotStack[inv.OnDragSlot];
					//Debug.Log("New stack: " + inv.SlotStack[slotIndex]);
					inv.Remove(inv.OnDragSlot);
				}
				else
				{
					inv.SlotStack[inv.OnDragSlot] -= (inv.Litems[slotIndex].MaxStack - inv.SlotStack[slotIndex]);
					inv.SlotStack[slotIndex] = inv.Litems[slotIndex].MaxStack;
				}
				return;
			}
			//Debug.Log("Swapped Item");
			inv.Litems[inv.OnDragSlot] = itemDB.GetItemByID(inv.Litems[slotIndex].ID);
			inv.Litems[slotIndex] = itemDB.GetItemByID(inv.OnDragItemID);
			int tempInt = inv.SlotStack[inv.OnDragSlot];
			inv.SlotStack[inv.OnDragSlot] = inv.SlotStack[slotIndex];
			inv.SlotStack[slotIndex] = tempInt;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		//Debug.Log("EndDrag");
		icon.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
		icon.transform.SetParent(originalParent);
		icon.transform.position = imageOriginalPosition;
		inv.OnDragItemID = 0;
		inv.OnDragSlot = -1;
		icon.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
		if (inv.onItemChangedCallback != null)
			inv.onItemChangedCallback();
	}
}
