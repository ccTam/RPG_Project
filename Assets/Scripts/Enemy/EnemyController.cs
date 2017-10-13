using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

	public float searchRadius = 5f;
	Transform target;
	NavMeshAgent agent;
	PlayerCombat playerCombat;
	EnemyStats stats;
	PlayerStats pStats;

	void Start()
	{
		target = PlayerManager.instance.player.transform;
		agent = GetComponent<NavMeshAgent>();
		playerCombat = PlayerCombat.instance;
		pStats = PlayerStats.instance;
	}

	void Update()
	{
		SearchAndAttack();
	}

	private void SearchAndAttack()
	{
		float distance = Vector3.Distance(transform.position, target.position);
		stats = GetComponent<EnemyStats>();
		stats.AttackSpeed.CurValue -= Time.deltaTime;
		if (distance <= searchRadius)
		{
			agent.SetDestination(target.position);
			if (distance <= agent.stoppingDistance && pStats.canAttack())
			{
				if (stats != null)
				{
					if (stats.AttackSpeed.CurValue <= 0)
					{
						playerCombat.Dot(stats.GetAttackDamage(), 0, 0);
						stats.AttackSpeed.CurValue = stats.AttackSpeed.FinalValue;
					}
				}
				FaceTarget(target);
			}
		}
	}

	private void FaceTarget(Transform target)
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, searchRadius);
	}
}
