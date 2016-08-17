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
    /// A tap gesture detects one or more consecutive taps. The ended state denotes a successful tap.
    /// </summary>
    public class TapGestureRecognizer : GestureRecognizer
    {
        private int tapCount;
        private readonly Stopwatch timer = new Stopwatch();
        private float lastTapX;
        private float lastTapY;
        private float tapDownX;
        private float tapDownY;
        private int lastTapId;

		private void VerifyFailGestureAfterDelay()
		{
            float elapsed = (float)timer.Elapsed.TotalSeconds;
			if (State == GestureRecognizerState.Possible && elapsed >= ThresholdSeconds)
			{
				SetState(GestureRecognizerState.Failed);
			}
		}

        private void FailGestureAfterDelayIfNoTap()
        {
            RunActionAfterDelay(ThresholdSeconds, VerifyFailGestureAfterDelay);
        }

        protected override void StateChanged()
        {
            base.StateChanged();

            if (State == GestureRecognizerState.Failed || State == GestureRecognizerState.Ended)
            {
                tapCount = 0;
                timer.Reset();
            }
        }

        protected override void TouchesBegan()
        {
            if (CurrentTouches.Count != 1)
            {
                SetState(GestureRecognizerState.Failed);
            }
            else if (SetState(GestureRecognizerState.Possible))
            {
                foreach (GestureTouch t in CurrentTouches)
                {
                    tapDownX = t.X;
                    tapDownY = t.Y;
                    lastTapId = t.Id;
                    if (tapCount == 0)
                    {
                        lastTapX = tapDownX;
                        lastTapY = tapDownY;
                    }
                }
                timer.Reset();
                timer.Start();
            }
        }

        protected override void TouchesMoved()
        {
            if (CurrentTouches.Count != 1 || State != GestureRecognizerState.Possible)
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesEnded()
        {
            if (CurrentTouches.Count != 1 || State != GestureRecognizerState.Possible)
            {
                SetState(GestureRecognizerState.Failed);
            }
            else if ((float)timer.Elapsed.TotalSeconds <= ThresholdSeconds)
            {
                float distance = DeviceInfo.UnitMultiplier * ThresholdUnits;
                foreach (GestureTouch t in CurrentTouches)
                {
                    if (t.Id == lastTapId &&
                        PointsAreWithinDistance(lastTapX, lastTapY, t.X, t.Y, distance) &&
                        PointsAreWithinDistance(tapDownX, tapDownY, t.X, t.Y, distance))
                    {
                        if (++tapCount == NumberOfTapsRequired)
                        {
                            SetState(GestureRecognizerState.Ended);
                        }
                        else
                        {
                            timer.Reset();
                            timer.Start();
                            FailGestureAfterDelayIfNoTap();
                        }
                    }
                    else
                    {
                        SetState(GestureRecognizerState.Failed);
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TapGestureRecognizer()
        {
            NumberOfTapsRequired = 1;
            ThresholdSeconds = 0.35f;
            ThresholdUnits = 0.35f;
        }

        /// <summary>
        /// How many taps must execute in order to end the gesture
        /// </summary>
        /// <value>The number of taps required to execute the gesture</value>
        public int NumberOfTapsRequired { get; set; }

        /// <summary>
        /// How many seconds can expire before the tap is released to still count as a tap
        /// </summary>
        /// <value>The threshold in seconds</value>
        public float ThresholdSeconds { get; set; }

        /// <summary>
        /// How many units away the tap down and up and subsequent taps can be to still be considered - must be greater than 0
        /// </summary>
        /// <value>The threshold in units</value>
        public float ThresholdUnits { get; set; }

        /// <summary>
        /// Tap location x value
        /// </summary>
        public float TapX { get { return tapDownX; } }

        /// <summary>
        /// Tap location y value
        /// </summary>
        public float TapY { get { return tapDownY; } }
    }
}

