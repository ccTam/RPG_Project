using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion")]
public class Potion : Item
{
	[SerializeField]
	private float HP = 0, MP = 0, SP = 0, Dur = 0;
	public override void Use()
	{
		base.Use();
		float interval = .1f;
		Combat.instance.Hot(HP * interval / Dur, MP * interval / Dur, SP * interval / Dur, interval, Dur);
	}
}
