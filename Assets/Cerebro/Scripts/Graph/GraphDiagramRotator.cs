using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class GraphDiagramRotator : MonoBehaviour, IDragHandler {
	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		transform.Rotate (Vector3.forward * Mathf.Sign(eventData.delta.x),eventData.delta.magnitude);
	}

	#endregion



}
