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
	bool isMoving = false, lastMoving = false;

	[SerializeField]
	List<RaycastResult> raycastResults;

	void Start()
	{
		cam = Camera.main;
		motor = GetComponent<PlayerMotor>();
		pStats = PlayerStats.instance;
	}

	void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject()) { return; }
		if (!pStats.bCanControl)
		{
			return;
		}
		MovingPenalty(.75f, .75f, .75f);
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
		if (Input.GetMouseButtonDown(0))
		{
			//PointerEventData pointer = new PointerEventData(EventSystem.current);
			//pointer.position = Input.mousePosition;

			//raycastResults = new List<RaycastResult>();
			//EventSystem.current.RaycastAll(pointer, raycastResults);

			//if (raycastResults.Count > 0)
			//{
			//	foreach (var curObj in raycastResults)
			//	{
			//		if (curObj.gameObject.layer == 5)
			//		{
			//			Debug.Log(curObj.gameObject.name, curObj.gameObject);
			//			return;
			//		}
			//	}
			//}
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
