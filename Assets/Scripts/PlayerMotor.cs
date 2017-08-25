using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
	Transform target;
	NavMeshAgent agent;
	// Use this for initialization
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		StartCoroutine(FollowingTarget());
	}

	public void MoveToPoint(Vector3 point)
	{
		agent.SetDestination(point);
	}
	private IEnumerator FollowingTarget()
	{
		while (true)
		{
			if (target != null)
			{
				agent.SetDestination(target.position);
				FaceTarget();
			}
			yield return new WaitForSeconds(.03f);
		}
	}
	public void FollowTarget (Interactable newTarget)
	{
		agent.stoppingDistance = newTarget.radius * .8f;
		//agent.updateRotation = false;
		target = newTarget.interactionTransform;
	}
	public void StopFollowingTarget()
	{
		agent.stoppingDistance = 0f;
		//agent.updateRotation = true;
		target = null;
	}
	void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}
}
