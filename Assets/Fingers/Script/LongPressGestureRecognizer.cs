//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Diagnostics;

#if PCL || PORTABLE || HAS_TASKS

using System.Threading.Tasks;

#endif

namespace DigitalRubyShared
{
    /// <summary>
    /// A long press gesture detects a tap and hold and then calls back for movement until
    /// the touch is released
    /// </summary>
    public class LongPressGestureRecognizer : GestureRecognizer
    {
        private int touchId = GestureTouch.PlatformSpecificIdInvalid;
        private float startX;
        private float startY;

        private void PrepareFailGestureAfterDelay()
        {
            VelocityTracker.Restart();
            SetState(GestureRecognizerState.Possible);
            touchId = CurrentTouches[0].Id;
        }

        private void VerifyFailGestureAfterDelay(GestureTouch currentTouch)
        {
            float elapsedSeconds = VelocityTracker.ElapsedSeconds;
            if (touchId == currentTouch.Id && State == GestureRecognizerState.Possible && elapsedSeconds >= MinimumDurationSeconds)
            {
                VelocityTracker.Restart(currentTouch.X, currentTouch.Y);
                SetState(GestureRecognizerState.Began);
            }
        }

#if PCL || PORTABLE || HAS_TASKS

        private async void AttemptToStartAfterDelay()
        {
            GestureTouch currentTouch = CurrentTouches[0];
            startX = FocusX = currentTouch.X;
            startY = FocusY = currentTouch.Y;
            PrepareFailGestureAfterDelay();

            await Task.Delay(TimeSpan.FromSeconds(MinimumDurationSeconds));

            VerifyFailGestureAfterDelay(currentTouch);
        }

#else

        private void AttemptToStartAfterDelay()
        {
            GestureTouch currentTouch = CurrentTouches[0];
            startX = FocusX = currentTouch.X;
            startY = FocusY = currentTouch.Y;
            PrepareFailGestureAfterDelay();

            MainThreadCallback(MinimumDurationSeconds, () =>
            {
                VerifyFailGestureAfterDelay(currentTouch);
            });
        }

#endif

        protected override void StateChanged()
        {
            base.StateChanged();

            if (State == GestureRecognizerState.Ended || State == GestureRecognizerState.Failed)
            {
                Reset();
            }
        }

        protected override void TouchesBegan()
        {
            if (touchId == GestureTouch.PlatformSpecificIdInvalid && CurrentTouches.Count == 1)
            {
                AttemptToStartAfterDelay();
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesMoved()
        {
            if (touchId == GestureTouch.PlatformSpecificIdInvalid ||
                CurrentTouches.Count != 1 ||
                CurrentTouches[0].Id != touchId)
            {
                SetState(GestureRecognizerState.Failed);
            }
            else if (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
            {
                VelocityTracker.Update((FocusX = CurrentTouches[0].X), (FocusY = CurrentTouches[0].Y));
                SetState(GestureRecognizerState.Executing);
            }
            else if (State == GestureRecognizerState.Possible)
            {
                float distance = DistanceBetweenPoints(startX, startY, CurrentTouches[0].X, CurrentTouches[0].Y);
                if (distance > (ThresholdUnits * DeviceInfo.UnitMultiplier))
                {
                    SetState(GestureRecognizerState.Failed);
                }
            }
        }

        protected override void TouchesEnded()
        {
            if (touchId == GestureTouch.PlatformSpecificIdInvalid ||
                CurrentTouches.Count != 1 ||
                CurrentTouches[0].Id != touchId ||
                (State != GestureRecognizerState.Began && State != GestureRecognizerState.Executing))
            {
                SetState(GestureRecognizerState.Failed);
            }
            else
            {
                VelocityTracker.Update((FocusX = CurrentTouches[0].X), (FocusY = CurrentTouches[0].Y));
                SetState(GestureRecognizerState.Ended);
            }
        }

        public LongPressGestureRecognizer()
        {
            MinimumDurationSeconds = 0.4f;
            ThresholdUnits = 0.35f;
        }

        public override void Reset()
        {
            base.Reset();

            touchId = GestureTouch.PlatformSpecificIdInvalid;
            VelocityTracker.Reset();
        }

        /// <summary>
        /// The number of seconds that the touch must stay down to begin executing
        /// </summary>
        /// <value>The minimum long press duration in seconds</value>
        public float MinimumDurationSeconds { get; set ;}

        /// <summary>
        /// How many units away the long press can move before failing. After the long press begins,
        /// it is allowed to move any distance and stay executing.
        /// </summary>
        /// <value>The threshold in units</value>
        public float ThresholdUnits { get; set; }

        /// <summary>
        /// Current x average of all touch points
        /// </summary>
        /// <value>The focus x.</value>
        public float FocusX { get; set; }

        /// <summary>
        /// Current y average of all touch points
        /// </summary>
        /// <value>The focus y.</value>
        public float FocusY { get; set; }
    }
}
