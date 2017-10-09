using UnityEngine;
using System.Collections;

public class UIDrag : MonoBehaviour
{
	private Vector2 offset;
	private float screenX, screenY;

	public void BeginDrag()
	{
		offset = (transform.position - Input.mousePosition);
		screenX = Screen.width;
		screenY = Screen.height;
	}

	public void OnDrag()
	{
		if (Input.mousePosition.x < 0 || Input.mousePosition.x > screenX || Input.mousePosition.y < 0|| Input.mousePosition.y> screenY)
		{
			return;
		}
		transform.position = new Vector3(offset.x + Input.mousePosition.x, offset.y + Input.mousePosition.y, 0);
	}
}
