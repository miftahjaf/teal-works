//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;

namespace DigitalRubyShared
{
    /// <summary>
    /// Swipe gesture directions - assumes 0,0 is in the bottom left
    /// </summary>
    public enum SwipeGestureRecognizerDirection
    {
        /// <summary>
        /// Swipe left
        /// </summary>
        Left,

        /// <summary>
        /// Swipe right
        /// </summary>
        Right,

        /// <summary>
        /// Swipe down
        /// </summary>
        Down,

        /// <summary>
        /// Swipe up
        /// </summary>
        Up,

        /// <summary>
        /// Any direction
        /// </summary>
        Any
    }

    /// <summary>
    /// A swipe gesture is a rapid movement in one of five directions: left, right, down, up or any.
    /// A swipe gesture only signals the Possible, Ended or Failed state.
    /// </summary>
    public class SwipeGestureRecognizer : GestureRecognizer
    {
        private int touchId = GestureTouch.PlatformSpecificIdInvalid;

        private void ResetFromWrongDirection(float x, float y)
        {
            StartX = x;
            StartY = y;
            VelocityTracker.Restart(x, y);
        }

        private bool CalculateEndDirection(float x, float y)
        {
            float xDiff = x - StartX;
            float yDiff = y - StartY;
			float absXDiff = Math.Abs(xDiff);
			float absYDiff = Math.Abs(yDiff);

			if (absXDiff > absYDiff)
            {
				if (absXDiff < absYDiff * DirectionThreshold)
				{
					return false;
				}
				else if (xDiff > 0)
				{
					EndDirection = SwipeGestureRecognizerDirection.Right;
				}
				else
				{
					EndDirection = SwipeGestureRecognizerDirection.Left;
				}
            }
            else
            {
				if (absYDiff < absXDiff * DirectionThreshold)
				{
					return false;
				}	
				else if (yDiff < 0)
				{
                	EndDirection = SwipeGestureRecognizerDirection.Down;
				}
				else
				{
					EndDirection = SwipeGestureRecognizerDirection.Up;
				}
            }

			return true;
        }

        private void CheckForSwipeCompletion(bool end)
        {
            if (touchId == GestureTouch.PlatformSpecificIdInvalid)
            {
                return;
            }

            float x = CurrentTouches[0].X;
            float y = CurrentTouches[0].Y;
            if (VelocityTracker.Update(CurrentTouches[0].X, CurrentTouches[0].Y) ||
                DistanceBetweenPoints(0.0f, 0.0f, VelocityX, VelocityY) < (MinimumSpeedUnits * DeviceInfo.UnitMultiplier))
            {
                StartX = x;
                StartY = y;
                VelocityTracker.Restart(StartX, StartY);
                return;
            }

            float distance = DistanceBetweenPoints(StartX, StartY, x, y);
			if (distance < (MinimumDistanceUnits * DeviceInfo.UnitMultiplier) || !CalculateEndDirection(x, y))
            {
                return;
            }

            if (Direction == SwipeGestureRecognizerDirection.Any || Direction == EndDirection)
            {
                if (end)
                {
                    EndX = x;
                    EndY = y;
                    SetState(GestureRecognizerState.Ended);
                }
            }
            else
            {
                ResetFromWrongDirection(x, y);
            }
        }

        protected override void StateChanged()
        {
            base.StateChanged();

            if (State == GestureRecognizerState.Failed || State == GestureRecognizerState.Ended)
            {
                if (State == GestureRecognizerState.Failed)
                {
                    VelocityTracker.Reset();
                }
                touchId = GestureTouch.PlatformSpecificIdInvalid;
                StartX = StartY = float.MinValue;
            }
        }

        protected override void TouchesBegan()
        {
            if (CurrentTouches.Count == 1 && StartX == float.MinValue && StartY == float.MinValue)
            {
                touchId = CurrentTouches[0].Id;
                StartX = CurrentTouches[0].X;
                StartY = CurrentTouches[0].Y;
                VelocityTracker.Restart(StartX, StartY);
                SetState(GestureRecognizerState.Possible);
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesMoved()
        {
            if (State == GestureRecognizerState.Possible && CurrentTouches.Count == 1 && CurrentTouches[0].Id == touchId)
            {
                CheckForSwipeCompletion(EndImmeditely);
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesEnded()
        {
            if (State == GestureRecognizerState.Possible && CurrentTouches.Count == 1 && CurrentTouches[0].Id == touchId)
            {
                CheckForSwipeCompletion(true);
            }

            // if we didn't end, fail
            if (State != GestureRecognizerState.Ended)
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        public SwipeGestureRecognizer()
        {
            Direction = SwipeGestureRecognizerDirection.Down;
            MinimumDistanceUnits = 0.2f; // default to 0.2 inches minimum distance
            MinimumSpeedUnits = 2.0f; // must move 2 inches / second speed to execute
            DirectionThreshold = 2.5f;
            StartX = StartY = float.MinValue;
        }

        /// <summary>
        /// The swipe direction required to recognize the gesture (default is down)
        /// </summary>
        /// <value>The swipe direction</value>
        public SwipeGestureRecognizerDirection Direction { get; set; }

        /// <summary>
        /// The minimum distance the swipe must travel to be recognized
        /// </summary>
        /// <value>The minimum distance in units</value>
        public float MinimumDistanceUnits { get; set; }

        /// <summary>
        /// The minimum units per second the swipe must travel to be recognized
        /// </summary>
        /// <value>The minimum speed in units</value>
        public float MinimumSpeedUnits { get; set; }

        /// <summary>
        /// For set directions, this is the amount that the swipe must be proportionally in that direction
        /// vs the other direction. For example, a swipe down gesture will need to move in the y axis
        /// by this multiplier more versus moving along the x axis. Default is 2.5, which means the swipe
        /// down gesture needs to be 2.5 times greater in the y axis vs. the x axis.
        /// </summary>
        /// <value>The direction threshold.</value>
        public float DirectionThreshold { get; set; }

        /// <summary>
        /// The speed of the swipe when it completed
        /// </summary>
        /// <value>The speed of the swipe</value>
        public float Speed { get; private set; }

        /// <summary>
        /// X start of the swipe
        /// </summary>
        /// <value>The start x value</value>
        public float StartX { get; private set; }

        /// <summary>
        /// Y start of the swipe
        /// </summary>
        /// <value>The start y value</value>
        public float StartY { get; private set; }

        /// <summary>
        /// X end of the swipe
        /// </summary>
        /// <value>The end x value</value>
        public float EndX { get; private set; }

        /// <summary>
        /// Y end of the swipe
        /// </summary>
        /// <value>The end y value</value>
        public float EndY { get; private set; }

        /// <summary>
        /// The direction of the completed swipe gesture
        /// </summary>
        /// <value>The end direction.</value>
        public SwipeGestureRecognizerDirection EndDirection { get; private set; }

        /// <summary>
        /// End the swipe gesture immediately if recognized, reglardless of whether the touch is lifted. Default is false.
        /// </summary>
        /// <value>True to end immediately if recognized, false otherwise</value>
        public bool EndImmeditely { get; set; }
    }
}
