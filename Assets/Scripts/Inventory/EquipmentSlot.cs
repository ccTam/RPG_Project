using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
	[SerializeField]
	Image slotImage;
	[SerializeField]
	Button button;

	public Image getSlotImage { get { return slotImage; } }
	public Button getButton { get { return button; } set { button = value; } }

	public void Unequip()
	{
		Debug.Log("Unequiped from EQ Window");
		EquipmentSlot[] slots = transform.parent.GetComponentsInChildren<EquipmentSlot>();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i] == this)
			{
				EquipmentManager.instance.Unequip(i);
				break;
			}
		}
	}
}
