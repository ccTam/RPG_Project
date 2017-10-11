using UnityEngine;
using System.Collections;

public class HealingZone : MonoBehaviour
{
	PlayerStats pStats;

	[SerializeField]
	float recoverSp = 2f, recoverRate = .03f, t;
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
		if (t >= recoverSp)
		{
			if (pStats.bIsAlive)
				Combat.instance.Hot(pStats.HP.FinalValue * recoverRate, pStats.MP.FinalValue * recoverRate, pStats.SP.FinalValue * recoverRate, 0f, 0f);
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
