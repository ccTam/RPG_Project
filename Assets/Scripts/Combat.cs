using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Combat : MonoBehaviour
{
	[SerializeField]
	private Transform bloodImages;
	[SerializeField]
	AudioSource audioSource;
	PlayerStats pStats;

	#region Singleton

	public static Combat instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Combat found!");
			return;
		}
		instance = this;
	}
	#endregion

	private void Start()
	{
		pStats = PlayerStats.instance;
		audioSource = GetComponent<AudioSource>();
	}
	private void Update()
	{
		foreach (Transform T in bloodImages)
		{
			Image im = T.GetComponent<Image>();
			if (im.color != Color.clear) { im.color = Color.Lerp(im.color, Color.clear, 3f * Time.deltaTime); }
		}
	}
	#region //Damage
	private void Damage(float amount)
	{
		pStats.curlastCombatTime = Time.time;
		if (!pStats.bIsImmune && pStats.bIsAlive && amount > 0)
		{
			if (pStats.bIsStatic) { return; }
			audioSource.Play();
			DrawBlood(amount);
			if (pStats.bIsDeadly) { Death(); return; }
			if (pStats.curHP >= pStats.maxHP * .5f && amount >= pStats.maxHP * .5f) { pStats.curHP -= amount; pStats.bIsDeadly = true; Debug.Log("Huge Damage Deadly"); return; }
			pStats.curHP -= amount;
			if (pStats.curHP <= 0)
			{
				if (Random.Range(0, 100) <= (30 + (pStats.curWill + pStats.curLuck - 20) * .07)) { pStats.bIsDeadly = true; Debug.Log("Lucky Deadly: " + (30 + (pStats.curWill + pStats.curLuck - 20) * .07)); return; }
				Death();
			}
		}
	}

	private void Damage(float damage, float mpReduce, float spReduce)
	{
		if (!pStats.bIsImmune && pStats.bIsAlive)
		{
			if (pStats.curMP < mpReduce)
			{
				Damage((mpReduce - pStats.curMP) / 2);
				pStats.curMP = 0;
			}
			else
				pStats.curMP -= mpReduce;
			pStats.curSP -= spReduce;
		}
		Damage(damage);
	}

	private void DrawBlood(float damage)
	{
		if (pStats.bIsAlive)
		{
			foreach (Transform T in bloodImages)
			{
				Image im = T.GetComponent<Image>();
				RectTransform recTran = T.GetComponent<RectTransform>();
				recTran.position = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
				im.color = new Vector4(1, 1, 1, Mathf.Clamp01(.2f + (damage / (pStats.maxHP * .25f))));
			}
		}
	}
	//Dot: Damage Over Time
	public void Dot(float damage, float damInterval, float dur)
	{
		if (dur == 0 || !pStats.bIsAlive) { Damage(damage, 0, 0); return; }
		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(DotCounter(K, damage, 0, 0, damInterval, dur));
	}
	public void Dot(float damage, float mpReduce, float spReduce, float damInterval, float dur)
	{
		if (dur == 0 || !pStats.bIsAlive) { Damage(damage, mpReduce, spReduce); return; }
		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(DotCounter(K, damage, mpReduce, spReduce, damInterval, dur));
	}

	IEnumerator DotCounter(int K, float damage, float mpReduce, float spReduce, float damInterval, float dur)
	{
		float count = dur / damInterval;
		while (true)
		{
			if (mpReduce == 0 && spReduce == 0)
				Damage(damage);
			else
				Damage(damage, mpReduce, spReduce);
			if (--count <= 0)
			{
				StopCoroutine(CoroutineController.instance.Inst[K]);
				CoroutineController.instance.RevokeK(K);
				Debug.Log((damage * dur / damInterval) + " damage taken over " + dur + "s");
			}
			if (pStats.bIsAlive)
				yield return new WaitForSeconds(damInterval);
			else
				yield return null;
		}
	}
	#endregion //Damage

	#region //Heal
	private void Heal(float hp, float mp, float sp)
	{
		if (pStats.bIsAlive)
		{
			pStats.curHP = Mathf.Clamp(pStats.curHP += hp, -pStats.maxHP, pStats.maxHP);
			pStats.curMP = Mathf.Clamp(pStats.curMP += hp, 0, pStats.maxMP);
			pStats.curSP = Mathf.Clamp(pStats.curSP += hp, 0, pStats.maxSP);
		}
	}

	//Hot: Heal Over Time
	public void Hot(float hp, float mp, float sp, float interva, float dur)
	{
		if (dur == 0) { Heal(hp, mp, sp); return; }
		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(HotCounter(K, hp, mp, sp, interva, dur));
	}

	IEnumerator HotCounter(int K, float hp, float mp, float sp, float healInterval, float dur)
	{
		float count = dur / healInterval;
		while (true)
		{
			if (mp == 0 && sp == 0)
				Heal(hp, 0, 0);
			else
				Heal(hp, mp, sp);
			if (--count <= 0)
			{
				StopCoroutine(CoroutineController.instance.Inst[K]);
				CoroutineController.instance.RevokeK(K);
				Debug.Log((hp * dur / healInterval) + " HP healed over " + dur + "s");
			}
			if (pStats.bIsAlive)
				yield return new WaitForSeconds(healInterval);
			else
				yield return null;
		}
	}
	#endregion //Heal

	#region Death
	private void Death()
	{
		if (pStats.bIsStatic) { return; }
		pStats.curHP = 0;
		pStats.curMP = 0;
		pStats.curSP = 0;
		pStats.bIsAlive = false;
		pStats.bCanControl = false;
		pStats.bIsImmune = true;
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		player.GetComponent<PlayerMotor>().MoveToPoint(player.transform.position);
		player.GetComponentInChildren<Animator>().SetBool("Dead", true);

		int K = CoroutineController.instance.GetK();
		if (K == -1) { return; }
		CoroutineController.instance.Inst[K] = StartCoroutine(Reviving(K, 5f, pStats.maxHP * .1f, pStats.maxMP * .1f, pStats.maxSP * .1f));
	}
	#endregion //Death
	#region //Revive
	public void Revive(float hp, float mp, float sp)
	{
		pStats.curHP = hp;
		pStats.curMP = mp;
		pStats.curSP = sp;
		Revive();
	}
	private void Revive()
	{
		pStats.bIsAlive = true;
		pStats.bCanControl = true;
		pStats.bIsImmune = false;
		pStats.bIsDeadly = false;
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		player.GetComponent<PlayerMotor>().MoveToPoint(player.transform.position);
		player.GetComponentInChildren<Animator>().SetBool("Dead", false);
	}

	IEnumerator Reviving(int K, float t, float hp, float mp, float sp)
	{
		yield return new WaitForSeconds(t);
		if (!pStats.bIsAlive)
			Revive(hp, mp, sp);
		StopCoroutine(CoroutineController.instance.Inst[K]);
		CoroutineController.instance.RevokeK(K);
	}

	#endregion Revive
}
