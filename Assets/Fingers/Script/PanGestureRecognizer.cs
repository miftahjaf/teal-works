//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Collections.Generic;

namespace DigitalRubyShared
{
    /// <summary>
    /// A pan gesture detects movement of a touch
    /// </summary>
    public class PanGestureRecognizer : GestureRecognizer
    {
        private float prevFocusX = float.MinValue;
        private float prevFocusY = float.MinValue;

        private void ProcessTouches()
        {
            float focusX, focusY;
            CalculateFocus(out focusX, out focusY);

            if (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
            {
                DeltaX = focusX - prevFocusX;
                DeltaY = focusY - prevFocusY;
                prevFocusX = focusX;
                prevFocusY = focusY;
                SetState(GestureRecognizerState.Executing);
                VelocityTracker.Update(focusX, focusY);
                return;
            }
            else if (prevFocusX == float.MinValue || prevFocusY == float.MinValue)
            {
                prevFocusX = focusX;
                prevFocusY = focusY;
                SetState(GestureRecognizerState.Possible);
            }
            else if (State == GestureRecognizerState.Possible)
            {
                float distance = DistanceBetweenPoints(focusX, focusY, prevFocusX, prevFocusY);
                if (distance >= (ThresholdUnits * DeviceInfo.UnitMultiplier))
                {
                    prevFocusX = focusX;
                    prevFocusY = focusY;            
                    VelocityTracker.Restart(focusX, focusY);
                    SetState(GestureRecognizerState.Began);
                }
            }
        }

        protected override void TouchesBegan()
        {
            if (prevFocusX == float.MinValue && prevFocusY == float.MinValue &&
                CurrentTouches.Count >= MinimumNumberOfTouchesRequired && CurrentTouches.Count <= MaximumNumberOfTouchesAllowed)
            {
                ProcessTouches();
            }
        }

        protected override void TouchesMoved()
        {
            if (CurrentTouches.Count >= MinimumNumberOfTouchesRequired && CurrentTouches.Count <= MaximumNumberOfTouchesAllowed)
            {
                ProcessTouches();
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesEnded()
        {
            if (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
            {
                if (CurrentTouches.Count >= MinimumNumberOfTouchesRequired && CurrentTouches.Count <= MaximumNumberOfTouchesAllowed)
                {
                    ProcessTouches();
                }
                SetState(GestureRecognizerState.Ended);
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }

            prevFocusX = prevFocusY = float.MinValue;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PanGestureRecognizer()
        {
            MinimumNumberOfTouchesRequired = 1;
            MaximumNumberOfTouchesAllowed = 1;
            ThresholdUnits = 0.1f;
        }
            
        /// <summary>
        /// Minimum number of touches that must be down for the pan to register
        /// </summary>
        /// <value>The number of touches required to pan</value>
        public int MinimumNumberOfTouchesRequired { get; set; }

        /// <summary>
        /// Maximum number of touches allowed
        /// </summary>
        /// <value>The maximum number of touches allowed.</value>
        public int MaximumNumberOfTouchesAllowed { get; set; }

        /// <summary>
        /// How many units away the pan must move to execute
        /// </summary>
        /// <value>The threshold in units</value>
        public float ThresholdUnits { get; set; }

        /// <summary>
        /// Change in x
        /// </summary>
        /// <value>The change in x</value>
        public float DeltaX { get; private set; }

        /// <summary>
        /// Change in y
        /// </summary>
        /// <value>The change in y</value>
        public float DeltaY { get; private set; }

        /// <summary>
        /// Focus x value (average of all touches)
        /// </summary>
        /// <value>The focus x.</value>
        public float FocusX { get { return prevFocusX; } }

        /// <summary>
        /// Focus y value (average of all touches)
        /// </summary>
        /// <value>The focus y.</value>
        public float FocusY { get { return prevFocusY; } }
    }
}

