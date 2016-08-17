using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Cerebro.WordTower {
	public class MovableScript : MonoBehaviour
	, IDragHandler
	, IBeginDragHandler
	, IEndDragHandler {
		Vector2 preDragPosition;
		Vector2 mouseStartPosition;

		float totalHeight;
		float screenHeight = 768f;

		void Start() {
			totalHeight = GetComponent<RectTransform> ().sizeDelta.y;
		}
		public void OnDrag(PointerEventData eventData)
		{	
			var posX = preDragPosition.x;
			var dragPositionY = Input.mousePosition.y;
			var diff = mouseStartPosition.y - dragPositionY;
			var posY = preDragPosition.y - (screenHeight * diff / Screen.height);
			if (posY > -screenHeight / 2) {
				posY = -screenHeight / 2;
			} else if (posY < -(totalHeight - screenHeight / 2)) {
				posY = -(totalHeight - screenHeight / 2);
			}
			transform.localPosition = new Vector2 (posX, posY);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			totalHeight = GetComponent<RectTransform> ().sizeDelta.y;
			preDragPosition = transform.localPosition;
			mouseStartPosition = Input.mousePosition;
		}

		public void OnEndDrag(PointerEventData eventData)
		{

		}
	}
}