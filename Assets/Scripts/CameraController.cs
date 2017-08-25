using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	Transform target;
	public Vector3 offset;
	public float pitch = 2f;
	public float zoomSpeed = 4f;
	public float minZoom = 5f;
	public float maxZoom = 15f;
	public float yawSpeed = 100f;
	public float yawInput = 0f;

	
	private float currentZoom = 10f;

	private void Update()
	{
		currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
		yawInput -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
	}
	private void LateUpdate()
	{
		transform.position = target.position - offset * currentZoom;
		transform.LookAt(target.position + Vector3.up * pitch);

		transform.RotateAround(target.position, Vector3.up, yawInput);
	}
}
