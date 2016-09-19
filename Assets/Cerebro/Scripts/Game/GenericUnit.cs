using System.Collections;
using UnityEngine;

namespace Cerebro
{
	public class GenericUnit : Unit
	{

		private Coroutine PulseCoroutine;

		private Vector3 initialScale;

		public override void Initialize ()
		{
			initialScale = transform.localScale;
			base.Initialize ();
			transform.position += new Vector3 (0, 0, -0.1f);
		}

		public override void OnUnitDeselected ()
		{
			base.OnUnitDeselected ();
//	        StopCoroutine(PulseCoroutine);
			transform.localScale = initialScale;
		}

		public override void MarkAsAttacking (Unit other)
		{
			StartCoroutine (Jerk (other));
		}

		public override void MarkAsDefending (Unit other)
		{
			StartCoroutine (Glow (new Color (1, 0, 0, 0.5f), 1));
		}

		public override void MarkAsDestroyed ()
		{
		}

		private IEnumerator Jerk (Unit other)
		{
//	        GetComponent<SpriteRenderer>().sortingOrder = 6;
			var heading = other.transform.position - transform.position;
			var direction = heading / heading.magnitude;
			float startTime = Time.time;

			while (startTime + 0.25f > Time.time) {
				transform.position = Vector3.Lerp (transform.position, transform.position + (direction / 50f), ((startTime + 0.25f) - Time.time));
				yield return 0;
			}
			startTime = Time.time;
			while (startTime + 0.25f > Time.time) {
				transform.position = Vector3.Lerp (transform.position, transform.position - (direction / 50f), ((startTime + 0.25f) - Time.time));
				yield return 0;
			}
			transform.position = Cell.transform.position + new Vector3 (0, 0, -0.1f);
//	        GetComponent<SpriteRenderer>().sortingOrder = 4;
		}

		private IEnumerator Glow (Color color, float cooloutTime)
		{
			var _renderer = transform.Find ("Marker").GetComponent<SpriteRenderer> ();
			float startTime = Time.time;

			while (startTime + cooloutTime > Time.time) {
				_renderer.color = Color.Lerp (new Color (1, 1, 1, 0), color, (startTime + cooloutTime) - Time.time);
				yield return 0;
			}

			_renderer.color = Color.clear;
		}

		private IEnumerator Pulse (float breakTime, float delay, float scaleFactor)
		{
			var baseScale = transform.localScale;
			while (true) {
				float growingTime = Time.time;
				while (growingTime + delay > Time.time) {
					transform.localScale = Vector3.Lerp (baseScale * scaleFactor, baseScale, (growingTime + delay) - Time.time);
					yield return 0;
				}

				float shrinkingTime = Time.time;
				while (shrinkingTime + delay > Time.time) {
					transform.localScale = Vector3.Lerp (baseScale, baseScale * scaleFactor, (shrinkingTime + delay) - Time.time);
					yield return 0;
				}

				yield return new WaitForSeconds (breakTime);
			}
		}

		public override void MarkAsWaiting ()
		{
			isWaiting = true;
			PulseCoroutine = StartCoroutine (Pulse (1.0f, 0.5f, 1.25f));
		}

		public override void MarkAsReady ()
		{
			isWaiting = false;
			if (PulseCoroutine != null) {
				StopCoroutine (PulseCoroutine);
				transform.localScale = initialScale;
			}
		}

		public override void MarkAsFriendly ()
		{
			//SetColor (new Color (0.8f, 1, 0.8f));
		}

		public override void MarkAsReachableEnemy ()
		{
			//SetColor (new Color (1, 0.8f, 0.8f));
		}

		public override void MarkAsSelected ()
		{
//	        PulseCoroutine = StartCoroutine(Pulse(1.0f, 0.5f, 1.25f));
			//SetColor (new Color (0.8f, 0.8f, 1));
		}

		public override void MarkAsFinished ()
		{
//			StopCoroutine(PulseCoroutine);
//			transform.localScale = new Vector3(1,1,1);
			SetColor (Color.gray);
		}

		public override void UnMark ()
		{
			SetColor (Color.white);
		}


		private void SetColor (Color color)
		{
			var _renderer = GetComponent<SpriteRenderer> ();
			if (_renderer != null) {
				_renderer.color = color;
			}
		}
	}
}

