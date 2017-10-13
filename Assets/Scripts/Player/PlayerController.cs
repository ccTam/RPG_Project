using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
	Interactable focus;
	public LayerMask movementMask;

	Vector3 lastpos;
	Camera cam;
	PlayerMotor motor;
	PlayerStats pStats;
	EnemyManager enemyManager;
	public GameObject closestObject;
	bool isMoving = false, lastMoving = false;

	List<RaycastResult> raycastResults;

	void Start()
	{
		cam = Camera.main;
		motor = GetComponent<PlayerMotor>();
		pStats = PlayerStats.instance;
		enemyManager = EnemyManager.instance;
	}

	void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject()) { return; }
		if (!pStats.bCanControl)
		{
			return;
		}
		MovingPenalty(.35f, .35f, .35f);
		//if (Input.GetMouseButtonDown(1))
		//{
		//Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		//RaycastHit hit;
		//if (Physics.Raycast(ray, out hit, 100))
		//{
		//	Interactable interactable = hit.collider.GetComponent<Interactable>();
		//	if (interactable != null)
		//	{
		//		SetFocus(interactable);
		//	}
		//}
		//}
		if (Input.GetKey(KeyCode.LeftControl))
		{
			Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100))
			{
				float closestdistance = Mathf.Infinity;
				closestObject = null;
				foreach (GameObject tObj in enemyManager.EnemyGroup)
				{
					float distance = Vector3.Distance(hit.point, tObj.transform.position);
					if (distance < closestdistance) 
					{
						closestdistance = distance;
						closestObject = tObj;
					}
				}
				//Debug.Log("Closest Obj: " + closestObject.name + " " + closestdistance);
				DrawLine(hit.point, closestObject.transform.position, Color.cyan);
			}
			if (Input.GetMouseButtonDown(0))
			{
				Interactable interactable = closestObject.GetComponent<Interactable>();
				if (interactable != null)
				{
					SetFocus(interactable);
					return;
				}
			}
		}
			
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100))
			{
				Interactable interactable = hit.collider.GetComponent<Interactable>();
				if (interactable != null)
				{
					SetFocus(interactable);
					return;
				}
			}
			if (Physics.Raycast(ray, out hit, 100, movementMask))
			{
				motor.MoveToPoint(hit.point);
				RemoveFocus();
			}
		}
	}

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f)
	{
		//GameObject myLine = new GameObject();
		//myLine.transform.position = start;
		//myLine.AddComponent<LineRenderer>();
		//LineRenderer lr = myLine.GetComponent<LineRenderer>();
		//lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		//lr.SetColors(color, color);
		//lr.SetWidth(0.1f, 0.1f);
		//lr.SetPosition(0, start);
		//lr.SetPosition(1, end);
		//GameObject.Destroy(myLine, duration);
		Debug.DrawLine(start, end, color, duration, false);
	}

	private void MovingPenalty(float HPv, float MPv, float SPv)
	{
		//Slow Regen while moving
		float mag = (transform.position - lastpos).magnitude;
		isMoving = mag > 0 ? true : false;
		if (lastMoving != isMoving)
		{
			if (isMoving)
			{
				pStats.HPR_Multiplier.DebuffValue += HPv;
				pStats.MPR_Multiplier.DebuffValue += MPv;
				pStats.SPR_Multiplier.DebuffValue += SPv;
			}
			else
			{
				pStats.HPR_Multiplier.DebuffValue -= HPv;
				pStats.MPR_Multiplier.DebuffValue -= MPv;
				pStats.SPR_Multiplier.DebuffValue -= SPv;
			}
			//Debug.Log(string.Format("H:{0} M:{1} S:{2}", pStats.HPR_Multiplier.FinalValue, pStats.MPR_Multiplier.FinalValue, pStats.SPR_Multiplier.FinalValue));
		}
		lastpos = transform.position;
		lastMoving = isMoving;
	}

	void SetFocus(Interactable newFocus)
	{
		if (newFocus != focus)
		{
			if (focus != null)
			{
				focus.OnDeFocus();
			}
			focus = newFocus;
			newFocus.OnFocused(transform);
		}
		motor.FollowTarget(newFocus);
	}

	void RemoveFocus()
	{
		if (focus != null)
		{
			focus.OnDeFocus();
		}
		focus = null;
		motor.StopFollowingTarget();
	}
}
