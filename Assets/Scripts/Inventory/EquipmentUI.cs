using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{

	public Transform itemParent;
	[SerializeField]
	EquipmentSlot[] equipSlot;
	EquipmentManager eqManager;

	void Start()
	{
		int numSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
		equipSlot = itemParent.GetComponentsInChildren<EquipmentSlot>();
		if (numSlots != equipSlot.Length)
			Debug.Log(string.Format("EqSlots doesn't match, Obj = {0} & System = {1}", equipSlot.Length, numSlots));
		eqManager = EquipmentManager.instance;
		eqManager.onEquipmentChanged += UpdateEquipmentUI;
	}

	void UpdateEquipmentUI(Equipment newItem, Equipment oldItem)
	{
		if (newItem != null)
		{
			equipSlot[(int)newItem.equipSlot].getSlotImage.sprite = newItem.Icon;
			equipSlot[(int)newItem.equipSlot].getSlotImage.enabled = true;
			equipSlot[(int)newItem.equipSlot].getButton.interactable = true;
			return;
		}
		if (oldItem != null)
		{
			equipSlot[(int)oldItem.equipSlot].getSlotImage.sprite = null;
			equipSlot[(int)oldItem.equipSlot].getSlotImage.enabled = false;
			equipSlot[(int)oldItem.equipSlot].getButton.interactable = false;
		}
	}
}
