using UnityEngine;
using System.Collections;

public class HealingZone : MonoBehaviour
{

	PlayerStats pStats;
	[SerializeField]
	float interval = 2f, recoverRate = .02f, t;
	void Start()
	{
		pStats = PlayerStats.instance;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Collider>().tag == "Player")
		{
			Debug.Log("Recovering: " + other.name);
			t = Time.fixedDeltaTime;
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (t >= interval)
		{
			if (pStats.bIsAlive)
				pStats.Hot(pStats.maxHP * recoverRate, pStats.maxMP * recoverRate, pStats.maxSP * recoverRate, 0f, 0f);
			t = 0;
		}
		t += Time.fixedDeltaTime;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Collider>().tag == "Player")
		{
			Debug.Log("Recovering Ended: " + other.name);
		}
	}
}
