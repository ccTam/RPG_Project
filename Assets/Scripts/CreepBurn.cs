using UnityEngine;

public class CreepBurn : MonoBehaviour
{
	float damInterval = .5f, t;

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
			float percentBurn = .04f;
			PlayerStats.instance.Dot(PlayerStats.instance.maxHP * percentBurn, PlayerStats.instance.maxMP * percentBurn, PlayerStats.instance.maxSP * percentBurn, 0f, 0f);
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
