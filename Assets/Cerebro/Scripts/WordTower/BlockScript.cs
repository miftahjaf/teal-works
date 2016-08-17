using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro.WordTower {
	public class BlockScript : MonoBehaviour
	, IDragHandler
	, IBeginDragHandler
	, IEndDragHandler
	{
		Image sprite;
		Text characterText;
		int charIndex = 0;
		Vector2 blockSize;
		Vector2 preDragPosition;
		bool allowDrag = false;
		int gridIndex;
		public bool isMovable;
		BoxCollider2D boxCollider;
		public bool toDestroy = false;
		float screenHeight_2 = 384;

		public void Initialize (int index, bool _isMovable = true, int _gridIndex = -1)
		{
			charIndex = index;
			gridIndex = _gridIndex;
			if (characterText == null) {
				characterText = transform.Find ("Text").gameObject.GetComponent<Text> ();
			}
			characterText.text = Constants.characters [charIndex];
			isMovable = _isMovable;

			if (!isMovable) {
				gameObject.GetComponent<Image> ().color = new Color (0.85f, 0.85f, 0.85f);
			}
		}

		void Awake ()
		{
			this.gridIndex = -1;
			sprite = GetComponent<Image> ();
			boxCollider = GetComponent<BoxCollider2D> ();
			boxCollider.enabled = false;
			blockSize = GetComponent<RectTransform> ().sizeDelta;
		}

		public void OnDrag (PointerEventData eventData)
		{
			if (!allowDrag) {
				return;
			}
			float diffY = 0f;
			if (transform.parent != World.instance.trayBG.transform) {
				diffY = World.instance.movableContainer.GetComponent<RectTransform> ().localPosition.y + screenHeight_2;
			}
			var posX = (1024 * Input.mousePosition.x / Screen.width);
			var posY = (768 * Input.mousePosition.y / Screen.height) - diffY;
			posX -= blockSize.x / 2;
			posY -= blockSize.y / 2;
			transform.localPosition = new Vector2 (posX, posY);
		}

		public void OnBeginDrag (PointerEventData eventData)
		{
			if (transform.localPosition.y >= 0 && isMovable) {
				transform.SetAsLastSibling ();
				allowDrag = true;
				preDragPosition = transform.localPosition;
				sprite.color = Color.green;
			} else {
				allowDrag = false;
			}
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			if (!allowDrag) {
				return;
			}
			sprite.color = Color.white;

			float diffY = 0;
			if (transform.parent != World.instance.trayBG.transform) {
				diffY = World.instance.movableContainer.GetComponent<RectTransform> ().localPosition.y + screenHeight_2;
			}
			Vector2 actualPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + diffY);

			if (actualPosition.y < blockSize.y) {
				transform.localPosition = preDragPosition;
				World.instance.CheckWorld (false);
				return;
			}

			int gIndex = World.instance.GetClosestGrid (actualPosition);
			if (gIndex == -1) {
				transform.localPosition = preDragPosition;
				World.instance.CheckWorld (false);
				return;
			}

			Cell newcell = World.instance.GetCellAtIndex (gIndex);
	//		Cell oldCell = World.instance.GetCellAtIndex (this.gridIndex);

			if (newcell != null) {
				diffY = 0;
				if (transform.parent == World.instance.trayBG.transform) {
					diffY = World.instance.movableContainer.GetComponent<RectTransform> ().localPosition.y + screenHeight_2;
				}

				transform.localPosition = new Vector2 (newcell.gridPosition.x, newcell.gridPosition.y + diffY);
				World.instance.ResetCellAtIndex (this.gridIndex);

				// If no validation
				this.gridIndex = gIndex;
				newcell.characterIndex = charIndex;
				newcell.block = this;
				this.gameObject.transform.SetParent (World.instance.blockContainer.transform);
				World.instance.RemoveFromTray (this);

				World.instance.CheckTowerHeight ();

				World.instance.AddMoveToFile (gIndex, charIndex, true);


				// If allow only blocks next to other blocks
	//			List<Cell> neighbours = newcell.GetAllNeighbours ();
	//			if (neighbours.Count == 0 && !World.instance.IsGridEmpty ()) {
	//				transform.localPosition = preDragPosition;
	//				if (oldCell != null) {
	//					oldCell.characterIndex = charIndex;
	//				}
	//			} else {
	//				if (World.instance.IsGridEmpty ()) {
	//					gIndex = gIndex % (int)World.instance.GetGridSize ().x;
	//					newcell = World.instance.GetCellAtIndex (gIndex);
	//					transform.localPosition = newcell.gridPosition;
	//
	//					newcell.isRoot = true;
	//				}
	//				if (oldCell != null && oldCell.isRoot) {
	//					newcell.isRoot = true;
	//					oldCell.isRoot = false;
	//				}
	//
	//				this.gridIndex = gIndex;
	//				newcell.characterIndex = charIndex;
	//				newcell.block = this;
	//				World.instance.RemoveFromTray (this);
	//			}
			} else {
				transform.localPosition = preDragPosition;
			}
			World.instance.CheckWorld (false);
		}

		//	IEnumerator CheckTowerHeight() {
		//		yield return new WaitForSeconds (1.0f);
		//		World.instance.CheckTowerHeight ();
		//	}

		void Update ()
		{
			if (transform.position.y < -blockSize.y && toDestroy) {
				Destroy (gameObject);
			}
		}
	}
}