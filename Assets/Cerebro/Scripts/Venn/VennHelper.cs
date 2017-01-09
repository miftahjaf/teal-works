using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Cerebro
{
	public enum VennType
	{
		None = -1,
		Overlapping =0,
		Disjoint = 1,
		Inside = 2

	}
	public class VennHelper : MonoBehaviour 
	{
		public GameObject[] vennDiagramPrefabs;
		private VennType vennType;
		private List<string> diagramValues; //rect, left, middle, right
		private GameObject diagramObj;
		private bool isInteractable;
		private bool[] filledValues = new bool[]{false,false,false,false};

		public void Reset()
		{
			//Destroy child
			foreach (Transform child in transform) 
			{
				GameObject.Destroy(child.gameObject);
			}

			diagramValues = new List<string> ();
			vennType = VennType.None;
			isInteractable = false;
			diagramObj = null;
			filledValues = new bool[]{false,false,false,false};
		}

		public void Draw()
		{
			GenerateDiagram ();
		}

		public void SetVennType(VennType _vennType)
		{
			vennType = _vennType;
		}

		public void SetDiagramValues(List<string> _diagramValues)
		{
			diagramValues = _diagramValues;
		}

		public void SetIsInteractable(bool _isInteractable)
		{
			isInteractable = _isInteractable;
		}

		public void SetFillValue(int index)
		{
			filledValues [index] = true;
		}

		public void GenerateDiagram()
		{
			int number = (int)vennType;
			if (number < 0) {
				return;
			}
			diagramObj = GameObject.Instantiate (vennDiagramPrefabs [number]);
			diagramObj.transform.SetParent (this.transform, false);
			int count = diagramObj.transform.childCount;
			foreach (UIPolygon uiPolygon in diagramObj.GetComponentsInChildren<UIPolygon>()) {
				uiPolygon.ReDraw ();
			}
			for(int i=0; i<count; i++)
			{
				int value = i;
				GameObject childObj = diagramObj.transform.FindChild (value.ToString ()).gameObject;

				if (diagramValues.Count > i) 
				{
					childObj.transform.FindChild ("Text").GetComponent<Text> ().text = diagramValues [i];
				}

				if (filledValues [i]) 
				{
					if (childObj.GetComponent<UIPolygon> ()) 
					{
						childObj.transform.FindChild ("Fill").GetComponent<UIPolygon> ().color = Color.blue;
					} else {
						childObj.GetComponent<Graphic> ().color =  Color.blue;
					}
				}

				if (isInteractable)
				{
					childObj.GetComponent<ColliderButton> ().OnClicked = delegate {
						bool isSelected = false;
						if (childObj.GetComponent<UIPolygon> ()) {
							isSelected = childObj.transform.FindChild("Fill").GetComponent<UIPolygon> ().color == Color.blue;
							childObj.transform.FindChild("Fill").GetComponent<UIPolygon> ().color = isSelected ? Color.white : Color.blue;
							childObj.transform.FindChild("Fill").GetComponent<UIPolygon> ().ReDraw();
						} else {
							isSelected = childObj.GetComponent<Graphic> ().color == Color.blue;
							childObj.GetComponent<Graphic> ().color = isSelected ? Color.white : Color.blue;
						}
					};
				}

			}
		}
	}
}
