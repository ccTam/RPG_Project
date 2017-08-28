using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	[SerializeField]
	Image icon;
	[SerializeField]
	Button removeButton;
	[SerializeField]
	Item EMPTY;
	[SerializeField]
	Item item;
	public int slotIndex;
	[SerializeField]
	private Transform originalParent;
	private Vector2 offset;
	private Vector2 imageOriginalPos;
	private Inventory inventory;

	private void Start()
	{
		inventory = Inventory.instance;
	}

	public void AddItem(Item newItem)
	{
		item = newItem;
		icon.sprite = item.icon;
		icon.enabled = true;
		imageOriginalPos = icon.transform.position;
		removeButton.interactable = true;
	}
	public void ClearSlot()
	{
		item = EMPTY;
		icon.sprite = null;
		icon.enabled = false;
		removeButton.interactable = false;
	}
	public void OnRemoveButton()
	{
		Debug.Log("Remove(" + item.name + ")");
		inventory.Remove(slotIndex);
	}

	public void UseItem()
	{
		if (item != null)
		{
			item.Use();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (item != EMPTY)
		{
			offset = eventData.position - (Vector2)icon.transform.position;
			icon.transform.SetParent(transform.parent);
			icon.transform.position = eventData.position - offset;
			icon.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
			removeButton.interactable = false;
			inventory.OnDragItemID = inventory.Litems[slotIndex].ID;
			inventory.OnDragSlot = slotIndex;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (item != EMPTY)
		{
			icon.transform.position = eventData.position /*- offset*/;
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		Image droppedItem = eventData.pointerDrag.GetComponent<Image>();
		if (inventory.Litems[slotIndex].ID == 0)
			inventory.Litems[inventory.OnDragSlot] = EMPTY;
		else
			inventory.Litems[inventory.OnDragSlot] = inventory.GetItemByID(inventory.Litems[slotIndex].ID);
		inventory.Litems[slotIndex] = inventory.GetItemByID(inventory.OnDragItemID);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		icon.transform.SetParent(originalParent);
		icon.transform.position = imageOriginalPos;
		icon.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
		inventory.OnDragItemID = 0;
		inventory.OnDragSlot = -1;
		if (inventory.onItemChangedCallback != null)
		{
			inventory.onItemChangedCallback();
		}
	}
}
