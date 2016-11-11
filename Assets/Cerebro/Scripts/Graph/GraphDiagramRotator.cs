using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
public class GraphDiagramRotator : MonoBehaviour, IDragHandler
{
	public bool canRotate = true;

	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		if (!canRotate) 
		{
			return;
		}
		float AngleRad = Mathf.Atan2(Input.mousePosition.y - transform.position.y, Input.mousePosition.x - transform.position.x);
		float AngleDeg = (180 / Mathf.PI) * AngleRad;
		this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);

	}
	#endregion







}
