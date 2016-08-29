using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Cerebro
{
	public class TilePurchaseScript : MonoBehaviour
	{

		public GameObject TileOwnerText, TileCostText;

		private int CurrTileCost;

		void Start ()
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0f, 0f);
		}

		void Initialize(string OwnerName, int TileCost)
		{
			TileOwnerText.GetComponent<Text>().text = OwnerName;
			TileCostText.GetComponent<Text>().text = TileCost.ToString ();
			CurrTileCost = TileCost;
		}

		public void BackPressed ()
		{
			Destroy (gameObject);
		}
	}
}
