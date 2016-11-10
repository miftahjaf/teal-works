using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class GraphDiagramRotator : MonoBehaviour, IDragHandler {

	public bool canRotate = true;
	#region IDragHandler implementation
	private int direction = 1;
	public void OnDrag (PointerEventData eventData)
	{
		if (!canRotate) 
		{
			return;
		}
		transform.Rotate (Vector3.forward * Mathf.Sign(eventData.delta.x),eventData.delta.magnitude);
	}

	#endregion





}
