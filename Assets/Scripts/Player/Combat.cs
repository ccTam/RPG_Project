using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Combat : MonoBehaviour
{
	[SerializeField]
	private Transform bloodImages;
	[SerializeField]
	//AudioSource audioSource;
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
		//audioSource = GetComponent<AudioSource>();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Dot(pStats.HP.FinalValue * .08f, 2.5f, 8f);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			Dot(20f, 50f, 60f, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			Hot(pStats.HP.FinalValue * .02f, pStats.MP.FinalValue * .02f, pStats.SP.FinalValue * .02f, 1f, 5f);
		}

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
			amount = Mathf.Clamp((amount - pStats.PDef.CurValue) * (1 - pStats.PPro.CurValue * .01f), 1f, Mathf.Infinity);
			Debug.Log("Actually Damage: " + amount);
			if (pStats.bIsStatic) { return; }
			//Play hurt sound
			//audioSource.Play();
			DrawBlood(amount);
			if (pStats.bIsDeadly) { Death(); return; }
			if (pStats.HP.CurValue >= pStats.HP.FinalValue * .5f && amount >= pStats.HP.FinalValue * .5f) { pStats.HP.CurValue -= amount; pStats.bIsDeadly = true; Debug.Log("Huge Damage Deadly"); return; }
			pStats.HP.CurValue -= amount;
			if (pStats.HP.CurValue <= 0)
			{
				if (Random.Range(0, 100) <= (30 + (pStats.Will.FinalValue + pStats.Luck.FinalValue - 20) * .07)) { pStats.bIsDeadly = true; Debug.Log("Lucky Deadly: " + (30 + (pStats.Will.FinalValue + pStats.Luck.FinalValue- 20) * .07)); return; }
				Death();
			}
		}
	}

	private void Damage(float damage, float mpReduce, float spReduce)
	{
		if (!pStats.bIsImmune && pStats.bIsAlive)
		{
			if (pStats.MP.CurValue < mpReduce)
			{
				Damage((mpReduce - pStats.MP.CurValue) / 2);
				pStats.MP.CurValue = 0;
			}
			else
				pStats.MP.CurValue -= mpReduce;
			pStats.SP.CurValue -= spReduce;
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
				im.color = new Vector4(1, 1, 1, Mathf.Clamp01(.2f + (damage / (pStats.HP.FinalValue * .25f))));
			}
		}
	}
	//Dot: Damage Over Time
	public void Dot(float damage, float interval, float dur)
	{
		if (dur == 0 || !pStats.bIsAlive) { Damage(damage, 0, 0); return; }
		int K = CoroutineManager.instance.GetK();
		if (K == -1) { return; }
		CoroutineManager.instance.Inst[K] = StartCoroutine(DotCounter(K, damage, 0, 0, interval, dur));
	}
	public void Dot(float damage, float mpReduce, float spReduce, float damInterval, float dur)
	{
		if (dur == 0 || !pStats.bIsAlive) { Damage(damage, mpReduce, spReduce); return; }
		int K = CoroutineManager.instance.GetK();
		if (K == -1) { return; }
		CoroutineManager.instance.Inst[K] = StartCoroutine(DotCounter(K, damage, mpReduce, spReduce, damInterval, dur));
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
				StopCoroutine(CoroutineManager.instance.Inst[K]);
				CoroutineManager.instance.RevokeK(K);
				Debug.Log(string.Format("{0}/{1}/{2} damage taken over {3}s", (damage * dur / damInterval), (mpReduce * dur / damInterval), (spReduce * dur / damInterval), dur));
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
			pStats.HP.CurValue = Mathf.Clamp(pStats.HP.CurValue += hp, -pStats.HP.FinalValue, pStats.HP.FinalValue);
			pStats.MP.CurValue = Mathf.Clamp(pStats.MP.CurValue += mp, 0, pStats.MP.FinalValue);
			pStats.SP.CurValue = Mathf.Clamp(pStats.SP.CurValue += sp, 0, pStats.SP.FinalValue);
		}
	}

	//Hot: Heal Over Time
	public void Hot(float hp, float mp, float sp, float interval, float dur)
	{
		if (dur == 0) { Heal(hp, mp, sp); return; }
		int K = CoroutineManager.instance.GetK();
		if (K == -1) { return; }
		CoroutineManager.instance.Inst[K] = StartCoroutine(HotCounter(K, hp, mp, sp, interval, dur));
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
				StopCoroutine(CoroutineManager.instance.Inst[K]);
				CoroutineManager.instance.RevokeK(K);
				Debug.Log(string.Format("{0}/{1}/{2} recovered over {3}s", (hp * dur / healInterval), (mp * dur / healInterval), (sp * dur / healInterval), dur));
			}
			if (pStats.bIsAlive)
				yield return new WaitForSeconds(healInterval);
			else
				yield return null;
		}
	}
	#endregion //Heal
	#region //Death
	private void Death()
	{
		if (pStats.bIsStatic) { return; }
		pStats.HP.CurValue = 0;
		pStats.MP.CurValue = 0;
		pStats.SP.CurValue = 0;
		pStats.bIsAlive = false;
		pStats.bCanControl = false;
		pStats.bIsImmune = true;
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		player.GetComponent<PlayerMotor>().MoveToPoint(player.transform.position);
		player.GetComponentInChildren<Animator>().SetBool("Dead", true);

		int K = CoroutineManager.instance.GetK();
		if (K == -1) { return; }
		CoroutineManager.instance.Inst[K] = StartCoroutine(Reviving(K, 5f, pStats.HP.FinalValue * .1f, pStats.MP.FinalValue * .1f, pStats.SP.FinalValue * .1f));
	}
	#endregion //Death
	#region //Revive
	public void Revive(float hp, float mp, float sp)
	{
		pStats.HP.CurValue = hp;
		pStats.MP.CurValue = mp;
		pStats.SP.CurValue = sp;
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
		StopCoroutine(CoroutineManager.instance.Inst[K]);
		CoroutineManager.instance.RevokeK(K);
	}

	#endregion Revive
}
