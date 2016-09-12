using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace Cerebro
{
	public class GenericPopup : MonoBehaviour
	{

		public UnityEvent OnOkPressed;

		GameObject Card;
		Text questionObject;

		void Start ()
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0f, 0f);
		}

		public void Initialise (string Question)
		{
			Card = transform.Find ("Card").gameObject;
			questionObject = Card.transform.Find ("Question").GetComponent<Text> ();
			questionObject.text = Question;
		}

		public void OkPressed() {
			OnOkPressed.Invoke ();
			BackPressed ();
		}

		public void CancelPressed() {
			BackPressed ();
		}

		public void BackPressed ()
		{
			Destroy (gameObject);
		}
	}
}
