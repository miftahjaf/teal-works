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
    /// A scale gesture detects two fingers moving towards or away from each other to scale something
    /// </summary>
    public class ScaleGestureRecognizer : GestureRecognizer
    {
        private float previousDistance;
        private float centerX;
        private float centerY;

        public ScaleGestureRecognizer()
        {
            ScaleMultiplier = 1.0f;
            ZoomSpeed = 3.0f;
            ThresholdUnits = 0.15f;
            ScaleThresholdPercent = 0.01f;
            ScaleFocusMoveThresholdUnits = 0.04f;
        }

        private void UpdateCenter(float distance)
        {
            previousDistance = distance;
            centerX = FocusX;
            centerY = FocusY;
        }

        private void ProcessTouches()
        {
            float distance = DistanceBetweenPoints(CurrentTouches[0].X, CurrentTouches[0].Y, CurrentTouches[1].X, CurrentTouches[1].Y);
            FocusX = (CurrentTouches[0].X + CurrentTouches[1].X) * 0.5f;
            FocusY = (CurrentTouches[0].Y + CurrentTouches[1].Y) * 0.5f;

            if (State == GestureRecognizerState.Possible)
            {
                if (previousDistance == 0.0f)
                {
                    previousDistance = distance;
                }
                else
                {
                    float diff = Math.Abs(previousDistance - distance);
                    if (diff >= (ThresholdUnits * DeviceInfo.UnitMultiplier))
                    {
                        UpdateCenter(distance);
                        SetState(GestureRecognizerState.Began);
                    }
                }
            }
            else if (State == GestureRecognizerState.Executing)
            {
                float focusChange = DistanceBetweenPoints(FocusX, FocusY, centerX, centerY);
                if (focusChange > (ScaleFocusMoveThresholdUnits * DeviceInfo.UnitMultiplier))
                {
                    UpdateCenter(distance);
                }
                else
                {
                    ScaleMultiplier = (previousDistance <= 0.0f ? 1.0f : distance / previousDistance);
                    if (ScaleMultiplier < (1.0f - ScaleThresholdPercent) || ScaleMultiplier > (1.0f + ScaleThresholdPercent))
                    {
                        float zoomDiff = (ScaleMultiplier - 1.0f) * ZoomSpeed;
                        ScaleMultiplier = 1.0f + zoomDiff;
                        previousDistance = distance;
                        SetState(GestureRecognizerState.Executing);
                    }
                }
            }
            else if (State == GestureRecognizerState.Began)
            {
                centerX = (CurrentTouches[0].X + CurrentTouches[1].X) * 0.5f;
                centerY = (CurrentTouches[0].Y + CurrentTouches[1].Y) * 0.5f;
                SetState(GestureRecognizerState.Executing);
            }
            else
            {
                SetState(GestureRecognizerState.Possible);
            }
        }

        protected override void TouchesBegan()
        {
            if (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
            {
                SetState(GestureRecognizerState.Failed);
            }
            else
            {
                SetState(GestureRecognizerState.Possible);
            }
        }

        protected override void TouchesMoved()
        {
            if (CurrentTouches.Count != 2)
            {
                SetState(GestureRecognizerState.Failed);
            }
            else
            {
                ProcessTouches();
            }
        }

        protected override void TouchesEnded()
        {
            if (State == GestureRecognizerState.Executing)
            {
                SetState(GestureRecognizerState.Ended);
            }
            else
            {
                SetState(GestureRecognizerState.Failed);
            }

            previousDistance = 0.0f;
            FocusX = 0.0f;
            FocusY = 0.0f;
        }

        /// <summary>
        /// The current scale multiplier. Multiply your current scale value by this to scale.
        /// </summary>
        /// <value>The scale multiplier.</value>
        public float ScaleMultiplier { get; set; }

        /// <summary>
        /// Focus x position (average of all touches)
        /// </summary>
        /// <value>The focus x.</value>
        public float FocusX { get; private set; }

        /// <summary>
        /// Focus y position (average of all touches)
        /// </summary>
        /// <value>The focus y.</value>
        public float FocusY { get; private set; }

        /// <summary>
        /// Additional multiplier for ScaleMultiplier. This will making scaling happen slower or faster.
        /// </summary>
        /// <value>The zoom speed.</value>
        public float ZoomSpeed { get; set; }

        /// <summary>
        /// How many units the distance between the fingers must increase or decrease from the start distance to begin executing
        /// </summary>
        /// <value>The threshold in units</value>
        public float ThresholdUnits { get; set; }

        /// <summary>
        /// The threshold in percent (i.e. 0.1) that must chabge to signal any listeners about a new scale
        /// </summary>
        /// <value>The threshold percent</value>
        public float ScaleThresholdPercent { get; set; }

        /// <summary>
        /// If the focus moves more than this amount, reset the scale threshold percent. This helps avoid
        /// a wobbly zoom when panning and zooming at the same time.
        /// </summary>
        /// <value>The scale focus threshold in units</value>
        public float ScaleFocusMoveThresholdUnits { get; set; }
    }
}


