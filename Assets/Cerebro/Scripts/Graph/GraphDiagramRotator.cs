using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
public class GraphDiagramRotator : MonoBehaviour, IDragHandler
{
	public bool canRotate = true;
	private float lastAngle;
	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		if (!canRotate) 
		{
			return;
		}
		float AngleRad = Mathf.Atan2(Input.mousePosition.y - transform.position.y, Input.mousePosition.x - transform.position.x);
		float AngleDeg = (180 / Mathf.PI) * AngleRad;
		float diff = lastAngle - AngleDeg;
		transform.Rotate(Vector3.forward, -Mathf.Sign(diff) *eventData.delta.magnitude);
		lastAngle = AngleDeg;

	}
	#endregion
}
