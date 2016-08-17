using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public enum Swipe { None, Up, Down, Left, Right };

	public class SwipeHelper : MonoBehaviour
	{
		public float minSwipeLength = 30f;

		float time = 0f;

		Vector2 firstPressPos;
		Vector2 secondPressPos;
		Vector2 currentSwipe;

		public static Swipe swipeDirection;

		void Update ()
		{
			time += Time.deltaTime;

			DetectSwipe();
		}

		public void DetectSwipe ()
		{
			if (Input.GetMouseButtonDown (0)) {
				time = 0f;
				firstPressPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			} else if (Input.GetMouseButtonUp (0)) {
				if (time > 0.5f) {
					return;
				}
				secondPressPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
				currentSwipe = new Vector3 (secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

				// Make sure it was a legit swipe, not a tap
				if (currentSwipe.magnitude < minSwipeLength) {
					swipeDirection = Swipe.None;
					return;
				}

				currentSwipe.Normalize ();

				// Swipe up
				if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
					swipeDirection = Swipe.Up;
					// Swipe down
				} else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
					swipeDirection = Swipe.Down;
					// Swipe left
				} else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
					swipeDirection = Swipe.Left;
					// Swipe right
				} else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
					swipeDirection = Swipe.Right;
				}
			} else {
				swipeDirection = Swipe.None;   
			}
		}
	}
}
