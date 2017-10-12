using UnityEngine;

public class CreepBurn : MonoBehaviour
{
	float damInterval = .75f, t;


	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Collider>().tag == "Player")
		{
			Debug.Log("Colliding: " + other.name);
			t = Time.fixedDeltaTime;
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (t >= damInterval)
		{
			float percentBurn = .08f;
			PlayerCombat.instance.Dot(PlayerStats.instance.HP.FinalValue * percentBurn, PlayerStats.instance.MP.FinalValue * percentBurn, PlayerStats.instance.SP.FinalValue * percentBurn, 0f, 0f);
			t = 0;
		}
		t += Time.fixedDeltaTime;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Collider>().tag == "Player")
		{
			Debug.Log("Exit Coll: " + other.name);
		}
	}
}
