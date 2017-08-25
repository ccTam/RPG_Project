using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName ="Inventory/Item")]
public class Item : ScriptableObject {

	new public string name = "New Item";
	public Sprite icon = null;
	public bool isDefaultItem = false;
	public bool isWeapon = false;
	public int weaponID = -1;

	public float minDam, maxDam;

	public virtual void Use()
	{
		Debug.Log("Using: " + name);
	}
}
